namespace menuMalin.Services;

/// <summary>
/// Service pour gérer les messages de contact
/// </summary>
public class ContactService : IContactService
{
    private readonly IHttpApiService _httpApiService;

    public ContactService(IHttpApiService httpApiService)
    {
        _httpApiService = httpApiService;
    }

    public async Task<bool> SendMessageAsync(string email, string subject, string message)
    {
        try
        {
            var request = new
            {
                email,
                subject,
                message
            };

            var result = await _httpApiService.PostAsync<object>("contact", request);
            return result != null;
        }
        catch (Exception ex)
        {
            // Logged in development
            return false;
        }
    }
}
