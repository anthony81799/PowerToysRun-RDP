using System;
using System.Collections.Generic;
using System.Linq;

namespace Community.PowerToys.Run.Plugin.RDP;

internal class RDPConnections
{
  private readonly List<string> _connections;

  private RDPConnections(IEnumerable<string> connections)
  {
    _connections = connections.ToList();
  }

  public static RDPConnections Create(IEnumerable<string> connections) => new(connections);

  public IReadOnlyCollection<string> Connections => _connections;

  public void Reload(IReadOnlyCollection<string> rdpConnections)
  {
    var newConnections = rdpConnections.Where(x => !_connections.Contains(x)).ToList();
    var oldConnections = _connections.Where(x => !rdpConnections.Contains(x)).ToList();

    foreach (var oldConnection in oldConnections)
    {
      _connections.Remove(oldConnection);
    }

    _connections.AddRange(newConnections);
  }

  public void ConnectionWasSelected(string connection)
  {
    var index = _connections.IndexOf(connection);
    if (index == -1)
    {
      return;
    }

    _connections.RemoveAt(index);
    _connections.Insert(0, connection);
  }

  public IReadOnlyCollection<(string Connection, int Score)> FindConnections(string querySearch)
  {
    if (string.IsNullOrWhiteSpace(querySearch))
    {
      return _connections
          .Select(MapToScore)
          .ToList();
    }

    return _connections
        .Where(x => x.Contains(querySearch, StringComparison.InvariantCultureIgnoreCase))
        .Select(MapToScore)
        .ToList();
  }

  private (string connection, int score) MapToScore(string x, int i) => (connection: x, score: _connections.Count + 1 - i);
}