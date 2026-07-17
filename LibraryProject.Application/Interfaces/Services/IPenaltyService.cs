using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Interfaces.Services;

public interface IPenaltyService
{
    Task<PenaltyResponseDto> CreateManualPenaltyAsync(CreatePenaltyDto dto);
    Task<PenaltyResponseDto> RemovePenaltyAsync(Guid penaltyId);
    Task<IEnumerable<PenaltyResponseDto>> GetByUserAsync(Guid userId);
    Task<IEnumerable<PenaltyResponseDto>> GetActivePenaltiesAsync();
    Task<PenaltyResponseDto> GetByIdAsync(Guid penaltyId);
}