using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace edu4.Application.Tests;
internal class DbTestUtils
{
    private readonly IConfiguration _configuration;

    public DbTestUtils(IConfiguration configuration) => _configuration = configuration;

    public async Task CleanDatabaseAsync()
    {
        var clusterConnectionString = _configuration["MongoDb:ClusterConnectionString"];
        var dbName = _configuration["MongoDb:DbName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);

        await mongoDb.DropCollectionAsync(_configuration["MongoDb:UsersCollectionName"]);
    }
}
