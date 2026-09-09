using CRUD_ADO.NET.Models;
using CRUD_ADO.NET.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_ADO.NET.Controllers
{
    [Route("api")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;

        public EmployeesController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Employees
        [HttpGet("/getall")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _repository.GetAllAsync();

            return Ok(employees);
        }

        // GET: api/Employees/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _repository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // POST: api/Employees
        [HttpPost("post")]
        public async Task<IActionResult> Create(Employee employee)
        {
            int id = await _repository.CreateAsync(employee);

            employee.EmployeeId = id;

            return CreatedAtAction(
                nameof(GetById),
                new { id = id },
                employee);
        }

        // PUT: api/Employees/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            Employee employee)
        {
            if (id != employee.EmployeeId)
                return BadRequest("ID mismatch.");

            var existingEmployee =
                await _repository.GetByIdAsync(id);

            if (existingEmployee == null)
                return NotFound();

            bool updated =
                await _repository.UpdateAsync(employee);

            if (!updated)
                return BadRequest();

            return Ok(employee);
        }

        // DELETE: api/Employees/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingEmployee =
                await _repository.GetByIdAsync(id);

            if (existingEmployee == null)
                return NotFound();

            bool deleted =
                await _repository.DeleteAsync(id);

            if (!deleted)
                return BadRequest();

            return NoContent();
        }
    }
}
