using Peer.Domain.Contributors;

namespace Peer.Tests.Utils.Factories;

public abstract class HatFactory
{
    public static HatFactory OfType(HatType type)
    {
        return type switch
        {
            HatType.Student => new StudentHatFactory(),
            HatType.Academic => new AcademicHatFactory(),
            _ => throw new NotImplementedException("Creating a test hat of the given type is not yet implemented")
        };
    }
    public abstract HatFactory WithStudyField(string studyField);
    public abstract HatFactory WithAcademicDegree(AcademicDegree academicDegree);
    public abstract HatFactory WithResearchField(string researchField);
    public abstract Hat Build();

    private class StudentHatFactory : HatFactory
    {
        private string _studyField = "Software Engineering";
        private AcademicDegree _academicDegree = AcademicDegree.Bachelors;

        public override HatFactory WithAcademicDegree(AcademicDegree academicDegree)
        {
            _academicDegree = academicDegree;
            return this;
        }

        public override HatFactory WithResearchField(string researchField) =>
            throw new InvalidOperationException("The parameter is not supported fot the given hat type");

        public override HatFactory WithStudyField(string studyField)
        {
            _studyField = studyField;
            return this;
        }

        public override Hat Build() => new StudentHat(_studyField, _academicDegree);
    }

    private class AcademicHatFactory : HatFactory
    {
        private string _researchField = "Computer Science";

        public override HatFactory WithAcademicDegree(AcademicDegree academicDegree) =>
            throw new InvalidOperationException("The parameter is not supported fot the given hat type");

        public override HatFactory WithResearchField(string researchField)
        {
            _researchField = researchField;
            return this;
        }

        public override HatFactory WithStudyField(string studyField) =>
            throw new InvalidOperationException("The parameter is not supported fot the given hat type");

        public override Hat Build() => new AcademicHat(_researchField);
    }
}
