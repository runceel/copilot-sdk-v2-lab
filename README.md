# copilot-sdk-v2-lab

GitHub Copilot SDK v2 for .NET hello-world sample.

## Requirements

- .NET 11 SDK.
- GitHub Copilot access and an existing Copilot CLI sign-in (`copilot login`).
- Network access for NuGet restore, the SDK runtime download, and GitHub Copilot.

## Run

```powershell
dotnet build .\CopilotSdkV2Lab.slnx
dotnet run --project .\HelloCopilotSDKV2 --no-build
```

The application sends one request using the signed-in account's default Copilot model and prints the response:

```text
Hello, world from GitHub Copilot SDK v2!
```

### Runtime hosting mode

By default, SDK v2 hosts the Copilot runtime **in-process**, loading
`copilot_runtime.dll` into the app process. To use the v1-compatible mode,
uncomment the `Runtime` property in `Program.cs`:

```csharp
Runtime = RuntimeConnection.ForSpawnedStdio()
```

This spawns `copilot-runtime.exe` as a child process and talks JSON-RPC over stdio.
Rebuild after changing the property. After the response, the app prints its process
information, loaded Copilot native modules, and Copilot-named processes started
since the app began. The list may include unrelated processes. The app then waits
for Enter so you can inspect processes (for example, in Task Manager).

`Microsoft.Copilot.SDK` 2.0.1 downloads its pinned Copilot runtime as needed.
The application uses the locally signed-in GitHub Copilot account, not a separate
model API key or BYOK provider. Requests consume that account's Copilot usage/AI
Credits according to its plan. No tools are exposed to the model.

The SDK can also use `COPILOT_GITHUB_TOKEN`, `GH_TOKEN`, or `GITHUB_TOKEN` from the
environment. Unset those variables before running if you want to use only the
stored CLI sign-in; do not put tokens in source files.

## References

- [Official .NET SDK documentation (v2.0.1)](https://github.com/github/copilot-sdk/blob/v2.0.1/dotnet/README.md)
- [SDK types (v2.0.1)](https://github.com/github/copilot-sdk/blob/v2.0.1/dotnet/src/Types.cs)