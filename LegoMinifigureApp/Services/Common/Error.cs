using LegoMinifigureApp.Entities.Enums;

namespace LegoMinifigureApp.Services.Common
{
    public sealed record Error(ErrorType Type, string Message, string? Field = null)
    {
        public static Error Validation(string message, string? field = null) => new(ErrorType.Validation, message, field);
        public static Error NotFound(string message) => new(ErrorType.NotFound, message);
        public static Error Conflict(string message, string? field = null) => new(ErrorType.Conflict, message, field);
    }
}
