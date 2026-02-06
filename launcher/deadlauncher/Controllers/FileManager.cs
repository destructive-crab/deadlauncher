using System.Diagnostics;
using System.IO.Compression;
using WindowsShortcutFactory;

namespace deadlauncher;

public abstract class FileManager
{
    public abstract void OpenFolderInExplorer(string path);

    /// <summary>
    /// Validate if end folder exists
    /// </summary>
    /// <returns>returns true if folder was created during method invocation</returns>
    public virtual bool ValidateFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return true;
        }

        return false;
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

    public virtual bool Exist(string path)
    {
        return Path.Exists(path);
    }

    public virtual string[] PullFiles(string path, string pattern)
    {
        if (!Exist(path)) return Array.Empty<string>();
        
        DirectoryInfo d = new DirectoryInfo(path); 

        FileInfo[] Files = d.GetFiles(pattern); 

        string[] result = new string[Files.Length];
        
        for (var i = 0; i < Files.Length; i++)
        {
            var file = Files[i];
            result[i] = Path.Combine(path, file.Name);
        }

        return result;
    }
}

public sealed class WindowsFileManager : FileManager
{
    public bool CreateShortcut(string path, string targetPath, string description)
    {
        try
        {
            using var shortcut = new WindowsShortcut()
            {
                Path = targetPath,
                Description = description
            };

            shortcut.Save(path);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
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