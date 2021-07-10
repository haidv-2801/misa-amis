using MISA.AMIS.ApplicationCore.Interfaces;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MISA.AMIS.ApplicationCoore.Entities;

namespace MISA.AMIS.Infrastructure
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        #region Constructer
        public EmployeeRepository(IConfiguration configuration) : base(configuration)
        {

        }
        #endregion

        #region Methods
        public Employee GetEmployeeByCode(string employeeCode)
        {
            var employee = GetEntityByProperty("EmployeeCode", employeeCode);
            return employee;
        }

        public string GetNewEmployeeCode()
        {
            //1. Tạo kết nối và truy vấn                        
            var employeeCode = _dbConnection.QueryFirstOrDefault<string>($"Proc_GetNext{_tableName}Code", commandType: CommandType.StoredProcedure).ToString();
            try
            {
                employeeCode = NextEmployeeCode(employeeCode);
            }
            catch (Exception){}
            //2. Trả về dữ liệu
            return employeeCode;
        }

        private string NextEmployeeCode(string employeeCode)
        {
            int firstDigitIndex = employeeCode.IndexOfAny(new char[]
                { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            string prefix = employeeCode.Substring(0, firstDigitIndex);
            string nextNum = (int.Parse(employeeCode.Substring(firstDigitIndex)) + 1).ToString().PadLeft(5, '0');
            return string.Concat(prefix, nextNum);
        }
        #endregion
    }
}
