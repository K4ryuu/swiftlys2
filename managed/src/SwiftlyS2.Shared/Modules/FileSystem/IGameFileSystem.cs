namespace SwiftlyS2.Shared.FileSystem;

public interface IGameFileSystem
{
    /// <summary>
    /// Prints the current search paths to the console.
    /// </summary>
    public void PrintSearchPaths();

    /// <summary>
    /// Checks if a directory exists at the given path and path ID.
    /// </summary>
    /// <param name="path">The path to check.</param>
    /// <param name="pathId">The ID of the path to search in.</param>
    /// <returns>True if the directory exists, false otherwise.</returns>
    public bool IsDirectory( string path, string pathId );

    /// <summary>
    /// Removes a search path from the file system.
    /// </summary>
    /// <param name="path">The path to remove.</param>
    /// <param name="pathId">The ID of the path to remove in.</param>
    /// <returns>True if the path was removed successfully, false otherwise.</returns>
    public bool RemoveSearchPath( string path, string pathId );

    /// <summary>
    /// Adds a search path to the file system.
    /// </summary>
    /// <param name="path">The path to add.</param>
    /// <param name="pathId">The ID of the path to add in.</param>
    /// <param name="addType">The type of addition to perform.</param>
    /// <param name="priority">The priority of the search path.</param>
    public void AddSearchPath( string path, string pathId, SearchPathAdd_t addType, SearchPathPriority_t priority );

    /// <summary>
    /// Checks if a file exists at the given file path and path ID.
    /// </summary>
    /// <param name="filePath">The file path to check.</param>
    /// <param name="pathId">The ID of the path to check in.</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    public bool FileExists( string filePath, string pathId );

    /// <summary>
    /// Gets the search path(s) for the given path ID and search path type.
    /// </summary>
    /// <param name="pathId">The ID of the path to get the search paths for.</param>
    /// <param name="searchPathType">The type of search path to get.</param>
    /// <param name="searchPathsToGet">The number of search paths to get.</param>
    /// <returns>The search path(s) for the given path ID and search path type.</returns>
    public string GetSearchPath( string pathId, GetSearchPathTypes_t searchPathType, int searchPathsToGet );
}