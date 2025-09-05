using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.GMS
{
    public class GameModel
    {
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("next")] public string? Next { get; set; }
        [JsonPropertyName("previous")] public string? Previous { get; set; }
        [JsonPropertyName("results")] public List<RawgGame> Results { get; set; } = new();

        // Extra SEO/meta fields present in your JSON
        [JsonPropertyName("seo_title")] public string? SeoTitle { get; set; }
        [JsonPropertyName("seo_description")] public string? SeoDescription { get; set; }
        [JsonPropertyName("seo_keywords")] public string? SeoKeywords { get; set; }
        [JsonPropertyName("seo_h1")] public string? SeoH1 { get; set; }
        [JsonPropertyName("noindex")] public bool? NoIndex { get; set; }
        [JsonPropertyName("nofollow")] public bool? NoFollow { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }

        // Filters block
        [JsonPropertyName("filters")] public RawgFilters? Filters { get; set; }

        // Collections with nofollow
        [JsonPropertyName("nofollow_collections")] public List<string> NoFollowCollections { get; set; } = new();
    }

    public sealed class RawgFilters
    {
        [JsonPropertyName("years")] public List<RawgFilterYearsBucket> Years { get; set; } = new();
    }

    public sealed class RawgFilterYearsBucket
    {
        [JsonPropertyName("from")] public int From { get; set; }
        [JsonPropertyName("to")] public int To { get; set; }

        // e.g. "2020-01-01,2025-12-31"
        [JsonPropertyName("filter")] public string? Filter { get; set; }

        [JsonPropertyName("decade")] public int Decade { get; set; }
        [JsonPropertyName("years")] public List<RawgYearItem> Years { get; set; } = new();

        [JsonPropertyName("nofollow")] public bool? NoFollow { get; set; }
        [JsonPropertyName("count")] public int Count { get; set; }
    }

    public sealed class RawgYearItem
    {
        [JsonPropertyName("year")] public int Year { get; set; }
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("nofollow")] public bool? NoFollow { get; set; }
    }

    public sealed class RawgGame
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("slug")] public string? Slug { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("released")] public string? Released { get; set; } // date-only string
        [JsonPropertyName("tba")] public bool Tba { get; set; }
        [JsonPropertyName("background_image")] public string? BackgroundImage { get; set; }
        [JsonPropertyName("rating")] public double Rating { get; set; }
        [JsonPropertyName("rating_top")] public int RatingTop { get; set; }
        [JsonPropertyName("ratings")] public List<RawgRating> Ratings { get; set; } = new();
        [JsonPropertyName("ratings_count")] public int RatingsCount { get; set; }
        [JsonPropertyName("reviews_text_count")] public int ReviewsTextCount { get; set; }
        [JsonPropertyName("added")] public int Added { get; set; }
        [JsonPropertyName("added_by_status")] public AddedByStatus? AddedByStatus { get; set; }
        [JsonPropertyName("metacritic")] public int? Metacritic { get; set; }
        [JsonPropertyName("playtime")] public int Playtime { get; set; }
        [JsonPropertyName("suggestions_count")] public int SuggestionsCount { get; set; }
        [JsonPropertyName("updated")] public DateTime? Updated { get; set; }
        [JsonPropertyName("user_game")] public JsonElement? UserGame { get; set; }
        [JsonPropertyName("reviews_count")] public int ReviewsCount { get; set; }
        [JsonPropertyName("saturated_color")] public string? SaturatedColor { get; set; }
        [JsonPropertyName("dominant_color")] public string? DominantColor { get; set; }

        [JsonPropertyName("platforms")] public List<PlatformEntry> Platforms { get; set; } = new();
        [JsonPropertyName("parent_platforms")] public List<ParentPlatformRef> ParentPlatforms { get; set; } = new();
        [JsonPropertyName("genres")] public List<Genre> Genres { get; set; } = new();
        [JsonPropertyName("stores")] public List<StoreEntry> Stores { get; set; } = new();

        [JsonPropertyName("clip")] public JsonElement? Clip { get; set; }
        [JsonPropertyName("tags")] public List<Tag> Tags { get; set; } = new();

        [JsonPropertyName("esrb_rating")] public EsrbRating? EsrbRating { get; set; }
        [JsonPropertyName("short_screenshots")] public List<ShortScreenshot> ShortScreenshots { get; set; } = new();
    }

    public sealed class RawgRating
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string? Title { get; set; }
        [JsonPropertyName("count")] public int Count { get; set; }
        [JsonPropertyName("percent")] public double Percent { get; set; }
    }

    public sealed class AddedByStatus
    {
        [JsonPropertyName("yet")] public int? Yet { get; set; }
        [JsonPropertyName("owned")] public int? Owned { get; set; }
        [JsonPropertyName("beaten")] public int? Beaten { get; set; }
        [JsonPropertyName("toplay")] public int? ToPlay { get; set; }
        [JsonPropertyName("dropped")] public int? Dropped { get; set; }
        [JsonPropertyName("playing")] public int? Playing { get; set; }
    }

    public sealed class PlatformEntry
    {
        [JsonPropertyName("platform")] public PlatformDetails? Platform { get; set; }
        [JsonPropertyName("released_at")] public string? ReleasedAt { get; set; }
        [JsonPropertyName("requirements_en")] public Requirements? RequirementsEn { get; set; }
        [JsonPropertyName("requirements_ru")] public Requirements? RequirementsRu { get; set; }
    }

    public sealed class PlatformDetails
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("slug")] public string? Slug { get; set; }
        [JsonPropertyName("image")] public string? Image { get; set; }
        [JsonPropertyName("year_end")] public int? YearEnd { get; set; }
        [JsonPropertyName("year_start")] public int? YearStart { get; set; }
        [JsonPropertyName("games_count")] public int GamesCount { get; set; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; set; }
    }

    public sealed class Requirements
    {
        [JsonPropertyName("minimum")] public string? Minimum { get; set; }
        [JsonPropertyName("recommended")] public string? Recommended { get; set; }
    }

    public sealed class ParentPlatformRef
    {
        [JsonPropertyName("platform")] public PlatformRef? Platform { get; set; }
    }

    public sealed class PlatformRef
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("slug")] public string? Slug { get; set; }
    }

    public sealed class Genre
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("slug")] public string? Slug { get; set; }
        [JsonPropertyName("games_count")] public int GamesCount { get; set; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; set; }
    }

    public sealed class StoreEntry
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("store")] public StoreDetails? Store { get; set; }
    }

    public sealed class StoreDetails
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("slug")] public string? Slug { get; set; }
        [JsonPropertyName("domain")] public string? Domain { get; set; }
        [JsonPropertyName("games_count")] public int GamesCount { get; set; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; set; }
    }

    public sealed class Tag
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("slug")] public string? Slug { get; set; }
        [JsonPropertyName("language")] public string? Language { get; set; }
        [JsonPropertyName("games_count")] public int GamesCount { get; set; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; set; }
    }

    public sealed class EsrbRating
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("slug")] public string? Slug { get; set; }
    }

    public sealed class ShortScreenshot
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("image")] public string? Image { get; set; }
    }
}
