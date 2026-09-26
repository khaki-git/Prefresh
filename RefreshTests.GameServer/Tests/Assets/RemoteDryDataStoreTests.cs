using Refresh.Core.Configuration;
using Refresh.Core.Storage;

namespace RefreshTests.GameServer.Tests.Assets;

public class RemoteDryDataStoreTests
{
    [Test]
    public void UsesDryArchiveHashLayout()
    {
        const string hash = "0123456789abcdef0123456789abcdef01234567";

        Assert.That(RemoteDryDataStore.GetAssetPath(hash),
            Is.EqualTo($"dry23r0/dry01.zip/01/23/{hash}"));
        Assert.That(RemoteDryDataStore.GetAssetPath("png/" + hash), Is.Null);
        Assert.That(RemoteDryDataStore.GetAssetPath("../etc/passwd"), Is.Null);
    }

    [Test]
    public void DownloadsAnArchivedAssetFromConfiguredSource()
    {
        const string hash = "0123456789abcdef0123456789abcdef01234567";
        byte[] expected = [1, 2, 3, 4];
        Uri? requestedUri = null;
        using HttpClient client = new(new ArchiveHandler(request =>
        {
            requestedUri = request.RequestUri;
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new ByteArrayContent(expected) };
        }));
        RemoteDryDataStore store = new(new DryArchiveConfig { RemoteBaseUrl = "https://example.org/download" }, client);

        Assert.That(store.ExistsInStore(hash), Is.True);
        Assert.That(store.GetDataFromStore(hash), Is.EqualTo(expected));
        Assert.That(requestedUri?.AbsoluteUri,
            Is.EqualTo($"https://example.org/download/dry23r0/dry01.zip/01/23/{hash}"));
    }

    private sealed class ArchiveHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(respond(request));
    }
}
