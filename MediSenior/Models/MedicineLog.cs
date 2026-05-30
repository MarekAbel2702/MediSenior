namespace MediSenior.Models;

public class MedicineLog
{
    public string Id { get; set; } = "";
    public string MedicineId { get; set; } = "";
    public string SeniorId { get; set; } = "";
    public string Date { get; set; } = ""; // yyyy-MM-dd
    public string Time { get; set; } = ""; // HH:mm
    public string Status { get; set; } = "pending"; // pending, taken
    public string TakenAt { get; set; } = ""; // ISO string
    public string ConfirmedBy { get; set; } = "";
}
