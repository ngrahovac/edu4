using System.Collections;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Peer.Domain.Contributors;

namespace Peer.Domain.Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class HatTests
{
    private class HatEqualityTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var sh1 = new StudentHat("Computer Science");
            var sh2 = new StudentHat("Computer Science");
            var sh3 = new StudentHat("Computer Science", AcademicDegree.Masters);
            var sh4 = new StudentHat("Electronics");

            yield return new object[] { sh1, sh1, true };
            yield return new object[] { sh1, sh2, true };
            yield return new object[] { sh1, sh3, false };
            yield return new object[] { sh1, sh4, false };

            var ah1 = new AcademicHat("Computer Science");
            var ah2 = new AcademicHat("Computer Science");
            var ah3 = new AcademicHat("Electronics");

            yield return new object[] { ah1, ah1, true };
            yield return new object[] { ah1, ah2, true };
            yield return new object[] { ah1, ah3, false };
            yield return new object[] { sh1, ah3, false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(HatEqualityTestData))]
    public void Only_hats_of_the_exact_same_type_and_matching_property_values_are_equal(Hat hat1, Hat hat2, bool equal)
    {
        hat1.Equals(hat2).Should().Be(equal);
    }


    private class StudentHatPositionFitTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var studentHat = new StudentHat("Software Engineering", AcademicDegree.Masters);

            var positionRequirements1 = new StudentHat("Software Engineering", AcademicDegree.Bachelors);
            var positionRequirements2 = new StudentHat("Software Engineering", AcademicDegree.Masters);
            var positionRequirements3 = new StudentHat("Software Engineering", AcademicDegree.Doctorate);
            var positionRequirements4 = new StudentHat("Computer Science", AcademicDegree.Bachelors);
            var positionRequirements5 = new StudentHat("Computer Science", AcademicDegree.Masters);
            var positionRequirements6 = new StudentHat("Computer Science", AcademicDegree.Doctorate);
            var positionRequirements7 = new AcademicHat("Computer Science");

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
            var academicHat = new AcademicHat("Software Engineering");

            var positionRequirements1 = new AcademicHat("Software Engineering");
            var positionRequirements2 = new AcademicHat("Neural Networks");
            var positionRequirements3 = new StudentHat("Software Engineering", AcademicDegree.Bachelors);

            yield return new object[] { academicHat, positionRequirements1, true };
            yield return new object[] { academicHat, positionRequirements2, false };
            yield return new object[] { academicHat, positionRequirements3, false };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Theory]
    [ClassData(typeof(AcademicHatPositionFitTestData))]
    public void Academic_hat_fits_only_the_academic_position_requirements_with_the_same_research_field(
        AcademicHat studentHat, Hat positionRequirements, bool fits)
    {
        studentHat.Fits(positionRequirements).Should().Be(fits);
    }



}
