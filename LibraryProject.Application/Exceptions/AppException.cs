namespace LibraryProject.Application.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message)
    {
    }
}

public class EmailAlreadyExistsException : AppException
{
    public EmailAlreadyExistsException(string email)
        : base($"'{email}' email adresi zaten kayıtlı.")
    {
    }
}

public class InvalidCredentialsException : AppException
{
    public InvalidCredentialsException()
        : base("Email veya şifre hatalı.")
    {
    }
}

public class InvalidRefreshTokenException : AppException
{
    public InvalidRefreshTokenException()
        : base("Geçersiz veya süresi dolmuş refresh token.")
    {
    }
}

public class BookNotFoundException : AppException
{
    public BookNotFoundException(Guid id)
        : base($"Id'si '{id}' olan kitap bulunamadı.")
    {
    }
}

public class InvalidBookCopyCountException : AppException
{
    public InvalidBookCopyCountException(int requestedTotal, int copiesOnLoan)
        : base($"TotalCopies ({requestedTotal}) ödünçteki kopya sayısından ({copiesOnLoan}) az olamaz.")
    {
    }
}

public class BookNotAvailableException : AppException
{
    public BookNotAvailableException(string bookTitle)
        : base($"'{bookTitle}' adlı kitabın şu anda müsait kopyası bulunmuyor.")
    {
    }
}

public class UserSuspendedException : AppException
{
    public UserSuspendedException()
        : base("Aktif bir cezanız bulunduğu için ödünç alma işlemi yapamazsınız. Lütfen kütüphane yönetimiyle iletişime geçin.")
    {
    }
}

public class MaxActiveLoansExceededException : AppException
{
    public MaxActiveLoansExceededException(int maxLoans)
        : base($"Aynı anda en fazla {maxLoans} aktif ödünç kitabınız olabilir.")
    {
    }
}

public class LoanNotFoundException : AppException
{
    public LoanNotFoundException(Guid loanId)
        : base($"Id'si {loanId} olan ödünç kaydı bulunamadı.")
    {
    }
}

public class LoanAlreadyReturnedException : AppException
{
    public LoanAlreadyReturnedException(Guid loanId)
        : base($"Id'si {loanId} olan kitap zaten iade edilmiş.")
    {
    }
}

public class ReviewNotFoundException : AppException
{
    public ReviewNotFoundException(Guid reviewId)
        : base($"Id'si {reviewId} olan yorum bulunamadı.")
    {
    }
}

public class DuplicateReviewException : AppException
{
    public DuplicateReviewException()
        : base("Bu kitap için zaten bir yorum yaptınız.")
    {
    }
}

public class BookNotBorrowedException : AppException
{
    public BookNotBorrowedException()
        : base("Yorum yapabilmek için bu kitabı daha önce ödünç almış olmanız gerekir.")
    {
    }
}

public class InvalidRatingException : AppException
{
    public InvalidRatingException(int rating)
        : base($"Puan (Rating) 1 ile 5 arasında olmalıdır. Girilen: {rating}")
    {
    }
}

public class UnauthorizedReviewAccessException : AppException
{
    public UnauthorizedReviewAccessException()
        : base("Bu yorumu güncelleme veya silme yetkiniz yok.")
    {
    }
}

public class PenaltyNotFoundException : AppException
{
    public PenaltyNotFoundException(Guid penaltyId)
        : base($"Id'si {penaltyId} olan ceza kaydı bulunamadı.")
    {
    }
}

public class PenaltyAlreadyCompletedException : AppException
{
    public PenaltyAlreadyCompletedException(Guid penaltyId)
        : base($"Id'si {penaltyId} olan ceza zaten kaldırılmış durumda.")
    {
    }
}

// UserNotFoundException zaten AuthService'te tanımlıysa bu kısmı atla
public class UserNotFoundException : AppException
{
    public UserNotFoundException(Guid userId)
        : base($"Id'si {userId} olan kullanıcı bulunamadı.")
    {
    }
}

public class CategoryNotFoundException : AppException
{
    public CategoryNotFoundException(Guid categoryId)
        : base($"Id'si {categoryId} olan kategori bulunamadı.")
    {
    }
}

public class CategoryHasBooksException : AppException
{
    public CategoryHasBooksException(string categoryName)
        : base($"'{categoryName}' kategorisine bağlı kitaplar olduğu için silinemez. Önce kitapları başka bir kategoriye taşıyın veya silin.")
    {
    }
}