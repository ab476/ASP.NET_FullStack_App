using AuthAPI.Data.Models;

namespace AuthAPI.Services;

public class EmailSender : IEmailSender<TUser>
{
    public Task SendConfirmationLinkAsync(TUser user, string email, string confirmationLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetCodeAsync(TUser user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(TUser user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }
}
