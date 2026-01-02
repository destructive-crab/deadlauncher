namespace deadlauncher;

public sealed class Launcher
{
    //M
    public readonly LauncherModel  Model;
    //V
    public readonly LauncherWindow Window;
    //C
    public readonly Downloader     Downloader;
    public readonly Runner         Runner;
    public readonly FileManager    FileManager = new WindowsFileManager();

    public Launcher()
    {
        Downloader = new Downloader(this);
        Runner     = new Runner(this);
        Model      = new LauncherModel();
        Window     = new LauncherWindow();
    }
}