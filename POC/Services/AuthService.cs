using Microsoft.Data.SqlClient;
using POC.Models;
using POC.DataAccess;
using System.Data;

namespace POC.Services
{
    public class AuthService
    {
        private readonly IDbHelper _dbHelper;

        public AuthService(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<Admin?> GetAdminByUsernameAsync(string username)
        {
            var parameters = new[]
            {
        new SqlParameter("@Username", username)
    };

            var table = await _dbHelper.ExecuteStoredProcedureAsync("GetAdminByUsername", parameters);

            if (table.Rows.Count == 0)
                return null;

            var row = table.Rows[0];

            return new Admin
            {
                AdminId = row.Field<int>("AdminId"),
                UserName = row.Field<string>("Username"),
                PasswordHash = row.Field<string>("PasswordHash")
            };
        }


    }
}

