using Bunkum.Core.Storage;
using Refresh.Common.Verification;
using Refresh.Core.Configuration;

namespace Refresh.Core.Storage;

public sealed class RemoteDryDataStore : IDataStore
{
    private const string WriteError = "The remote dry archive is read-only.";

    private readonly HttpClient _client;
    private readonly Uri _baseUri;

    public RemoteDryDataStore(DryArchiveConfig config, HttpClient? client = null)
    {
        this._baseUri = new Uri(config.RemoteBaseUrl.TrimEnd('/') + "/", UriKind.Absolute);
        this._client = client ?? new HttpClient();
        this._client.Timeout = TimeSpan.FromSeconds(30);
    }

    public static string? GetAssetPath(string key)
    {
        string hash = key.ToLowerInvariant();
        if (!CommonPatterns.Sha1Regex().IsMatch(hash)) return null;

        return $"dry23r{hash[0]}/dry{hash[..2]}.zip/{hash[..2]}/{hash.Substring(2, 2)}/{hash}";
    }

    private Uri GetAssetUri(string key)
    {
        string path = GetAssetPath(key) ?? throw new FormatException("The asset hash was invalid.");
        return new Uri(this._baseUri, path);
    }

    public bool ExistsInStore(string key)
    {
        string? path = GetAssetPath(key);
        if (path == null) return false;

        using HttpResponseMessage response = this._client.GetAsync(new Uri(this._baseUri, path), HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
        return response.IsSuccessStatusCode;
    }

    public byte[] GetDataFromStore(string key)
    {
        using HttpResponseMessage response = this._client.GetAsync(this.GetAssetUri(key), HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
    }

    public Stream GetStreamFromStore(string key) => new MemoryStream(this.GetDataFromStore(key));
    public string[] GetKeysFromStore() => [];

    public bool WriteToStore(string key, byte[] data) => throw new InvalidOperationException(WriteError);
    public bool WriteToStoreFromStream(string key, Stream data) => throw new InvalidOperationException(WriteError);
    public Stream OpenWriteStream(string key) => throw new InvalidOperationException(WriteError);
    public bool RemoveFromStore(string key) => throw new InvalidOperationException(WriteError);
}
