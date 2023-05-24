using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Peer.Application.Services;
using Peer.Domain.Projects;
using Peer.Infrastructure;
using Peer.Tests.Application.TestData;

namespace Peer.Tests.Application;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("App services integration tests")]
public class ApplicationsServiceTests
{
    [Fact]
    public async void A_contributor_can_apply_for_a_position_on_a_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new NullLogger<ApplicationsService>());

        var author = await new ContributorFactory().SeedAsync();
        var project = await new ProjectFactory()
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionFactory().Build() })
            .SeedAsync();

        var contributorToApply = await new ContributorFactory().SeedAsync();

        // ACT
        var application = await sut.SubmitAsync(
            contributorToApply.Id,
            project.Id,
            project.Positions.ElementAt(0).Id);

        // ASSERT
        var retrievedApplication = await applications.GetByApplicantAndPosition(
            contributorToApply.Id,
            project.Positions.ElementAt(0).Id);

        retrievedApplication.Should().NotBeNull();
        retrievedApplication.Id.Should().Be(application.Id);
        retrievedApplication.ApplicantId.Should().Be(application.ApplicantId);
        retrievedApplication.ProjectId.Should().Be(application.ProjectId);
        retrievedApplication.PositionId.Should().Be(application.PositionId);
    }

    [Fact]
    public async void A_contributor_cant_apply_for_a_position_on_an_authored_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new NullLogger<ApplicationsService>());

        var author = await new ContributorFactory().SeedAsync();
        var project = await new ProjectFactory()
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionFactory().Build() })
            .SeedAsync();

        // ACT
        var applyingForAPositionOnOwnProject = async () => await sut.SubmitAsync(
            author.Id,
            project.Id,
            project.Positions.ElementAt(0).Id);

        // ASSERT
        await applyingForAPositionOnOwnProject.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_cant_apply_for_a_position_on_a_non_existing_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new NullLogger<ApplicationsService>());

        var contributor = await new ContributorFactory().SeedAsync();

        // ACT
        var applyingForAPositionOnANonExistantProject = async () => await sut.SubmitAsync(
            contributor.Id,
            Guid.NewGuid(),
            Guid.NewGuid());

        // ASSERT
        await applyingForAPositionOnANonExistantProject.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_cant_apply_for_a_non_existing_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new NullLogger<ApplicationsService>());

        var contributor = await new ContributorFactory().SeedAsync();

        var project = await new ProjectFactory()
            .WithPositions(new List<Position>() { new PositionFactory().Build() })
            .SeedAsync();

        // ACT
        var applyingForANonExistantPosition = async () => await sut.SubmitAsync(
            contributor.Id,
            project.Id,
            Guid.NewGuid());

        // ASSERT
        await applyingForANonExistantPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_cant_apply_for_a_position_they_already_applied_for()
    {
        // ARRANGE
        var config = new ConfigurationBuilder()
            .AddUserSecrets(GetType().Assembly)
            .Build();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new NullLogger<ApplicationsService>());

        var project = await new ProjectFactory()
            .WithPositions(new List<Position>() { new PositionFactory().Build() })
            .SeedAsync();

        var contributor = await new ContributorFactory().SeedAsync();

        var application = await sut.SubmitAsync(
           contributor.Id,
           project.Id,
           project.Positions.ElementAt(0).Id);

        // ACT
        var submittingApplicationForTheSamePositionForTheSecondTime = async () => await sut.SubmitAsync(
            contributor.Id,
            project.Id,
            project.Positions.ElementAt(0).Id);


        // ASSERT
        await submittingApplicationForTheSamePositionForTheSecondTime.Should().ThrowAsync<InvalidOperationException>();
    }
}
