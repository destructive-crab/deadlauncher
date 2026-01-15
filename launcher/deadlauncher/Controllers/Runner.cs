using System.Diagnostics;

namespace deadlauncher;

public abstract class Runner
{
    public abstract void RunSelectedVersion();
}

public sealed class WindowsRunner : Runner
{
    private Launcher l;
    private const string ExecutableName = "Deadays.exe";

    public WindowsRunner(Launcher l)
    {
        this.l = l;
    }

    public override void RunSelectedVersion()
    {
        if (l.Model.IsInstalled(l.Model.SelectedVersionID))
        {
            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(l.Model.ExecutableFolder(l.Model.SelectedVersionID), ExecutableName);
            process.Start();
        }
    }
}

public sealed class LinuxRunner : Runner
{
    private Launcher l;
    private const string ExecutableName = "Deadays";

    public LinuxRunner(Launcher l)
    {
        this.l = l;
    }

    public override void RunSelectedVersion()
    {
        if (l.Model.IsInstalled(l.Model.SelectedVersionID))
        {
            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(l.Model.ExecutableFolder(l.Model.SelectedVersionID), ExecutableName);
            process.Start();
        }
    }
}