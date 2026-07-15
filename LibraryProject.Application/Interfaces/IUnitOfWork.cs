using LibraryProject.Application.Interfaces.Repositories;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IBookRepository Books { get; }
    IGenericRepository<Category> Categories { get; }
    IUserRepository Users { get; }
    ILoanRepository Loans { get; }
    IReviewRepository Reviews { get; }
    IPenaltyRepository Penalties { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync();
}