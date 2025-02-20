namespace DarkMode.Service;

// for now assuming day starts at midnight in dark mode, changes to light mode once at a specific time,
// then back to dark mode at another specific time, which stays through the rest of the night.
internal sealed class Schedule
{
    public required TimeOnly LightModeStart { get; init; }

    public required TimeOnly DarkModeStart { get; init; }
}
