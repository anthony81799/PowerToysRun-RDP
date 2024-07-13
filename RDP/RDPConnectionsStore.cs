using System.Collections.Generic;
using System.IO;

namespace Community.PowerToys.Run.Plugin.RDP;

internal class RDPConnectionsStore
{
  private readonly string _storageFile;

  public RDPConnectionsStore(string storageFile)
  {
    _storageFile = storageFile;
  }

  public RDPConnections Load()
  {
    EnsureDirectoryExists();

    var lines = new List<string>();

    using var fileStream = new FileStream(
        _storageFile,
        FileMode.OpenOrCreate,
        FileAccess.Read,
        FileShare.ReadWrite);
    using var reader = new StreamReader(fileStream);

    while (reader.ReadLine() is { } line)
    {
      lines.Add(line);
    }

    return RDPConnections.Create(lines);
  }

  public void Save(RDPConnections rdpConnections)
  {
    EnsureDirectoryExists();

    var lines = rdpConnections.Connections;

    using var fileStream = new FileStream(
        _storageFile,
        FileMode.Create,
        FileAccess.Write,
        FileShare.Read);
    using var writer = new StreamWriter(fileStream);

    foreach (var line in lines)
    {
      writer.WriteLine(line);
    }
  }

  private void EnsureDirectoryExists()
  {
    var directory = Path.GetDirectoryName(_storageFile);
    if (directory is not null)
    {
      Directory.CreateDirectory(directory);
    }
  }
}