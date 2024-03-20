using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Peer.Application.Services;
using Peer.Domain.Applications;
using Peer.Domain.Projects;
using Peer.Infrastructure;
using Peer.Tests.Utils.Seeders;
using Peer.Tests.Utils.Factories;

namespace Peer.Tests.Application;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
[Collection("App services integration tests")]
public class ApplicationsServiceTests
{
    [Fact]
    public async void A_contributor_can_apply_for_a_position_on_a_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var contributorToApply = await new ContributorsSeeder(config).SeedAsync();

        // ACT
        var application = await sut.SubmitAsync(
            contributorToApply.Id,
            project.Id,
            project.Positions.ElementAt(0).Id
        );

        // ASSERT
        var retrievedApplication = await applications.GetByApplicantAndPositionAsync(
            contributorToApply.Id,
            project.Positions.ElementAt(0).Id
        );

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
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();
        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        // ACT
        var applyingForAPositionOnOwnProject = async () =>
            await sut.SubmitAsync(author.Id, project.Id, project.Positions.ElementAt(0).Id);

        // ASSERT
        await applyingForAPositionOnOwnProject.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_cant_apply_for_a_position_on_a_non_existing_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var contributor = await new ContributorsSeeder(config).SeedAsync();

        // ACT
        var applyingForAPositionOnANonExistantProject = async () =>
            await sut.SubmitAsync(contributor.Id, Guid.NewGuid(), Guid.NewGuid());

        // ASSERT
        await applyingForAPositionOnANonExistantProject
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_cant_apply_for_a_non_existing_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var contributor = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        // ACT
        var applyingForANonExistantPosition = async () =>
            await sut.SubmitAsync(contributor.Id, project.Id, Guid.NewGuid());

        // ASSERT
        await applyingForANonExistantPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_cant_apply_for_a_position_they_already_applied_for()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var contributor = await new ContributorsSeeder(config).SeedAsync();

        var application = await sut.SubmitAsync(
            contributor.Id,
            project.Id,
            project.Positions.ElementAt(0).Id
        );

        // ACT
        var submittingApplicationForTheSamePositionForTheSecondTime = async () =>
            await sut.SubmitAsync(contributor.Id, project.Id, project.Positions.ElementAt(0).Id);

        // ASSERT
        await submittingApplicationForTheSamePositionForTheSecondTime
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_can_reapply_for_a_position_after_revoking_the_first_application()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var contributor = await new ContributorsSeeder(config).SeedAsync();

        var submittedApplication = await sut.SubmitAsync(
            contributor.Id,
            project.Id,
            project.Positions.ElementAt(0).Id
        );

        await sut.RevokeAsync(contributor.Id, submittedApplication.Id);

        // ACT
        var submittingApplicationForTheSamePositionForTheSecondTime = async () =>
            await sut.SubmitAsync(contributor.Id, project.Id, project.Positions.ElementAt(0).Id);

