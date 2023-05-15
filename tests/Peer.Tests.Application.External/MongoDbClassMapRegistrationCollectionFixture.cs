using Peer.Infrastructure;

namespace Peer.Tests.Application.External;
internal class MongoDbClassMapRegistrationCollectionFixture
{
    public MongoDbClassMapRegistrationCollectionFixture() => MongoDBSetupUtils.RegisterClassMaps();
}
