namespace Peer.Tests.Application;

[CollectionDefinition("App services integration tests")]
public class AppServicesIntegrationTestsDefinition : ICollectionFixture<MongoDbClassMapRegistrationCollectionFixture>
{
}
