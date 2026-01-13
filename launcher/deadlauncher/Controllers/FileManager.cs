using System.Diagnostics;
using System.IO.Compression;

namespace deadlauncher;

public abstract class FileManager
{
    public abstract void OpenFolderInExplorer(string path);
    public abstract void ValidateFolder(string path);
    public abstract string? ReadFile(string path);
    public abstract void WriteFile(string path, string content);
    public abstract void Delete(string path);
    public abstract void ExtractZipTo(string zipPath, string extractPath);
}

public sealed class WindowsFileManager : FileManager
{
    public override void OpenFolderInExplorer(string path)
    {
        if(Path.Exists(path)) Process.Start("explorer.exe", @$"{path}"); 
    }

    public override void ValidateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public override string? ReadFile(string path)
    {
        if (!Path.Exists(path)) return null;

        return File.ReadAllText(path);
    }

    public override void WriteFile(string path, string content)
    {
        if (!File.Exists(path)) File.Create(path).Close();
        File.WriteAllText(path, content);
    }

    public override void Delete(string path)
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

    public override void ExtractZipTo(string zipPath, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipPath, extractPath);
    }
}