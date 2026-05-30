using Plugin.LocalNotification;
using MediSenior.Models;

namespace MediSenior.Services;

public class NotificationService
{
    public async Task ScheduleMedicineRemindersAsync(Medicine medicine)
    {
#if ANDROID || IOS
        foreach (var time in medicine.Times)
        {
            await ScheduleMedicineReminderAsync(medicine, time);
        }
#else
        await Task.CompletedTask;
#endif
    }

    private async Task ScheduleMedicineReminderAsync(Medicine medicine, string medicineTime)
    {
#if ANDROID || IOS
        if (!TimeSpan.TryParse(medicineTime, out var time))
            return;

        var notifyTime = DateTime.Today.Add(time);

        if (notifyTime <= DateTime.Now)
            notifyTime = notifyTime.AddDays(1);

        var request = new NotificationRequest
        {
            NotificationId = Math.Abs($"{medicine.Id}_{medicineTime}".GetHashCode()),
            Title = "Czas na lek",
            Description = $"{medicine.Name} - {medicine.Dose}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyTime,
                RepeatType = NotificationRepeat.Daily
            }
        };

        await LocalNotificationCenter.Current.Show(request);
#else
        await Task.CompletedTask;
#endif
    }
}