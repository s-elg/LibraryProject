namespace LibraryProject.API.Common;

public record ErrorResponseDto(string Message, string? TraceId = null);