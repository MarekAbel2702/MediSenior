namespace MediSenior.Models;

public class MedicineTodayItem
{
    public string MedicineId { get; set; } = "";
    public string SeniorId { get; set; } = "";

    public string Name { get; set; } = "";
    public string Dose { get; set; } = "";
    public string Notes { get; set; } = "";

    public string Time { get; set; } = "";

    public string Status { get; set; } = "pending";
    public string TakenAt { get; set; } = "";

    public bool IsTaken => Status == "taken";

    public bool CanMarkAsTaken => Status != "taken";

    public string MarkButtonText => Status == "taken"
        ? "Już przyjęto"
        : "Wziąłem lek";

    public string StatusText
    {
        get
        {
            if (Status == "taken")
                return string.IsNullOrWhiteSpace(TakenAt)
                    ? "Przyjęto"
                    : $"Przyjęto: {TakenAt}";

            return "Jeszcze nie przyjęto";
        }
    }
}