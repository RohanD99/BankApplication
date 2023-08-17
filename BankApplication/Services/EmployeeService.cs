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
                response.Message = Constants.EmployeeCreationSuccess;
                response.Data = employee.Id;
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = Constants.EmployeeCreationFailure;
            }

            return response;
        }

        public Employee GetEmployeeByBankId(string bankId)
        {
            return DataStorage.Employees.Find(emp => emp.Type == Enums.UserType.Employee && emp.BankId == bankId);         
        }
    }
}

     


