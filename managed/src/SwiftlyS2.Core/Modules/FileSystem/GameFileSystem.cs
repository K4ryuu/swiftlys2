using SwiftlyS2.Core.Natives;
using SwiftlyS2.Shared.FileSystem;

namespace SwiftlyS2.Core.FileSystem;

internal class GameFileSystem : IGameFileSystem
{
    public void AddSearchPath( string path, string pathId, SearchPathAdd_t addType, SearchPathPriority_t priority )
    {
        NativeFileSystem.AddSearchPath(path, pathId, (int)addType, (int)priority);
    }

    public bool FileExists( string filePath, string pathId )
    {
        return NativeFileSystem.FileExists(filePath, pathId);
    }

    public string GetSearchPath( string pathId, GetSearchPathTypes_t searchPathType, int searchPathsToGet )
    {
        return NativeFileSystem.GetSearchPath(pathId, (int)searchPathType, searchPathsToGet);
    }

    public bool IsDirectory( string path, string pathId )
    {
        return NativeFileSystem.IsDirectory(path, pathId);
    }

    public void PrintSearchPaths()
    {
        NativeFileSystem.PrintSearchPaths();
    }

    public bool RemoveSearchPath( string path, string pathId )
    {
        return NativeFileSystem.RemoveSearchPath(path, pathId);
    }
}