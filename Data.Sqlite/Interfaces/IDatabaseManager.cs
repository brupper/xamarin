using System.Threading.Tasks;

namespace Brupper.Data.Sqlite
{
    public interface IDatabaseManager
    {
        string DatabaseName { get; }

        string DatabasePath { get; }

        Task InitializeSqliteServiceAsync();

        Task<ISqliteService> GetSqliteServiceAsync();

        Task ResetDatabaseAsync();
    }
}
