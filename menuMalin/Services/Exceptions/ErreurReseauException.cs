namespace menuMalin.Services.Exceptions;

public class ErreurReseauException : Exception
{
    public ErreurReseauException(Exception innerException)
        : base("Erreur réseau", innerException) { }
}
