namespace LibraryProject.Domain.Entities;

public enum LoanStatus
{
    Active = 0,
    Returned = 1,
    Overdue = 2
}

public class Loan : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;

    public DateTime LoanDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; } = LoanStatus.Active;

    public Penalty? Penalty { get; set; }
}