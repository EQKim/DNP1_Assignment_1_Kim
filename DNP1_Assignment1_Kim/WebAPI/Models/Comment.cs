using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string Username { get; set; }
        public string CommentText { get; set; }

        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }
}