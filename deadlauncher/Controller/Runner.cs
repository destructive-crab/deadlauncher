using System.Diagnostics;

namespace deadlauncher;

public sealed class Runner
{
    private Launcher l;

    public Runner(Launcher l)
    {
        this.l = l;
    }

    public void RunSelectedVersion()
    {
        if (l.Model.IsInstalled(l.Model.SelectedVersionId))
        {
            Process process = new Process();
            process.StartInfo.FileName = l.Model.ExecutablePath(l.Model.SelectedVersionId);
            process.Start();
        }
    }
}