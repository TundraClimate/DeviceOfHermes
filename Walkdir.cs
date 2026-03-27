namespace System.Collections.Generic;

public static class Walkdir
{
    public static List<string> GetFilesRecursive(string path)
    {
        List<string> paths = new List<string>();
        List<string> stepped = new List<string>();

        Walkdir.Walk(paths, stepped, dirPath: path);

        return paths;
    }

    private static void Walk(List<string> paths, List<string> stepped, string dirPath)
    {
        if (!Directory.Exists(dirPath) || File.Exists(dirPath))
        {
            return;
        }

        if (stepped.Contains(dirPath))
        {
            return;
        }
        else
        {
            stepped.Add(dirPath);
        }

        string[] files = Directory.GetFiles(dirPath);

        paths.AddRange(files);

        foreach (string dir in Directory.GetDirectories(dirPath))
        {
            Walkdir.Walk(paths, stepped, dir);
        }
    }
}
