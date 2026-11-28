using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GameBackendAPI.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool IsDuplicateEntryException(this DbUpdateException dbUpdateException)
        {
            return dbUpdateException.InnerException is PostgresException { SqlState: "23505" };
        }

        public static bool IsForeignKeyViolationException(this DbUpdateException dbUpdateException)
        {
            return dbUpdateException.InnerException is PostgresException { SqlState: "23503" };
        }
    }
}
