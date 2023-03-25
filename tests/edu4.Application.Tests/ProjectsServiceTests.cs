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
}
