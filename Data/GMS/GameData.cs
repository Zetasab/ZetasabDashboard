using ZetaDashboard.Common.GMS;

namespace ZetaDashboard.Data.GMS
{
    public class GameData
    {
        #region GenreList
        public static List<GameGenre> Games_Genres = new List<GameGenre>() 
        {
            new GameGenre { Id = 4, Name = "Action", Name_esp ="Acción", Slug = "action", GamesCount = 189491, ImageBackground = "https://media.rawg.io/media/games/942/9424d6bb763dc38d9378b488603c87fa.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background1.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon1.png" },
            new GameGenre { Id = 51, Name = "Indie",Name_esp ="Indie", Slug = "indie", GamesCount = 82589, ImageBackground = "https://media.rawg.io/media/games/baf/baf9905270314e07e6850cffdb51df41.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background32.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon32.png" },
            new GameGenre { Id = 3, Name = "Adventure",Name_esp ="Aventura", Slug = "adventure", GamesCount = 149581, ImageBackground = "https://media.rawg.io/media/games/253/2534a46f3da7fa7c315f1387515ca393.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background4.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon4.png" },
            new GameGenre { Id = 5, Name = "RPG",Name_esp ="RPG", Slug = "role-playing-games-rpg", GamesCount = 60899, ImageBackground = "https://media.rawg.io/media/games/530/5302dd22a190e664531236ca724e8726.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background11.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon11.png" },
            new GameGenre { Id = 10, Name = "Strategy",Name_esp ="Estrategia", Slug = "strategy", GamesCount = 61374, ImageBackground = "https://media.rawg.io/media/games/40a/40ab95c1639aa1d7ec04d4cd523af6b1.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background17.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon17.png" },
            new GameGenre { Id = 2, Name = "Shooter",Name_esp ="Shooter", Slug = "shooter", GamesCount = 59592, ImageBackground = "https://media.rawg.io/media/games/c80/c80bcf321da44d69b18a06c04d942662.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background9.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon9.png" },
            new GameGenre { Id = 40, Name = "Casual",Name_esp ="Casual", Slug = "casual", GamesCount = 65170, ImageBackground = "https://media.rawg.io/media/games/270/270b412b66688081497b3d70c100b208.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background47.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon47.png" },
            new GameGenre { Id = 14, Name = "Simulation",Name_esp ="Simulación", Slug = "simulation", GamesCount = 75460, ImageBackground = "https://media.rawg.io/media/games/270/270b412b66688081497b3d70c100b208.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background15.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon15.png" },
            new GameGenre { Id = 7, Name = "Puzzle",Name_esp ="Puzzle", Slug = "puzzle", GamesCount = 97354, ImageBackground = "https://media.rawg.io/media/games/d5a/d5a24f9f71315427fa6e966fdd98dfa6.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background13.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon13.png" },
            new GameGenre { Id = 11, Name = "Arcade",Name_esp ="Arcade", Slug = "arcade", GamesCount = 22665, ImageBackground = "https://media.rawg.io/media/games/a5a/a5abaa1b5cc1567b026fa3aa9fbd828e.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background2.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon2.png" },
            new GameGenre { Id = 83, Name = "Platformer",Name_esp ="Platformer", Slug = "platformer", GamesCount = 100897, ImageBackground = "https://media.rawg.io/media/games/f46/f466571d536f2e3ea9e815ad17177501.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background18.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon18.png" },
            new GameGenre { Id = 59, Name = "Massively Multiplayer", Name_esp ="Multijugador masivo", Slug = "massively-multiplayer", GamesCount = 4163, ImageBackground = "https://media.rawg.io/media/games/b69/b69a67833630dd96d8eee9d2c8c27574.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background54.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon54.png" },
            new GameGenre { Id = 1, Name = "Racing",Name_esp ="Carreras", Slug = "racing", GamesCount = 25625, ImageBackground = "https://media.rawg.io/media/games/662/66261db966238da20c306c4b78ae4603.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background8.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon8.png" },
            new GameGenre { Id = 15, Name = "Sports",Name_esp ="Deportes", Slug = "sports", GamesCount = 22437, ImageBackground = "https://media.rawg.io/media/screenshots/1be/1be2141edae05d4ba9858182b081e604.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background16.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon16.png" },
            new GameGenre { Id = 6, Name = "Fighting",Name_esp ="Peleas", Slug = "fighting", GamesCount = 11770, ImageBackground = "https://media.rawg.io/media/games/684/684ecc08397479de72c5f89ef6f16f4f.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background7.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon7.png" },
            new GameGenre { Id = 19, Name = "Family",Name_esp ="Familia", Slug = "family", GamesCount = 5408, ImageBackground = "https://media.rawg.io/media/games/a87/a8743bdee8627c55bb9f2f01b9136ac1.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background10.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon10.png" },
            new GameGenre { Id = 28, Name = "Board Games", Name_esp ="Juegos de mesa", Slug = "board-games", GamesCount = 8389, ImageBackground = "https://media.rawg.io/media/screenshots/8ff/8ffe8f19d2e764867c8ed625ddf4e368.jpg", ImageBackground2 = "", ImageBackgroundShort = "" },
            new GameGenre { Id = 17, Name = "Card",Name_esp ="Cartas", Slug = "card", GamesCount = 4537, ImageBackground = "https://media.rawg.io/media/screenshots/bf8/bf87ef7d08a80006f0f65df6d30174e6.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background23.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon23.png" },
            new GameGenre { Id = 34, Name = "Educational",Name_esp ="Educacional", Slug = "educational", GamesCount = 15694, ImageBackground = "https://media.rawg.io/media/screenshots/49d/49dae660a0fc843b23d63af8ce34e33c.jpg", ImageBackground2 = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-background53.jpg", ImageBackgroundShort = "https://www.instant-gaming.com/themes/igv2/modules/categoryMenu/images/category-icon53.png" }
        };
        #endregion

        #region PlatformList
        public static List<PlatformCategory> Platforms = new List<PlatformCategory>()
        {
            new PlatformCategory
            {
                Id = 1,
                Name = "PC",
                Slug = "pc",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail
                    {
                        Id = 4,
                        Name = "PC",
                        Slug = "pc",
                        GamesCount = 555182,
                        ImageBackground = "https://media.rawg.io/media/games/021/021c4e21a1824d2526f925eff6324653.jpg",
                        Image = null,
                        YearStart = null,
                        YearEnd = null
                    }
        }
            },
            new PlatformCategory
            {
                Id = 2,
                Name = "PlayStation",
                Slug = "playstation",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail { Id = 187, Name = "PlayStation 5", Slug = "playstation5", GamesCount = 1329, ImageBackground = "https://media.rawg.io/media/games/709/709bf81f874ce5d25d625b37b014cb63.jpg", Image = null, YearStart = 2020, YearEnd = null },
                    new PlatformDetail { Id = 18, Name = "PlayStation 4", Slug = "playstation4", GamesCount = 6935, ImageBackground = "https://media.rawg.io/media/games/be0/be01c3d7d8795a45615da139322ca080.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 16, Name = "PlayStation 3", Slug = "playstation3", GamesCount = 3164, ImageBackground = "https://media.rawg.io/media/games/2ba/2bac0e87cf45e5b508f227d281c9252a.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 15, Name = "PlayStation 2", Slug = "playstation2", GamesCount = 2068, ImageBackground = "https://media.rawg.io/media/games/615/615e9fc0a325e0d87b84dad029b8b7b9.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 27, Name = "PlayStation", Slug = "playstation1", GamesCount = 1682, ImageBackground = "https://media.rawg.io/media/games/8fc/8fcc2ff5c7bcdb58199b1a4326817ceb.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 19, Name = "PS Vita", Slug = "ps-vita", GamesCount = 1453, ImageBackground = "https://media.rawg.io/media/games/be0/be084b850302abe81675bc4ffc08a0d0.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 17, Name = "PSP", Slug = "psp", GamesCount = 1376, ImageBackground = "https://media.rawg.io/media/games/172/172198bbcf9680d00fb621b50cbe630c.jpg", Image = null, YearStart = null, YearEnd = null }
                }
            },
            new PlatformCategory
            {
                Id = 3,
                Name = "Xbox",
                Slug = "xbox",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail { Id = 1, Name = "Xbox One", Slug = "xbox-one", GamesCount = 5704, ImageBackground = "https://media.rawg.io/media/games/da1/da1b267764d77221f07a4386b6548e5a.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 186, Name = "Xbox Series S/X", Slug = "xbox-series-x", GamesCount = 1143, ImageBackground = "https://media.rawg.io/media/games/6cc/6cc23249972a427f697a3d10eb57a820.jpg", Image = null, YearStart = 2020, YearEnd = null },
                    new PlatformDetail { Id = 14, Name = "Xbox 360", Slug = "xbox360", GamesCount = 2806, ImageBackground = "https://media.rawg.io/media/games/c80/c80bcf321da44d69b18a06c04d942662.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 80, Name = "Xbox", Slug = "xbox-old", GamesCount = 744, ImageBackground = "https://media.rawg.io/media/screenshots/ca0/ca06700d8184f451b99396c23b4ffbe4.jpg", Image = null, YearStart = null, YearEnd = null }
                }
            },
            new PlatformCategory
            {
                Id = 4,
                Name = "iOS",
                Slug = "ios",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail { Id = 3, Name = "iOS", Slug = "ios", GamesCount = 77406, ImageBackground = "https://media.rawg.io/media/games/af7/af7a831001c5c32c46e950cc883b8cb7.jpg", Image = null, YearStart = null, YearEnd = null }
                }
            },
            new PlatformCategory
            {
                Id = 8,
                Name = "Android",
                Slug = "android",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail { Id = 21, Name = "Android", Slug = "android", GamesCount = 52470, ImageBackground = "https://media.rawg.io/media/games/be0/be084b850302abe81675bc4ffc08a0d0.jpg", Image = null, YearStart = null, YearEnd = null }
                }
            },
            new PlatformCategory
            {
                Id = 5,
                Name = "Apple Macintosh",
                Slug = "mac",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail { Id = 5, Name = "macOS", Slug = "macos", GamesCount = 107240, ImageBackground = "https://media.rawg.io/media/games/b8c/b8c243eaa0fbac8115e0cdccac3f91dc.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 55, Name = "Classic Macintosh", Slug = "macintosh", GamesCount = 674, ImageBackground = "https://media.rawg.io/media/games/0c5/0c5fcdf04048200da14b90e0e6cfaf6b.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 41, Name = "Apple II", Slug = "apple-ii", GamesCount = 424, ImageBackground = "https://media.rawg.io/media/games/941/94139518bc51a86b9e1b762e0b8b62c8.jpg", Image = null, YearStart = null, YearEnd = null }
                }
            },
            new PlatformCategory
            {
                Id = 6,
                Name = "Linux",
                Slug = "linux",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail { Id = 6, Name = "Linux", Slug = "linux", GamesCount = 79711, ImageBackground = "https://media.rawg.io/media/games/562/562553814dd54e001a541e4ee83a591c.jpg", Image = null, YearStart = null, YearEnd = null }
                }
            },
            new PlatformCategory
            {
                Id = 7,
                Name = "Nintendo",
                Slug = "nintendo",
                Platforms = new List<PlatformDetail>
                {
                    new PlatformDetail { Id = 7, Name = "Nintendo Switch", Slug = "nintendo-switch", GamesCount = 5689, ImageBackground = "https://media.rawg.io/media/games/737/737ea5662211d2e0bbd6f5989189e4f1.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 8, Name = "Nintendo 3DS", Slug = "nintendo-3ds", GamesCount = 1682, ImageBackground = "https://media.rawg.io/media/games/0a5/0a5085a3fbe26be9cf9f96ff8c12746d.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 9, Name = "Nintendo DS", Slug = "nintendo-ds", GamesCount = 2485, ImageBackground = "https://media.rawg.io/media/games/dc6/dc68ca77e06ad993aade7faf645f5ec2.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 13, Name = "Nintendo DSi", Slug = "nintendo-dsi", GamesCount = 37, ImageBackground = "https://media.rawg.io/media/screenshots/078/078629e997421ca28e9098bd7a87cb10.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 10, Name = "Wii U", Slug = "wii-u", GamesCount = 1114, ImageBackground = "https://media.rawg.io/media/games/1f1/1f1888e1308959dfd3be4c144a81d19c.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 11, Name = "Wii", Slug = "wii", GamesCount = 2208, ImageBackground = "https://media.rawg.io/media/screenshots/157/1571cdfb52888191eabaf53c2b897240.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 105, Name = "GameCube", Slug = "gamecube", GamesCount = 636, ImageBackground = "https://media.rawg.io/media/games/d9b/d9bbb8e69f53c4c42b8ff928cb581548.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 83, Name = "Nintendo 64", Slug = "nintendo-64", GamesCount = 363, ImageBackground = "https://media.rawg.io/media/screenshots/f7a/f7a19f45bbe8dfb2960ce167ad879d3e.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 24, Name = "Game Boy Advance", Slug = "game-boy-advance", GamesCount = 956, ImageBackground = "https://media.rawg.io/media/screenshots/33c/33c4f185c9f312cfcf5243d496178b11.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 43, Name = "Game Boy Color", Slug = "game-boy-color", GamesCount = 428, ImageBackground = "https://media.rawg.io/media/screenshots/c38/c381fe2913f790fc4d66620e8add37b0.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 26, Name = "Game Boy", Slug = "game-boy", GamesCount = 617, ImageBackground = "https://media.rawg.io/media/games/057/0573c1c9e1f2414c1f4acabe86ee9fd9.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 79, Name = "SNES", Slug = "snes", GamesCount = 987, ImageBackground = "https://media.rawg.io/media/screenshots/1b2/1b2b82bd26350d64c3d057c0206010ae.jpg", Image = null, YearStart = null, YearEnd = null },
                    new PlatformDetail { Id = 49, Name = "NES", Slug = "nes", GamesCount = 1024, ImageBackground = "https://media.rawg.io/media/games/f78/f7809ab885f7464845681e5931aabeb8.jpg", Image = null, YearStart = null, YearEnd = null }
                }
            },
        };
        #endregion

