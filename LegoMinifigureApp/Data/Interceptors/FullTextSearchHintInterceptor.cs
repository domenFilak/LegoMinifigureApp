using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace LegoMinifigureApp.Data.Interceptors
{
    //adds a query hint to queries tagged with FullTextSearchHintInterceptor.Tag.
    //without it, SQL Server might walk sort index row by row looking for matches
    public sealed class FullTextSearchHintInterceptor : DbCommandInterceptor
    {
        public const string Tag = "FullTextSearch";
        private const string Hint = "OPTION (USE HINT('DISABLE_OPTIMIZER_ROWGOAL'))";

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            AddHintIfTagged(command);

            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            AddHintIfTagged(command);

            return ValueTask.FromResult(result);
        }

        private static void AddHintIfTagged(DbCommand command)
        {
            if (command.CommandText.StartsWith($"-- {Tag}", StringComparison.Ordinal))
            {
                command.CommandText = command.CommandText + Environment.NewLine + Hint;
            }
        }
    }
}
