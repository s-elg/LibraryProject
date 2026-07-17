using LibraryProject.Application.DTOs.Loan;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Services;

public class LoanService : ILoanService
{
    private const int LoanDurationDays = 14;
    private const int MaxActiveLoansPerUser = 3;

    private readonly IUnitOfWork _unitOfWork;

    public LoanService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LoanResponseDto> BorrowBookAsync(Guid userId, Guid bookId)
    {
        // 1) Cezalı mı kontrol et
        var hasActivePenalty = await _unitOfWork.Penalties.HasActivePenaltyAsync(userId);
        if (hasActivePenalty)
        {
            throw new UserSuspendedException();
        }

        // 2) Aktif ödünç limiti kontrol et
        var userLoans = await _unitOfWork.Loans.GetByUserAsync(userId);
        var activeLoanCount = userLoans.Count(l => l.Status == LoanStatus.Active);
        if (activeLoanCount >= MaxActiveLoansPerUser)
        {
            throw new MaxActiveLoansExceededException(MaxActiveLoansPerUser);
        }

        // 3) Kitap müsaitliği kontrol et
        var book = await _unitOfWork.Books.GetByIdAsync(bookId);
        if (book is null || book.AvailableCopies <= 0)
        {
            throw new BookNotAvailableException(book?.Title ?? "Bilinmeyen kitap");
        }

        // 4) Loan oluştur
        var loanDate = DateTime.UtcNow;
        var loan = new Loan
        {
            UserId = userId,
            BookId = bookId,
            LoanDate = loanDate,
            DueDate = loanDate.AddDays(LoanDurationDays),
            Status = LoanStatus.Active
        };

        await _unitOfWork.Loans.AddAsync(loan);

        // 5) Kitabın müsait kopya sayısını düşür
        book.AvailableCopies -= 1;
        _unitOfWork.Books.Update(book);

        // Tek transaction: loan eklenmesi ve kopya sayısının düşmesi ya birlikte
        // kaydedilmeli ya da hiç kaydedilmemeli (tutarlılık).
        await _unitOfWork.SaveChangesAsync();

        loan.Book = book; // DTO map için
        return MapToDto(loan);
    }

    public async Task<LoanResponseDto> ReturnBookAsync(Guid loanId)
    {
        var loan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loanId);
        if (loan is null)
        {
            throw new LoanNotFoundException(loanId);
        }

        if (loan.Status == LoanStatus.Returned)
        {
            throw new LoanAlreadyReturnedException(loanId);
        }

        var returnDate = DateTime.UtcNow;
        var isLate = returnDate > loan.DueDate;

        loan.ReturnDate = returnDate;
        loan.Status = LoanStatus.Returned;
        _unitOfWork.Loans.Update(loan);

        // Kitabın müsait kopya sayısını artır
        var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
        if (book is not null)
        {
            book.AvailableCopies += 1;
            _unitOfWork.Books.Update(book);
        }

        // Geç iade varsa ceza oluştur
        if (isLate)
        {
            var lateDays = (returnDate - loan.DueDate).Days;
            var penalty = new Penalty
            {
                UserId = loan.UserId,
                LoanId = loan.Id,
                Reason = $"'{loan.Book.Title}' adlı kitap {lateDays} gün gecikmeyle iade edildi.",
                SuspensionEndDate = returnDate, // bilgi amaçlı; gating Status'a göre yapılıyor
                Status = PenaltyStatus.Active
            };

            await _unitOfWork.Penalties.AddAsync(penalty);
        }

        // Tek transaction: iade, kopya artışı ve (varsa) ceza birlikte kaydedilir.
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(loan);
    }

    public async Task<IEnumerable<LoanResponseDto>> GetUserLoansAsync(Guid userId)
    {
        var loans = await _unitOfWork.Loans.GetByUserAsync(userId);
        return loans.Select(MapToDto);
    }

    public async Task<IEnumerable<LoanResponseDto>> GetActiveLoansAsync()
    {
        var loans = await _unitOfWork.Loans.GetActiveLoansAsync();
        return loans.Select(MapToDto);
    }

    public async Task<LoanResponseDto> GetByIdAsync(Guid loanId)
    {
        var loan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loanId);
        if (loan is null)
        {
            throw new LoanNotFoundException(loanId);
        }

        return MapToDto(loan);
    }

    private static LoanResponseDto MapToDto(Loan loan)
    {
        return new LoanResponseDto(
            loan.Id,
            loan.UserId,
            loan.BookId,
            loan.Book?.Title ?? string.Empty,
            loan.LoanDate,
            loan.DueDate,
            loan.ReturnDate,
            loan.Status.ToString()
        );
    }
}