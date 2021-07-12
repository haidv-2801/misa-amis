using ClosedXML.Excel;
using MISA.AMIS.ApplicationCoore.Entities;
using MISA.AMIS.ApplicationCore.Entities;
using MISA.AMIS.ApplicationCore.Interfaces;
using MISA.AMIS.Entities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MISA.AMIS.ApplicationCore.Interfaces
{
    public class EmployeeService : BaseService<Employee>, IEmployeeService
    {
        #region Declare
        IEmployeeRepository _employeeRepository;
        #endregion

        #region Constructer
        public EmployeeService(IEmployeeRepository employeeRepository) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        #endregion

        #region Methods


        /// <summary>
        /// Lấy mã mới nhất của nhân viên cộng với 1
        /// </summary>
        /// <returns>Mã nhân viên</returns>
        ///CREATED BY: HAIDV 09/07/2021
        public string GetNewEmployeeCode()
        {
            var employeeCode = _employeeRepository.GetNewEmployeeCode();
            try
            {
                employeeCode = NextEmployeeCode(employeeCode);
            }
            catch (Exception) { }

            return employeeCode;
        }

        /// <summary>
        /// Lấy nhân viên theo mã
        /// </summary>
        /// <returns>Nhân viên</returns>
        /// CREATED BY: HAIDV 09/07/2021
        public Employee GetEmployeeByCode(string employeeCode)
        {
            return _employeeRepository.GetEmployeeByCode(employeeCode);
        }

        /// <summary>
        /// Tính toán mã nhân viên mới nhất
        /// </summary>
        /// <param name="employeeCode">Mã cũ</param>
        /// <returns></returns>
        private string NextEmployeeCode(string employeeCode)
        {
            int firstDigitIndex = employeeCode.IndexOfAny(new char[]
                { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
            string prefix = employeeCode.Substring(0, firstDigitIndex);
            string nextNum = (int.Parse(employeeCode.Substring(firstDigitIndex)) + 1).ToString().PadLeft(5, '0');
            return string.Concat(prefix, nextNum);
        }

        /// <summary>
        /// Lấy danh sách nhân viên phân trang, tìm kiếm
        /// </summary>
        /// <param name="filterValue">Giá trị tìm kiếm</param>
        /// <param name="pageSize">Số bản ghi trên 1 trang</param>
        /// <param name="offset">Số trang</param>
        /// <returns>Danh sách nhân viên</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public ServiceResult GetEmployeesFilterPaging(string filterValue, int pageSize, int pageNumber)
        {
            var employeeFilterPaging = _employeeRepository.GetEmployeesFilterPaging(filterValue, pageSize, pageNumber).ToList();
            int totalRecord = _employeeRepository.GetTotalEmployeeFilter(filterValue);

            _serviceResult.Data = new
            {
                totalRecord = totalRecord,
                totalPage = totalRecord % pageSize == 0 ? (totalRecord / pageSize) : (totalRecord / pageSize) + 1,
                pageSize = pageSize,
                pageNumber = pageNumber,
                data = employeeFilterPaging
            };

            return _serviceResult;
        }

        /// <summary>
        /// Validate tùy chỉn theo màn hình nhân viên
        /// </summary>
        /// <param name="entity">Thực thể nhân viên</param>
        /// <returns>(true-false)</returns>
        protected override bool ValidateCustom(Employee employee)
        {
            bool isValid = true;

            //1. Đọc các property
            var properties = employee.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (isValid && property.IsDefined(typeof(IDuplicate), false))
                {
                    //1.1 Check trùng
                    isValid = ValidateDuplicate(employee, property);
                }

                if (isValid && property.IsDefined(typeof(IEmailFormat), false))
                {
                    //1.2 Kiểm tra định dạng email
                    isValid = ValidateEmail(employee, property);
                }
            }
            return isValid;
        }

        /// <summary>
        /// Validate trùng
        /// </summary>
        /// <param name="entity">Thực thể</param>
        /// <param name="propertyInfo">Thuộc tính của thực thể</param>
        /// <returns>(true-đúng false-sai)</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private bool ValidateDuplicate(Employee employee, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            //1. Tên trường
            var propertyName = propertyInfo.Name;

            //2. Tên hiển thị
            var propertyDisplayName = GetAttributeDisplayName(propertyName);

            //3. Tùy chỉnh nguồn dữ liệu để validate, trạng thái thêm hoắc sửa
            var entityDuplicate = _employeeRepository.GetEntityByProperty(employee, propertyInfo);

            if (entityDuplicate != null)
            {
                isValid = false;

                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
                _serviceResult.Data = string.Format(Properties.Resources.Msg_Duplicate, propertyDisplayName);
            }

            return isValid;
        }

        /// <summary>
        /// Validate định dạng email
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="propertyInfo"></param>
        /// <returns>(true-đúng false-sai)</returns>
        /// CREATED BY: DVHAI (07/07/2021)
        private bool ValidateEmail(Employee employee, PropertyInfo propertyInfo)
        {
            bool isValid = true;

            //1. Tên trường
            var propertyName = propertyInfo.Name;

            //2. Tên hiển thị
            var propertyDisplayName = GetAttributeDisplayName(propertyName);

            //3. Gía trị
            var value = propertyInfo.GetValue(employee);

            //Không validate required
            if (string.IsNullOrEmpty(value.ToString()))
                return isValid;


            isValid = new EmailAddressAttribute().IsValid(value.ToString());

            if (!isValid)
            {
                _serviceResult.MISACode = MISACode.InValid;
                _serviceResult.Messasge = Properties.Resources.Msg_NotValid;
                _serviceResult.Data = string.Format(Properties.Resources.Msg_NotFormat, propertyDisplayName);
            }

            return isValid;
        }

        /// <summary>
        /// Xuất excel
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        /// CREATED BY: DVHAI (07/07/2021)
        public MemoryStream Export(CancellationToken cancellationToken, string filterValue)
        {
            var employees = _employeeRepository.GetEntities();
            var exportColumns = _employeeRepository.GetExportColumns();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");




                workSheet.Cells.LoadFromCollection(employees, true);
                package.Save();
            }
            stream.Position = 0;
            return stream;
        }

        public string ExportEmployee()
        {
            //Lấy ra danh sách nhân viên
            List<Employee> employees = _employeeRepository.GetAllEmployee();
            //Lấy ra danh sách cột cần xuất khẩu
            List<ExportColumn> exportColumns = _employeeRepository.GetExportColumn();
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Employees");
                worksheet.Cell(3, 1).Value = "STT";
                worksheet.Cell(3, 1).Style.Font.SetBold();
                worksheet.Cell(3, 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                worksheet.Cell(3, 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(3, 1).Style.Border.TopBorderColor = XLColor.Black;
                worksheet.Cell(3, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(3, 1).Style.Border.LeftBorderColor = XLColor.Black;
                worksheet.Cell(3, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(3, 1).Style.Border.RightBorderColor = XLColor.Black;
                worksheet.Cell(3, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(3, 1).Style.Border.BottomBorderColor = XLColor.Black;
                for (int i = 1; i < exportColumns.Count; i++)
                {
                    worksheet.Cell(3, i + 1).Value = exportColumns[i - 1].DisplayName;
                    worksheet.Cell(3, i + 1).Style.Font.SetBold();
                    worksheet.Cell(3, i + 1).Style.Fill.SetBackgroundColor(XLColor.LightGray);
                    worksheet.Cell(3, i + 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(3, i + 1).Style.Border.TopBorderColor = XLColor.Black;
                    worksheet.Cell(3, i + 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(3, i + 1).Style.Border.LeftBorderColor = XLColor.Black;
                    worksheet.Cell(3, i + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(3, i + 1).Style.Border.RightBorderColor = XLColor.Black;
                    worksheet.Cell(3, i + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(3, i + 1).Style.Border.BottomBorderColor = XLColor.Black;
                    worksheet.Column(i + 1).Width = exportColumns[i - 1].Width;
                }
                for (int index = 1; index <= employees.Count; index++)
                {
                    worksheet.Cell(index + 3, 1).Value = index;
                    worksheet.Cell(index + 3, 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(index + 3, 1).Style.Border.TopBorderColor = XLColor.Black;
                    worksheet.Cell(index + 3, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(index + 3, 1).Style.Border.LeftBorderColor = XLColor.Black;
                    worksheet.Cell(index + 3, 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(index + 3, 1).Style.Border.RightBorderColor = XLColor.Black;
                    worksheet.Cell(index + 3, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    worksheet.Cell(index + 3, 1).Style.Border.BottomBorderColor = XLColor.Black;
                    for (int i = 1; i < exportColumns.Count; i++)
                    {
                        //Xử lý màu
                        worksheet.Cell(index + 3, i + 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(index + 3, i + 1).Style.Border.TopBorderColor = XLColor.Black;
                        worksheet.Cell(index + 3, i + 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(index + 3, i + 1).Style.Border.LeftBorderColor = XLColor.Black;
                        worksheet.Cell(index + 3, i + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(index + 3, i + 1).Style.Border.RightBorderColor = XLColor.Black;
                        worksheet.Cell(index + 3, i + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(index + 3, i + 1).Style.Border.BottomBorderColor = XLColor.Black;
                        //Xử lý trường hợp là giới tính
                        if (exportColumns[i - 1].FieldName == "Gender")
                        {
                            if (GetValueByProperty(employees[index - 1], exportColumns[i - 1].FieldName) is null)
                            {
                                worksheet.Cell(index + 3, i + 1).Value = "";
                                continue;
                            }
                            switch (int.Parse(GetValueByProperty(employees[index - 1], exportColumns[i - 1].FieldName).ToString()))
                            {
                                case 0:
                                    worksheet.Cell(index + 3, i + 1).Value = "Nữ";
                                    break;
                                case 1:
                                    worksheet.Cell(index + 3, i + 1).Value = "Nam";
                                    break;
                                case 2:
                                    worksheet.Cell(index + 3, i + 1).Value = "Khác";
                                    break;
                                default:
                                    worksheet.Cell(index + 3, i + 1).Value = "";
                                    break;
                            }
                            continue;
                        }
                        worksheet.Cell(index + 3, i + 1).Value = GetValueByProperty(employees[index - 1], exportColumns[i - 1].FieldName);
                    }
                }
                worksheet.Range("A1:J1").Merge();
                worksheet.Cell(1, 1).Value = "DANH SÁCH NHÂN VIÊN";
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Font.SetBold();
                worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range("A2:J2").Merge();
                // Xóa file trước khi tạo
                if (File.Exists(Path.Combine("./Resources/Danh_sach_nhan_vien.xlsx")))
                {
                    File.Delete(Path.Combine("./Resources/Danh_sach_nhan_vien.xlsx"));
                }
                workbook.SaveAs("./Resources/Danh_sach_nhan_vien.xlsx");
                string path = Path.Combine("Resources", "Danh_sach_nhan_vien.xlsx");
                return _configuration.GetConnectionString("HostUpload") + path.Replace("\\", "/");
            }
        }
        #endregion
    }
}
