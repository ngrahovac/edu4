namespace edu4.Domain.Users;

public class AcademicHat : Hat
{
    public string ResearchField { get; }

    public AcademicHat(string researchField) =>
        ResearchField = researchField;


    protected override bool EqualsCore(Hat other)
    {
        var otherAcademicHat = other as AcademicHat;

        return ResearchField.Equals(otherAcademicHat!.ResearchField, StringComparison.Ordinal);
    }

    protected override int GetHashCodeCore() => HashCode.Combine(ResearchField);
}
