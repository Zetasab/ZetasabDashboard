namespace ZetaDashboard.Data.MOV
{
    public static class MovieData
    {
        #region Genres
        public static List<Genre> Genres { get; set; } = new List<Genre>()
        {
            new Genre { Id = 28, Esp_Name = "Acción" , Name = "Action"},
            new Genre { Id = 12, Esp_Name = "Aventura" , Name = "Adventure"},
            new Genre { Id = 16, Esp_Name = "Animación" , Name = "Animation"},
            new Genre { Id = 35, Esp_Name = "Comedia" , Name = "Comedy"},
            new Genre { Id = 80, Esp_Name = "Crimen" , Name = "Crime"},
            new Genre { Id = 99, Esp_Name = "Documental" , Name = "Documentary"},
            new Genre { Id = 18, Esp_Name = "Drama" , Name = "Drama"},
            new Genre { Id = 10751, Esp_Name = "Familia" , Name = "Family"},
            new Genre { Id = 14, Esp_Name = "Fantasía" , Name = "Fantasy"},
            new Genre { Id = 36, Esp_Name = "Historia" , Name = "History"},
            new Genre { Id = 27, Esp_Name = "Terror" , Name = "Horror"},
            new Genre { Id = 10402, Esp_Name = "Música" , Name = "Music"},
            new Genre { Id = 9648, Esp_Name = "Misterio" , Name = "Mystery"},
            new Genre { Id = 10749, Esp_Name = "Romance" , Name = "Romance"},
            new Genre { Id = 878, Esp_Name= "Ciencia ficción" , Name = "Science Fiction"},
            new Genre { Id = 10770, Esp_Name = "Película de TV" , Name = "TV Movie"},
            new Genre { Id = 53, Esp_Name = "Suspense" , Name = "Thriller"},
            new Genre { Id = 10752, Esp_Name = "Bélica" , Name = "War"},
            new Genre { Id = 37, Esp_Name = "Western", Name = "Western" }
        };
        #endregion
        #region Watch_providers
        public static List<Watch_provider> WhatProviders { get; set; } = new List<Watch_provider>
        {
            new Watch_provider{ Display_priority = 4, Logo_path = "/pbpMk2JmcoNnQwx5JGpXngfoWtp.jpg", Provider_name = "Netflix", provider_id = 8},
            new Watch_provider{ Display_priority = 1, Logo_path = "/97yvRBw1GzX7fXprcF80er19ot.jpg", Provider_name = "Disney Plus", provider_id = 337},
            new Watch_provider{ Display_priority = 2, Logo_path = "/pvske1MyAoymrs5bguRfVqYiM9a.jpg", Provider_name = "Amazon Prime Video", provider_id = 9},
            new Watch_provider{ Display_priority = 27, Logo_path = "/jbe4gVSfRlbPTdESXhEKpornsfu.jpg", Provider_name = "HBO Max", provider_id = 1899},
        };
        #endregion

        #region SortBy

        public static List<Order> SortsBy { get; set; } = new List<Order>
        {
             new Order{ eng_order = "original_title.asc", esp_order = "Nombre (A-Z)" },
            new Order{ eng_order = "original_title.desc", esp_order = "Nombre (Z-A)" },

            new Order{ eng_order = "popularity.asc", esp_order = "Menor popularidad" },
            new Order{ eng_order = "popularity.desc", esp_order = "Mayor popularidad" },

            new Order{ eng_order = "revenue.asc", esp_order = "Menor recaudación" },
            new Order{ eng_order = "revenue.desc", esp_order = "Mayor recaudación" },

            new Order{ eng_order = "primary_release_date.asc", esp_order = "Estreno más antiguo" },
            new Order{ eng_order = "primary_release_date.desc", esp_order = "Estreno más reciente" },

            new Order{ eng_order = "title.asc", esp_order = "Título (A-Z)" },
            new Order{ eng_order = "title.desc", esp_order = "Título (Z-A)" },

            new Order{ eng_order = "vote_average.asc", esp_order = "Peor valoración" },
            new Order{ eng_order = "vote_average.desc", esp_order = "Mejor valoración" },

            new Order{ eng_order = "vote_count.asc", esp_order = "Menos votos" },
            new Order{ eng_order = "vote_count.desc", esp_order = "Más votos" }
        };
        #endregion
    }
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Esp_Name { get; set; }
    }

    public class Order
    {
        public string eng_order { get; set; }
        public string esp_order { get; set; }
    }
    public class Watch_provider
    {
        public int Display_priority { get; set; }
        public string Logo_path { get; set; }

        public string Provider_name { get; set; }
         
        public int provider_id { get; set; }
    }

}
