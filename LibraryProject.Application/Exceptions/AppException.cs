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