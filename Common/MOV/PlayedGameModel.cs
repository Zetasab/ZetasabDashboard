using ZetaDashboard.Common.GMS;

namespace ZetaDashboard.Common.MOV
{
    public class PlayedGameModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public List<RawgGame> Games { get; set; } = new List<RawgGame>();

        public PlayedGameModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
