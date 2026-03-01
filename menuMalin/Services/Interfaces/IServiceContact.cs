namespace menuMalin.Services;

public interface IServiceContact
{
    Task<bool> SendMessageAsync(string email, string? name, string subject, string message, bool subscribeNewsletter);
}
