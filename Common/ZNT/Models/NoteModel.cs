using MongoDB.Bson;

namespace ZetaDashboard.Common.ZNT.Models
{
    public class NoteModel
    {
        public string Id { get; set; }

        public string UserId { get; set; } 

        public string Title { get; set; } 

        public string Content { get; set; } = ""; 

        public bool IsPinned { get; set; } = false; 

        public bool IsFavorite { get; set; } = false;
        public bool IsArchivaded { get; set; } = false;

        public bool IsDeleted { get; set; } = false; 

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public NoteModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
