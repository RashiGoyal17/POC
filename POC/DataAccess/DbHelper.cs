using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace POC.DataAccess
{
    public class DbHelper : IDbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task ExecuteStoredProcedureNonQueryAsync(string procedureName, SqlParameter[]? parameters = null)
        {
            await using var conn = new SqlConnection(_connectionString);
            await using var cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }



        public async Task<DataTable> ExecuteStoredProcedureAsync(string procedureName, SqlParameter[]? parameters = null)
        {
            var dataTable = new DataTable();

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            dataTable.Load(reader);

            return dataTable;
        }


        public async Task<List<string>> ExecuteQueryAsync(string sql)
        {
            var result = new List<string>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                if (!reader.IsDBNull(0))
                    result.Add(reader.GetString(0));
            }

            return result;
        }

        public async Task<T> ExecuteMultiResultStoredProcedureAsync<T>(
            string storedProcedureName,
            Func<SqlDataReader, Task<T>> mapFunction,
            SqlParameter[]? parameters = null)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(storedProcedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters?.Length > 0)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();
            return await mapFunction(reader);
        }


    }
}

