namespace ZetaDashboard.Data.MOV
{
    public static class MovieData
    {
        public static List<Genre> Genres { get; set; } = new List<Genre>()
        {
            new Genre { Id = 28, Name = "Acción" },
            new Genre { Id = 12, Name = "Aventura" },
            new Genre { Id = 16, Name = "Animación" },
            new Genre { Id = 35, Name = "Comedia" },
            new Genre { Id = 80, Name = "Crimen" },
            new Genre { Id = 99, Name = "Documental" },
            new Genre { Id = 18, Name = "Drama" },
            new Genre { Id = 10751, Name = "Familia" },
            new Genre { Id = 14, Name = "Fantasía" },
            new Genre { Id = 36, Name = "Historia" },
            new Genre { Id = 27, Name = "Terror" },
            new Genre { Id = 10402, Name = "Música" },
            new Genre { Id = 9648, Name = "Misterio" },
            new Genre { Id = 10749, Name = "Romance" },
            new Genre { Id = 878, Name = "Ciencia ficción" },
            new Genre { Id = 10770, Name = "Película de TV" },
            new Genre { Id = 53, Name = "Suspense" },
            new Genre { Id = 10752, Name = "Bélica" },
            new Genre { Id = 37, Name = "Western" }
        };

        public static List<Watch_provider> WhatProviders { get; set; } = new List<Watch_provider>
        {
            new Watch_provider{ Display_priority = 4, Logo_path = "/pbpMk2JmcoNnQwx5JGpXngfoWtp.jpg", Provider_name = "Netflix", provider_id = 8},
            new Watch_provider{ Display_priority = 1, Logo_path = "/97yvRBw1GzX7fXprcF80er19ot.jpg", Provider_name = "Disney Plus", provider_id = 337},
            new Watch_provider{ Display_priority = 2, Logo_path = "/pvske1MyAoymrs5bguRfVqYiM9a.jpg", Provider_name = "Amazon Prime Video", provider_id = 9},
            new Watch_provider{ Display_priority = 27, Logo_path = "/jbe4gVSfRlbPTdESXhEKpornsfu.jpg", Provider_name = "HBO Max", provider_id = 1899},
        };
    }
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Watch_provider
    {
        public int Display_priority { get; set; }
        public string Logo_path { get; set; }

        public string Provider_name { get; set; }
         
        public int provider_id { get; set; }
    }
}
