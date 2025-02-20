using DarkMode.Service;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options => options.ServiceName = "Auto Dark Mode");

LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IScheduleProvider, ScheduleProvider>();
builder.Services.AddSingleton<IModeSetter, RegistryModeSetter>();

var host = builder.Build();
host.Run();