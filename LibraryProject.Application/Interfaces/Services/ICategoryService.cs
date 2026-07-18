using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<CategoryResponseDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryResponseDto> UpdateAsync(Guid categoryId, UpdateCategoryDto dto);
    Task DeleteAsync(Guid categoryId);
    Task<CategoryResponseDto> GetByIdAsync(Guid categoryId);
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
}