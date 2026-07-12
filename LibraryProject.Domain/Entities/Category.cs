namespace LibraryProject.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = new List<Book>();
}