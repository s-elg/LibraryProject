using LibraryProject.Application.DTOs;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;
using LibraryProject.Application.Exceptions;

namespace LibraryProject.Application.Services;

public class PenaltyService : IPenaltyService
{
    private readonly IUnitOfWork _unitOfWork;

    public PenaltyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PenaltyResponseDto> CreateManualPenaltyAsync(CreatePenaltyDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
        if (user is null)
        {
            throw new UserNotFoundException(dto.UserId);
        }

        var penalty = new Penalty
        {
            UserId = dto.UserId,
            LoanId = null, // manuel ceza, bir ödünç kaydına bağlı değil
            Reason = dto.Reason,
            SuspensionEndDate = DateTime.UtcNow, // bilgi amaçlı; gating Status'a göre
            Status = PenaltyStatus.Active
        };

        await _unitOfWork.Penalties.AddAsync(penalty);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(penalty);
    }

    public async Task<PenaltyResponseDto> RemovePenaltyAsync(Guid penaltyId)
    {
        var penalty = await _unitOfWork.Penalties.GetByIdAsync(penaltyId);
        if (penalty is null)
        {
            throw new PenaltyNotFoundException(penaltyId);
        }

        if (penalty.Status == PenaltyStatus.Completed)
        {
            throw new PenaltyAlreadyCompletedException(penaltyId);
        }

        penalty.Status = PenaltyStatus.Completed;
        _unitOfWork.Penalties.Update(penalty);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(penalty);
    }

    public async Task<IEnumerable<PenaltyResponseDto>> GetByUserAsync(Guid userId)
    {
        var penalties = await _unitOfWork.Penalties.GetByUserAsync(userId);
        return penalties.Select(MapToDto);
    }

    public async Task<IEnumerable<PenaltyResponseDto>> GetActivePenaltiesAsync()
    {
        var penalties = await _unitOfWork.Penalties.GetActivePenaltiesAsync();
        return penalties.Select(MapToDto);
    }

    public async Task<PenaltyResponseDto> GetByIdAsync(Guid penaltyId, Guid currentUserId, bool isAdmin)
    {
        var penalty = await _unitOfWork.Penalties.GetByIdAsync(penaltyId);
        if (penalty is null)
        {
            throw new PenaltyNotFoundException(penaltyId);
        }
        if (!isAdmin && penalty.UserId != currentUserId)
        {
            throw new UnauthorizedPenaltyAccessException();
        }
        return MapToDto(penalty);
    }

    private static PenaltyResponseDto MapToDto(Penalty penalty)
    {
        return new PenaltyResponseDto(
            penalty.Id,
            penalty.UserId,
            penalty.LoanId,
            penalty.Reason,
            penalty.SuspensionEndDate,
            penalty.Status.ToString(),
            penalty.CreatedDate
        );
    }
}