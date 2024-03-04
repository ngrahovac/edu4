using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Peer.Application.Models;
using Peer.Domain.Contributors;
using Peer.Infrastructure;
using Peer.Application.Services;
using Peer.Domain.Projects;
using Peer.Tests.Utils.Seeders;
using Peer.Tests.Utils.Factories;

namespace Peer.Tests.Application;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("App services integration tests")]
public class ProjectsServiceTests
{
    [Fact]
    public async void Publishes_the_project_if_valid_data_is_provided()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var author = await new ContributorsSeeder(config)
            .WithHats(
                new List<Hat>()
                {
                    HatsFactory.OfType(HatType.Student).WithStudyField("Computer Science").Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        var positions = new List<PositionDTO>()
        {
            new(
                string.Empty,
                string.Empty,
                new(HatType.Student, new() { { "StudyField", "Computer Science" } })
            ),
            new(
                string.Empty,
                string.Empty,
                new(HatType.Academic, new() { { "ResearchField", "Computer Science" } })
            )
        };

        var title = "foo";
        var description = "bar";
        var datePosted = DateTime.UtcNow.Date;
        var startDate = DateTime.UtcNow.AddDays(1).Date;
        var endDate = DateTime.UtcNow.AddDays(3).Date;

        // ACT
        var publishedProject = await sut.PublishProjectAsync(
            title,
            description,
            author.Id,
            datePosted,
            startDate,
            endDate,
            positions
        );

        // ASSERT
        publishedProject.Id.Should().NotBe(Guid.Empty);

        var retrievedProject = await projects.GetByIdAsync(publishedProject.Id);

        retrievedProject.Title.Should().Be(title);
        retrievedProject.Description.Should().Be(description);
        retrievedProject.AuthorId.Should().Be(author.Id);
        retrievedProject.DatePosted.Should().Be(datePosted);
        retrievedProject.Duration?.StartDate.Should().Be(startDate);
        retrievedProject.Duration?.EndDate.Should().Be(endDate);
        retrievedProject.Positions.Count.Should().Be(positions.Count);
    }

    [Fact]
    public async Task Project_author_cant_add_a_new_position_to_an_existing_project_with_the_same_name_and_requirements_as_an_existing_position()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat(
            "Software Engineering",
            AcademicDegree.Masters
        );

        var existingProject = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(existingPositionName)
                        .WithRequirements(existingPositionRequirements)
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var addingNewPosition = async () =>
            await sut.AddPositionAsync(
                existingProject.Id,
                existingProject.AuthorId,
                existingPositionName,
                "Position description",
                HatDTO.FromHat(existingPositionRequirements)
            );

        // ASSERT
        await addingNewPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Project_author_can_add_a_new_position_to_an_existing_project_with_name_different_from_all_existing_positions()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat(
            "Software Engineering",
            AcademicDegree.Masters
        );

        var existingProject = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(existingPositionName)
                        .WithRequirements(existingPositionRequirements)
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var addingNewPosition = async () =>
            await sut.AddPositionAsync(
                existingProject.Id,
                existingProject.AuthorId,
                "test2",
                "Position description",
                HatDTO.FromHat(existingPositionRequirements)
            );

        // ASSERT
        await addingNewPosition.Should().NotThrowAsync<InvalidOperationException>();

        var retrievedProject = await projects.GetByIdAsync(existingProject.Id);
        retrievedProject.Positions.Count.Should().Be(existingProject.Positions.Count + 1);
    }

    [Fact]
    public async Task Project_author_can_add_a_new_position_to_an_existing_project_with_requirements_different_from_all_existing_positions()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat(
            "Software Engineering",
            AcademicDegree.Masters
        );

        var existingProject = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(existingPositionName)
                        .WithRequirements(existingPositionRequirements)
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var addingNewPosition = async () =>
            await sut.AddPositionAsync(
                existingProject.Id,
                existingProject.AuthorId,
                existingPositionName,
                "Position description",
                HatDTO.FromHat(new AcademicHat("Computer Science"))
            );

        // ASSERT
        await addingNewPosition.Should().NotThrowAsync<InvalidOperationException>();

