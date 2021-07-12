using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using MISA.AMIS.Entities;
using MISA.AMIS.ApplicationCore.Entities;

namespace MISA.AMIS.ApplicationCore.Interfaces
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>, IDisposable where TEntity : BaseEntity
    {
        #region Declare
        IConfiguration _configuration;
        protected IDbConnection _dbConnection = null;
        string _connectionString = string.Empty;
        protected string _tableName;
        #endregion

        #region Constructer
        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("MISAAMISConnectionString");
            _dbConnection = new MySqlConnection(_connectionString);
            _tableName = typeof(TEntity).Name;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Lấy tất cả
        /// </summary>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public IEnumerable<TEntity> GetEntities()
        {
            //1. Tạo kết nối và truy vấn                        
            var entities = _dbConnection.Query<TEntity>($"Proc_Get{_tableName}s", commandType: CommandType.StoredProcedure).ToList();

            //2. Trả về dữ liệu
            return entities;
        }

        /// <summary>
        /// Lấy theo id
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public TEntity GetEntityById(Guid entityId)
        {
            //1. Lấy tên của khóa chính
            var keyName = GetKeyProperty().Name;

            var dynamicParams = new DynamicParameters();
            dynamicParams.Add($"@{keyName}", entityId);

            //2. Tạo kết nối và truy vấn
            var entity = _dbConnection.Query<TEntity>($"Proc_Get{_tableName}ById", param: dynamicParams, commandType: CommandType.StoredProcedure).FirstOrDefault();

            //3. Trả về dữ liệu
            return entity;
        }

        /// <summary>
        /// Xóa theo mã
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public int Delete(Guid entityId)
        {
            var rowAffects = 0;
            _dbConnection.Open();

            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //1. Lấy tên của khóa chính
                    var keyName = GetKeyProperty().Name;

                    var dynamicParams = new DynamicParameters();
                    dynamicParams.Add($"@m_{keyName}", entityId);

                    //2. Kết nối tới CSDL:
                    rowAffects = _dbConnection.Execute($"Proc_Delete{_tableName}ById", param: dynamicParams, transaction: transaction,commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                }
                catch { transaction.Rollback(); }
            }

            //3. Trả về số bản ghi bị ảnh hưởng
            return rowAffects;
        }

        /// <summary>
        /// Thêm bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public int Insert(TEntity entity)
        {
            var rowAffects = 0;
            _dbConnection.Open();
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //1.Duyệt các thuộc tính trên bản ghi và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2.Thực hiện thêm bản ghi
                    rowAffects = _dbConnection.Execute($"Proc_Insert{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }

            //3.Trả về số bản ghi thêm mới
            return rowAffects;
        }

        /// <summary>
        /// Cập nhập bản ghi
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        public int Update(Guid entityId, TEntity entity)
        {
            var rowAffects = 0;
            _dbConnection.Open();
            using (var transaction = _dbConnection.BeginTransaction())
            {
                try
                {
                    //1. Duyệt các thuộc tính trên customer và tạo parameters
                    var parameters = MappingDbType(entity);

                    //2. Ánh xạ giá trị id
                    var keyName = GetKeyProperty().Name;
                    entity.GetType().GetProperty(keyName).SetValue(entity, entityId);

                    //3. Kết nối tới CSDL:
                    rowAffects = _dbConnection.Execute($"Proc_Update{_tableName}", param: parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
            //4. Trả về dữ liệu
            return rowAffects;
        }

        /// <summary>
        /// Ánh xạ các thuộc tính sang kiểu dynamic
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <returns>Dan sách các biến động</returns>
        private DynamicParameters MappingDbType(TEntity entity)
        {
            var parameters = new DynamicParameters();
            try
            {
                //1. Duyệt các thuộc tính trên entity và tạo parameters
                var properties = entity.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var propertyValue = property.GetValue(entity);
                    var propertyType = property.PropertyType;

                    if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                        parameters.Add($"@{propertyName}", propertyValue, DbType.String);
                    else
                        parameters.Add($"@{propertyName}", propertyValue);
                }
            }
            catch { }
            //2. Trả về danh sách các parameter
            return parameters;
        }

        /// <summary>
        /// Lấy thuộc tính giá tị khóa
        /// </summary>
        /// <returns></returns>
        /// CREATED BY: DVHAI (11/07/2021)
        private PropertyInfo GetKeyProperty()
        {
            try
            {
                var keyProperty = typeof(TEntity)
                .GetProperties()
                .Where(p => p.IsDefined(typeof(KeyAttribute), false))
                .FirstOrDefault();
                return keyProperty;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Lấy thưc thể theo thuộc tính
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="property">Thuộc tính trong thực thể</param>
        /// <returns>Thực thể</returns>
        /// CREATED BY: DVHAI 08/07/2021
        public TEntity GetEntityByProperty(TEntity entity, PropertyInfo property)
        {
            //1. Thông tin của trường hiện tại
            var propertyName = property.Name;
            var propertyValue = property.GetValue(entity);

            //2. Thông tin khóa
            var keyName = GetKeyProperty().Name;
            var keyValue = GetKeyProperty().GetValue(entity);

            string query = string.Empty;

            //3. Kiểm tra kiểu form
            if (entity.EntityState == EntityState.Add)
                query = $"SELECT * FROM {_tableName} WHERE {propertyName} = '{propertyValue}'";
            else if (entity.EntityState == EntityState.Update)
                query = $"SELECT * FROM {_tableName} WHERE {propertyName} = '{propertyValue}' AND {keyName} <> '{keyValue}'";
            else
                return null;

            var entityReturn = _dbConnection.Query<TEntity>(query, commandType: CommandType.Text).FirstOrDefault();
            return entityReturn;
        }


        /// <summary>
        /// Lấy thưc thể theo thuộc tính
        /// </summary>
        /// <param name="propertyName">Thuộc tính</param>
        /// <param name="propertyValue">Giá trị của thuộc tính</param>
        /// <returns>Thực thể</returns>
        /// CREATED BY: DVHAI 08/07/2021
        public TEntity GetEntityByProperty(string propertyName, object propertyValue)
        {
            string query = $"SELECT * FROM {_tableName} WHERE {propertyName} = '{propertyValue}'";
            var entityReturn = _dbConnection.Query<TEntity>(query, commandType: CommandType.Text).FirstOrDefault();
            return entityReturn;
        }

        /// <summary>
        /// Đóng kết nối
        /// </summary>
        public void Dispose()
        {
            if (_dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
            }
        }
        #endregion
    }
}