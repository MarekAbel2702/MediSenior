using MediSenior.Models;

namespace MediSenior.Services;

public class UserService
{
    private readonly FirestoreService _firestore;
    private readonly SessionService _session;

    public UserService(FirestoreService firestore, SessionService session)
    {
        _firestore = firestore;
        _session = session;
    }

    public async Task CreateUserProfileAsync(string name, string role)
    {
        var seniorCode = role == "senior" ? GenerateSeniorCode() : "";

        var user = new AppUser
        {
            Id = _session.LocalId,
            Name = name,
            Email = _session.Email,
            Role = role,
            SeniorCode = seniorCode
        };

        await SaveUserAsync(user);
        _session.CurrentUser = user;
    }

    public async Task<AppUser?> LoadCurrentUserAsync()
    {
        var data = await _firestore.GetDocumentAsync("users", _session.LocalId);
        if (data == null)
            return null;

        var user = MapUser(_session.LocalId, data);
        _session.CurrentUser = user;
        return user;
    }

    public async Task<AppUser?> GetSeniorByCodeAsync(string code)
    {
        var users = await _firestore.QueryEqualAsync("users", "seniorCode", code.Trim().ToUpper());

        var found = users.FirstOrDefault(x =>
            x.Data.TryGetValue("role", out var role) &&
            role?.ToString() == "senior");

        if (string.IsNullOrWhiteSpace(found.Id))
            return null;

        return MapUser(found.Id, found.Data);
    }

    public async Task LinkCaregiverWithSeniorAsync(string seniorCode)
    {
        if (_session.CurrentUser == null)
            throw new Exception("Brak zalogowanego użytkownika.");

        var caregiver = _session.CurrentUser;

        if (caregiver.Role != "caregiver")
            throw new Exception("Tylko opiekun może połączyć konto z seniorem.");

        var senior = await GetSeniorByCodeAsync(seniorCode);

        if (senior == null)
            throw new Exception("Nie znaleziono seniora z takim kodem.");

        if (!caregiver.SeniorIds.Contains(senior.Id))
            caregiver.SeniorIds.Add(senior.Id);

        if (!senior.CaregiverIds.Contains(caregiver.Id))
            senior.CaregiverIds.Add(caregiver.Id);

        await SaveUserAsync(caregiver);
        await SaveUserAsync(senior);

        _session.CurrentUser = caregiver;
    }

    public async Task SaveUserAsync(AppUser user)
    {
        await _firestore.SetDocumentAsync("users", user.Id, new Dictionary<string, object?>
        {
            ["name"] = user.Name,
            ["email"] = user.Email,
            ["role"] = user.Role,
            ["seniorCode"] = user.SeniorCode,
            ["caregiverIds"] = user.CaregiverIds,
            ["seniorIds"] = user.SeniorIds
        });
    }

    public async Task<AppUser?> GetUserByIdAsync(string id)
    {
        var data = await _firestore.GetDocumentAsync("users", id);
        return data == null ? null : MapUser(id, data);
    }

    private static AppUser MapUser(string id, Dictionary<string, object?> data)
    {
        return new AppUser
        {
            Id = id,
            Name = data.GetValueOrDefault("name")?.ToString() ?? "",
            Email = data.GetValueOrDefault("email")?.ToString() ?? "",
            Role = data.GetValueOrDefault("role")?.ToString() ?? "",
            SeniorCode = data.GetValueOrDefault("seniorCode")?.ToString() ?? "",
            CaregiverIds = data.GetValueOrDefault("caregiverIds") as List<string> ?? new(),
            SeniorIds = data.GetValueOrDefault("seniorIds") as List<string> ?? new()
        };
    }

    private static string GenerateSeniorCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
