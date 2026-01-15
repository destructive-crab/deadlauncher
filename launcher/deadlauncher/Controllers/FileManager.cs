using System.Diagnostics;
using System.IO.Compression;

namespace deadlauncher;

public abstract class FileManager
{
    public abstract void OpenFolderInExplorer(string path);

    public virtual void ValidateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public virtual string? ReadFile(string path)
    {
        if (!Path.Exists(path)) return null;

        return File.ReadAllText(path);
    }

    public virtual void WriteFile(string path, string content)
    {
        if (!File.Exists(path)) File.Create(path).Close();
        File.WriteAllText(path, content);
    }

    public virtual void Delete(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            return;
        }
        else if(Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                File.Delete(file);
            }
            Directory.Delete(path);
        }
    }

    public virtual void ExtractZipTo(string zipPath, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipPath, extractPath);
    }
}

public sealed class WindowsFileManager : FileManager
{
    public override void OpenFolderInExplorer(string path)
    {
        if(Path.Exists(path)) Process.Start("explorer.exe", @$"{path}"); 
    }
}

public sealed class LinuxFileManager : FileManager
{
    public override void OpenFolderInExplorer(string path)
    {
        if(Path.Exists(path)) Process.Start("xdg-open", @$"{path}"); 
    }
}