using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class ExportIssueOfficer_Sailor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private DataTable FetchIssueData(string monthYear)
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT * FROM IssueMaster WHERE ISDATE(CONVERT(VARCHAR(10), Date, 120)) = 1 AND CONVERT(VARCHAR(7), Date, 120) = @Month";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", monthYear);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }

            return dt;
        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            string monthYear = monthYearPicker.Value; // Assuming monthYearPicker.Value holds the selected month and year in YYYY-MM format
            DataTable dt = FetchIssueData(monthYear);
            object sumNonEntitledOfficer = dt.Compute("SUM(NonEntitled_Officer)", "");
            object sumNonEntitledSailor = dt.Compute("SUM(NonEntitled_Sailor)", "");
            object sumCivilian = dt.Compute("SUM(Civilian)", "");
            dt.Columns.Remove("NonEntitled_Officer");
            dt.Columns.Remove("NonEntitled_Sailor");
            dt.Columns.Remove("Civilian");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Export to Excel
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Strength Report");
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells.Style.Font.Size = 12;

                // Setting the column widths and header values
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[4, i + 1].Value = dt.Columns[i].ColumnName;
                    worksheet.Column(i + 1).Width = 15;
                }

                // Formatting for header rows
                ExcelRange headerRange = worksheet.Cells["A1:J1"];
                headerRange.Merge = true;
                headerRange.Style.Font.Bold = true;
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                headerRange.Value = "PAGE 13";

                worksheet.Cells["A2:J2"].Merge = true;
                worksheet.Cells["A2:J2"].Style.Font.Bold = true;
                worksheet.Cells["A2:J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A2:J2"].Value = DateTime.Parse(monthYear).ToString("MMMM yyyy").ToUpper();

                worksheet.Cells["A3:J3"].Merge = true;
                worksheet.Cells["A3:J3"].Style.Font.Bold = true;
                worksheet.Cells["A3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A3:J3"].Value = "NUMBER VICTUALLED";

                worksheet.Cells["A4:J4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A4:J4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A4:J4"].Style.Font.Bold = true;
                worksheet.Cells["A4"].Value = "Date";
                worksheet.Cells["B4:F4"].Merge = true;
                worksheet.Cells["B4:F4"].Value = "OFFICERS";
                worksheet.Cells["G4:J4"].Merge = true;
                worksheet.Cells["G4:J4"].Value = "SAILORS";

                worksheet.Cells["A5:J5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A5:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A5:J5"].Style.Font.Bold = true;
                worksheet.Cells["A5"].Value = "";
                worksheet.Cells["B5"].Value = "S";
                worksheet.Cells["C5"].Value = "V";
                worksheet.Cells["D5"].Value = "RIK";
                worksheet.Cells["E5"].Value = "Hon.";
                worksheet.Cells["F5"].Value = "Total";
                worksheet.Cells["G5"].Value = "S";
                worksheet.Cells["H5"].Value = "V";
                worksheet.Cells["I5"].Value = "RIK";
                worksheet.Cells["J5"].Value = "Total";

                worksheet.Cells["A37:C37"].Merge = true;
                worksheet.Cells["A37:C37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A37:C37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A37:C37"].Value = "NON ENTITLED";
                worksheet.Cells["D37:E37"].Merge = true;
                worksheet.Cells["D37:E37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D37:E37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["D37:E37"].Value = "OFFICERS";
                worksheet.Cells["G37:I37"].Merge = true;
                worksheet.Cells["G37:I37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["G37:I37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["G37:I37"].Value = "SAILORS";
                worksheet.Cells["A38:I38"].Merge = true;
                worksheet.Cells["A38:I38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A38:I38"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A38:I38"].Value = "NON ENTITLED CIVILIANS";
                worksheet.Cells["A39"].Style.Font.Bold = true;
                worksheet.Cells["A39"].Value = "TOTAL";
                worksheet.Cells["F37"].Value = sumNonEntitledOfficer;
                worksheet.Cells["F37"].Style.Font.Bold = true;
                worksheet.Cells["F37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J37"].Value = sumNonEntitledSailor;
                worksheet.Cells["J37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J37"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J37"].Style.Font.Bold = true;
                worksheet.Cells["J38"].Value = sumCivilian;
                worksheet.Cells["J38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J38"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J38"].Style.Font.Bold = true;
                worksheet.Cells["B39"].Formula = string.Format("SUM(B6:B36)");
                worksheet.Cells["B39"].Style.Font.Bold = true;
                worksheet.Cells["B39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["C39"].Formula = string.Format("SUM(C6:C36)");
                worksheet.Cells["C39"].Style.Font.Bold = true;
                worksheet.Cells["C39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["C39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["D39"].Formula = string.Format("SUM(D6:D36)");
                worksheet.Cells["D39"].Style.Font.Bold = true;
                worksheet.Cells["D39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["D39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["F39"].Formula = string.Format("SUM(F6:F36)");
                worksheet.Cells["F39"].Style.Font.Bold = true;
                worksheet.Cells["F39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["F39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["G39"].Formula = string.Format("SUM(G6:G36)");
                worksheet.Cells["G39"].Style.Font.Bold = true;
                worksheet.Cells["G39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["G39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["H39"].Formula = string.Format("SUM(H6:H36)");
                worksheet.Cells["H39"].Style.Font.Bold = true;
                worksheet.Cells["H39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["H39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["I39"].Formula = string.Format("SUM(I6:I36)");
                worksheet.Cells["I39"].Style.Font.Bold = true;
                worksheet.Cells["I39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["I39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["J39"].Formula = string.Format("SUM(J6:J38)");
                worksheet.Cells["J39"].Style.Font.Bold = true;
                worksheet.Cells["J39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["J39"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Set the row height
                for (int i = 1; i <= 39; i++)
                {
                    worksheet.Row(i).Height = 15;
                }

                // Formatting for data rows and header rows
                using (ExcelRange rng = worksheet.Cells["A1:J39"])
                {
                    rng.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    rng.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                    rng.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    rng.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                int rowIndex = 6; // Starting row for data
                foreach (DataRow row in dt.Rows)
                {
                    DateTime dateValue = (DateTime)row["Date"];
                    worksheet.Cells[rowIndex, 1].Value = dateValue.ToString("dd MMM yyyy");
                    worksheet.Cells[rowIndex, 2].Value = row["Veg_Officer"];
                    worksheet.Cells[rowIndex, 3].Value = row["NonVeg_Officer"];
                    worksheet.Cells[rowIndex, 4].Value = row["RIK_Officer"];
                    worksheet.Cells[rowIndex, 6].Value = row["Total_day_officer"];
                    worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 7].Value = row["Veg_Sailor"];
                    worksheet.Cells[rowIndex, 8].Value = row["NonVeg_Sailor"];
                    worksheet.Cells[rowIndex, 9].Value = row["RIK_Sailor"];
                    worksheet.Cells[rowIndex, 10].Value = row["Total_day_sailor"];
                    worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                    rowIndex++;
                }

                // Save the file
                string fileName = $"Strength_{DateTime.Parse(monthYear).ToString("MMMM_yyyy")}.xlsx";
                FileInfo excelFile = new FileInfo(Server.MapPath($"~/{fileName}"));
                excelPackage.SaveAs(excelFile);

                // Provide download link
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