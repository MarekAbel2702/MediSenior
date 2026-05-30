using System.Net.Http.Json;
using System.Text.Json;

namespace MediSenior.Services;

public class FirebaseAuthService
{
    private readonly HttpClient _httpClient;
    private readonly SessionService _session;

    public FirebaseAuthService(HttpClient httpClient, SessionService session)
    {
        _httpClient = httpClient;
        _session = session;
    }

    public async Task RegisterAsync(string email, string password)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FirebaseConfig.ApiKey}";

        var response = await _httpClient.PostAsJsonAsync(url, new
        {
            email,
            password,
            returnSecureToken = true
        });

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(ParseFirebaseError(json));

        SaveAuthSession(json);
    }

    public async Task LoginAsync(string email, string password)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseConfig.ApiKey}";

        var response = await _httpClient.PostAsJsonAsync(url, new
        {
            email,
            password,
            returnSecureToken = true
        });

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(ParseFirebaseError(json));

        SaveAuthSession(json);
    }

    public Task LogoutAsync()
    {
        _session.Clear();
        return Task.CompletedTask;
    }

    private void SaveAuthSession(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        _session.IdToken = root.GetProperty("idToken").GetString() ?? "";
        _session.LocalId = root.GetProperty("localId").GetString() ?? "";
        _session.Email = root.GetProperty("email").GetString() ?? "";
    }

    private static string ParseFirebaseError(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var message = doc.RootElement
                .GetProperty("error")
                .GetProperty("message")
                .GetString();

            return message switch
            {
                "EMAIL_EXISTS" => "Ten adres email jest już używany.",
                "EMAIL_NOT_FOUND" => "Nie znaleziono konta z takim adresem email.",
                "INVALID_EMAIL" => "Nieprawidłowy adres email.",
                "USER_DISABLED" => "To konto zostało wyłączone.",
                "INVALID_PASSWORD" => "Nieprawidłowe hasło.",
                "INVALID_LOGIN_CREDENTIALS" => "Nieprawidłowy email lub hasło.",
                "WEAK_PASSWORD : Password should be at least 6 characters" => "Hasło musi mieć minimum 6 znaków.",
                _ => message ?? "Błąd Firebase."
            };
        }
        catch
        {
            return "Błąd połączenia z Firebase.";
        }
    }

    public async Task SendPasswordResetEmailAsync(string email)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={FirebaseConfig.ApiKey}";

        var response = await _httpClient.PostAsJsonAsync(url, new
        {
            requestType = "PASSWORD_RESET",
            email = email
        });

        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(ParseFirebaseError(json));
    }
}
