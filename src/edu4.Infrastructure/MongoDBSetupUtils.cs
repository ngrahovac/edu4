using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using edu4.Domain.Common;
using edu4.Domain.Users;
using edu4.Domain.Projects;

namespace edu4.Infrastructure;
public class MongoDBSetupUtils
{
    public static void RegisterClassMaps()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterIdGenerator(typeof(Guid), GuidGenerator.Instance);

        BsonClassMap.RegisterClassMap<AbstractEntity>(cm =>
        {
            cm.AutoMap();
            cm.MapIdProperty(ae => ae.Id);
        });

        BsonClassMap.RegisterClassMap<Hat>(cm => cm.AutoMap());

        BsonClassMap.RegisterClassMap<StudentHat>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(sh => sh.StudyField);
            cm.MapProperty(sh => sh.AcademicDegree)
            .SetSerializer(new EnumSerializer<AcademicDegree>(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<AcademicHat>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(ah => ah.ResearchField);
        });

        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.MapProperty(u => u.AccountId);
            cm.MapProperty(u => u.ContactEmail);
            cm.MapProperty(u => u.FullName);

            // Hats are mapped so they can be used in the creator map (below)
            // instead of breaking encapsulation;
            // as a consequence, both _hats and Hats are mapped and a part of documents
            cm.MapProperty(u => u.Hats);
            cm.MapField("_hats").SetElementName("_hats");
            cm.MapCreator(u => new User(
                u.AccountId,
                u.FullName,
                u.ContactEmail,
                u.Hats.ToList()));
        });

        BsonClassMap.RegisterClassMap<Project>(cm =>
        {
            cm.MapProperty(p => p.Title);
            cm.MapProperty(p => p.Description);
            cm.MapProperty(p => p.DatePosted);
            cm.MapProperty(p => p.Author);

            // Mapping both for the creator map and not breaking encapsulation
            cm.MapProperty(p => p.Positions);
            cm.MapField("_positions").SetElementName("_positions");

            cm.MapCreator(p => new Project(
                p.Title,
                p.Description,
                p.Author,
                p.Positions.ToList()));
        });

        BsonClassMap.RegisterClassMap<Position>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(p => p.DatePosted);
            cm.MapProperty(p => p.Name);
            cm.MapProperty(p => p.Description);
            cm.MapProperty(p => p.Requirements);
        });
    }
}
