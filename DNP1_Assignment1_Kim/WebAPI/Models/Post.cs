namespace WebAPI.Models;

public class Post
{
    public int ID;
    public String title;
    public String description;

    

    public Post(int id, string title, string description)
    {
        ID = id;
        this.title = title;
        this.description = description;
    }
}