using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

public class Comment
{
    [Key]
    public int CommentID { get; set; }
    public string CommentText { get; set; }

    [ForeignKey("Post")]
    public int PostID { get; set; } // Renamed to 'PostId' to match database
    
    [ForeignKey("User")]
    public string Username { get; set; } // Renamed to 'Username' to match database
 
}