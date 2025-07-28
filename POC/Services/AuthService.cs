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
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Username", username)
            };

            var table = await _dbHelper.ExecuteStoredProcedureAsync("GetAdminByUsername", parameters);

            if (table.Rows.Count == 0)
                return null;

            var row = table.Rows[0];

            return new Admin
            {
                AdminId = Convert.ToInt32(row["AdminId"]),
                UserName = row["Username"].ToString(),
                PasswordHash = row["PasswordHash"].ToString()
            };
        }
    }
}

