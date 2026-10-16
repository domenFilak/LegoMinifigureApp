using LegoMinifigureApp.Entities;
using LegoMinifigureApp.Services.Common;

namespace LegoMinifigureApp.Services.Themes
{
    public static class ThemeErrors
    {
        public static Error NotFound(int id) =>
            Error.NotFound($"Theme {id} was not found.");

        public static Error NameRequired =>
            Error.Validation("Name is required.");

        public static Error NameTooLong =>
            Error.Validation($"Name can be at most {Theme.NameMaxLength} characters.");

        public static Error DuplicateName(string name) =>
            Error.Conflict($"A theme named '{name}' already exists.");

        public static Error HasFigures(string name) =>
            Error.Conflict($"Theme '{name}' still has figures and can't be deleted.");
    }
}
