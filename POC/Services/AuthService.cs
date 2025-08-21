using Microsoft.Data.SqlClient;
using POC.DataAccess;
using POC.Models;
using POC.Models.Auth;
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

        public async Task<LoginResult?> GetAdminByUsernameAsync(string username)
        {
            var parameters = new[]
            {
        new SqlParameter("@Username", username)
    };

            var table = await _dbHelper.ExecuteStoredProcedureAsync("GetAdminByUsername", parameters);

            if (table.Rows.Count == 0)
                return null;

            var row = table.Rows[0];

            return new LoginResult
            {
                PasswordHash = row.Field<string>("PasswordHash"),
                RoleName = row.Field<string>("RoleName")
            };
        }


        public async Task<bool> UserExistsAsync(string username)
        {
            var existingAdmin = await GetAdminByUsernameAsync(username);
            return existingAdmin != null;
        }

        public async Task<SignupResult> CreateAdminAsync(SignUpRequest signUp)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(signUp.Password);

            var parameters = new[]
            {
                new SqlParameter("@Username", signUp.Username),
                new SqlParameter("@Email", signUp.Email),
                new SqlParameter("@PasswordHash", passwordHash),
                new SqlParameter("@RoleId", signUp.RoleId)
            };

            var table = await _dbHelper.ExecuteStoredProcedureAsync("CreateAdmin", parameters);

            if (table.Rows.Count == 0)
                throw new Exception("Failed to create admin");

            var row = table.Rows[0];

            return new SignupResult
            {
                RoleName = row.Field<string>("RoleName")
            };
        }

        public async Task<List<Role>?> GetRoleOptionsAsync()
        {
            var table = await _dbHelper.ExecuteStoredProcedureAsync("sp_GetRoleOptions");

            if (table == null || table.Rows.Count == 0)
                throw new Exception("No roles found.");

            List<Role> roleOptions = new List<Role>();
            foreach (DataRow row in table.Rows)
            {
                roleOptions.Add(new Role
                {
                    Id = row.Field<int>("Role_id"),
                    Name = row.Field<string>("RoleName"),
                });
            }
            return roleOptions;
        }

        public async Task<List<user>> GetAuthUser()
        {
            var result = await _dbHelper.ExecuteStoredProcedureAsync("GetAuthUser");

            return result.AsEnumerable().Select(row => new user
            {
                Username = row["Username"].ToString(),
                Email = row["Email"].ToString(),
                Role = row["Role"].ToString(),
                Doj = row["Doj"] == DBNull.Value ? null : Convert.ToDateTime(row["Doj"])
            }).ToList();
        }



    }
}

