namespace Peer.Tests.Application.External;

[CollectionDefinition("External services integration tests")]
public class ExternalServicesTestsDefinition : ICollectionFixture<MongoDbClassMapRegistrationCollectionFixture>
{
}
