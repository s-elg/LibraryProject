namespace LibraryProject.Application.DTOs.Book;

public record BookDto(
    Guid Id,
    string Title,
    string Author,
    string ISBN,
    int TotalCopies,
    int AvailableCopies,
    Guid CategoryId,
    string CategoryName);

public record CreateBookRequestDto(
    string Title,
    string Author,
    string ISBN,
    int TotalCopies,
    Guid CategoryId);

public record UpdateBookRequestDto(
    string Title,
    string Author,
    string ISBN,
    int TotalCopies,
    Guid CategoryId);