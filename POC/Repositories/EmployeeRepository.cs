using System.Data;
using POC.Models;
using Microsoft.Data.SqlClient;
using POC.DataAccess;
using System.Threading.Tasks;

namespace POC.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbHelper _dbHelper;

        public EmployeeRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            var result = await _dbHelper.ExecuteStoredProcedureAsync("GetAllUsers");

            //await _dbHelper.ExecuteStoredProcedureAsync("GetAllResources");

            return result.AsEnumerable().Select(row => new Employee
            {
                empId = Convert.ToInt32(row["EmpId"]),
                name = row["EmployeeName"].ToString(),
                designation = row["Designation"].ToString(),
                ReportingTo = row["ReportingManagers"].ToString(),
                billable = Convert.ToBoolean(row["Billable"]),
                skill = row["Skills"].ToString(),
                project = row["Projects"].ToString(),
                location = row["LocationName"].ToString(),
                email = row["EmailId"].ToString(),
                doj = DateOnly.FromDateTime(Convert.ToDateTime(row["CteDoj"])),
                remarks = row["Remarks"].ToString()
            }).ToList();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int empId)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", empId)
            };

            var result = await _dbHelper.ExecuteStoredProcedureAsync("GetUserById", parameters);

            //await _dbHelper.ExecuteStoredProcedureAsync("GetAllResourceByEmpId", param);

            if (result.Rows.Count == 0)
                return null;

            var row = result.Rows[0];
            return new Employee
            {
                empId = Convert.ToInt32(row["EmpId"]),
                name = row["EmployeeName"].ToString(),
                designation = row["Designation"].ToString(),
                ReportingTo = row["ReportingManagers"].ToString(),
                billable = Convert.ToBoolean(row["Billable"]),
                skill = row["Skills"].ToString(),
                project = row["Projects"].ToString(),
                location = row["LocationName"].ToString(),
                email = row["EmailId"].ToString(),
                doj = DateOnly.FromDateTime(Convert.ToDateTime(row["CteDoj"])),
                remarks = row["Remarks"].ToString()
            };
        }

        public async Task<int> AddEmployeeAsync(Employee emp)
        {

            var outputParam = new SqlParameter
            {
                ParameterName = "@NewEmpId",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            var parameters = new SqlParameter[]
            {

                new SqlParameter("@EmployeeName", emp.name),
                new SqlParameter("@EmailId", emp.email),
                new SqlParameter("@CteDoj", emp.doj.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@LocationName", emp.location),
                new SqlParameter("@DesignationTitle", emp.designation),
                new SqlParameter("@Remarks", emp.remarks ?? string.Empty),
                new SqlParameter("@SkillNames", emp.skill ?? string.Empty),
                new SqlParameter("@ProjectNames", emp.project ?? string.Empty),
                new SqlParameter("@ManagerNames", emp.ReportingTo ?? string.Empty),
                outputParam
            };

            await _dbHelper.ExecuteStoredProcedureNonQueryAsync("AddUserWithSkillsAndProjectsByNames", parameters);

            // Read the value of the output parameter
            return outputParam.Value != DBNull.Value ? Convert.ToInt32(outputParam.Value) : 0;

        }

        public async Task<bool> UpdateEmployeeAsync(Employee emp)
        {

            await _dbHelper.ExecuteStoredProcedureAsync("UpdateUserWithDetails", new SqlParameter[]
            {
            new SqlParameter("@UserId", emp.empId),
            new SqlParameter("@EmployeeName", emp.name),
            new SqlParameter("@EmailId", emp.email),
            new SqlParameter("@CteDoj", emp.doj.ToDateTime(TimeOnly.MinValue)),
            new SqlParameter("@LocationName", emp.location),
            new SqlParameter("@DesignationTitle", emp.designation),
            new SqlParameter("@Remarks", emp.remarks ?? string.Empty),
            new SqlParameter("@SkillNames", emp.skill ?? string.Empty),
            new SqlParameter("@ProjectNames", emp.project ?? string.Empty),
            new SqlParameter("@ManagerNames", emp.ReportingTo ?? string.Empty)
            });


            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int empId)
        {

            await _dbHelper.ExecuteStoredProcedureAsync("DeleteUser", new SqlParameter[]
            {
                new SqlParameter("@UserId", empId)
            });

            return true;
        }


        public async Task <List<string>> GetDesignationsAsync()
        {
            var sql = "SELECT Title FROM Designations ORDER BY Title";
            var result = await _dbHelper.ExecuteQueryAsync(sql);

            var titles = result.AsEnumerable().Select(row => row.ToString());
            return result;

        }

        public async Task<List<string>> GetLocationsAsync()
        {
            var sql = "SELECT LocationName FROM Locations ORDER BY LocationName";
            var result = await _dbHelper.ExecuteQueryAsync(sql);
            return result.AsEnumerable().Select(r => r.ToString()).ToList();
        }

        public async Task<List<string>> GetSkillsAsync()
        {
            var sql = "SELECT SkillName FROM Skills ORDER BY SkillName";
            var result = await _dbHelper.ExecuteQueryAsync(sql);
            return result.AsEnumerable().Select(r => r.ToString()).ToList();
        }

        public async Task<List<string>> GetProjectsAsync()
        {
            var sql = "SELECT ProjectName FROM Projects ORDER BY ProjectName";
            var result = await _dbHelper.ExecuteQueryAsync(sql);
            return result.AsEnumerable().Select(r => r.ToString()).ToList();
        }

        public async Task<List<string>> GetManagersAsync()
        {
            var sql = @"
        SELECT DISTINCT u.EmployeeName
        FROM Users u
        WHERE u.empId IN (SELECT DISTINCT ManagerId FROM ReportingManagers)
        ORDER BY u.EmployeeName";

            var result = await _dbHelper.ExecuteQueryAsync(sql);
            return result.AsEnumerable().Select(r => r.ToString()).ToList();
        }


        public async Task AddEmployeesAsync(List<Employee> employees)
        {
            foreach (var emp in employees)
            {
                var outputParam = new SqlParameter
                {
                    ParameterName = "@NewEmpId",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@EmployeeName", emp.name),
            new SqlParameter("@EmailId", emp.email),
            new SqlParameter("@CteDoj", emp.doj),
            new SqlParameter("@LocationName", emp.location),
            new SqlParameter("@DesignationTitle", emp.designation),
            new SqlParameter("@Remarks", emp.remarks ?? string.Empty),
            new SqlParameter("@SkillNames", emp.skill ?? string.Empty),
            new SqlParameter("@ProjectNames", emp.project ?? string.Empty),
            new SqlParameter("@ManagerNames", emp.ReportingTo ?? string.Empty),
            outputParam
                };

                await _dbHelper.ExecuteStoredProcedureNonQueryAsync("AddUserWithSkillsAndProjectsByNames", parameters);
            }
        }


    }
}

