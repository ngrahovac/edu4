using edu4.Infrastructure;

namespace edu4.Application.Tests;
internal class MongoDbClassMapRegistrationCollectionFixture
{
    public MongoDbClassMapRegistrationCollectionFixture() => MongoDBSetupUtils.RegisterClassMaps();
}
