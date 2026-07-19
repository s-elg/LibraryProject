using System.Net;

namespace LibraryProject.Application.Exceptions;

public abstract class AppException : Exception
{
    public abstract HttpStatusCode StatusCode { get; }

    protected AppException(string message) : base(message)
    {
    }
}

// ----- Ara kademe kategori sınıfları -----

public abstract class NotFoundException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
    protected NotFoundException(string message) : base(message) { }
}

public abstract class ConflictException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
    protected ConflictException(string message) : base(message) { }
}

public abstract class BusinessRuleException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
    protected BusinessRuleException(string message) : base(message) { }
}

public abstract class UnauthorizedAppException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
    protected UnauthorizedAppException(string message) : base(message) { }
}

public abstract class ForbiddenException : AppException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
    protected ForbiddenException(string message) : base(message) { }
}

// ----- Auth -----

public class EmailAlreadyExistsException : ConflictException
{
    public EmailAlreadyExistsException(string email)
        : base($"'{email}' email adresi zaten kayıtlı.")
    {
    }
}

public class InvalidCredentialsException : UnauthorizedAppException
{
    public InvalidCredentialsException()
        : base("Email veya şifre hatalı.")
    {
    }
}

public class InvalidRefreshTokenException : UnauthorizedAppException
{
    public InvalidRefreshTokenException()
        : base("Geçersiz veya süresi dolmuş refresh token.")
    {
    }
}

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid userId)
        : base($"Id'si {userId} olan kullanıcı bulunamadı.")
    {
    }
}

// ----- Book -----

public class BookNotFoundException : NotFoundException
{
    public BookNotFoundException(Guid id)
        : base($"Id'si '{id}' olan kitap bulunamadı.")
    {
    }
}

public class InvalidBookCopyCountException : BusinessRuleException
{
    public InvalidBookCopyCountException(int requestedTotal, int copiesOnLoan)
        : base($"TotalCopies ({requestedTotal}) ödünçteki kopya sayısından ({copiesOnLoan}) az olamaz.")
    {
    }
}

public class BookNotAvailableException : BusinessRuleException
{
    public BookNotAvailableException(string bookTitle)
        : base($"'{bookTitle}' adlı kitabın şu anda müsait kopyası bulunmuyor.")
    {
    }
}

// ----- Loan -----

public class UserSuspendedException : ForbiddenException
{
    public UserSuspendedException()
        : base("Aktif bir cezanız bulunduğu için ödünç alma işlemi yapamazsınız. Lütfen kütüphane yönetimiyle iletişime geçin.")
    {
    }
}

public class MaxActiveLoansExceededException : BusinessRuleException
{
    public MaxActiveLoansExceededException(int maxLoans)
        : base($"Aynı anda en fazla {maxLoans} aktif ödünç kitabınız olabilir.")
    {
    }
}

public class LoanNotFoundException : NotFoundException
{
    public LoanNotFoundException(Guid loanId)
        : base($"Id'si {loanId} olan ödünç kaydı bulunamadı.")
    {
    }
}

public class LoanAlreadyReturnedException : ConflictException
{
    public LoanAlreadyReturnedException(Guid loanId)
        : base($"Id'si {loanId} olan kitap zaten iade edilmiş.")
    {
    }
}

// ----- Review -----

public class ReviewNotFoundException : NotFoundException
{
    public ReviewNotFoundException(Guid reviewId)
        : base($"Id'si {reviewId} olan yorum bulunamadı.")
    {
    }
}

public class DuplicateReviewException : ConflictException
{
    public DuplicateReviewException()
        : base("Bu kitap için zaten bir yorum yaptınız.")
    {
    }
}

public class BookNotBorrowedException : BusinessRuleException
{
    public BookNotBorrowedException()
        : base("Yorum yapabilmek için bu kitabı daha önce ödünç almış olmanız gerekir.")
    {
    }
}

public class InvalidRatingException : BusinessRuleException
{
    public InvalidRatingException(int rating)
        : base($"Puan (Rating) 1 ile 5 arasında olmalıdır. Girilen: {rating}")
    {
    }
}

public class UnauthorizedReviewAccessException : ForbiddenException
{
    public UnauthorizedReviewAccessException()
        : base("Bu yorumu güncelleme veya silme yetkiniz yok.")
    {
    }
}

// ----- Penalty -----

public class PenaltyNotFoundException : NotFoundException
{
    public PenaltyNotFoundException(Guid penaltyId)
        : base($"Id'si {penaltyId} olan ceza kaydı bulunamadı.")
    {
    }
}

public class PenaltyAlreadyCompletedException : ConflictException
{
    public PenaltyAlreadyCompletedException(Guid penaltyId)
        : base($"Id'si {penaltyId} olan ceza zaten kaldırılmış durumda.")
    {
    }
}

public class UnauthorizedPenaltyAccessException : ForbiddenException
{
    public UnauthorizedPenaltyAccessException()
        : base("Bu ceza kaydına erişim yetkiniz yok.")
    {
    }
}

// ----- Category -----

public class CategoryNotFoundException : NotFoundException
{
    public CategoryNotFoundException(Guid categoryId)
        : base($"Id'si {categoryId} olan kategori bulunamadı.")
    {
    }
}

public class CategoryHasBooksException : ConflictException
{
    public CategoryHasBooksException(string categoryName)
        : base($"'{categoryName}' kategorisine bağlı kitaplar olduğu için silinemez. Önce kitapları başka bir kategoriye taşıyın veya silin.")
    {
    }
}