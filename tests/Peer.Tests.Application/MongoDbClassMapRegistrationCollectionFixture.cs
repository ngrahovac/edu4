using Peer.Infrastructure;

namespace Peer.Tests.Application;
internal class MongoDbClassMapRegistrationCollectionFixture
{
    public MongoDbClassMapRegistrationCollectionFixture() => MongoDBSetupUtils.RegisterClassMaps();
}
