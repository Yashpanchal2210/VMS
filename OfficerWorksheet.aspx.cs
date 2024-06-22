using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace VMS_1
{
    public partial class OfficerWorksheet : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            LoadOfficerSheet();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
        }

        //protected void ExportToExcelButton_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string[] selectedDate = monthYearPicker.Value.Split('-');
        //        DataTable strengthData = GetStrengthtData(selectedDate);


        //        string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
        //        int sumVegOfficer = 0;
        //        int sumVegRikOfficer = 0;
        //        int sumVegNonEntitledOfficer = 0;
        //        int sumNonVegOfficer = 0;
        //        int sumNonVegRikOfficer = 0;
        //        int sumNonVegEntitledOfficer = 0;

        //        sumVegOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("TotalVegOfficers"));
        //        sumVegRikOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("TotalVegrikOfficers"));
        //        sumVegNonEntitledOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("TotalVegNonEntitledOfficers"));
        //        sumNonVegOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("TotalNonVegOfficers"));
        //        sumNonVegRikOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("TotalNonVegRikOfficer"));
        //        sumNonVegEntitledOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("TotalNonVegNonEntitledOfficer"));

        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //        using (ExcelPackage excelPackage = new ExcelPackage())
        //        {
        //            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add($"WKSHT OFFICERS {selectedDate[1]}-{selectedDate[0]}");

        //            // Set the title

        //            worksheet.Cells["A1"].Value = $"WKSHT OFFICERS - {DateTime.Now.ToString("MMMM yyyy")}";
        //            worksheet.Cells["A1:I1"].Merge = true;
        //            worksheet.Cells["A1:I1"].Style.Font.Bold = true;
        //            worksheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //            // Set the 'S', 'V', and Total columns
        //            worksheet.Cells["C2"].Value = "'S'";
        //            worksheet.Cells["C2"].Style.Font.Bold = true;
        //            worksheet.Cells["E2"].Value = "'V'";
        //            worksheet.Cells["E2"].Style.Font.Bold = true;
        //            worksheet.Cells["G2"].Value = "Total";
        //            worksheet.Cells["G2"].Style.Font.Bold = true;
        //            worksheet.Cells["C2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        //            // Set the values for 'S', 'V', and Total columns
        //            worksheet.Cells["B3"].Value = "GEN";
        //            worksheet.Cells["B4"].Value = "RIK + HON. OFFICER";
        //            worksheet.Cells["B5"].Value = "NON ENTITLED MESSING";
        //            worksheet.Cells["B6"].Value = "Total";
        //            worksheet.Cells["B6"].Style.Font.Bold = true;

        //            worksheet.Cells["C3"].Value = sumNonVegOfficer;
        //            worksheet.Cells["E3"].Value = sumVegOfficer;
        //            worksheet.Cells["F3"].Value = "=";
        //            worksheet.Cells["C3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            worksheet.Cells["G3"].Formula = "C3 + E3";
        //            worksheet.Cells["G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //            worksheet.Cells["C4"].Value = sumNonVegRikOfficer;
        //            worksheet.Cells["E4"].Value = sumVegRikOfficer;
        //            worksheet.Cells["F4"].Value = "=";
        //            worksheet.Cells["C4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            worksheet.Cells["G4"].Formula = "C4 + E4";
        //            worksheet.Cells["G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //            worksheet.Cells["C5"].Value = sumNonVegEntitledOfficer;
        //            worksheet.Cells["E5"].Value = sumVegNonEntitledOfficer;
        //            worksheet.Cells["F5"].Value = "=";
        //            worksheet.Cells["C5:F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            worksheet.Cells["G5"].Formula = "C5 + E5";
        //            worksheet.Cells["G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

        //            worksheet.Cells["C6"].Formula = "SUM(C3:C5)";
        //            worksheet.Cells["E6"].Formula = "SUM(E3:E5)";
        //            worksheet.Cells["F6"].Value = "=";
        //            worksheet.Cells["C6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //            worksheet.Cells["G6"].Formula = "SUM(G3:G5)";
        //            worksheet.Cells["G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //            worksheet.Cells["G3:G6"].Style.Font.Bold = true;
        //            worksheet.Cells["C3:F6"].Style.Font.Bold = true;


        //            var range = worksheet.Cells["A1:I10"];

        //            // Apply border to the range
        //            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;


        //            worksheet.Cells["A8"].Value = "Ser.";
        //            worksheet.Cells["A8"].Style.Font.UnderLine = true;
        //            worksheet.Cells["B8"].Value = "Item";
        //            worksheet.Cells["B8"].Style.Font.UnderLine = true;
        //            worksheet.Cells["C8"].Value = "Strength";
        //            worksheet.Cells["C8"].Style.Font.UnderLine = true;
        //            worksheet.Cells["E8"].Value = "Scale";
        //            worksheet.Cells["E8"].Style.Font.UnderLine = true;
        //            worksheet.Cells["G8"].Value = "Qty Entitled";
        //            worksheet.Cells["G8"].Style.Font.UnderLine = true;
        //            worksheet.Cells["I8"].Value = "Qty Issued";
        //            worksheet.Cells["I8"].Style.Font.UnderLine = true;


        //            // Save the file
        //            string fileName = $"Officers_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
        //            FileInfo excelFile = new FileInfo(Server.MapPath($"~/{fileName}"));
        //            excelPackage.SaveAs(excelFile);

        //            // Provide download link
        //            Response.Clear();
        //            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
        //            Response.TransmitFile(excelFile.FullName);
        //            Response.Flush();
        //            Response.End();
        //        }
        //    }
        //    catch (ThreadAbortException)
        //    {
        //        // Catch the ThreadAbortException to prevent it from propagating
        //        // This exception is expected when using Response.End()
        //    }
        //    catch (Exception ex)
        //    {
        //        lblStatus.Text = "An error occurred while exporting data: " + ex.Message;
        //    }
        //}

        private DataTable GetStrengthtData(string[] selectedDate)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            DataTable dataTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SUM(vegOfficers) AS TotalVegOfficers, SUM(vegrikOfficers) AS TotalVegrikOfficers, SUM(vegNonEntitledOfficer) AS TotalVegNonEntitledOfficers, SUM(nonVegOfficers) AS TotalNonVegOfficers, SUM(nonVegrikOfficers) AS TotalNonVegRikOfficer, SUM(nonVegNonEntitledOfficer) AS TotalNonVegNonEntitledOfficer FROM strength WHERE MONTH(dates) = @Month AND YEAR(dates) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private void LoadOfficerSheet()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("WITH ItemDetails AS (SELECT i.ItemID, i.ItemName, i.RationScaleOfficer FROM Items i) SELECT id.ItemName, id.RationScaleOfficer, STUFF((SELECT ', ' + a.AltItemName + ' (' + CAST(a.AltRationScaleOfficer AS VARCHAR) + ')'FROM AlternateItem a WHERE a.ItemID = id.ItemID FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS AlternateItems FROM ItemDetails id GROUP BY id.ItemName, id.RationScaleOfficer, id.ItemID", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewOfficersSheet.DataSource = dt;
                    GridViewOfficersSheet.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}