using Microsoft.AspNetCore.Mvc;
using POC.Models;
using POC.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace POC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(EmployeeService employeeService, ILogger<HomeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        // POST api/<HomeController>/AddEmployee
        [Authorize(Roles = "Admin")]
        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            try
            {
                var result = await _employeeService.AddEmployeeAsync(employee);
                _logger.LogInformation("Employee Added");
                return Ok(new { message = "Employee added", empId = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee");
                return StatusCode(500, new { message = "Error", error = ex.Message });
            }
        }

        // GET api/<HomeController>/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var employees = await _employeeService.GetAllAsync();
                _logger.LogInformation("Fetched all employees");
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all employees");
                return StatusCode(500, new { message = "Error fetching data", error = ex.Message });
            }
        }

        // GET api/<HomeController>/{empId}
        [HttpGet("{empId}")]
        public async Task<IActionResult> GetById(int empId)
        {
            try
            {
                var employee = await _employeeService.GetByIdAsync(empId);
                if (employee == null)
                    return NotFound(new { message = "Employee not found" });

                _logger.LogInformation($"Fetched employee with ID: {empId}");
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching employee with ID: {empId}");
                return StatusCode(500, new { message = "Error fetching employee", error = ex.Message });
            }
        }

        // PUT api/<HomeController>/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee updatedEmployee)
        {
            try
            {
                var success = await _employeeService.UpdateAsync(id, updatedEmployee);
                if (!success)
                    return NotFound(new { message = "Employee not found" });

                _logger.LogInformation($"Updated employee with ID: {id}");
                return Ok(new { message = "Employee updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating employee with ID: {id}");
                return StatusCode(500, new { message = "Error updating employee", error = ex.Message });
            }
        }

        // DELETE api/<HomeController>/Delete/{id}
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _employeeService.DeleteAsync(id);
                if (!success)
                    return NotFound(new { message = "Employee not found" });

                _logger.LogInformation($"Deleted employee with ID: {id}");
                return Ok(new { message = "Employee deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID: {id}");
                return StatusCode(500, new { message = "Error deleting employee", error = ex.Message });
            }
        }

        [HttpGet("designations")]
        public async Task<IActionResult> GetDesignations() => Ok(await _employeeService.GetDesignationsAsync());

        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations() => Ok(await _employeeService.GetLocationsAsync());

        [HttpGet("skills")]
        public async Task<IActionResult> GetSkills() => Ok(await _employeeService.GetSkillsAsync());

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects() => Ok(await _employeeService.GetProjectsAsync());

        [HttpGet("managers")]
        public async Task<IActionResult> GetManagers() => Ok(await _employeeService.GetManagersAsync());

        [HttpPost("AddEmployees")]
        public async Task<IActionResult> AddEmployees([FromBody] List<Employee> employees)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Will return detailed validation errors
            }


            if (employees == null || !employees.Any())
                return BadRequest("No employee data received.");

            try
            {
                await _employeeService.AddEmployeesAsync(employees);
                return Ok(new { message = "Employees added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk insert failed");
                return StatusCode(500, "Error inserting employees");
            }
        }

    }
}
