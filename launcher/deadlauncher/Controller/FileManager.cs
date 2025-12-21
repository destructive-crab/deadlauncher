using System.Diagnostics;

namespace deadlauncher;

public abstract class FileManager
{
    public abstract void OpenFolderInExplorer(string path);
    public abstract void ValidateFolder(string path);
}

public sealed class WindowsFileManager : FileManager
{
    public override void OpenFolderInExplorer(string path)
    {
        if(Path.Exists(path)) Process.Start("explorer.exe", @$"{path}"); 
    }

    public override void ValidateFolder(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}