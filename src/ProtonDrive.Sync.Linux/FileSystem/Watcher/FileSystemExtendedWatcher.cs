using System;
using System.IO;

namespace ProtonDrive.Sync.Linux.FileSystem.Watcher;

/// <summary>
/// Linux file system watcher using .NET FileSystemWatcher with inotify backend
/// </summary>
public class FileSystemExtendedWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    private bool _disposed;

    public FileSystemExtendedWatcher(string path)
    {
        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException($"Directory not found: {path}");
        }

        _watcher = new FileSystemWatcher(path)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName 
                         | NotifyFilters.DirectoryName 
                         | NotifyFilters.LastWrite
                         | NotifyFilters.Size
                         | NotifyFilters.Attributes
        };

        _watcher.Created += OnCreated;
        _watcher.Changed += OnChanged;
        _watcher.Deleted += OnDeleted;
        _watcher.Renamed += OnRenamed;
        _watcher.Error += OnError;
    }

    public event EventHandler<FileSystemEventArgs>? Created;
    public event EventHandler<FileSystemEventArgs>? Changed;
    public event EventHandler<FileSystemEventArgs>? Deleted;
    public event EventHandler<RenamedEventArgs>? Renamed;
    public event EventHandler<ErrorEventArgs>? Error;

    public bool EnableRaisingEvents
    {
        get => _watcher.EnableRaisingEvents;
        set => _watcher.EnableRaisingEvents = value;
    }

    public string Path
    {
        get => _watcher.Path;
        set => _watcher.Path = value;
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        Created?.Invoke(this, e);
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        Changed?.Invoke(this, e);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        Deleted?.Invoke(this, e);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        Renamed?.Invoke(this, e);
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        Error?.Invoke(this, e);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _watcher.Dispose();
        _disposed = true;
    }
}
