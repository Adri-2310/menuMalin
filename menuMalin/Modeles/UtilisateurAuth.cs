namespace menuMalin.Modeles;

public class UtilisateurAuth
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public bool IsAuthenticated { get; set; }
    public string? Error { get; set; }
}
