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

namespace MISA.CukCuk.Web.Controllers
{
    [ApiController]
    public class EmployeesController : BaseEntityController<Employee>
    {
        #region Declare
        IEmployeeService _employeeService;
        #endregion

        #region Methods
        public EmployeesController(IEmployeeService employeeService) : base(employeeService)
        {
            _employeeService = employeeService;
        }

        [EnableCors("AllowCROSPolicy")]
        [Route("NextEmployeeCode")]
        [HttpGet]
        public ActionResult GetNextEmployeeCode()
        {
            return Ok(_employeeService.GetNewEmployeeCode());
        }
        #endregion
    }
}
