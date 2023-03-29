using System.Diagnostics.CodeAnalysis;
using edu4.Application.Models;
using edu4.Application.Services;
using edu4.Application.Tests.TestData;
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
            new MongoDbUsersRepository(config),
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
    public async void Returns_all_projects_with_a_position_with_requirements_fit_for_a_student()
    {
        // ARRANGE
        var studentHat = new StudentHat("Software Engineering", AcademicDegree.Masters);

        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Software Engineering")
                    .WithAcademicDegree(AcademicDegree.Bachelors)
                    .Build())
                .Build(),

                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Software Engineering")
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

        var sut = new ProjectsService(
            new MongoDBProjectsRepository(config),
            new MongoDbUsersRepository(config),
            new NullLogger<ProjectsService>());

        // ACT
        var retrievedProjects = await sut.GetRecommendedForUserWearing(studentHat);

        // ASSERT
        retrievedProjects.Count.Should().Be(2);
    }

    [Fact]
    public async void Returns_all_projects_with_a_position_with_requirements_fit_for_an_academic()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var usersAcademicHat = new AcademicHat("Software Engineering");

        await new ProjectFactory().WithPositions(
            new List<Position>()
            {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Student)
                    .WithStudyField("Software Engineering")
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
                    .WithResearchField("Software Engineering")
                    .Build())
                .Build()
            })
            .SeedAsync();

        await new ProjectFactory().WithPositions(
           new List<Position>()
           {
                new PositionFactory().WithRequirements(
                    HatFactory.OfType(HatType.Academic)
                    .WithResearchField("Electronics Engineering")
                    .Build())
                .Build()
           })
           .SeedAsync();


        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbUsersRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var retrievedProjects = await sut.GetRecommendedForUserWearing(usersAcademicHat);

        // ASSERT
        retrievedProjects.Count.Should().Be(1);
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_return_projects_containing_the_keyword_in_title_or_description_or_position_name_or_description()
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
        var users = new MongoDbUsersRepository(config);

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
    public async void Discovering_projects_by_keyword_should_also_return_projects_with_the_keyword_in_title_only()
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
        var users = new MongoDbUsersRepository(config);

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
    public async void Discovering_projects_by_keyword_should_also_return_projects_with_the_keyword_in_description_only()
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
        var users = new MongoDbUsersRepository(config);

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
    public async void Discovering_projects_by_keyword_should_also_return_projects_with_the_keyword_in_position_name_only()
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
        var users = new MongoDbUsersRepository(config);

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
    public async void Discovering_projects_by_keyword_should_also_return_projects_with_the_keyword_in_position_description_only()
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
        var users = new MongoDbUsersRepository(config);

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
    public async Task Discovering_projects_and_requesting_them_to_be_sorted_from_oldest_posted_should_return_a_sorted_collection()
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
        var users = new MongoDbUsersRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword, ProjectsSortOption.OldestFirst);

        // ASSERT
        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderBy(p => p.DatePosted), options => options.WithStrictOrdering());
    }

    [Fact]
    public async Task Discovering_projects_and_requesting_them_to_be_sorted_from_newest_posted_should_return_a_sorted_collection()
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
        var users = new MongoDbUsersRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new NullLogger<ProjectsService>());

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(keyword, ProjectsSortOption.NewestFirst);

        // ASSERT
        discoveredProjects.Should().BeEquivalentTo(discoveredProjects.OrderByDescending(p => p.DatePosted), options => options.WithStrictOrdering());
    }
}
