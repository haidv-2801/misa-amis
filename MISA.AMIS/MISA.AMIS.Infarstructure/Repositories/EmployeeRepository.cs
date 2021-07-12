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
    /// <summary>
    /// Repository danh mục nhân viên
    /// </summary>
    /// CREATED BY: DVHAI (07/07/2021)
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        #region Constructer
        public EmployeeRepository(IConfiguration configuration) : base(configuration)
        {

        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy thông tin nhân viên theo mã 
        /// </summary>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <returns>Bản ghi nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public Employee GetEmployeeByCode(string employeeCode)
        {
            var employee = GetEntityByProperty("EmployeeCode", employeeCode);
            return employee;
        }

        /// <summary>
        /// Lấy danh sách nhân viên phân trang, tìm kiếm
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <param name="limit">Số bản ghi trên 1 trang</param>
        /// <param name="offset">Số trang</param>
        /// <returns>Danh sách nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public IEnumerable<Employee> GetEmployeesFilterPaging(string filterValue, int limit, int offset)
        {
            //1. Ánh xạ giá trị
            var param = new DynamicParameters();
            param.Add($"@FilterValue", filterValue);
            param.Add($"@PageSize", limit);
            param.Add($"@PageNum", offset);

            //2. Tạo kết nối và truy vấn                        
            var employees = _dbConnection.Query<Employee>($"Proc_Get{_tableName}sFilterPaging", param: param, commandType: CommandType.StoredProcedure);

            //3. Trả về dữ liệu
            return employees;
        }


        /// <summary>
        /// Lấy tất cả cột trong Exportcolumn để map
        /// </summary>
        /// <returns>Tổng bản ghi</returns>
        /// CREATED BY: DVHAI (12/07/2021)
        public IEnumerable<Employee> GetExportColumns()
        {
            //1. Tạo kết nối và truy vấn                        
            var exportColumns = _dbConnection.Query<Employee>($"Proc_GetExportColumn", commandType: CommandType.StoredProcedure);

            //1. Trả về dữ liệu
            return exportColumns;
        }

        /// <summary>
        /// Lấy mã nhân viên mới
        /// </summary>
        /// <returns>Mã nhân viên mới</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public string GetNewEmployeeCode()
        {
            //1. Tạo kết nối và truy vấn                        
            var employeeCode = _dbConnection.QueryFirstOrDefault<string>($"Proc_GetNext{_tableName}Code", commandType: CommandType.StoredProcedure).ToString();

            //2. Trả về dữ liệu
            return employeeCode;
        }

        /// <summary>
        /// Lấy tổng số bản ghi sau khi lọc
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <returns>Tổng bản ghi</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public int GetTotalEmployeeFilter(string filterValue)
        {
            //1. Ánh xạ giá trị
            var param = new DynamicParameters();
            param.Add($"@filterValue", filterValue);

            //2. Tạo kết nối và truy vấn                        
            int totalRecord = _dbConnection.Query<int>($"Proc_GetTotal{_tableName}sFilter", param: param, commandType: CommandType.StoredProcedure).SingleOrDefault();
            
            return totalRecord;
        }
        #endregion
    }
}
