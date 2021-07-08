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
    public class DepartmentsController : BaseEntityController<Department>
    {
        #region Declare
        IDepartmentService _departmentService;
        #endregion

        #region Constructer
        public DepartmentsController(IDepartmentService departmentService) : base(departmentService)
        {
            _departmentService = departmentService;
        }
        #endregion
    }
}
