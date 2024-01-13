using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using Peer.Domain.Common;
using Peer.Domain.Contributors;
using Peer.Domain.Projects;
using Peer.Domain.Collaborations;
using Peer.Domain.Applications;
using Peer.Domain.Notifications;

namespace Peer.Infrastructure;

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

        BsonClassMap.RegisterClassMap<Contributor>(cm =>
        {
            cm.MapProperty(u => u.AccountId);
            cm.MapProperty(u => u.ContactEmail);
            cm.MapProperty(u => u.FullName);

            // Hats are mapped so they can be used in the creator map (below)
            // instead of breaking encapsulation;
            // as a consequence, both _hats and Hats are mapped and a part of documents
            cm.MapProperty(u => u.Hats);
            cm.MapField("_hats").SetElementName("_hats");

            cm.MapProperty(u => u.Removed);

            cm.MapCreator(
                u => new Contributor(u.AccountId, u.FullName, u.ContactEmail, u.Hats.ToList())
            );
        });

        BsonClassMap.RegisterClassMap<Project>(cm =>
        {
            cm.MapProperty(p => p.Title);
            cm.MapProperty(p => p.Description);
            cm.MapProperty(p => p.DatePosted);
            cm.MapProperty(p => p.AuthorId);

            // Mapping both for the creator map and not breaking encapsulation
            cm.MapProperty(p => p.Positions);
            cm.MapField("_positions").SetElementName("_positions");

            cm.MapProperty(p => p.Removed);

            cm.MapCreator(
                p =>
                    new Project(
                        p.Title,
                        p.Description,
                        p.AuthorId,
                        p.DatePosted,
                        p.Positions.ToList()
                    )
            );
        });

        BsonClassMap.RegisterClassMap<Position>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(p => p.DatePosted);
            cm.MapProperty(p => p.Name);
            cm.MapProperty(p => p.Description);
            cm.MapProperty(p => p.Requirements);
            cm.MapProperty(p => p.Open);
            cm.MapProperty(p => p.Removed);
        });

        BsonClassMap.RegisterClassMap<Domain.Applications.Application>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(cm => cm.ApplicantId);
            cm.MapProperty(cm => cm.ProjectId);
            cm.MapProperty(cm => cm.PositionId);
            cm.MapProperty(cm => cm.DateSubmitted);
            cm.MapProperty(cm => cm.Status)
                .SetSerializer(new EnumSerializer<ApplicationStatus>(BsonType.String));
        });

        BsonClassMap.RegisterClassMap<Collaboration>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(c => c.CollaboratorId);
            cm.MapProperty(c => c.ProjectId);
            cm.MapProperty(c => c.PositionId);
            cm.MapProperty(c => c.Status);
        });

        BsonClassMap.RegisterClassMap<AbstractDomainEvent>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(ade => ade.Processed);
            cm.MapProperty(ade => ade.TimeRaised);
        });

        BsonClassMap.RegisterClassMap<ApplicationAccepted>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(aa => aa.ApplicationId);
        });

        BsonClassMap.RegisterClassMap<ApplicationSubmitted>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(@as => @as.ApplicationId);
        });

        BsonClassMap.RegisterClassMap<ContributorRemoved>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(cr => cr.ContributorId);
        });

        BsonClassMap.RegisterClassMap<AbstractNotification>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(an => an.Processed);
        });

        BsonClassMap.RegisterClassMap<NewApplicationReceived>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(ar => ar.ApplicationId);
        });

        BsonClassMap.RegisterClassMap<OwnApplicationAccepted>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(aa => aa.ApplicationId);
        });
    }
}
