namespace System.Collections.Generic;

/// <summary>
/// Provides utilities for recursively walking directories.
/// </summary>
/// <remarks>
/// This class performs a depth-first traversal of directories and collects file paths.
/// <para/>
/// It keeps track of visited directories to prevent infinite loops caused by symbolic links
/// or junctions.
/// <para/>
/// Invalid paths or non-directory inputs are silently ignored.
/// </remarks>
/// <example>
/// <code>
/// var files = Walkdir.GetFilesRecursive(path);
///
/// foreach (var file in files)
/// {
///     Console.WriteLine(file);
/// }
/// </code>
/// </example>
public static class Walkdir
{
    /// <summary>
    /// Recursively retrieves all file paths under the specified directory.
    /// </summary>
    /// <param name="path">The root directory to traverse.</param>
    /// <returns>
    /// A list of file paths found under the specified directory.
    /// Returns an empty list if the path does not exist or is not a directory.
    /// </returns>
    /// <remarks>
    /// This method performs a depth-first traversal.
    /// <para/>
    /// Directories that have already been visited are skipped to prevent infinite recursion.
    /// <para/>
    /// No exceptions are thrown for invalid paths; such inputs are ignored.
    /// </remarks>
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
