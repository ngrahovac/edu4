using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Peer.Infrastructure;
public class DbUtils
{
    private readonly IConfiguration _configuration;

    public DbUtils(IConfiguration configuration) => _configuration = configuration;

    public async Task CleanDatabaseAsync()
    {
        var clusterConnectionString = _configuration["MongoDb:ClusterConnectionString"];
        var dbName = _configuration["MongoDb:DbName"];

        var mongoDb = new MongoClient(clusterConnectionString).GetDatabase(dbName);

        await mongoDb.DropCollectionAsync(_configuration["MongoDb:UsersCollectionName"]);
        await mongoDb.DropCollectionAsync(_configuration["MongoDb:ProjectsCollectionName"]);
        await mongoDb.DropCollectionAsync(_configuration["MongoDb:ApplicationsCollectionName"]);
        await mongoDb.DropCollectionAsync(_configuration["MongoDb:CollaborationsCollectionName"]);
        await mongoDb.DropCollectionAsync(_configuration["MongoDb:DomainEventsCollectionName"]);
    }
}
