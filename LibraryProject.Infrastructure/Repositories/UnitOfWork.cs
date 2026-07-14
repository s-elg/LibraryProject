using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Repositories;
using LibraryProject.Domain.Entities;
using LibraryProject.Infrastructure.Data;

namespace LibraryProject.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IBookRepository? _books;
    private IGenericRepository<Category>? _categories;
    private IUserRepository? _users;
    private ILoanRepository? _loans;
    private IReviewRepository? _reviews;
    private IPenaltyRepository? _penalties;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IBookRepository Books => _books ??= new BookRepository(_context);
    public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public ILoanRepository Loans => _loans ??= new LoanRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
    public IPenaltyRepository Penalties => _penalties ??= new PenaltyRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}