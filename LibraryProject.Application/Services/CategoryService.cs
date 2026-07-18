using LibraryProject.Application.DTOs;
using LibraryProject.Application.Exceptions;
using LibraryProject.Application.Interfaces;
using LibraryProject.Application.Interfaces.Services;
using LibraryProject.Domain.Entities;

namespace LibraryProject.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<CategoryResponseDto> UpdateAsync(Guid categoryId, UpdateCategoryDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (category is null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        category.Name = dto.Name;
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task DeleteAsync(Guid categoryId)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (category is null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        var booksInCategory = await _unitOfWork.Books.GetByCategoryAsync(categoryId, pageNumber: 1, pageSize: 1);
        if (booksInCategory.TotalCount > 0)
        {
            throw new CategoryHasBooksException(category.Name);
        }

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<CategoryResponseDto> GetByIdAsync(Guid categoryId)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (category is null)
        {
            throw new CategoryNotFoundException(categoryId);
        }

        return MapToDto(category);
    }

    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return categories.Select(MapToDto);
    }

    private static CategoryResponseDto MapToDto(Category category)
    {
        return new CategoryResponseDto(category.Id, category.Name);
    }
}