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
using MISA.AMIS.ApplicationCore.Interfaces.IRepositories;

namespace MISA.AMIS.Infrastructure
{
    public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
    {
        #region Constructer
        public DepartmentRepository(IConfiguration configuration) : base(configuration)
        {

        }
        #endregion

        #region Methods
      
        #endregion
    }
}
