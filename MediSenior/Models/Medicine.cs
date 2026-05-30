namespace MediSenior.Models;

public class Medicine
{
    public string Id { get; set; } = "";
    public string SeniorId { get; set; } = "";

    public string Name { get; set; } = "";
    public string Dose { get; set; } = "";
    public string Notes { get; set; } = "";

    public List<string> Times { get; set; } = new();

    public bool IsActive { get; set; } = true;
    public string CreatedBy { get; set; } = "";
}
