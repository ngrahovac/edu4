using System.Diagnostics.CodeAnalysis;
using edu4.Application.Services;
using edu4.Domain.Projects;
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
    public async void Publishes_project_if_valid_data_is_provided()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var projects = new MongoDBProjectsRepository(config);

        var sut = new ProjectsService(projects, new NullLogger<ProjectsService>());

        var positions = new List<PositionDTO>()
        {
            new(
                string.Empty,
                string.Empty,
                new("Student", new(){ { "StudyField", "Computer Science"} })),

            new(
                string.Empty,
                string.Empty,
                new("Academic", new(){ { "ResearchField", "Computer Science"} }))
        };
        var authorId = Guid.NewGuid();
        var title = "foo";
        var description = "bar";

        // ACT
        var publishedProject = await sut.PublishProjectAsync(
            title,
            description,
            authorId,
            positions);

        // ASSERT
        publishedProject.Id.Should().NotBe(Guid.Empty);

        var retrievedProject = await projects.GetByIdAsync(publishedProject.Id);

        retrievedProject.Title.Should().Be(title);
        retrievedProject.Description.Should().Be(description);
        retrievedProject.Author.Should().Be(new Author(authorId));
        retrievedProject.Positions.Count.Should().Be(positions.Count);
    }
}
