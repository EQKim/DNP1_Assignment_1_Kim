namespace WebAPI.Models;

public class Post
{
    public int ID { get; set; }
    public String title { get; set; }
    public String description { get; set; }

    

    public Post(int id, string title, string description)
    {
        ID = id;
        this.title = title;
        this.description = description;
    }
}