        // ASSERT
        await submittingApplicationForTheSamePositionForTheSecondTime.Should().NotThrowAsync();
    }

    [Fact]
    public async void An_applicant_can_revoke_own_submitted_application()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var applicant = await new ContributorsSeeder(config).SeedAsync();

        var submittedApplication = await sut.SubmitAsync(
            applicant.Id,
            project.Id,
            project.Positions.ElementAt(0).Id
        );

        // ACT
        await sut.RevokeAsync(applicant.Id, submittedApplication.Id);

        // ASSERT
        var retrievedApplication = await applications.GetByIdAsync(submittedApplication.Id);
        retrievedApplication.Status.Should().Be(ApplicationStatus.Revoked);
    }

    [Fact]
    public async void A_contributor_cannot_revoke_an_application_submitted_by_another()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var applicant = await new ContributorsSeeder(config).SeedAsync();

        var submittedApplication = await sut.SubmitAsync(
            applicant.Id,
            project.Id,
            project.Positions.ElementAt(0).Id
        );

        var requester = await new ContributorsSeeder(config).SeedAsync();

        // ACT
        var revokingAnothersApplication = async () =>
            await sut.RevokeAsync(requester.Id, submittedApplication.Id);

        // ASSERT
        await revokingAnothersApplication.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void An_applicant_cannot_revoke_own_application_twice()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var applicant = await new ContributorsSeeder(config).SeedAsync();

        var submittedApplication = await sut.SubmitAsync(
            applicant.Id,
            project.Id,
            project.Positions.ElementAt(0).Id
        );

        await sut.RevokeAsync(applicant.Id, submittedApplication.Id);

        // ACT
        var revokingOwnApplicationForTheSecondTime = async () =>
            await sut.RevokeAsync(applicant.Id, submittedApplication.Id);

        // ASSERT
        await revokingOwnApplicationForTheSecondTime
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void Project_author_can_accept_a_submitted_application()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var domainEvents = new MongoDbDomainEventsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            domainEvents,
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var application = await new ApplicationsSeeder(config)
            .WithProjectId(project.Id)
            .WithPositionId(project.Positions.ElementAt(0).Id)
            .SeedAsync();

        // ACT
        await sut.AcceptAsync(project.AuthorId, application.Id);

        // ASSERT
        var retrievedApplication = await applications.GetByIdAsync(application.Id);
        retrievedApplication.Status.Should().Be(ApplicationStatus.Accepted);

        var retrievedDomainEvents = await domainEvents.GetUnprocessedBatchAsync(5);
        retrievedDomainEvents.Count.Should().Be(1);
        retrievedDomainEvents[0].Should().BeOfType<ApplicationAccepted>();
    }

    [Fact]
    public async void Application_cannot_be_accepted_by_a_contributor_that_is_not_the_project_author()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var domainEvents = new MongoDbDomainEventsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            domainEvents,
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var requester = await new ContributorsSeeder(config).SeedAsync();

        var application = await new ApplicationsSeeder(config)
            .WithProjectId(project.Id)
            .WithPositionId(project.Positions.ElementAt(0).Id)
            .SeedAsync();

        // ACT
        var acceptingApplicationForAProjectAuthoredByAnother = async () =>
            await sut.AcceptAsync(requester.Id, application.Id);

        // ASSERT
        await acceptingApplicationForAProjectAuthoredByAnother
            .Should()
            .ThrowAsync<InvalidOperationException>();

        var retrievedDomainEvents = await domainEvents.GetUnprocessedBatchAsync(5);
        retrievedDomainEvents.Count.Should().Be(0);
    }

    [Fact]
    public async void Project_author_cannot_accept_the_same_application_twice()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var domainEvents = new MongoDbDomainEventsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            domainEvents,
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var application = await new ApplicationsSeeder(config)
            .WithProjectId(project.Id)
            .WithPositionId(project.Positions.ElementAt(0).Id)
            .SeedAsync();

        await sut.AcceptAsync(project.AuthorId, application.Id);

        // ACT
        var acceptingTheSameApplicationForTheSecondTime = async () =>
            await sut.AcceptAsync(project.AuthorId, application.Id);

        // ASSERT
        await acceptingTheSameApplicationForTheSecondTime
            .Should()
            .ThrowAsync<InvalidOperationException>();

        var retrievedDomainEvents = await domainEvents.GetUnprocessedBatchAsync(5);
        retrievedDomainEvents.Count.Should().Be(1);
    }

    [Fact]
    public async void Project_author_can_reject_a_submitted_application()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var application = await new ApplicationsSeeder(config)
            .WithProjectId(project.Id)
            .WithPositionId(project.Positions.ElementAt(0).Id)
            .SeedAsync();

        // ACT
        await sut.RejectAsync(project.AuthorId, application.Id);

        // ASSERT
        var retrievedApplication = await applications.GetByIdAsync(application.Id);
        retrievedApplication.Status.Should().Be(ApplicationStatus.Rejected);
    }

    [Fact]
    public async void Application_cannot_be_rejected_by_a_contributor_that_is_not_the_project_author()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var requester = await new ContributorsSeeder(config).SeedAsync();

        var application = await new ApplicationsSeeder(config)
            .WithProjectId(project.Id)
            .WithPositionId(project.Positions.ElementAt(0).Id)
            .SeedAsync();

        // ACT
        var rejectingApplicationForAProjectAuthoredByAnother = async () =>
            await sut.RejectAsync(requester.Id, application.Id);

        // ASSERT
        await rejectingApplicationForAProjectAuthoredByAnother
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void Project_author_cannot_reject_the_same_application_twice()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var application = await new ApplicationsSeeder(config)
            .WithProjectId(project.Id)
            .WithPositionId(project.Positions.ElementAt(0).Id)
            .SeedAsync();

        await sut.RejectAsync(project.AuthorId, application.Id);

        // ACT
        var rejectingTheSameApplicationTwice = async () =>
            await sut.RejectAsync(project.AuthorId, application.Id);

        // ASSERT
        await rejectingTheSameApplicationTwice.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void Project_author_cannot_reject_an_accepted_application()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();

        var project = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(new List<Position>() { new PositionsFactory().Build() })
            .SeedAsync();

        var application = await new ApplicationsSeeder(config)
            .WithProjectId(project.Id)
            .WithPositionId(project.Positions.ElementAt(0).Id)
            .SeedAsync();

        await sut.AcceptAsync(author.Id, application.Id);

        // ACT
        var rejectingAnAcceptedApplication = async () =>
            await sut.RejectAsync(project.AuthorId, application.Id);

        // ASSERT
        await rejectingAnAcceptedApplication.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void A_contributor_cannot_submit_an_application_for_a_closed_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);
        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var project = await new ProjectsSeeder(config)
            .WithPositions(new List<Position>() { new PositionsFactory().WithOpen(false).Build() })
            .SeedAsync();

        var contributorToApply = await new ContributorsSeeder(config).SeedAsync();

        // ACT
        var applyingForAClosedPosition = async () =>
            await sut.SubmitAsync(
                contributorToApply.Id,
                project.Id,
                project.Positions.ElementAt(0).Id
            );

        // ASSERT
        await applyingForAClosedPosition.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async void Retrieving_received_applications_without_specifying_search_refinements_should_return_all_submitted_applications_for_positions_on_authored_projects()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();
        var applicant1 = await new ContributorsSeeder(config).SeedAsync();
        var applicant2 = await new ContributorsSeeder(config).SeedAsync();
        var applicant3 = await new ContributorsSeeder(config).SeedAsync();
        var applicant4 = await new ContributorsSeeder(config).SeedAsync();
        var applicant5 = await new ContributorsSeeder(config).SeedAsync();
        var applicant6 = await new ContributorsSeeder(config).SeedAsync();

        var authoredProject1 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var authoredProject2 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var submittedApplication1 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant1.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication2 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant2.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(1).Id)
            .SeedAsync();

        var submittedApplication3 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant3.Id)
            .WithProjectId(authoredProject2.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var revokedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant4.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Revoked)
            .SeedAsync();

        var rejectedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant5.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Rejected)
            .SeedAsync();

        var acceptedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant6.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Accepted)
            .SeedAsync();

        // ACT
        var receivedApplications = await sut.GetReceivedAsync(
            author.Id,
            projectId: null,
            positionId: null,
            applicationsSortOption: ApplicationsSortOption.Default
        );

        // ASSERT
        receivedApplications.Count.Should().Be(3);
    }

    [Fact]
    public async void Retrieving_received_applications_for_selected_project_should_return_all_submitted_applications_for_that_project()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();
        var applicant1 = await new ContributorsSeeder(config).SeedAsync();
        var applicant2 = await new ContributorsSeeder(config).SeedAsync();
        var applicant3 = await new ContributorsSeeder(config).SeedAsync();
        var applicant4 = await new ContributorsSeeder(config).SeedAsync();
        var applicant5 = await new ContributorsSeeder(config).SeedAsync();
        var applicant6 = await new ContributorsSeeder(config).SeedAsync();

        var authoredProject1 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var authoredProject2 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var submittedApplication1 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant1.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication2 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant2.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(1).Id)
            .SeedAsync();

        var submittedApplication3 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant3.Id)
            .WithProjectId(authoredProject2.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var revokedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant4.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Revoked)
            .SeedAsync();

        var rejectedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant5.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Rejected)
            .SeedAsync();

        var acceptedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant6.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Accepted)
            .SeedAsync();

        // ACT
        var receivedApplications = await sut.GetReceivedAsync(
            author.Id,
            projectId: authoredProject1.Id,
            positionId: null,
            applicationsSortOption: ApplicationsSortOption.Default
        );

        // ASSERT
        receivedApplications.Count.Should().Be(2);
    }

    [Fact]
    public async void Retrieving_received_applications_for_selected_project_and_position_should_return_all_submitted_applications_for_that_project_position()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();
        var applicant1 = await new ContributorsSeeder(config).SeedAsync();
        var applicant2 = await new ContributorsSeeder(config).SeedAsync();
        var applicant3 = await new ContributorsSeeder(config).SeedAsync();
        var applicant4 = await new ContributorsSeeder(config).SeedAsync();
        var applicant5 = await new ContributorsSeeder(config).SeedAsync();
        var applicant6 = await new ContributorsSeeder(config).SeedAsync();

        var authoredProject1 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var authoredProject2 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var submittedApplication1 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant1.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication2 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant2.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(1).Id)
            .SeedAsync();

        var submittedApplication3 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant3.Id)
            .WithProjectId(authoredProject2.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var revokedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant4.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Revoked)
            .SeedAsync();

        var rejectedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant5.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Rejected)
            .SeedAsync();

        var acceptedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant6.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Accepted)
            .SeedAsync();

        // ACT
        var receivedApplications = await sut.GetReceivedAsync(
            author.Id,
            projectId: authoredProject1.Id,
            positionId: authoredProject1.Positions.ElementAt(1).Id,
            applicationsSortOption: ApplicationsSortOption.Default
        );

        // ASSERT
        receivedApplications.Count.Should().Be(1);
    }

    [Fact]
    public async void Retrieving_received_applications_for_selected_project_and_position_by_newest_first_should_return_all_submitted_applications_for_that_project_position_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();
        var applicant1 = await new ContributorsSeeder(config).SeedAsync();
        var applicant2 = await new ContributorsSeeder(config).SeedAsync();
        var applicant3 = await new ContributorsSeeder(config).SeedAsync();
        var applicant4 = await new ContributorsSeeder(config).SeedAsync();
        var applicant5 = await new ContributorsSeeder(config).SeedAsync();
        var applicant6 = await new ContributorsSeeder(config).SeedAsync();

        var authoredProject1 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var submittedApplication1 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant1.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication2 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant2.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication3 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant3.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(1).Id)
            .SeedAsync();

        var revokedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant4.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Revoked)
            .SeedAsync();

        var rejectedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant5.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Rejected)
            .SeedAsync();

        var acceptedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant6.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Accepted)
            .SeedAsync();

        // ACT
        var receivedApplications = await sut.GetReceivedAsync(
            author.Id,
            projectId: authoredProject1.Id,
            positionId: authoredProject1.Positions.ElementAt(0).Id,
            applicationsSortOption: ApplicationsSortOption.NewestFirst
        );

        // ASSERT
        receivedApplications.Count.Should().Be(2);
        receivedApplications
            .Should()
            .BeEquivalentTo(
                new List<Domain.Applications.Application>()
                {
                    submittedApplication1,
                    submittedApplication2
                }
                    .OrderByDescending(a => a.DateSubmitted)
                    .ToList(),
                b => b.WithStrictOrdering()
            );
    }

    [Fact]
    public async void Retrieving_received_applications_for_selected_project_and_position_by_oldest_first_should_return_all_submitted_applications_for_that_project_position_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var author = await new ContributorsSeeder(config).SeedAsync();
        var applicant1 = await new ContributorsSeeder(config).SeedAsync();
        var applicant2 = await new ContributorsSeeder(config).SeedAsync();
        var applicant3 = await new ContributorsSeeder(config).SeedAsync();
        var applicant4 = await new ContributorsSeeder(config).SeedAsync();
        var applicant5 = await new ContributorsSeeder(config).SeedAsync();
        var applicant6 = await new ContributorsSeeder(config).SeedAsync();

        var authoredProject1 = await new ProjectsSeeder(config)
            .WithAuthorId(author.Id)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var submittedApplication1 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant1.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication2 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant2.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication3 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant3.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(1).Id)
            .SeedAsync();

        var revokedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant4.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Revoked)
            .SeedAsync();

        var rejectedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant5.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Rejected)
            .SeedAsync();

        var acceptedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant6.Id)
            .WithProjectId(authoredProject1.Id)
            .WithPositionId(authoredProject1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Accepted)
            .SeedAsync();

        // ACT
        var receivedApplications = await sut.GetReceivedAsync(
            author.Id,
            projectId: authoredProject1.Id,
            positionId: authoredProject1.Positions.ElementAt(0).Id,
            applicationsSortOption: ApplicationsSortOption.OldestFirst
        );

        // ASSERT
        receivedApplications.Count.Should().Be(2);
        receivedApplications
            .Should()
            .BeEquivalentTo(
                new List<Domain.Applications.Application>()
                {
                    submittedApplication1,
                    submittedApplication2
                }
                    .OrderBy(a => a.DateSubmitted)
                    .ToList(),
                b => b.WithStrictOrdering()
            );
    }

    [Fact]
    public async void Retrieving_sent_applications_without_specifying_search_refinements_should_return_all_applications_submitted_by_the_applicant()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var applicant = await new ContributorsSeeder(config).SeedAsync();

        var project1 = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var project2 = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var submittedApplication1 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication2 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(1).Id)
            .SeedAsync();

        var submittedApplication3 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project2.Id)
            .WithPositionId(project2.Positions.ElementAt(0).Id)
            .SeedAsync();

        var revokedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Revoked)
            .SeedAsync();

        var rejectedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Rejected)
            .SeedAsync();

        var acceptedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Accepted)
            .SeedAsync();

        // ACT
        var receivedApplications = await sut.GetSentAsync(
            applicant.Id,
            projectId: null,
            positionId: null,
            applicationsSortOption: ApplicationsSortOption.Default
        );

        // ASSERT
        receivedApplications.Items.Count.Should().Be(3);
    }

    [Fact]
    public async void Retrieving_sent_applications_for_selected_project_by_oldest_first_should_return_all_submitted_applications_for_that_project_position_as_a_sorted_collection()
    {
        // ARRANGE
        var config = new ConfigurationBuilder().AddUserSecrets(GetType().Assembly).Build();

        await new DbUtils(config).CleanDatabaseAsync();

        var applications = new MongoDbApplicationsRepository(config);

        var sut = new ApplicationsService(
            applications,
            new MongoDbContributorsRepository(config),
            new MongoDBProjectsRepository(config),
            new MongoDbDomainEventsRepository(config),
            new NullLogger<ApplicationsService>()
        );

        var applicant = await new ContributorsSeeder(config).SeedAsync();

        var project1 = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var project2 = await new ProjectsSeeder(config)
            .WithPositions(
                new List<Position>()
                {
                    new PositionsFactory().Build(),
                    new PositionsFactory().Build()
                }
            )
            .SeedAsync();

        var submittedApplication1 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication2 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project2.Id)
            .WithPositionId(project2.Positions.ElementAt(0).Id)
            .SeedAsync();

        var submittedApplication3 = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(1).Id)
            .SeedAsync();

        var revokedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Revoked)
            .SeedAsync();

        var rejectedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Rejected)
            .SeedAsync();

        var acceptedApplication = await new ApplicationsSeeder(config)
            .WithApplicantId(applicant.Id)
            .WithProjectId(project1.Id)
            .WithPositionId(project1.Positions.ElementAt(0).Id)
            .WithStatus(ApplicationStatus.Accepted)
            .SeedAsync();

        // ACT
        var receivedApplications = await sut.GetSentAsync(
            applicant.Id,
            projectId: project1.Id,
            positionId: null,
            applicationsSortOption: ApplicationsSortOption.OldestFirst
        );

        // ASSERT
        receivedApplications.Items.Count.Should().Be(2);
        receivedApplications
            .Should()
            .BeEquivalentTo(
                new List<Domain.Applications.Application>()
                {
                    submittedApplication1,
                    submittedApplication3
                }
                    .OrderBy(a => a.DateSubmitted)
                    .ToList(),
                b => b.WithStrictOrdering()
            );
    }
}
