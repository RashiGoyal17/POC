using POC.Models;
using POC.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POC.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            return await _repository.GetAllEmployeesAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _repository.GetEmployeeByIdAsync(id);
        }

        public async Task<int> AddEmployeeAsync(Employee emp)
        {
            return await _repository.AddEmployeeAsync(emp);
        }

        public async Task<bool> UpdateAsync(int id, Employee updated)
        {
            updated.empId = id;
            return await _repository.UpdateEmployeeAsync(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteEmployeeAsync(id);
        }


        public Task<List<string>> GetDesignationsAsync() => _repository.GetDesignationsAsync();
        public Task<List<string>> GetLocationsAsync() => _repository.GetLocationsAsync();
        public Task<List<string>> GetSkillsAsync() => _repository.GetSkillsAsync();
        public Task<List<string>> GetProjectsAsync() => _repository.GetProjectsAsync();
        public Task<List<string>> GetManagersAsync() => _repository.GetManagersAsync();

        public async Task AddEmployeesAsync(List<Employee> employees)
        {
            await _repository.AddEmployeesAsync(employees);
        }

    }
}
