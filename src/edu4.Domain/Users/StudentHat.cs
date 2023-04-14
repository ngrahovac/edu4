namespace edu4.Domain.Users;
public class StudentHat : Hat
{
    public override HatType Type => HatType.Student;

    public string StudyField { get; }

    public AcademicDegree AcademicDegree { get; }


    public StudentHat(
        string studyField,
        AcademicDegree academicDegree = AcademicDegree.Bachelors)
    {
        StudyField = studyField;
        AcademicDegree = academicDegree;
    }

    protected override bool EqualsCore(Hat other)
    {
        var otherStudentHat = other as StudentHat;

        return
            StudyField.Equals(otherStudentHat!.StudyField, StringComparison.Ordinal) &&
            AcademicDegree.Equals(otherStudentHat.AcademicDegree);
    }

    protected override int GetHashCodeCore() => HashCode.Combine(StudyField, AcademicDegree);

    public override bool Fits(Hat positionRequirements) =>
        positionRequirements is StudentHat studentRequirements &&
        StudyField.Equals(studentRequirements.StudyField, StringComparison.Ordinal) &&
        AcademicDegree >= studentRequirements.AcademicDegree;

}
