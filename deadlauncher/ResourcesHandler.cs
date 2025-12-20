using System.Reflection;

namespace deadlauncher;

public static class ResourcesHandler
{
    private const string assetsFolder = "deadlauncher.assets";

    public static Stream Load(string name)
    {
        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assetsFolder+"."+name);
        return stream;
    }
}