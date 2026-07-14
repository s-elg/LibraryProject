using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Interfaces.Repositories;

public interface IPenaltyRepository : IGenericRepository<Penalty>
{
    Task<bool> HasActivePenaltyAsync(Guid userId);
    Task<IEnumerable<Penalty>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Penalty>> GetActivePenaltiesAsync();
    Task<IEnumerable<Penalty>> GetExpiredButActivePenaltiesAsync();
}