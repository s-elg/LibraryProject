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