using POC.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POC.Repositories
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int empId);
        Task<int> AddEmployeeAsync(Employee emp);
        Task<bool> UpdateEmployeeAsync(Employee emp);
        Task<bool> DeleteEmployeeAsync(int empId);
        Task<List<string>> GetDesignationsAsync();
        Task<List<string>> GetLocationsAsync();
        Task<List<string>> GetSkillsAsync();
        Task<List<string>> GetManagersAsync();
        Task<List<string>> GetProjectsAsync();
        Task AddEmployeesAsync(List<Employee> employees);
        Task<DashboardData> GetDashboardDataAsync();

    }
}
