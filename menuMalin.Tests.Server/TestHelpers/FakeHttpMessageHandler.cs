using System.Net;

namespace menuMalin.Tests.Server.TestHelpers;

/// <summary>
/// Handler HTTP fictif pour tester sans vraie connexion Internet
/// Permet de simuler des réponses API sans appeler TheMealDB
/// </summary>
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private HttpStatusCode _statusCode = HttpStatusCode.OK;
    private string _responseContent = "{}";
    private Queue<(HttpStatusCode, string)> _responseQueue = new();
    public int CallCount { get; private set; }

    public void SetupResponse(HttpStatusCode statusCode, string content)
    {
        _statusCode = statusCode;
        _responseContent = content;
    }

    public void SetupSequence(HttpStatusCode status1, HttpStatusCode status2, string content)
    {
        _responseQueue.Clear();
        _responseQueue.Enqueue((status1, ""));
        _responseQueue.Enqueue((status2, content));
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        CallCount++;

        var (statusCode, content) = _responseQueue.Count > 0
            ? _responseQueue.Dequeue()
            : (_statusCode, _responseContent);

        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content)
        };

        return Task.FromResult(response);
    }
}
