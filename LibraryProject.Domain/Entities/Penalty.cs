namespace LibraryProject.Domain.Entities;

public enum PenaltyStatus
{
    Active = 0,
    Completed = 1
}

public class Penalty : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? LoanId { get; set; }
    public Loan? Loan { get; set; }

    public string Reason { get; set; } = string.Empty;
    public DateTime SuspensionEndDate { get; set; }
    public PenaltyStatus Status { get; set; } = PenaltyStatus.Active;
}