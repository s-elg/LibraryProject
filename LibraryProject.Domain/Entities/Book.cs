namespace LibraryProject.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}