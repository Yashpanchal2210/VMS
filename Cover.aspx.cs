using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace VMS_1
{
    public partial class Page1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                // FormsAuthentication.RedirectToLoginPage();
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            string monthYear = monthYearPicker.Value; // Assuming monthYearPicker is a control in your ASPX page for selecting month and year

            // Parse the monthYear string to get month and year
            DateTime selectedDate = DateTime.ParseExact(monthYear, "yyyy-MM", null);
            string formattedMonthYear = selectedDate.ToString("MMMM yyyy");

            // Create Excel package
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                // Add a new worksheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Page1 Report");

                // Set default font for the entire worksheet
                worksheet.Cells.Style.Font.Name = "Arial";

                // Set column widths (adjust as necessary)
                worksheet.Column(1).Width = 15; // Adjust column widths as per your content
                worksheet.Column(2).Width = 15;

                // Add your content and formatting here
                // Merge cells for the title
                ExcelRange titleRange = worksheet.Cells["A1:N4"];
                titleRange.Merge = true;
                titleRange.Value = "IN LIEU OF IN 213";
                titleRange.Style.Font.Size = 36;
                titleRange.Style.Font.Bold = true;
                titleRange.Style.Font.Italic = true;
                titleRange.Style.Font.Name = "Times New Roman";
                titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Add blank rows (if needed)
                int blankRowOffset = 1;
                worksheet.InsertRow(5, blankRowOffset); // Insert blank row after title

                // Second line
                ExcelRange secondLineRange = worksheet.Cells["A6:N9"];
                secondLineRange.Merge = true;
                secondLineRange.Value = "VICTUALLING ACCOUNT";
                secondLineRange.Style.Font.Size = 36;
                secondLineRange.Style.Font.Italic = true;
                secondLineRange.Style.Font.Name = "Arial";
                secondLineRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                secondLineRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Add blank rows (if needed)
                worksheet.InsertRow(10, blankRowOffset); // Insert blank row after second line

                // Third line
                ExcelRange thirdLineRange = worksheet.Cells["A11:N14"];
                thirdLineRange.Merge = true;
                thirdLineRange.Value = $"for the month of {formattedMonthYear}";
                thirdLineRange.Style.Font.Size = 36;
                thirdLineRange.Style.Font.Bold = true;
                thirdLineRange.Style.Font.Name = "Arial";
                thirdLineRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                thirdLineRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Add blank rows (if needed)
                worksheet.InsertRow(15, blankRowOffset); // Insert blank row after third line

                // Fourth line (INS HAMLA)
                ExcelRange fourthLineRange = worksheet.Cells["A16:N19"];
                fourthLineRange.Merge = true;
                fourthLineRange.Value = "INS HAMLA";
                fourthLineRange.Style.Font.Size = 36;
                fourthLineRange.Style.Font.Bold = true;
                fourthLineRange.Style.Font.Name = "Times New Roman";
                fourthLineRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                fourthLineRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Set print area to fit on one page
                worksheet.PrinterSettings.FitToPage = true;
                worksheet.PrinterSettings.FitToWidth = 1;
                worksheet.PrinterSettings.FitToHeight = 0;

                // Save and download the Excel file
                string fileName = $"Page1_Report.xlsx";
                FileInfo excelFile = new FileInfo(Server.MapPath($"~/{fileName}"));
                excelPackage.SaveAs(excelFile);

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(excelFile.FullName);
                Response.Flush();
                Response.End();
            }
        }
    }
}
