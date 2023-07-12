using Peer.Domain.Contributors;

namespace Peer.Tests.Utils.Factories;

public abstract class HatsFactory
{
    public static HatsFactory OfType(HatType type)
    {
        return type switch
        {
            HatType.Student => new StudentHatFactory(),
            HatType.Academic => new AcademicHatFactory(),
            _ => throw new NotImplementedException("Creating a test hat of the given type is not yet implemented")
        };
    }
    public abstract HatsFactory WithStudyField(string studyField);
    public abstract HatsFactory WithAcademicDegree(AcademicDegree academicDegree);
    public abstract HatsFactory WithResearchField(string researchField);
    public abstract Hat Build();

    private class StudentHatFactory : HatsFactory
    {
        private string _studyField = "Software Engineering";
        private AcademicDegree _academicDegree = AcademicDegree.Bachelors;

        public override HatsFactory WithAcademicDegree(AcademicDegree academicDegree)
        {
            _academicDegree = academicDegree;
            return this;
        }

        public override HatsFactory WithResearchField(string researchField) =>
            throw new InvalidOperationException("The parameter is not supported fot the given hat type");

        public override HatsFactory WithStudyField(string studyField)
        {
            _studyField = studyField;
            return this;
        }

        public override Hat Build() => new StudentHat(_studyField, _academicDegree);
    }

    private class AcademicHatFactory : HatsFactory
    {
        private string _researchField = "Computer Science";

        public override HatsFactory WithAcademicDegree(AcademicDegree academicDegree) =>
            throw new InvalidOperationException("The parameter is not supported fot the given hat type");

        public override HatsFactory WithResearchField(string researchField)
        {
            _researchField = researchField;
            return this;
        }

        public override HatsFactory WithStudyField(string studyField) =>
            throw new InvalidOperationException("The parameter is not supported fot the given hat type");

        public override Hat Build() => new AcademicHat(_researchField);
    }
}
