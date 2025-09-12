using System.Text.Json.Serialization;

namespace ZetaDashboard.Common.GMS
{
    public sealed record RawgGameDetail
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("slug")] public string? Slug { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("name_original")] public string? NameOriginal { get; init; }

        // HTML string
        [JsonPropertyName("description")] public string? DescriptionHtml { get; init; }

        [JsonPropertyName("metacritic")] public int? Metacritic { get; init; }

        [JsonPropertyName("metacritic_platforms")] public List<MetacriticPlatform>? MetacriticPlatforms { get; init; }

        // Date-only string in sample; puede ser nulo o formato fecha
        [JsonPropertyName("released")] public string? Released { get; init; }

        [JsonPropertyName("tba")] public bool Tba { get; init; }

        // ISO 8601 datetime
        [JsonPropertyName("updated")] public DateTime? Updated { get; init; }

        [JsonPropertyName("background_image")] public string? BackgroundImage { get; init; }
        [JsonPropertyName("background_image_additional")] public string? BackgroundImageAdditional { get; init; }
        [JsonPropertyName("website")] public string? Website { get; init; }

        [JsonPropertyName("rating")] public double Rating { get; init; }
        [JsonPropertyName("rating_top")] public int? RatingTop { get; init; }

        [JsonPropertyName("ratings")] public List<GameRating>? Ratings { get; init; }

        // Map de id (string) -> count
        [JsonPropertyName("reactions")] public Dictionary<string, int>? Reactions { get; init; }

        [JsonPropertyName("added")] public int? Added { get; init; }

        [JsonPropertyName("added_by_status")] public AddedByStatus? AddedByStatus { get; init; }

        [JsonPropertyName("playtime")] public int? Playtime { get; init; }
        [JsonPropertyName("screenshots_count")] public int? ScreenshotsCount { get; init; }
        [JsonPropertyName("movies_count")] public int? MoviesCount { get; init; }
        [JsonPropertyName("creators_count")] public int? CreatorsCount { get; init; }
        [JsonPropertyName("achievements_count")] public int? AchievementsCount { get; init; }
        [JsonPropertyName("parent_achievements_count")] public int? ParentAchievementsCount { get; init; }

        [JsonPropertyName("reddit_url")] public string? RedditUrl { get; init; }
        [JsonPropertyName("reddit_name")] public string? RedditName { get; init; }
        [JsonPropertyName("reddit_description")] public string? RedditDescription { get; init; }
        [JsonPropertyName("reddit_logo")] public string? RedditLogo { get; init; }
        [JsonPropertyName("reddit_count")] public int? RedditCount { get; init; }
        [JsonPropertyName("twitch_count")] public int? TwitchCount { get; init; }
        [JsonPropertyName("youtube_count")] public int? YoutubeCount { get; init; }
        [JsonPropertyName("reviews_text_count")] public int? ReviewsTextCount { get; init; }
        [JsonPropertyName("ratings_count")] public int? RatingsCount { get; init; }
        [JsonPropertyName("suggestions_count")] public int? SuggestionsCount { get; init; }

        [JsonPropertyName("alternative_names")] public List<string>? AlternativeNames { get; init; }

        [JsonPropertyName("metacritic_url")] public string? MetacriticUrl { get; init; }

        [JsonPropertyName("parents_count")] public int? ParentsCount { get; init; }
        [JsonPropertyName("additions_count")] public int? AdditionsCount { get; init; }
        [JsonPropertyName("game_series_count")] public int? GameSeriesCount { get; init; }

        [JsonPropertyName("user_game")] public object? UserGame { get; init; } // puede ser null u objeto no documentado

        [JsonPropertyName("reviews_count")] public int? ReviewsCount { get; init; }
        [JsonPropertyName("saturated_color")] public string? SaturatedColor { get; init; }
        [JsonPropertyName("dominant_color")] public string? DominantColor { get; init; }

        [JsonPropertyName("parent_platforms")] public List<ParentPlatformWrapper>? ParentPlatforms { get; init; }
        [JsonPropertyName("platforms")] public List<GamePlatform>? Platforms { get; init; }
        [JsonPropertyName("stores")] public List<GameStore>? Stores { get; init; }

        [JsonPropertyName("developers")] public List<EntityRef>? Developers { get; init; }
        [JsonPropertyName("genres")] public List<EntityRef>? Genres { get; init; }
        [JsonPropertyName("tags")] public List<TagRef>? Tags { get; init; }
        [JsonPropertyName("publishers")] public List<EntityRef>? Publishers { get; init; }

        [JsonPropertyName("esrb_rating")] public EsrbRating? EsrbRating { get; init; }

        [JsonPropertyName("clip")] public object? Clip { get; init; } // null en sample

        // Texto plano duplicado del HTML anterior
        [JsonPropertyName("description_raw")] public string? DescriptionRaw { get; init; }
    }

    public sealed record MetacriticPlatform
    {
        [JsonPropertyName("metascore")] public int? Metascore { get; init; }
        [JsonPropertyName("url")] public string? Url { get; init; }
        [JsonPropertyName("platform")] public SimplePlatform? Platform { get; init; }
    }

    public sealed record SimplePlatform
    {
        [JsonPropertyName("platform")] public int? PlatformId { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("slug")] public string? Slug { get; init; }
    }

    public sealed record GameRating
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("title")] public string? Title { get; init; }
        [JsonPropertyName("count")] public int Count { get; init; }
        [JsonPropertyName("percent")] public double Percent { get; init; }
    }


    public sealed record ParentPlatformWrapper
    {
        [JsonPropertyName("platform")] public ParentPlatform? Platform { get; init; }
    }

    public sealed record ParentPlatform
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("slug")] public string? Slug { get; init; }
    }

    public sealed record GamePlatform
    {
        [JsonPropertyName("platform")] public PlatformDetail? Platform { get; init; }

        // released_at es string (fecha), RAWG a veces entrega null
        [JsonPropertyName("released_at")] public string? ReleasedAt { get; init; }

        [JsonPropertyName("requirements")] public PlatformRequirements? Requirements { get; init; }
    }

    public sealed record PlatformDetail
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("slug")] public string? Slug { get; init; }
        [JsonPropertyName("image")] public string? Image { get; init; }
        [JsonPropertyName("year_end")] public int? YearEnd { get; init; }
        [JsonPropertyName("year_start")] public int? YearStart { get; init; }
        [JsonPropertyName("games_count")] public int? GamesCount { get; init; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; init; }
    }

    public sealed record PlatformRequirements
    {
        // RAWG devuelve texto multilinea; lo dejamos tal cual
        [JsonPropertyName("minimum")] public string? Minimum { get; init; }
        [JsonPropertyName("recommended")] public string? Recommended { get; init; }
    }

    public sealed record GameStore
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("url")] public string? Url { get; init; }
        [JsonPropertyName("store")] public StoreRef? Store { get; init; }
    }

    public sealed record StoreRef
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("slug")] public string? Slug { get; init; }
        [JsonPropertyName("domain")] public string? Domain { get; init; }
        [JsonPropertyName("games_count")] public int? GamesCount { get; init; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; init; }
    }

    public sealed record EntityRef
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("slug")] public string? Slug { get; init; }
        [JsonPropertyName("games_count")] public int? GamesCount { get; init; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; init; }
    }

    public sealed record TagRef
    {
        [JsonPropertyName("id")] public int Id { get; init; }
        [JsonPropertyName("name")] public string? Name { get; init; }
        [JsonPropertyName("slug")] public string? Slug { get; init; }
        [JsonPropertyName("language")] public string? Language { get; init; }
        [JsonPropertyName("games_count")] public int? GamesCount { get; init; }
        [JsonPropertyName("image_background")] public string? ImageBackground { get; init; }
    }

    
}
