# Toddler Coder

A quiet full-screen keyboard-mashing game that makes random keypresses look like a tiny coding workspace.

The screen has a project list on the left, a fake editor and output panel in the middle, and a live diff/review panel on the right.

## Run Kid Mode

Double-click:

```text
run-kid-mode.bat
```

Or from PowerShell:

```powershell
dotnet run --project .\ToddlerCoder.csproj --no-launch-profile
```

Kid mode is borderless and full-screen. It blocks common accidental exit/task-switch shortcuts such as `Alt+F4`, `Alt+Tab`, the Windows keys, and `Ctrl+Esc`.

To exit kid mode, hold `Ctrl+Shift+Q` for 3 seconds.

You can also press `Ctrl+Alt+Del`, open Task Manager, select `ToddlerCoder`, and end the task. Windows does not allow normal apps to block `Ctrl+Alt+Del`.

## Debug Windowed Mode

Double-click:

```text
run-debug-windowed.bat
```

Or from PowerShell:

```powershell
dotnet run --project .\ToddlerCoder.csproj --no-launch-profile -- --debug-windowed
```

This opens a normal resizable window and does not install the keyboard guard. Use this mode while changing the visuals or behavior.

## Publish Release Exe

Double-click:

```text
publish-release.bat
```

Or from PowerShell:

```powershell
dotnet publish .\ToddlerCoder.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

The published exe is written to:

```text
bin\Release\net10.0-windows\win-x64\publish\ToddlerCoder.exe
```

## Visual Studio

Open `ToddlerCoder.csproj` or the solution file in Visual Studio. For regular debugging, set the command-line argument to:

```text
--debug-windowed
```

For the real child-safe run, launch without arguments.
