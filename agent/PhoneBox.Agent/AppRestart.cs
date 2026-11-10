using System.Diagnostics;

namespace PhoneBox.Agent;

/// <summary>Set before stopping the host to make Program start a fresh instance afterwards.</summary>
public static class AppRestart
{
    public static volatile bool Requested;

    /// <summary>
    /// Exit code that makes systemd (Restart=on-failure) and launchd (KeepAlive SuccessfulExit=false) start the agent
    /// again. Both kill whatever the service leaves behind, so a fresh instance started by the agent itself would die.
    /// </summary>
    public const int ExitCodeForRestart = 75;

    /// <param name="invocationId">INVOCATION_ID, set by systemd for its services.</param>
    /// <param name="xpcServiceName">XPC_SERVICE_NAME: the launchd job label; "0" in a Terminal session.</param>
    internal static bool ServiceManaged(string? invocationId, string? xpcServiceName) =>
        !string.IsNullOrEmpty(invocationId) || xpcServiceName is { Length: > 0 } and not "0";

    internal static ProcessStartInfo StartInfo(string executable, string[] args)
    {
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, WorkingDirectory = AppContext.BaseDirectory };
        if (string.Equals(Path.GetFileNameWithoutExtension(executable), "dotnet", StringComparison.OrdinalIgnoreCase))
            start.ArgumentList.Add(typeof(AppRestart).Assembly.Location);
        foreach (var arg in args.Where(a => a != "--restart")) start.ArgumentList.Add(arg);
        start.ArgumentList.Add("--restart");
        return start;
    }
}
