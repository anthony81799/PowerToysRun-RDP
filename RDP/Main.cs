using Community.PowerToys.Run.Plugin.ProcessKiller.Properties;
using System.Windows.Controls;
using Wox.Infrastructure;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.ProcessKiller;

public class Main : IPlugin, IPluginI18n, IReloadable, IDisposable
{
  public static string PluginID => "DF7413853DC54C2287390EE0E0C5BF42";
  private bool _disposed;
  private PluginInitContext _context;
  private RDPConnections _rdpConnections;
  private RDPConnectionsStore _store;
  private SearchPhraseProvider _searchPhraseProvider;

  /// <summary>
  /// initialize the plugin.
  /// </summary>
  /// <param name="context"></param>
  public void Init(PluginInitContext context)
  {
    _context = context;
    _store = new RDPConnectionsStore(Path.Combine(
        context.CurrentPluginMetadata.PluginDirectory,
        "data",
        "connections.txt"));
    _rdpConnections = _store.Load();
    _searchPhraseProvider = new SearchPhraseProvider();
  }

  /// <summary>
  /// return results for the given query, starting with rpd
  /// </summary>
  /// <param name="query">search query provided by PowerToys Run</param>
  /// <returns></returns>
  public List<Result> Query(Query query)
  {
    _searchPhraseProvider.Search = query.Search;
    _rdpConnections.Reload(GetRdpConnectionsFromRegistry());

    var connections = _rdpConnections.FindConnections(query.Search);

    var results = new[] { CreateDefaultResult() }
        .Concat(connections.Select(MapToResult))
        .ToList();

    LogResults(results);

    return results;
  }

  private static IReadOnlyCollection<string> GetRdpConnectionsFromRegistry()
  {
    var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Terminal Server Client\Servers");
    if (key is null)
    {
      return Array.Empty<string>();
    }

    return key.GetSubKeyNames();
  }

  private Result MapToResult((string connection, int score) item) =>
      new()
      {
        // For some reason SubTitle must be unique otherwise score is not respected
        Title = $"{item.connection}",
        SubTitle = $"Connect to {item.connection} via RDP",
        IcoPath = "Images\\screen-mirroring.png",
        Score = item.score,
        Action = c =>
          {
            _rdpConnections.ConnectionWasSelected(item.connection);
            _store.Save(_rdpConnections);

            StartMstsc(item.connection);
            return true;
          }
      };

  private void LogResults(IReadOnlyCollection<Result> results)
  {
    _context.API.LogInfo("RDP", "Results: ");
    foreach (var result in results)
    {
      _context.API.LogInfo("RDP", $"{result.Title} - {result.Score}");
    }
  }

  private Result CreateDefaultResult() =>
      new()
      {
        Title = "RDP",
        SubTitle = "Establish a new RDP connection",
        IcoPath = "Images\\screen-mirroring.png",
        Score = 100,
        Action = c =>
          {
            StartMstsc(_searchPhraseProvider.Search);
            return true;
          }
      };

  private static void StartMstsc(string connection)
  {
    if (string.IsNullOrWhiteSpace(connection))
    {
      Process.Start("mstsc");
    }
    else
    {
      Process.Start("mstsc", "/v:" + connection);
    }
  }

  /// <summary>
  /// In order to have a newest search phrase in Result.Action lambda.
  /// Passing unwrapped Search string to Action for some reason doesn't work,
  /// since it keeps the value of first search which is empty string
  /// </summary>
  private class SearchPhraseProvider
  {
    public string Search { get; set; }
  }

  public void ReloadData()
	{
		if (_context is null)
		{
			return;
		}
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			_disposed = true;
		}
	}
}