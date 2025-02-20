using Microsoft.Win32;

namespace DarkMode.Service;

internal enum Mode { DarkMode, LightMode }

internal interface IModeSetter
{
    void SetMode(Mode mode);
}

internal sealed class RegistryModeSetter : IModeSetter, IDisposable
{
    private const string RegistryKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
    private const string AppsUseLightThemeKey = "AppsUseLightTheme";
    private const string SystemUsesLightThemeKey = "SystemUsesLightTheme";

    private readonly RegistryKey _key = Registry.CurrentUser.OpenSubKey(RegistryKey, writable: true)
        ?? throw new InvalidOperationException($"Unable to open registry key HKEY_CURRENT_USER\\{RegistryKey}");

    public void SetMode(Mode mode)
    {
        var value = mode switch
        {
            Mode.DarkMode => 0,
            Mode.LightMode => 1,
            _ => throw new InvalidOperationException($"Unexpected mode {mode}"),
        };

        this._key.SetValue(AppsUseLightThemeKey, value, RegistryValueKind.DWord);
        this._key.SetValue(SystemUsesLightThemeKey, value, RegistryValueKind.DWord);
        this._key.Flush();
    }

    public void Dispose() => this._key.Dispose();
}
