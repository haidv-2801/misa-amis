using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using System.Data;
using MISA.AMIS.ApplicationCore;
using MISA.AMIS.ApplicationCore.Interfaces;
using MISA.AMIS.ApplicationCore.Entities;
using MISA.AMIS.Web.Controllers;
using MISA.AMIS.ApplicationCoore.Entities;
using Microsoft.AspNetCore.Cors;
using MISA.AMIS.Entities;

namespace MISA.CukCuk.Web.Controllers
{
    [ApiController]
    public class EmployeesController : BaseEntityController<Employee>
    {
        #region Declare
        IEmployeeService _employeeService;
        #endregion

        #region Constructer
        public EmployeesController(IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy mã nhân viên lớn nhất và tăng lên 1
        /// </summary>
        /// <returns>String: mã mới</returns>
        /// CREATED BY: DVHAI 09/07/2021
        [EnableCors("AllowCROSPolicy")]
        [Route("NextEmployeeCode")]
        [HttpGet]
        public ActionResult GetNextEmployeeCode()
        {
            return Ok(_employeeService.GetNewEmployeeCode());
        }

        /// <summary>
        /// Hàm lấy nhân viên theo mã nhân viên
        /// </summary>
        /// <param name="employeeCode">Mã nhân viên</param>
        /// <returns>Nhân viên</returns>
        /// CREATED BY: DVHAI 07/10/2021
        [EnableCors("AllowCROSPolicy")]
        [Route("EmployeeByCode/{employeeCode}")]
        [HttpGet]
        public ActionResult GetEmployeeByCode(string employeeCode)
        {
            var employee = _employeeService.
                GetEmployeeByCode(employeeCode);

            return Ok(employee);
        }

        /// <summary>
        /// Lấy danh sách nhân viên phân trang, tìm kiếm
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <param name="limit">Số bản ghi trên 1 trang</param>
        /// <param name="offset">Số trang</param>
        /// <returns>Danh sách nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        [EnableCors("AllowCROSPolicy")]
        [Route("EmployeesFilterPaging")]
        [HttpGet]
        public ActionResult GetEmployeesFilterPaging([FromQuery]string filterValue, [FromQuery] int limit, [FromQuery] int offset)
        {
            return Ok(_employeeService.GetEmployeesFilterPaging(filterValue, limit, offset));
        }
        #endregion
    }
}
