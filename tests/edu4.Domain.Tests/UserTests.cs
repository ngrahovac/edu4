using System.Diagnostics.CodeAnalysis;
using edu4.Domain.Users;
using FluentAssertions;

namespace edu4.Domain.Tests;

[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores")]
public class UserTests
{
    [Fact]
    public void A_user_can_wear_multiple_distinct_hats()
    {
        var hat1 = new StudentHat("Computer Science", AcademicDegree.Masters);
        var hat2 = new AcademicHat("Computer Science");

        var createUser = () => new User(
            "google-auth|0",
            "John Doe",
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
            "John Doe",
            "example@mail.com",
            new List<Hat>() { hat1, hat2 });

        createUser.Should().Throw<InvalidOperationException>();
    }
}
