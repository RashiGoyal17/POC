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


        public async Task<bool> UserExistsAsync(string username)
        {
            var existingAdmin = await GetAdminByUsernameAsync(username);
            return existingAdmin != null;
        }

        public async Task<Admin> CreateAdminAsync(string username, string email, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var parameters = new[]
            {
        new SqlParameter("@Username", username),
        new SqlParameter("@Email", email),
        new SqlParameter("@PasswordHash", passwordHash)
    };

            var table = await _dbHelper.ExecuteStoredProcedureAsync("CreateAdmin", parameters);

            if (table.Rows.Count == 0)
                throw new Exception("Failed to create admin");

            var row = table.Rows[0];

            return new Admin
            {
                AdminId = row.Field<int>("AdminId"),
                UserName = row.Field<string>("Username"),
                PasswordHash = row.Field<string>("PasswordHash"),
                Email = row.Field<string>("Email")
            };
        }



    }
}

