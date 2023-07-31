using BankApplication.Common;
using BankApplication.Models;

namespace BankApplication.Services
{
    internal class EmployeeService
    {      
        static Response<string> Response = new Response<string>();

        public Response<string> Create(Employee employee)
        {
            try
            {
                employee.Id = Utility.GenerateEmployeeID();
                DataStorage.Employees.Add(employee);
                Response.IsSuccess = true;
                Response.Message = Constants.EmployeeSuccess;
                Response.Data = employee.Id;
            }
            catch
            {
                Response.IsSuccess = false;
                Response.Message = Constants.EmployeeFailure;
            }
            return Response;
        }
    }
}

     


