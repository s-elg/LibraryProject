using LibraryProject.Application.Common;
using LibraryProject.Application.DTOs.Book;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BookDto> GetByIdAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetByIdWithCategoryAsync(id);
        if (book == null)
            throw new BookNotFoundException(id);

        return MapToDto(book);
    }

    public async Task<PagedResult<BookDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        var result = await _unitOfWork.Books.GetAllPagedAsync(pageNumber, pageSize);
        return MapToPagedDto(result);
    }

    public async Task<PagedResult<BookDto>> SearchAsync(string searchTerm, int pageNumber, int pageSize)
    {
        var result = await _unitOfWork.Books.SearchAsync(searchTerm, pageNumber, pageSize);
        return MapToPagedDto(result);
    }

    public async Task<PagedResult<BookDto>> GetByCategoryAsync(Guid categoryId, int pageNumber, int pageSize)
    {
        var result = await _unitOfWork.Books.GetByCategoryAsync(categoryId, pageNumber, pageSize);
        return MapToPagedDto(result);
    }

    public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
    {
        var books = await _unitOfWork.Books.GetAvailableBooksAsync();
        return books.Select(MapToDto);
    }

    public async Task<IEnumerable<BookDto>> GetRecentlyAddedAsync(int count = 10)
    {
        var books = await _unitOfWork.Books.GetRecentlyAddedAsync(count);
        return books.Select(MapToDto);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequestDto request)
    {
        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            TotalCopies = request.TotalCopies,
            AvailableCopies = request.TotalCopies, // yeni kitapta hepsi müsait
            CategoryId = request.CategoryId
        };

        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Books.GetByIdWithCategoryAsync(book.Id);
        return MapToDto(created!);
    }

    public async Task<BookDto> UpdateAsync(Guid id, UpdateBookRequestDto request)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
            throw new BookNotFoundException(id);

        var copiesOnLoan = book.TotalCopies - book.AvailableCopies;

        if (request.TotalCopies < copiesOnLoan)
            throw new InvalidBookCopyCountException(request.TotalCopies, copiesOnLoan);

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.TotalCopies = request.TotalCopies;
        book.AvailableCopies = request.TotalCopies - copiesOnLoan;
        book.CategoryId = request.CategoryId;

        _unitOfWork.Books.Update(book);
        await _unitOfWork.SaveChangesAsync();

        var updated = await _unitOfWork.Books.GetByIdWithCategoryAsync(id);
        return MapToDto(updated!);
    }

    public async Task DeleteAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
            throw new BookNotFoundException(id);

        _unitOfWork.Books.Delete(book);
        await _unitOfWork.SaveChangesAsync();
    }

    private static BookDto MapToDto(Book book) => new(
        book.Id,
        book.Title,
        book.Author,
        book.ISBN,
        book.TotalCopies,
        book.AvailableCopies,
        book.CategoryId,
        book.Category?.Name ?? string.Empty);

    private static PagedResult<BookDto> MapToPagedDto(PagedResult<Book> source) => new()
    {
        Items = source.Items.Select(MapToDto),
        TotalCount = source.TotalCount,
        PageNumber = source.PageNumber,
        PageSize = source.PageSize
    };
}