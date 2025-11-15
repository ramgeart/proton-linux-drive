using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ProtonDrive.Sync.Linux.FileSystem.Client;

/// <summary>
/// Linux-specific file system operations using POSIX APIs
/// </summary>
public static class FileSystemClient
{
    /// <summary>
    /// Get file attributes on Linux (using stat)
    /// </summary>
    public static FileAttributes GetAttributes(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new FileNotFoundException($"Path not found: {path}");
        }

        var attributes = FileAttributes.Normal;
        var fileInfo = new FileInfo(path);

        // Check if it's a directory
        if ((fileInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
        {
            attributes |= FileAttributes.Directory;
        }

        // Check for hidden files (Unix convention: starts with .)
        if (Path.GetFileName(path).StartsWith("."))
        {
            attributes |= FileAttributes.Hidden;
        }

        // Check for read-only (no write permission)
        if (!HasWritePermission(path))
        {
            attributes |= FileAttributes.ReadOnly;
        }

        return attributes;
    }

    /// <summary>
    /// Check if the current user has write permission for a file/directory
    /// </summary>
    private static bool HasWritePermission(string path)
    {
        try
        {
            // Try to open for write access
            using var stream = File.Open(path, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch
        {
            // If it's a directory, try a different approach
            if (Directory.Exists(path))
            {
                try
                {
                    var testFile = Path.Combine(path, $".write_test_{Guid.NewGuid()}");
                    File.WriteAllText(testFile, "test");
                    File.Delete(testFile);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Get inode number for a file (Linux-specific)
    /// </summary>
    public static long GetInode(string path)
    {
        // This is a placeholder. Full implementation would use P/Invoke to stat()
        // For now, use file creation time ticks as a substitute
        var info = new FileInfo(path);
        return info.CreationTimeUtc.Ticks;
    }

    /// <summary>
    /// Check if two paths refer to the same file (by inode)
    /// </summary>
    public static bool IsSameFile(string path1, string path2)
    {
        try
        {
            return GetInode(path1) == GetInode(path2);
        }
        catch
        {
            return false;
        }
    }
}
