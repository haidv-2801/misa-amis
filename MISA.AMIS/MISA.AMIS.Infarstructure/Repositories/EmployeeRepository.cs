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

            //2. Trả về dữ liệu
            return employeeCode;
        }
        #endregion
    }
}
