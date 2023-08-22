using BankApplication.Common;
using BankApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApplication.Services
{
    internal class EmployeeService
    {
        User LoggedInUser { get; set; }

        public EmployeeService()
        {
            this.LoggedInUser = new User();
        }
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

        public Response<string> Update(Employee updatedEmployee)
        {
            Response<string> response = new Response<string>();
            try
            {
                Employee employee = DataStorage.Employees.Find(emp => emp.Id == updatedEmployee.Id);
                if (employee != null)
                {
                    employee.Name = updatedEmployee.Name;
                    employee.UserName = updatedEmployee.UserName;                  
                    employee.Password = updatedEmployee.Password;
                    employee.Email = updatedEmployee.Email;
                    employee.ModifiedOn = DateTime.Now;
                    employee.ModifiedBy = this.LoggedInUser.Id;

                    int index = DataStorage.Employees.FindIndex(emp => emp.Id == updatedEmployee.Id);
                    if (index != -1)
                    {
                        DataStorage.Employees[index] = employee;
                    }

                    response.IsSuccess = true;
                    response.Message = Constants.EmployeeUpdateSuccess;

                    response.IsSuccess = true;
                    response.Message = Constants.EmployeeUpdateSuccess;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.EmployeeNotFound;
                }
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = Constants.EmployeeUpdateFailure;
            }

            return response;
        }

        public Response<string> Delete(string employeeId)
        {
            Response<string> response = new Response<string>();
            try
            {
                Employee employee = DataStorage.Employees.Find(emp => emp.Id == employeeId);
                if (employee != null)
                {
                    DataStorage.Employees.Remove(employee);
                    response.IsSuccess = true;
                    response.Message = Constants.EmployeeDeletionSuccess;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = Constants.EmployeeNotFound;
                }
            }
            catch
            {
                response.IsSuccess = false;
                response.Message = Constants.EmployeeDeletionFailure;
            }

            return response;
        }

        public List<Employee> GetAllEmployees()
        {
            return DataStorage.Employees.Where(emp => emp.Type == Enums.UserType.Employee).ToList();
        }

        public Employee GetEmployeeByBankId(string bankId)
        {
            return DataStorage.Employees.Find(emp => emp.Type == Enums.UserType.Employee && emp.BankId == bankId);         
        }

    }
}

     


