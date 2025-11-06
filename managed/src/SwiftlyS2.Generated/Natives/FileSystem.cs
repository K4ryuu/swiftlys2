#pragma warning disable CS0649
#pragma warning disable CS0169

using System.Buffers;
using System.Text;
using System.Threading;
using SwiftlyS2.Shared.Natives;

namespace SwiftlyS2.Core.Natives;

internal static class NativeFileSystem {
  private static int _MainThreadID;

  private unsafe static delegate* unmanaged<byte*, byte*, int, int, int> _GetSearchPath;

  public unsafe static string GetSearchPath(string pathId, int searchPathType, int searchPathsToGet) {
    var pool = ArrayPool<byte>.Shared;
    var pathIdLength = Encoding.UTF8.GetByteCount(pathId);
    var pathIdBuffer = pool.Rent(pathIdLength + 1);
    Encoding.UTF8.GetBytes(pathId, pathIdBuffer);
    pathIdBuffer[pathIdLength] = 0;
    fixed (byte* pathIdBufferPtr = pathIdBuffer) {
      var ret = _GetSearchPath(null, pathIdBufferPtr, searchPathType, searchPathsToGet);
      var retBuffer = pool.Rent(ret + 1);
      fixed (byte* retBufferPtr = retBuffer) {
        ret = _GetSearchPath(retBufferPtr, pathIdBufferPtr, searchPathType, searchPathsToGet);
        var retString = Encoding.UTF8.GetString(retBufferPtr, ret);
        pool.Return(retBuffer);
        pool.Return(pathIdBuffer);
        return retString;
      }
    }
  }

  private unsafe static delegate* unmanaged<byte*, byte*, byte> _FileExists;

  public unsafe static bool FileExists(string fileName, string pathId) {
    var pool = ArrayPool<byte>.Shared;
    var fileNameLength = Encoding.UTF8.GetByteCount(fileName);
    var fileNameBuffer = pool.Rent(fileNameLength + 1);
    Encoding.UTF8.GetBytes(fileName, fileNameBuffer);
    fileNameBuffer[fileNameLength] = 0;
    var pathIdLength = Encoding.UTF8.GetByteCount(pathId);
    var pathIdBuffer = pool.Rent(pathIdLength + 1);
    Encoding.UTF8.GetBytes(pathId, pathIdBuffer);
    pathIdBuffer[pathIdLength] = 0;
    fixed (byte* fileNameBufferPtr = fileNameBuffer) {
      fixed (byte* pathIdBufferPtr = pathIdBuffer) {
        var ret = _FileExists(fileNameBufferPtr, pathIdBufferPtr);
        pool.Return(fileNameBuffer);
        pool.Return(pathIdBuffer);
        return ret == 1;
      }
    }
  }

  private unsafe static delegate* unmanaged<byte*, byte*, int, int, void> _AddSearchPath;

  public unsafe static void AddSearchPath(string path, string pathId, int searchPathAdd, int searchPathPriority) {
    var pool = ArrayPool<byte>.Shared;
    var pathLength = Encoding.UTF8.GetByteCount(path);
    var pathBuffer = pool.Rent(pathLength + 1);
    Encoding.UTF8.GetBytes(path, pathBuffer);
    pathBuffer[pathLength] = 0;
    var pathIdLength = Encoding.UTF8.GetByteCount(pathId);
    var pathIdBuffer = pool.Rent(pathIdLength + 1);
    Encoding.UTF8.GetBytes(pathId, pathIdBuffer);
    pathIdBuffer[pathIdLength] = 0;
    fixed (byte* pathBufferPtr = pathBuffer) {
      fixed (byte* pathIdBufferPtr = pathIdBuffer) {
        _AddSearchPath(pathBufferPtr, pathIdBufferPtr, searchPathAdd, searchPathPriority);
        pool.Return(pathBuffer);
        pool.Return(pathIdBuffer);
      }
    }
  }

  private unsafe static delegate* unmanaged<byte*, byte*, byte> _RemoveSearchPath;

  public unsafe static bool RemoveSearchPath(string path, string pathId) {
    var pool = ArrayPool<byte>.Shared;
    var pathLength = Encoding.UTF8.GetByteCount(path);
    var pathBuffer = pool.Rent(pathLength + 1);
    Encoding.UTF8.GetBytes(path, pathBuffer);
    pathBuffer[pathLength] = 0;
    var pathIdLength = Encoding.UTF8.GetByteCount(pathId);
    var pathIdBuffer = pool.Rent(pathIdLength + 1);
    Encoding.UTF8.GetBytes(pathId, pathIdBuffer);
    pathIdBuffer[pathIdLength] = 0;
    fixed (byte* pathBufferPtr = pathBuffer) {
      fixed (byte* pathIdBufferPtr = pathIdBuffer) {
        var ret = _RemoveSearchPath(pathBufferPtr, pathIdBufferPtr);
        pool.Return(pathBuffer);
        pool.Return(pathIdBuffer);
        return ret == 1;
      }
    }
  }

  private unsafe static delegate* unmanaged<byte*, byte*, byte> _IsDirectory;

  public unsafe static bool IsDirectory(string path, string pathId) {
    var pool = ArrayPool<byte>.Shared;
    var pathLength = Encoding.UTF8.GetByteCount(path);
    var pathBuffer = pool.Rent(pathLength + 1);
    Encoding.UTF8.GetBytes(path, pathBuffer);
    pathBuffer[pathLength] = 0;
    var pathIdLength = Encoding.UTF8.GetByteCount(pathId);
    var pathIdBuffer = pool.Rent(pathIdLength + 1);
    Encoding.UTF8.GetBytes(pathId, pathIdBuffer);
    pathIdBuffer[pathIdLength] = 0;
    fixed (byte* pathBufferPtr = pathBuffer) {
      fixed (byte* pathIdBufferPtr = pathIdBuffer) {
        var ret = _IsDirectory(pathBufferPtr, pathIdBufferPtr);
        pool.Return(pathBuffer);
        pool.Return(pathIdBuffer);
        return ret == 1;
      }
    }
  }

  private unsafe static delegate* unmanaged<void> _PrintSearchPaths;

  public unsafe static void PrintSearchPaths() {
    _PrintSearchPaths();
  }
}