        var retrievedProject = await projects.GetByIdAsync(existingProject.Id);
        retrievedProject.Positions.Count.Should().Be(existingProject.Positions.Count + 1);
    }

    [Fact]
    public async Task Only_the_project_author_can_add_positions_to_an_existing_project()
    {
        //  ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var existingPositionName = "test";
        var existingPositionRequirements = new StudentHat(
            "Software Engineering",
            AcademicDegree.Masters
        );

        var existingProject = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(existingPositionName)
                        .WithRequirements(existingPositionRequirements)
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var addingNewPosition = async () =>
            await sut.AddPositionAsync(
                existingProject.Id,
                Guid.NewGuid(),
                existingPositionName,
                "Position description",
                HatDTO.FromHat(new AcademicHat("Computer Science"))
            );

        // ASSERT
        await addingNewPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_include_projects_with_the_keyword_in_title_only()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid(), keyword);

        // ASSERT
        discoveredProjects.Count.Should().Be(1);

        foreach (var project in discoveredProjects)
        {
            project.Title.Should().Contain(keyword);
        }
    }

    [Fact]
    public async void Discovering_projects_by_keyword_should_include_projects_with_the_keyword_in_description_only()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectsSeeder(config)
            .WithDescription(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid(), keyword);

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
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>() { new PositionsFactory().WithName(keyword).Build() }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid(), keyword);

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
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>() { new PositionsFactory().WithDescription(keyword).Build() }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid(), keyword);

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
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithDescription(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(keyword)
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithDescription(keyword)
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid(), keyword);

        // ASSERT
        discoveredProjects.Count.Should().Be(4);

        foreach (var project in discoveredProjects)
        {
            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription = project.Positions.Any(
                p => p.Name.Contains(keyword) || p.Description.Contains(keyword)
            );

            (
                titleContainsKeyword
                || descriptionContainsKeyword
                || hasAPositionWithTheKeywordInTitleOrDescription
            )
                .Should()
                .BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_without_specifying_search_refinements_should_return_all_projects()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        await new ProjectsSeeder(config)
            .WithTitle("foo")
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithDescription("bar")
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName("baz")
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid());

        // ASSERT
        discoveredProjects.Count.Should().Be(3);
    }

    [Fact]
    public async Task Discovering_projects_and_requesting_sorting_by_oldest_first_should_return_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Academic).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid(), null, ProjectsSortOption.ByDatePostedAsc);

        // ASSERT
        discoveredProjects.Count.Should().Be(3);
        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderBy(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task Discovering_projects_and_requesting_sorting_by_newest_first_should_return_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Academic).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(Guid.NewGuid(), null, ProjectsSortOption.ByDatePostedDesc);

        // ASSERT
        discoveredProjects.Count.Should().Be(3);
        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderByDescending(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_requesting_sorting_by_oldest_first_posted_should_return_projects_containing_the_keyword_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            keyword,
            ProjectsSortOption.ByDatePostedAsc
        );

        // ASSERT
        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderBy(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );

        foreach (var project in discoveredProjects)
        {
            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription = project.Positions.Any(
                p => p.Name.Contains(keyword) || p.Description.Contains(keyword)
            );

            (
                titleContainsKeyword
                || descriptionContainsKeyword
                || hasAPositionWithTheKeywordInTitleOrDescription
            )
                .Should()
                .BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_requesting_sorting_by_newest_first_should_return_projects_containing_the_keyword_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var keyword = "test";

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            keyword,
            ProjectsSortOption.ByDatePostedDesc
        );

        // ASSERT
        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderByDescending(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );

        foreach (var project in discoveredProjects)
        {
            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription = project.Positions.Any(
                p => p.Name.Contains(keyword) || p.Description.Contains(keyword)
            );

            (
                titleContainsKeyword
                || descriptionContainsKeyword
                || hasAPositionWithTheKeywordInTitleOrDescription
            )
                .Should()
                .BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_student_hat_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Electronics Engineering", AcademicDegree.Masters);

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Bachelors)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName("Recommended position 1")
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithName("Recommended position 2")
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Doctorate)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Software Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Academic).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            null,
            ProjectsSortOption.Unspecified,
            studentHat
        );

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
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Electronics Engineering")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            null,
            ProjectsSortOption.Unspecified,
            academicHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var discoveredProject in discoveredProjects)
        {
            discoveredProject.Positions
                .Any(p => academicHat.Fits(p.Requirements))
                .Should()
                .BeTrue();
        }
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_a_student_hat_returns_projects_containing_the_keyword_with_min_one_position_with_requirements_that_the_hat_fits()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Electronics Engineering", AcademicDegree.Masters);
        var keyword = "keyword";

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Bachelors)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName("Recommended position 1")
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithName("Recommended position 2")
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Doctorate)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Software Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Academic).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Bachelors)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithDescription(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(keyword)
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithName("Recommended position 2")
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithDescription(keyword)
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Electronics Engineering")
                                .WithAcademicDegree(AcademicDegree.Doctorate)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            keyword,
            ProjectsSortOption.Unspecified,
            studentHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(3);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => studentHat.Fits(p.Requirements)).Should().BeTrue();

            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription = project.Positions.Any(
                p => p.Name.Contains(keyword) || p.Description.Contains(keyword)
            );
        }
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_specifying_users_academic_hat_returns_projects_containing_the_keyword_with_min_one_position_with_requirements_that_the_hat_fits()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");
        var keyword = "keyword";

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Electronics Engineering")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithDescription(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Electronics Engineering")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(keyword)
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithDescription(keyword)
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            keyword,
            ProjectsSortOption.Unspecified,
            academicHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(4);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => academicHat.Fits(p.Requirements)).Should().BeTrue();

            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription = project.Positions.Any(
                p => p.Name.Contains(keyword) || p.Description.Contains(keyword)
            );
        }
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_academic_hat_and_requesting_sorting_from_oldest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Electronics Engineering")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            null,
            ProjectsSortOption.ByDatePostedAsc,
            academicHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var discoveredProject in discoveredProjects)
        {
            discoveredProject.Positions
                .Any(p => academicHat.Fits(p.Requirements))
                .Should()
                .BeTrue();
        }

        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderBy(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_academic_hat_and_requesting_sorting_by_newest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Electronics Engineering")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            null,
            ProjectsSortOption.ByDatePostedDesc,
            academicHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var discoveredProject in discoveredProjects)
        {
            discoveredProject.Positions
                .Any(p => academicHat.Fits(p.Requirements))
                .Should()
                .BeTrue();
        }

        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderByDescending(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_student_hat_and_requesting_sorting_from_oldest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Computer Science", AcademicDegree.Masters);

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Computer Science")
                                .WithAcademicDegree(AcademicDegree.Bachelors)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Computer Science")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithAcademicDegree(AcademicDegree.Doctorate)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            null,
            ProjectsSortOption.ByDatePostedAsc,
            studentHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => studentHat.Fits(p.Requirements)).Should().BeTrue();
        }

        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderBy(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task Discovering_projects_by_specifying_users_student_hat_and_requesting_sorting_by_newest_first_returns_projects_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var studentHat = new StudentHat("Computer Science", AcademicDegree.Masters);

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Computer Science")
                                .WithAcademicDegree(AcademicDegree.Bachelors)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithStudyField("Computer Science")
                                .WithAcademicDegree(AcademicDegree.Masters)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Student)
                                .WithAcademicDegree(AcademicDegree.Doctorate)
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            null,
            ProjectsSortOption.ByDatePostedDesc,
            studentHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(2);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => studentHat.Fits(p.Requirements)).Should().BeTrue();
        }

        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderByDescending(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task Discovering_projects_by_keyword_and_specifying_users_academic_hat_and_requesting_sorting_by_newest_first_returns_projects_containing_the_keyword_with_min_one_position_with_requirements_that_the_hat_fits_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var academicHat = new AcademicHat("Computer Science");
        var keyword = "keyword";

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Electronics Engineering")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(HatsFactory.OfType(HatType.Student).Build())
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithTitle(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithDescription(keyword)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build(),
                    new PositionsFactory()
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Electronics Engineering")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithName(keyword)
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory()
                        .WithDescription(keyword)
                        .WithRequirements(
                            HatsFactory
                                .OfType(HatType.Academic)
                                .WithResearchField("Computer Science")
                                .Build()
                        )
                        .Build()
                }
            )
            .SeedAsync();

        var projects = new MongoDBProjectsRepository(config);
        var users = new MongoDbContributorsRepository(config);

        var sut = new ProjectsService(
            projects,
            users,
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        // ACT
        var discoveredProjects = await sut.DiscoverAsync(
            Guid.NewGuid(),
            keyword,
            ProjectsSortOption.ByDatePostedDesc,
            academicHat
        );

        // ASSERT
        discoveredProjects.Count.Should().Be(4);

        foreach (var project in discoveredProjects)
        {
            project.Positions.Any(p => academicHat.Fits(p.Requirements)).Should().BeTrue();

            var titleContainsKeyword = project.Title.Contains(keyword);
            var descriptionContainsKeyword = project.Description.Contains(keyword);
            var hasAPositionWithTheKeywordInTitleOrDescription = project.Positions.Any(
                p => p.Name.Contains(keyword) || p.Description.Contains(keyword)
            );

            (
                titleContainsKeyword
                || descriptionContainsKeyword
                || hasAPositionWithTheKeywordInTitleOrDescription
            )
                .Should()
                .BeTrue();
        }

        discoveredProjects
            .Should()
            .BeEquivalentTo(
                discoveredProjects.OrderByDescending(p => p.DatePosted),
                options => options.WithStrictOrdering()
            );
    }

    [Fact]
    public async Task Project_author_can_close_a_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);
        var domainEvents = new MongoDbDomainEventsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            domainEvents,
            new NullLogger<ProjectsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        // ACT
        await sut.ClosePositionAsync(author.Id, project.Id, project.Positions.ElementAt(0).Id);

        // ASSERT
        var retrievedProject = await projects.GetByIdAsync(project.Id);
        retrievedProject.Positions.ElementAt(0).Open.Should().BeFalse();

        var retrievedDomainEvents = await domainEvents.GetUnprocessedBatchAsync(5);
        retrievedDomainEvents.Count.Should().Be(1);
        retrievedDomainEvents[0].Should().BeOfType<PositionClosed>();
    }

    [Fact]
    public async Task Positions_of_a_published_project_are_open_by_default()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var positions = new List<Position>()
        {
            new PositionsFactory().Build(),
            new PositionsFactory().Build()
        };

        var publishedProject = await sut.PublishProjectAsync(
            "test",
            "test",
            author.Id,
            DateTime.UtcNow,
            DateTime.UtcNow,
            DateTime.UtcNow,
            positions
                .Select(p => new PositionDTO(p.Name, p.Description, HatDTO.FromHat(p.Requirements)))
                .ToList()
        );

        // ASSERT
        var retrievedProject = await projects.GetByIdAsync(publishedProject.Id);
        retrievedProject.Positions.Count.Should().Be(positions.Count);
        retrievedProject.Positions.Select(p => p.Open).Should().AllBeEquivalentTo(true);
    }

    [Fact]
    public async Task Collaborator_that_is_not_the_project_author_cannot_close_a_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var requester = await new ContributorsSeeder(config).SeedAsync();

        // ACT
        var closingAPositionByACollaboratorThatIsNotTheProjectAuthor = async () =>
            await sut.ClosePositionAsync(
                requester.Id,
                project.Id,
                project.Positions.ElementAt(0).Id
            );

        // ASSERT
        await closingAPositionByACollaboratorThatIsNotTheProjectAuthor
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task A_non_existing_user_cannot_close_a_project_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        // ACT
        var nonExistingContributorClosingAPosition = async () =>
            await sut.ClosePositionAsync(
                Guid.NewGuid(),
                project.Id,
                project.Positions.ElementAt(0).Id
            );

        // ASSERT
        await nonExistingContributorClosingAPosition
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task A_position_cannot_be_closed_on_a_non_existing_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        var requester = await new ContributorsSeeder(config).SeedAsync();

        // ACT
        var closingAPositionOnANonExistingProject = async () =>
            await sut.ClosePositionAsync(requester.Id, Guid.NewGuid(), Guid.NewGuid());

        // ASSERT
        await closingAPositionOnANonExistingProject
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    // TODO: consider testing the domain invariant
    [Fact]
    public async Task Author_cannot_close_a_position_on_own_project_that_doesnt_exist()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        // ACT
        var authorClosingANonExistingPosition = async () =>
            await sut.ClosePositionAsync(author.Id, project.Id, Guid.NewGuid());

        // ASSERT
        await authorClosingANonExistingPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Project_author_can_reopen_a_closed_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ProjectsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().WithOpen(false).Build() })
            .SeedAsync();

        // ACT
        await sut.ReopenPositionAsync(author.Id, project.Id, project.Positions.ElementAt(0).Id);

        // ASSERT
        var retrievedProject = await projects.GetByIdAsync(project.Id);
        retrievedProject.Positions.ElementAt(0).Open.Should().BeTrue();
    }

    [Fact]
    public async Task Project_author_can_remove_a_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);
        var domainEvents = new MongoDbDomainEventsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            domainEvents,
            new NullLogger<ProjectsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        // ACT
        await sut.RemovePositionAsync(author.Id, project.Id, project.Positions.ElementAt(0).Id);

        // ASSERT
        var retrievedProject = await projects.GetByIdAsync(project.Id);
        retrievedProject.Positions.ElementAt(0).Removed.Should().BeTrue();

        var retrievedDomainEvents = await domainEvents.GetUnprocessedBatchAsync(5);
        retrievedDomainEvents.Count.Should().Be(1);
        retrievedDomainEvents[0].Should().BeOfType<PositionRemoved>();
    }

    [Fact]
    public async Task Project_author_can_remove_a_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);
        var domainEvents = new MongoDbDomainEventsRepository(config);

        var sut = new ProjectsService(
            projects,
            new MongoDbContributorsRepository(config),
            domainEvents,
            new NullLogger<ProjectsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        // ACT
        await sut.RemoveAsync(project.Id, author.Id);

        // ASSERT
        var retrievedProject = await projects.GetByIdAsync(project.Id);
        retrievedProject.Should().BeNull();

        var retrievedDomainEvents = await domainEvents.GetUnprocessedBatchAsync(5);
        retrievedDomainEvents.Count.Should().Be(1);
        retrievedDomainEvents[0].Should().BeOfType<ProjectRemoved>();
    }
}
