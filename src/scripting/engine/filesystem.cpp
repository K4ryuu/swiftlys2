/************************************************************************************************
 *  SwiftlyS2 is a scripting framework for Source2-based games.
 *  Copyright (C) 2025 Swiftly Solution SRL via Sava Andrei-Sebastian and it's contributors
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 ************************************************************************************************/

#include <scripting/scripting.h>
#include <api/interfaces/manager.h>

#include <public/filesystem.h>

int Bridge_FileSystem_GetSearchPath(char* out, char* pathId, int32_t searchPathType, int32_t searchPathsToGet)
{
    static auto filesystem = g_ifaceService.FetchInterface<IFileSystem>(FILESYSTEM_INTERFACE_VERSION);

    CBufferStringGrowable<MAX_PATH> searchPath;
    filesystem->GetSearchPath(pathId, (GetSearchPathTypes_t)searchPathType, searchPath, searchPathsToGet);

    std::string result = searchPath.Get();

    if (out) strcpy(out, result.c_str());

    return result.size();
}

bool Bridge_FileSystem_FileExists(char* fileName, char* pathId)
{
    static auto filesystem = g_ifaceService.FetchInterface<IFileSystem>(FILESYSTEM_INTERFACE_VERSION);

    return filesystem->FileExists(fileName, pathId);
}

void Bridge_FileSystem_AddSearchPath(char* path, char* pathId, int32_t searchPathAdd, int32_t searchPathPriority)
{
    static auto filesystem = g_ifaceService.FetchInterface<IFileSystem>(FILESYSTEM_INTERFACE_VERSION);

    filesystem->AddSearchPath(path, pathId, (SearchPathAdd_t)searchPathAdd, (SearchPathPriority_t)searchPathPriority, 0);
}

bool Bridge_FileSystem_RemoveSearchPath(char* path, char* pathId)
{
    static auto filesystem = g_ifaceService.FetchInterface<IFileSystem>(FILESYSTEM_INTERFACE_VERSION);

    return filesystem->RemoveSearchPath(path, pathId);
}

bool Bridge_FileSystem_IsDirectory(char* path, char* pathId)
{
    static auto filesystem = g_ifaceService.FetchInterface<IFileSystem>(FILESYSTEM_INTERFACE_VERSION);

    return filesystem->IsDirectory(path, pathId);
}

void Bridge_FileSystem_PrintSearchPaths()
{
    static auto filesystem = g_ifaceService.FetchInterface<IFileSystem>(FILESYSTEM_INTERFACE_VERSION);

    filesystem->PrintSearchPaths();
}

DEFINE_NATIVE("FileSystem.GetSearchPath", Bridge_FileSystem_GetSearchPath);
DEFINE_NATIVE("FileSystem.FileExists", Bridge_FileSystem_FileExists);
DEFINE_NATIVE("FileSystem.AddSearchPath", Bridge_FileSystem_AddSearchPath);
DEFINE_NATIVE("FileSystem.RemoveSearchPath", Bridge_FileSystem_RemoveSearchPath);
DEFINE_NATIVE("FileSystem.IsDirectory", Bridge_FileSystem_IsDirectory);
DEFINE_NATIVE("FileSystem.PrintSearchPaths", Bridge_FileSystem_PrintSearchPaths);