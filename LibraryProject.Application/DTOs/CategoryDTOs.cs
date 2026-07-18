namespace LibraryProject.Application.DTOs;

public record CreateCategoryDto(string Name);

public record UpdateCategoryDto(string Name);

public record CategoryResponseDto(Guid Id, string Name);