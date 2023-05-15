namespace edu4.Domain.Contributors;

public class AcademicHat : Hat
{
    public override HatType Type => HatType.Academic;

    public string ResearchField { get; }

    public AcademicHat(string researchField) =>
        ResearchField = researchField;


    protected override bool EqualsCore(Hat other)
    {
        var otherAcademicHat = other as AcademicHat;

        return ResearchField.Equals(otherAcademicHat!.ResearchField, StringComparison.Ordinal);
    }

    protected override int GetHashCodeCore()
        => HashCode.Combine(ResearchField);

    public override bool Fits(Hat positionRequirements) =>
        positionRequirements is AcademicHat academicRequirements &&
        ResearchField.Equals(academicRequirements.ResearchField, StringComparison.OrdinalIgnoreCase);
}
