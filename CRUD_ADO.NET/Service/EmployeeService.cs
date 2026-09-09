using CRUD_ADO.NET.Models;
using CRUD_ADO.NET.Repository;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CRUD_ADO.NET.Service
{
    public class EmployeeService : IEmployeeRepository
    {
        private readonly IConfiguration config;


        public EmployeeService(IConfiguration config)
        {
            this.config = config;
        }


        private SqlConnection GetConnection()
        {
            return new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }


        //  GET ALL

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var employees = new List<Employee>();

            using var connection = GetConnection();

            using var command = new SqlCommand("sp_GetEmployees", connection);

            command.CommandType = CommandType.StoredProcedure;

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            while(await reader.ReadAsync())
            {
                employees.Add(new Employee
                {
                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Department = reader["Department"].ToString(),
                    Salary = Convert.ToDecimal(reader["Salary"])
                });
            }
            return employees;
        }



        //  GET BY ID


        public async Task<Employee?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            
            using var command = new SqlCommand("sp_GetEmployeeById", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@EmployeeId", id);

            await connection.OpenAsync();

            using var reader = await command.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                return new Employee
                {
                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    Department = reader["Department"].ToString(),
                    Salary = Convert.ToDecimal(reader["Salary"])
                };
            }
            return null;
        }


        //  Create

        public async Task<int> CreateAsync(Employee employee)
        {
            using var connection = GetConnection();

            using var command = new SqlCommand(
                "sp_CreateEmployee",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", employee.Name);
            command.Parameters.AddWithValue("@Email", employee.Email);
            command.Parameters.AddWithValue("@Department",
                (object?)employee.Department ?? DBNull.Value);
            command.Parameters.AddWithValue("@Salary", employee.Salary);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }


        //   UPDATE 

        public async Task<bool> UpdateAsync(Employee employee)
        {
            using var connection = GetConnection();

            using var command = new SqlCommand(
                "sp_UpdateEmployee",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@EmployeeId",
                employee.EmployeeId);

            command.Parameters.AddWithValue("@Name", employee.Name);

            command.Parameters.AddWithValue("@Email", employee.Email);

            command.Parameters.AddWithValue("@Department",
                (object?)employee.Department ?? DBNull.Value);

            command.Parameters.AddWithValue("@Salary", employee.Salary);

            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }


        //  DELETE

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = GetConnection();

            using var command = new SqlCommand(
                "sp_DeleteEmployee",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@EmployeeId", id);

            await connection.OpenAsync();

            int rowsAffected = await command.ExecuteNonQueryAsync();

            return rowsAffected > 0;
        }
    }
}
