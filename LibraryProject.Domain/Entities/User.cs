namespace LibraryProject.Domain.Entities;

public enum UserRole
{
    Member = 0,
    Admin = 1
}

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Member;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
}