namespace MediSenior.Models;

public class AppUser
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = ""; // senior albo caregiver
    public string SeniorCode { get; set; } = "";

    public List<string> CaregiverIds { get; set; } = new();
    public List<string> SeniorIds { get; set; } = new();
}
