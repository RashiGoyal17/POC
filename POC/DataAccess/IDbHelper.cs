using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace POC.DataAccess
{
    public interface IDbHelper
    {
        Task<DataTable> ExecuteStoredProcedureAsync(string procedureName, SqlParameter[]? parameters = null);
        Task ExecuteStoredProcedureNonQueryAsync(string procedureName, SqlParameter[]? parameters = null);

        Task<List<string>> ExecuteQueryAsync(string sql);

        Task<T> ExecuteMultiResultStoredProcedureAsync<T>(string storedProcedureName, Func<SqlDataReader, Task<T>> mapFunction, SqlParameter[]? parameters = null);

    }
}

