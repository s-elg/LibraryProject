namespace LibraryProject.Domain.Entities;

public class Review : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
}