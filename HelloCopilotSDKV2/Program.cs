using System.Diagnostics;
using Microsoft.Copilot;

var startedAt = DateTime.Now;

await using var client = new CopilotClient(new CopilotClientOptions
{
    Mode = CopilotClientMode.CodingAgent,
    GitHub = new ClientGitHubOptions { UseLoggedInUser = true },
    // Runtime = RuntimeConnection.ForSpawnedStdio() // RuntimeConnection.ForInProcess() これがデフォルト
});
await client.StartAsync();

await using var session = await client.CreateSessionAsync(new CreateSessionOptions
{
    Tools = new SessionToolOptions { AvailableTools = [] }
});

var response = await session.SendAndWaitAsync(
    "Reply with exactly: Hello, world from GitHub Copilot SDK v2!");

if (string.IsNullOrWhiteSpace(response?.Data.Content))
{
    throw new InvalidOperationException("Copilot did not return an assistant message.");
}

Console.WriteLine(response.Data.Content);
Console.WriteLine();
PrintProcessInfo(startedAt);

Console.WriteLine();
Console.WriteLine("Press Enter to exit (inspect processes in Task Manager meanwhile)...");
_ = Console.ReadLine();

static void PrintProcessInfo(DateTime startedAt)
{
    using var current = Process.GetCurrentProcess();
    current.Refresh();
    Console.WriteLine($"This process : {current.ProcessName} (PID {current.Id}), threads {current.Threads.Count}, working set {current.WorkingSet64 / 1024 / 1024} MB");

    var runtimeModules = current.Modules.Cast<ProcessModule>()
        .Where(m => m.ModuleName.StartsWith("copilot", StringComparison.OrdinalIgnoreCase))
        .Select(m => m.FileName)
        .ToList();
    Console.WriteLine($"Copilot native runtime modules loaded in this process: {runtimeModules.Count}");
    foreach (var module in runtimeModules)
    {
        Console.WriteLine($"  {module}");
    }

    Console.WriteLine("Copilot runtime processes started after this app:");
    var found = false;
    foreach (var p in Process.GetProcesses())
    {
        using (p)
        {
            try
            {
                if (p.Id != current.Id
                    && p.ProcessName.Contains("copilot", StringComparison.OrdinalIgnoreCase)
                    && p.StartTime >= startedAt.AddSeconds(-1))
                {
                    Console.WriteLine($"  {p.ProcessName} (PID {p.Id}), working set {p.WorkingSet64 / 1024 / 1024} MB, path {p.MainModule?.FileName}");
                    found = true;
                }
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
            {
                // Process exited or access was denied.
            }
        }
    }

    if (!found)
    {
        Console.WriteLine("  (none)");
    }
}
