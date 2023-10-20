using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Post
    {
        [Key]
        public int PostID { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Context is required.")]
        public string Context { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}