        #region TagsList
        public static List<GameTag> TagList = new List<GameTag>
        {
            new GameTag
            {
                Id = 31,
                Name = "Singleplayer",
                Slug = "singleplayer",
                GamesCount = 245944,
                ImageBackground = "https://media.rawg.io/media/games/b8c/b8c243eaa0fbac8115e0cdccac3f91dc.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 40847,
                Name = "Steam Achievements",
                Slug = "steam-achievements",
                GamesCount = 48951,
                ImageBackground = "https://media.rawg.io/media/games/da1/da1b267764d77221f07a4386b6548e5a.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 7,
                Name = "Multiplayer",
                Slug = "multiplayer",
                GamesCount = 41730,
                ImageBackground = "https://media.rawg.io/media/games/da1/da1b267764d77221f07a4386b6548e5a.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 40836,
                Name = "Full controller support",
                Slug = "full-controller-support",
                GamesCount = 22971,
                ImageBackground = "https://media.rawg.io/media/games/736/73619bd336c894d6941d926bfd563946.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 40849,
                Name = "Steam Cloud",
                Slug = "steam-cloud",
                GamesCount = 24156,
                ImageBackground = "https://media.rawg.io/media/games/120/1201a40e4364557b124392ee50317b99.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 13,
                Name = "Atmospheric",
                Slug = "atmospheric",
                GamesCount = 38475,
                ImageBackground = "https://media.rawg.io/media/games/490/49016e06ae2103881ff6373248843069.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 7808,
                Name = "steam-trading-cards",
                Slug = "steam-trading-cards",
                GamesCount = 7568,
                ImageBackground = "https://media.rawg.io/media/games/48c/48cb04ca483be865e3a83119c94e6097.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 42,
                Name = "Great Soundtrack",
                Slug = "great-soundtrack",
                GamesCount = 3437,
                ImageBackground = "https://media.rawg.io/media/games/7cf/7cfc9220b401b7a300e409e539c9afd5.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 24,
                Name = "RPG",
                Slug = "rpg",
                GamesCount = 25603,
                ImageBackground = "https://media.rawg.io/media/games/713/713269608dc8f2f40f5a670a14b2de94.jpg",
                Language = "eng"
            },
            new GameTag
            {
                Id = 18,
                Name = "Co-op",
                Slug = "co-op",
                GamesCount = 13866,
                ImageBackground = "https://media.rawg.io/media/games/20a/20aa03a10cda45239fe22d035c0ebe64.jpg",
                Language = "eng"
            }
        };
        #endregion
    }

    #region TagModel
    public class GameTag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int GamesCount { get; set; }
        public string ImageBackground { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
    }
    #endregion
    #region PlatformModel
    public class PlatformCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public List<PlatformDetail> Platforms { get; set; } = new();
    }
    public class PlatformDetail
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int GamesCount { get; set; }
        public string? ImageBackground { get; set; }
        public string? Image { get; set; }
        public int? YearStart { get; set; }
        public int? YearEnd { get; set; }
    }
    #endregion

    #region GenreModel
    public class GameGenre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Name_esp { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int GamesCount { get; set; }
        public string ImageBackground { get; set; } = string.Empty;

        public string ImageBackground2 { get; set; } = string.Empty;
        public string ImageBackgroundShort {  get; set; } = string.Empty;

    }
    #endregion
}
