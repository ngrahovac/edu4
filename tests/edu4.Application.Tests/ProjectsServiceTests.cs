using System.Diagnostics.CodeAnalysis;
using edu4.Application.Models;
using edu4.Application.Services;
using edu4.Application.Tests.TestData;
using edu4.Domain.Contributors;
using edu4.Domain.Projects;
using edu4.Domain.Users;
using edu4.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace edu4.Application.Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("App services integration tests")]
public class ProjectsServiceTests
{
    [Fact]
    public async void Publishes_the_project_if_valid_data_is_provided()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var author = await new UserFactory().WithHats(
            new List<Hat>()
            {
                HatFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new NullLogger<ProjectsService>());

        var positions = new List<PositionDTO>()
        {
            new(
                string.Empty,
                string.Empty,
                new(HatType.Student, new(){ { "StudyField", "Computer Science"} })),

            new(
                string.Empty,
                string.Empty,
                new(HatType.Academic, new(){ { "ResearchField", "Computer Science"} }))
        };

        var title = "foo";
        var description = "bar";

        // ACT
        var publishedProject = await sut.PublishProjectAsync(
            title,
            description,
            author.Id,
            positions);

        // ASSERT
        publishedProject.Id.Should().NotBe(Guid.Empty);

        var retrievedProject = await projects.GetByIdAsync(publishedProject.Id);

        retrievedProject.Title.Should().Be(title);
        retrievedProject.Description.Should().Be(description);
        retrievedProject.Author.Should().Be(new Author(author.Id));
        retrievedProject.Positions.Count.Should().Be(positions.Count);
    }

    [Fact]
    public async Task Project_author_cant_add_a_new_position_to_an_existing_project_with_the_same_name_and_requirements_as_an_existing_position()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat("Software Engineering", AcademicDegree.Masters);

        var existingProject = await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(existingPositionName)
                .WithRequirements(existingPositionRequirements)
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var addingNewPosition = async () => await sut.AddPositionAsync(
            existingProject.Id,
            existingProject.Author.Id,
            existingPositionName,
            "Position description",
            HatDTO.FromHat(existingPositionRequirements));

        // ASSERT
        await addingNewPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Project_author_can_add_a_new_position_to_an_existing_project_with_name_different_from_all_existing_positions()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat("Software Engineering", AcademicDegree.Masters);

        var existingProject = await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(existingPositionName)
                .WithRequirements(existingPositionRequirements)
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var addingNewPosition = async () => await sut.AddPositionAsync(
            existingProject.Id,
            existingProject.Author.Id,
            "test2",
            "Position description",
            HatDTO.FromHat(existingPositionRequirements));

        // ASSERT
        await addingNewPosition.Should().NotThrowAsync<InvalidOperationException>();

