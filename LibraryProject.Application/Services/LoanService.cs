using LibraryProject.Application.DTOs;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Services;

public class LoanService : ILoanService
{
    private const int LoanDurationDays = 14;
    private const int MaxActiveLoansPerUser = 3;
    private const int PickupWindowDays = 3; 

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
        // NOT: Reserved durumundaki loan'lar da limite dahil edildi, çünkü kullanıcı
        // o kitabı zaten "üstlenmiş" durumda, teslim almamış olsa bile.
        var userLoans = await _unitOfWork.Loans.GetByUserAsync(userId);
        var activeLoanCount = userLoans.Count(l =>
            l.Status == LoanStatus.Active || l.Status == LoanStatus.Reserved);
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

        // 4) Loan'ı "Reserved" olarak oluştur — henüz fiziksel teslim yok
        var now = DateTime.UtcNow;
        var pickupDeadline = now.AddDays(PickupWindowDays);

        var loan = new Loan
        {
            UserId = userId,
            BookId = bookId,
            LoanDate = now,            // rezervasyon anı; teslim alınınca ConfirmPickupAsync güncelleyecek
            DueDate = pickupDeadline,  // 14 günlük sayaç henüz başlamadı, DueDate şimdilik pickup deadline'a eşit
            Status = LoanStatus.Reserved,
            PickupDeadline = pickupDeadline
        };

        await _unitOfWork.Loans.AddAsync(loan);

        // 5) Kitabın müsait kopya sayısını düşür (rezervasyon anında düşüyor,
        // çünkü kitap fiziksel olarak ayrılmış sayılıyor)
        book.AvailableCopies -= 1;
        _unitOfWork.Books.Update(book);

        await _unitOfWork.SaveChangesAsync();

        loan.Book = book;
        return MapToDto(loan);
    }

    public async Task<LoanResponseDto> CancelReservationAsync(Guid loanId, Guid userId)
    {
        var loan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loanId);
        if (loan is null)
        {
            throw new LoanNotFoundException(loanId);
        }

        if (loan.UserId != userId)
        {
            throw new UnauthorizedLoanAccessException();
        }

        if (loan.Status != LoanStatus.Reserved)
        {
            throw new InvalidLoanStatusException(
                "Sadece teslim alınmayı bekleyen rezervasyonlar iptal edilebilir.");
        }

        loan.Status = LoanStatus.Cancelled;
        loan.PickupDeadline = null;
        _unitOfWork.Loans.Update(loan);

        // Stok geri iade
        var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
        if (book is not null)
        {
            book.AvailableCopies += 1;
            _unitOfWork.Books.Update(book);
        }

        await _unitOfWork.SaveChangesAsync();
        return MapToDto(loan);
    }

    public async Task<LoanResponseDto> ConfirmPickupAsync(Guid loanId)
    {
        var loan = await _unitOfWork.Loans.GetByIdWithDetailsAsync(loanId);
        if (loan is null)
        {
            throw new LoanNotFoundException(loanId);
        }

        if (loan.Status != LoanStatus.Reserved)
        {
            throw new InvalidLoanStatusException(
                $"Bu ödünç kaydı 'Reserved' durumunda değil (mevcut durum: {loan.Status}), teslim onayı verilemez.");
        }

        // 14 günlük ödünç süresi burada, fiziksel teslim anında başlıyor
        var pickupDate = DateTime.UtcNow;
        loan.LoanDate = pickupDate;
        loan.DueDate = pickupDate.AddDays(LoanDurationDays);
        loan.PickupDeadline = null;
        loan.Status = LoanStatus.Active;

        _unitOfWork.Loans.Update(loan);
        await _unitOfWork.SaveChangesAsync();

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

        var book = await _unitOfWork.Books.GetByIdAsync(loan.BookId);
        if (book is not null)
        {
            book.AvailableCopies += 1;
            _unitOfWork.Books.Update(book);
        }

        if (isLate)
        {
            var lateDays = (returnDate - loan.DueDate).Days;
            var penalty = new Penalty
            {
                UserId = loan.UserId,
                LoanId = loan.Id,
                Reason = $"'{loan.Book.Title}' adlı kitap {lateDays} gün gecikmeyle iade edildi.",
                SuspensionEndDate = returnDate,
                Status = PenaltyStatus.Active
            };

            await _unitOfWork.Penalties.AddAsync(penalty);
        }

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
            loan.Status.ToString(),
            loan.PickupDeadline
        );
    }
}