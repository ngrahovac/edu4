using System.Collections;
using System.Diagnostics.CodeAnalysis;
using edu4.Domain.Users;
using FluentAssertions;

namespace edu4.Domain.Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class UserTests
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

    [Fact]
    public void A_user_can_wear_multiple_distinct_hats()
    {
        var hat1 = new StudentHat("Computer Science", AcademicDegree.Masters);
        var hat2 = new AcademicHat("Computer Science");

        var createUser = () => new User(
            "google-auth|0",
            "example@mail.com",
            new List<Hat>() { hat1, hat2 });

        createUser.Should().NotThrow();
        var createdUser = createUser();
        createdUser.Hats.Should().BeEquivalentTo(new List<Hat>() { hat2, hat1 });
    }

    [Fact]
    public void A_user_cannot_wear_duplicate_hats()
    {
        var hat1 = new StudentHat("Computer Science");
        var hat2 = new StudentHat("Computer Science");

        var createUser = () => new User(
            "google-auth|0",
            "example@mail.com",
            new List<Hat>() { hat1, hat2 });

        createUser.Should().Throw<InvalidOperationException>();
    }
}
