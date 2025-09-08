using ZetaDashboard.Common.GMS;

namespace ZetaDashboard.Common.MOV
{
    public class LikedGameModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public List<RawgGame> Games { get; set; } = new List<RawgGame>();

        public LikedGameModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
