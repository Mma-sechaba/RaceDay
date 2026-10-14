using RaceDay.Api.Security;
using Xunit;

namespace RaceDay.Api.Tests;

public class PasswordServiceTests
{
    [Fact]
    public void HashDoesNotExposePassword_AndVerificationWorks()
    {
        const string password = "SecurePass123!";
        var hash = PasswordService.Hash(password);
        Assert.DoesNotContain(password, hash);
        Assert.True(PasswordService.Verify(password, hash));
        Assert.False(PasswordService.Verify("WrongPassword!", hash));
    }
}
