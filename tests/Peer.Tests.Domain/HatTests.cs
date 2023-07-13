using System.Collections;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Peer.Domain.Contributors;
using Peer.Tests.Utils.Factories;

namespace Peer.Tests.Domain;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class HatTests
{
    private class HatEqualityTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var sh1 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .Build();

            var sh2 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .Build();

            var sh3 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Masters)
                .Build();

            var sh4 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Electronics")
                .Build();

            yield return new object[] { sh1, sh1, true };
            yield return new object[] { sh1, sh2, true };
            yield return new object[] { sh1, sh3, false };
            yield return new object[] { sh1, sh4, false };

            var ah1 = HatsFactory.OfType(HatType.Academic)
                .WithResearchField("Computer Science")
                .Build();

            var ah2 = HatsFactory.OfType(HatType.Academic)
                .WithResearchField("Computer Science")
                .Build();

            var ah3 = HatsFactory.OfType(HatType.Academic)
                .WithResearchField("Electronics")
                .Build();

            yield return new object[] { ah1, ah1, true };
            yield return new object[] { ah1, ah2, true };
            yield return new object[] { ah1, ah3, false };
            yield return new object[] { sh1, ah3, false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(HatEqualityTestData))]
    public void Only_the_hats_of_the_same_type_and_matching_property_values_are_equal(Hat hat1, Hat hat2, bool equal)
    {
        hat1.Equals(hat2).Should().Be(equal);
    }


    private class StudentHatPositionFitTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var studentHat = new StudentHat("Software Engineering", AcademicDegree.Masters);

            var positionRequirements1 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Software Engineering")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build();

            var positionRequirements2 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Software Engineering")
                .WithAcademicDegree(AcademicDegree.Masters)
                .Build();

            var positionRequirements3 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Software Engineering")
                .WithAcademicDegree(AcademicDegree.Doctorate)
                .Build();

            var positionRequirements4 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Bachelors)
                .Build();

            var positionRequirements5 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Masters)
                .Build();

            var positionRequirements6 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Computer Science")
                .WithAcademicDegree(AcademicDegree.Doctorate)
                .Build();

            var positionRequirements7 = HatsFactory.OfType(HatType.Academic)
                .WithResearchField("Computer Science")
                .Build();

            yield return new object[] { studentHat, positionRequirements1, true };
            yield return new object[] { studentHat, positionRequirements2, true };
            yield return new object[] { studentHat, positionRequirements3, false };
            yield return new object[] { studentHat, positionRequirements4, false };
            yield return new object[] { studentHat, positionRequirements5, false };
            yield return new object[] { studentHat, positionRequirements6, false };
            yield return new object[] { studentHat, positionRequirements7, false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(StudentHatPositionFitTestData))]
    public void Student_hat_fits_only_the_student_position_requirements_with_the_same_study_field_and_equal_or_greater_academic_degree(
        StudentHat studentHat, Hat positionRequirements, bool fits)
    {
        studentHat.Fits(positionRequirements).Should().Be(fits);
    }


    private class AcademicHatPositionFitTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var academicHat = HatsFactory.OfType(HatType.Academic)
                .WithResearchField("Software Engineering")
                .Build();

            var positionRequirements1 = HatsFactory.OfType(HatType.Academic)
                .WithResearchField("Software Engineering")
                .Build();

            var positionRequirements2 = HatsFactory.OfType(HatType.Academic)
                .WithResearchField("Neural Networks")
                .Build();

            var positionRequirements3 = HatsFactory.OfType(HatType.Student)
                .WithStudyField("Software Engineering")
                .Build();

            yield return new object[] { academicHat, positionRequirements1, true };
            yield return new object[] { academicHat, positionRequirements2, false };
            yield return new object[] { academicHat, positionRequirements3, false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(AcademicHatPositionFitTestData))]
    public void Academic_hat_fits_only_the_academic_position_requirements_with_the_same_research_field(
        AcademicHat academicHat, Hat positionRequirements, bool fits)
    {
        academicHat.Fits(positionRequirements).Should().Be(fits);
    }
}
