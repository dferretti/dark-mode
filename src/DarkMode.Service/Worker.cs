namespace DarkMode.Service;

// https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service
internal sealed class Worker(
    ILogger<Worker> logger,
    IScheduleProvider scheduleProvider,
    IModeSetter modeSetter) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IScheduleProvider _scheduleProvider = scheduleProvider;
    private readonly IModeSetter _modeSetter = modeSetter;

    // https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service#rewrite-the-worker-class
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await this.FollowScheduleAsync(stoppingToken);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            this._logger.LogError(ex, "Error during processing");
            Environment.Exit(1);
        }
    }

    private async Task FollowScheduleAsync(CancellationToken cancel)
    {
        var schedule = this._scheduleProvider.GetSchedule();

        // loop runs once per day. needs to handle case where loop is started in the middle of the day
        while (!cancel.IsCancellationRequested)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var lightModeStart = today.ToDateTime(schedule.LightModeStart);
            var darkModeStart = today.ToDateTime(schedule.DarkModeStart);

            // if we are before the switch to light mode (in the morning still), make sure we are in dark mode.
            // then wait til we should switch to light
            if (DateTime.Now < lightModeStart)
            {
                this._logger.LogInformation("Setting to dark mode");
                this._modeSetter.SetMode(Mode.DarkMode);
                await Task.Delay(lightModeStart - DateTime.Now, cancel);
            }

            if (DateTime.Now < darkModeStart)
            {
                this._logger.LogInformation("Setting to light mode");
                this._modeSetter.SetMode(Mode.LightMode);
                await Task.Delay(darkModeStart - DateTime.Now, cancel);
            }

            // if we are past the nighttime dark mode start, make sure we set to dark mode, and wait til tomorrow
            this._logger.LogInformation("Setting to dark mode");
            this._modeSetter.SetMode(Mode.DarkMode);

            this._logger.LogInformation("Waiting until tomorrow");
            await Task.Delay(DateTime.Today.AddDays(1) - DateTime.Now, cancel);
        }
    }
}
