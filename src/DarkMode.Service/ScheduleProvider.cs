namespace DarkMode.Service;

internal interface IScheduleProvider
{
    Schedule GetSchedule();
}

internal sealed class ScheduleProvider : IScheduleProvider
{
    // TODO: read from APPDATA config file
    // TODO: include file change token to reload on saved changes
    public Schedule GetSchedule() => new()
    {
        LightModeStart = new(8, 0), // 8am
        DarkModeStart = new(18, 0), // 6pm
    };
}
