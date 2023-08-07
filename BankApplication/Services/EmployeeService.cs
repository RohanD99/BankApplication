using BankApplication.Common;
using BankApplication.Models;

namespace BankApplication.Services
{
    internal class EmployeeService
    {
        public Response<string> Create(Employee employee)
        {
            Response<string> response = new Response<string>();
            try
            {
                employee.Id = Utility.GenerateEmployeeID();
                DataStorage.Employees.Add(employee);
                response.IsSuccess = true;
                response.Message = Constants.EmployeeSuccess;
                response.Data = employee.Id;
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = Constants.EmployeeFailure;
            }
            return response;
        }
    }   
}

     


