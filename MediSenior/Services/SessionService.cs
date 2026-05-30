using MediSenior.Models;

namespace MediSenior.Services;

public class SessionService
{
    public string IdToken { get; set; } = "";
    public string LocalId { get; set; } = "";
    public string Email { get; set; } = "";
    public AppUser? CurrentUser { get; set; }

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(IdToken) && !string.IsNullOrWhiteSpace(LocalId);

    public void Clear()
    {
        IdToken = "";
        LocalId = "";
        Email = "";
        CurrentUser = null;
    }
}