        var retrievedProject = await projects.GetByIdAsync(existingProject.Id);
        retrievedProject.Positions.Count.Should().Be(existingProject.Positions.Count + 1);
    }

    [Fact]
    public async Task Project_author_can_add_a_new_position_to_an_existing_project_with_requirements_different_from_all_existing_positions()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat("Software Engineering", AcademicDegree.Masters);

        var existingProject = await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(existingPositionName)
                .WithRequirements(existingPositionRequirements)
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var addingNewPosition = async () => await sut.AddPositionAsync(
            existingProject.Id,
            existingProject.Author.Id,
            existingPositionName,
            "Position description",
            HatDTO.FromHat(new AcademicHat("Computer Science")));

        // ASSERT
        await addingNewPosition.Should().NotThrowAsync<InvalidOperationException>();

        var retrievedProject = await projects.GetByIdAsync(existingProject.Id);
        retrievedProject.Positions.Count.Should().Be(existingProject.Positions.Count + 1);
    }

    [Fact]
    public async Task Only_the_project_author_can_add_positions_to_an_existing_project()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat("Software Engineering", AcademicDegree.Masters);

        var existingProject = await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(existingPositionName)
                .WithRequirements(existingPositionRequirements)
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var addingNewPosition = async () => await sut.AddPositionAsync(
            existingProject.Id,
            Guid.NewGuid(),
            existingPositionName,
            "Position description",
            HatDTO.FromHat(new AcademicHat("Computer Science")));

        // ASSERT
        await addingNewPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_include_projects_with_the_keyword_in_title_only()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectFactory().WithTitle(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword);

        // ASSERT
        discoveredProjects.Count.Should().Be(1);

        foreach (var project in discoveredProjects)
        {
            project.Title.Should().Contain(keyword);
        }
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_includen_projects_with_the_keyword_in_description_only()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectFactory().WithDescription(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword);

        // ASSERT
        discoveredProjects.Count.Should().Be(1);

        foreach (var project in discoveredProjects)
        {
            project.Description.Should().Contain(keyword);
        }
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_include_projects_with_the_keyword_in_position_name_only()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(keyword)
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword);

        // ASSERT
        discoveredProjects.Count.Should().Be(1);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => p.Name.Contains(keyword)).Should().BeTrue();
        }
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_include_projects_with_the_keyword_in_position_description_only()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithDescription(keyword)
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword);

        // ASSERT
        discoveredProjects.Count.Should().Be(1);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => p.Description.Contains(keyword)).Should().BeTrue();
        }
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_return_projects_with_the_keyword_in_title_or_description_or_in_any_position_name_or_description()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectFactory().WithTitle(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithDescription(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithDescription(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword);

        // ASSERT
        discoveredProjects.Count.Should().Be(4);

        foreach (var project in discoveredProjects)
        {
            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription =
                project.Positions.Any(
                    p => p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword));

            (titleContainsKeyword || descriptionContainsKeyword || hasAPositionWithTheKeywordInTitleOrDescription)
                .Should().BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_without_specifying_search_refinements_should_return_all_projects()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        await new ProjectFactory().WithTitle("foo")
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithDescription("bar")
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName("baz")
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync();

        // ASSERT
        discoveredProjects.Count.Should().Be(3);
    }

    [Fact]
    public async Task Discovering_projects_and_requesting_sorting_by_oldest_first_should_return_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Asc);

        // ASSERT
        discoveredProjects.Count.Should().Be(3);
        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderBy(p => p.DatePosted), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Discovering_projects_and_requesting_sorting_by_newest_first_should_return_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Desc);

        // ASSERT
        discoveredProjects.Count.Should().Be(3);
        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderByDescending(p => p.DatePosted), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_requesting_sorting_by_oldest_first_posted_should_return_projects_containing_the_keyword_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectFactory().WithTitle(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithTitle(keyword)
             .WithPositions(
             new List<Position>()
             {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
             })
             .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword, ProjectsSortOption.Asc);

        // ASSERT
        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderBy(p => p.DatePosted), options => options.WithStrictOrdering());

        foreach (var project in discoveredProjects)
        {
            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription =
                project.Positions.Any(
                    p => p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword));

            (titleContainsKeyword || descriptionContainsKeyword || hasAPositionWithTheKeywordInTitleOrDescription)
                .Should().BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_requesting_sorting_by_newest_first_should_return_projects_containing_the_keyword_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectFactory().WithTitle(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithTitle(keyword)
             .WithPositions(
             new List<Position>()
             {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
             })
             .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword, ProjectsSortOption.Desc);

        // ASSERT
        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderByDescending(p => p.DatePosted), options => options.WithStrictOrdering());

        foreach (var project in discoveredProjects)
        {
            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription =
                project.Positions.Any(
                    p => p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword));

            (titleContainsKeyword || descriptionContainsKeyword || hasAPositionWithTheKeywordInTitleOrDescription)
                .Should().BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_student_hat_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Electronics Engineering", AcademicDegree.Masters);

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Bachelors)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName("Recommended position 1")
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build(),

                new PositionFactory().WithName("Recommended position 2")
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Doctorate)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Software Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Default, studentHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(3);

        foreach (var discoveredProject in discoveredProjects)
        {
            discoveredProject.Positions.Any(p => studentHat.Fits(p.Requirements)).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_academic_hat_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Default, academicHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var discoveredProject in discoveredProjects)
        {
            discoveredProject.Positions.Any(p => academicHat.Fits(p.Requirements)).Should().BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_a_student_hat_returns_projects_containing_the_keyword_with_min_one_position_with_requirements_that_the_hat_fits()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Electronics Engineering", AcademicDegree.Masters);
        var keyword = "keyword";

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Bachelors)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName("Recommended position 1")
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build(),

                new PositionFactory().WithName("Recommended position 2")
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Doctorate)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Software Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithTitle(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Bachelors)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithDescription(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build(),

                new PositionFactory().WithName("Recommended position 2")
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithDescription(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Electronics Engineering")
                    .WithAcademicDegree(AcademicDegree.Doctorate)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword, ProjectsSortOption.Default, studentHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(3);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => studentHat.Fits(p.Requirements)).Should().BeTrue();

            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription =
                project.Positions.Any(
                    p => p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword));
        }
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_specifying_users_academic_hat_returns_projects_containing_the_keyword_with_min_one_position_with_requirements_that_the_hat_fits()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");
        var keyword = "keyword";

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithTitle(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithDescription(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
             new List<Position>()
             {
                new PositionFactory().WithName(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
             })
             .SeedAsync();

        await new ProjectFactory().WithPositions(
             new List<Position>()
             {
                new PositionFactory().WithDescription(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
             })
             .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword, ProjectsSortOption.Default, academicHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(4);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => academicHat.Fits(p.Requirements)).Should().BeTrue();

            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription =
                project.Positions.Any(
                    p => p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword));
        }
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_academic_hat_and_requesting_sorting_from_oldest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Asc, academicHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var discoveredProject in discoveredProjects)
        {
            discoveredProject.Positions.Any(p => academicHat.Fits(p.Requirements)).Should().BeTrue();
        }

        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderBy(p => p.DatePosted), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_academic_hat_and_requesting_sorting_by_newest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Desc, academicHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var discoveredProject in discoveredProjects)
        {
            discoveredProject.Positions.Any(p => academicHat.Fits(p.Requirements)).Should().BeTrue();
        }

        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderByDescending(p => p.DatePosted), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_student_hat_and_requesting_sorting_from_oldest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Computer Science", AcademicDegree.Masters);

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Computer Science")
                    .WithAcademicDegree(AcademicDegree.Bachelors)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Computer Science")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithAcademicDegree(AcademicDegree.Doctorate)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Asc, studentHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => studentHat.Fits(p.Requirements)).Should().BeTrue();
        }

        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderBy(p => p.DatePosted), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_student_hat_and_requesting_sorting_by_newest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Computer Science", AcademicDegree.Masters);

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Computer Science")
                    .WithAcademicDegree(AcademicDegree.Bachelors)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Computer Science")
                    .WithAcademicDegree(AcademicDegree.Masters)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithAcademicDegree(AcademicDegree.Doctorate)
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(null, ProjectsSortOption.Desc, studentHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => studentHat.Fits(p.Requirements)).Should().BeTrue();
        }

        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderByDescending(p => p.DatePosted), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_specifying_users_academic_hat_and_requesting_sorting_by_newest_first_returns_projects_containing_the_keyword_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");
        var keyword = "keyword";

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithTitle(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithDescription(keyword)
            .WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithName(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithDescription(keyword)
                .WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Computer Science")
                    .Build())
                .Build()
            })
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword, ProjectsSortOption.Desc, academicHat);

        // ASSERT
        discoveredProjects.Count.Should().Be(4);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => academicHat.Fits(p.Requirements)).Should().BeTrue();

            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription =
                project.Positions.Any(
                    p => p.Name.Contains(keyword) ||
                    p.Description.Contains(keyword));

            (titleContainsKeyword || descriptionContainsKeyword || hasAPositionWithTheKeywordInTitleOrDescription)
                .Should().BeTrue();
        }

        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderByDescending(p => p.DatePosted), options => options.WithStrictOrdering());
    }
}
