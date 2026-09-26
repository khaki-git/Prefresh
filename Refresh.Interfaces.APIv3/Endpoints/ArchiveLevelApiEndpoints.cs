using AttribDoc.Attributes;
using Bunkum.Core;
using Bunkum.Core.Endpoints;
using Bunkum.Core.RateLimit;
using Bunkum.Protocols.Http;
using Refresh.Core.Configuration;
using Refresh.Core.Services;
using Refresh.Core.Storage;
using Refresh.Core.Types.Data;
using Refresh.Database.Models.Users;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes;
using Refresh.Interfaces.APIv3.Endpoints.ApiTypes.Errors;
using Refresh.Interfaces.APIv3.Endpoints.DataTypes;

namespace Refresh.Interfaces.APIv3.Endpoints;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public sealed class ApiArchiveLevelResponse : IApiResponse
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string RootHash { get; init; }
    public required int Game { get; init; }

    public static ApiArchiveLevelResponse FromRecord(ArchiveLevelRecord level) => new()
    {
        Id = level.Id,
        Title = level.Title,
        Author = level.Author,
        RootHash = level.RootHash,
        Game = level.Game,
    };
}

public class ArchiveLevelApiEndpoints : EndpointGroup
{
    [ApiV3Endpoint("archive/levels/search/{query}"), Authentication(false)]
    [DocSummary("Searches preserved LittleBigPlanet levels by title or creator")]
    [RateLimitSettings(60, 20, 60, "archive-level-search")]
    public ApiListResponse<ApiArchiveLevelResponse> SearchArchiveLevels(
        DryArchiveConfig config, string query)
    {
        ArchiveLevelCatalog catalog = new(config.MetadataPath);
        if (!config.Enabled || !catalog.IsAvailable)
            return new ApiListResponse<ApiArchiveLevelResponse>(
                new ApiValidationError("The archive catalog is not available on this server."));

        return new ApiListResponse<ApiArchiveLevelResponse>(
            catalog.Search(query).Select(ApiArchiveLevelResponse.FromRecord));
    }

    [ApiV3Endpoint("archive/levels/{id}/play", HttpMethods.Post)]
    [DocSummary("Opens a preserved level in the connected game")]
    [RateLimitSettings(60, 10, 60, "archive-level-play")]
    public ApiOkResponse PlayArchiveLevel(
        DryArchiveConfig config, GameUser user, PlayNowService playNow, long id)
    {
        ArchiveLevelCatalog catalog = new(config.MetadataPath);
        if (!config.Enabled || !catalog.IsAvailable)
            return new ApiValidationError("The archive catalog is not available on this server.");

        ArchiveLevelRecord? level = catalog.FindById(id);
        if (level == null)
            return ApiNotFoundError.LevelMissingError;

        playNow.PlayNowHash(user, level.RootHash);
        return new ApiOkResponse();
    }
}
