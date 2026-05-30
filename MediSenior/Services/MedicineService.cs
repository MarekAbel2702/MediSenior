using MediSenior.Models;

namespace MediSenior.Services;

public class MedicineService
{
    private readonly FirestoreService _firestore;
    private readonly SessionService _session;
    private readonly NotificationService _notificationService;

    public MedicineService(
        FirestoreService firestore,
        SessionService session,
        NotificationService notificationService)
    {
        _firestore = firestore;
        _session = session;
        _notificationService = notificationService;
    }

    public async Task AddMedicineAsync(Medicine medicine)
    {
        if (_session.CurrentUser == null)
            throw new Exception("Brak zalogowanego użytkownika.");

        medicine.CreatedBy = _session.CurrentUser.Id;

        if (_session.CurrentUser.Role == "senior")
            medicine.SeniorId = _session.CurrentUser.Id;

        if (string.IsNullOrWhiteSpace(medicine.SeniorId))
            throw new Exception("Nie wskazano seniora dla leku.");

        if (medicine.Times == null || medicine.Times.Count == 0)
            throw new Exception("Dodaj przynajmniej jedną godzinę przyjmowania leku.");

        var id = await _firestore.AddDocumentAsync("medicines", new Dictionary<string, object?>
        {
            ["seniorId"] = medicine.SeniorId,
            ["name"] = medicine.Name,
            ["dose"] = medicine.Dose,
            ["notes"] = medicine.Notes,
            ["times"] = medicine.Times,
            ["isActive"] = medicine.IsActive,
            ["createdBy"] = medicine.CreatedBy
        });

        medicine.Id = id;

#if ANDROID || IOS
        if (_session.CurrentUser.Role == "senior")
            await _notificationService.ScheduleMedicineRemindersAsync(medicine);
#endif
    }

    public async Task<List<Medicine>> GetMedicinesForSeniorAsync(string seniorId)
    {
        var rows = await _firestore.QueryEqualAsync("medicines", "seniorId", seniorId);

        return rows
            .Select(x => MapMedicine(x.Id, x.Data))
            .Where(x => x.IsActive)
            .OrderBy(x => x.Times.FirstOrDefault())
            .ToList();
    }

    public async Task<List<MedicineTodayItem>> GetTodayMedicinesForSeniorAsync(string seniorId)
    {
        var medicines = await GetMedicinesForSeniorAsync(seniorId);
        var logs = await GetTodayLogsForSeniorAsync(seniorId);
        var today = DateTime.Today.ToString("yyyy-MM-dd");

        var result = new List<MedicineTodayItem>();

        foreach (var med in medicines)
        {
            foreach (var time in med.Times)
            {
                var log = logs.FirstOrDefault(x =>
                    x.MedicineId == med.Id &&
                    x.Date == today &&
                    x.Time == time);

                result.Add(new MedicineTodayItem
                {
                    MedicineId = med.Id,
                    SeniorId = med.SeniorId,
                    Name = med.Name,
                    Dose = med.Dose,
                    Notes = med.Notes,
                    Time = time,
                    Status = log?.Status ?? "pending",
                    TakenAt = FormatTakenAt(log?.TakenAt)
                });
            }
        }

        return result.OrderBy(x => x.Time).ToList();
    }

    public async Task MarkAsTakenAsync(MedicineTodayItem item)
    {
        if (_session.CurrentUser == null)
            throw new Exception("Brak zalogowanego użytkownika.");

        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var logId = $"{item.SeniorId}_{item.MedicineId}_{today}_{item.Time.Replace(":", "")}";

        await _firestore.SetDocumentAsync("medicine_logs", logId, new Dictionary<string, object?>
        {
            ["medicineId"] = item.MedicineId,
            ["seniorId"] = item.SeniorId,
            ["date"] = today,
            ["time"] = item.Time,
            ["status"] = "taken",
            ["takenAt"] = DateTime.Now.ToString("O"),
            ["confirmedBy"] = _session.CurrentUser.Id
        });
    }

    private async Task<List<MedicineLog>> GetTodayLogsForSeniorAsync(string seniorId)
    {
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var rows = await _firestore.QueryEqualAsync("medicine_logs", "seniorId", seniorId);

        return rows
            .Select(x => MapLog(x.Id, x.Data))
            .Where(x => x.Date == today)
            .ToList();
    }

    private static Medicine MapMedicine(string id, Dictionary<string, object?> data)
    {
        var times = data.GetValueOrDefault("times") as List<string> ?? new List<string>();

        if (times.Count == 0)
        {
            var oldTime = data.GetValueOrDefault("time")?.ToString();

            if (!string.IsNullOrWhiteSpace(oldTime))
                times.Add(oldTime);
        }

        return new Medicine
        {
            Id = id,
            SeniorId = data.GetValueOrDefault("seniorId")?.ToString() ?? "",
            Name = data.GetValueOrDefault("name")?.ToString() ?? "",
            Dose = data.GetValueOrDefault("dose")?.ToString() ?? "",
            Notes = data.GetValueOrDefault("notes")?.ToString() ?? "",
            Times = times.OrderBy(x => x).ToList(),
            IsActive = data.GetValueOrDefault("isActive") is bool b && b,
            CreatedBy = data.GetValueOrDefault("createdBy")?.ToString() ?? ""
        };
    }

    private static MedicineLog MapLog(string id, Dictionary<string, object?> data)
    {
        return new MedicineLog
        {
            Id = id,
            MedicineId = data.GetValueOrDefault("medicineId")?.ToString() ?? "",
            SeniorId = data.GetValueOrDefault("seniorId")?.ToString() ?? "",
            Date = data.GetValueOrDefault("date")?.ToString() ?? "",
            Time = data.GetValueOrDefault("time")?.ToString() ?? "",
            Status = data.GetValueOrDefault("status")?.ToString() ?? "pending",
            TakenAt = data.GetValueOrDefault("takenAt")?.ToString() ?? "",
            ConfirmedBy = data.GetValueOrDefault("confirmedBy")?.ToString() ?? ""
        };
    }

    private static string FormatTakenAt(string? iso)
    {
        if (string.IsNullOrWhiteSpace(iso))
            return "";

        return DateTime.TryParse(iso, out var date)
            ? date.ToString("HH:mm")
            : "";
    }

    public async Task DeleteMedicineAsync(MedicineTodayItem item)
    {
        if (item == null)
            throw new Exception("Nie wybrano leku.");

        await _firestore.SetDocumentAsync("medicines", item.MedicineId, new Dictionary<string, object?>
        {
            ["seniorId"] = item.SeniorId,
            ["name"] = item.Name,
            ["dose"] = item.Dose,
            ["notes"] = item.Notes,
            ["times"] = new List<string> { item.Time },
            ["isActive"] = false,
            ["createdBy"] = _session.CurrentUser?.Id ?? ""
        });
    }
}
