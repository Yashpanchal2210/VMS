using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;
using System.IO;

namespace VMS_1
{
    public partial class Page14 : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetCONames();
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetCONames()
        {
            HashSet<string> CoNames = new HashSet<string>();
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "select name from usermaster where Role = 'Commanding Officer'";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        CoNames.Add(reader["name"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading item names: " + ex.Message);
            }

            return CoNames.ToArray();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetLONames()
        {
            HashSet<string> CoNames = new HashSet<string>();
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "select name from usermaster where Role = 'Logistic Officer'";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        CoNames.Add(reader["name"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading item names: " + ex.Message);
            }

            return CoNames.ToArray();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static string[] GetAONames()
        {
            HashSet<string> AoNames = new HashSet<string>();
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "select name from usermaster where Role = 'Accounting Officer'";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(query, conn);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        AoNames.Add(reader["name"].ToString());
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while loading item names: " + ex.Message);
            }

            return AoNames.ToArray();
        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            string monthYear = monthYearPicker.Value;
            string COName = Request.Form["coVal"]; ;
            string LOName = Request.Form["loVal"]; ;
            string AOName = Request.Form["aoVal"]; ;

            DateTime selectedDate = DateTime.ParseExact(monthYear, "yyyy-MM", null);
            string formattedMonthYear = selectedDate.ToString("MMMM yyyy");

            string commandingOfficer = COName;
            string logisticsOfficer = LOName;
            string accountingOfficer = AOName;
            string currentUser = HttpContext.Current.User.Identity.Name;
            string currentUserRank = Session["Rank"].ToString();
            string currentUserNudID = Session["NudId"].ToString();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Page14 Report");

                worksheet.Cells.Style.Font.Name = "Arial";

                // Title and content based on provided image
                worksheet.Cells["A1:M1"].Merge = true;
                worksheet.Cells["A1:M1"].Value = $"Page - 14";
                worksheet.Cells["A1:M1"].Style.Font.Size = 12;
                worksheet.Cells["A1:M1"].Style.Font.Bold = true;
                worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A3:M3"].Value = "CERTIFICATE NO. 1 OF MUSTER";
                worksheet.Cells["A3:M3"].Style.Font.Size = 12;
                worksheet.Cells["A3:M3"].Style.Font.Bold = true;
                worksheet.Cells["A3:M3"].Style.Font.UnderLine = true;
                worksheet.Cells["A3:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:M3"].Merge = true;

                worksheet.Cells["B5:I5"].Merge = true;
                worksheet.Cells["B5:I5"].Value = $"Victualling stores have been mustered by the Officer-in-Charge on {formattedMonthYear} and found correct.";
                worksheet.Cells["B5:I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                worksheet.Cells["F9:H9"].Merge = true;
                worksheet.Cells["F9:H9"].Value = "Signature and Rank of Accounting Officer.";
                worksheet.Cells["F9:H9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["L9:M9"].Merge = true;
                worksheet.Cells["L9:M9"].Value = "("+logisticsOfficer+")";
                worksheet.Cells["L9:M9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L10:M10"].Merge = true;
                worksheet.Cells["L10:M10"].Value = "Lieutenant Commander";
                worksheet.Cells["L10:M10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L11:M11"].Merge = true;
                worksheet.Cells["L11:M11"].Value = "Logistics Officer";
                worksheet.Cells["L11:M11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                worksheet.Cells["A13:M13"].Merge = true;
                worksheet.Cells["A13:M13"].Style.Font.Size = 12;
                worksheet.Cells["A13:M13"].Style.Font.Bold = true;
                worksheet.Cells["A13:M13"].Style.Font.UnderLine = true;
                worksheet.Cells["A13:M13"].Value = "CERTIFICATE NO. 2 OF CREDIT OF GOVT. CASH RECOVERIES";
                worksheet.Cells["A13:M13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A15:M15"].Merge = true;
                worksheet.Cells["A15:M15"].Value = "Certified that the sum of Rs.Nil  on account of payment issues as details on page 10 & 11  has been credited by the Logistics Officer of the Ship through MRO vide MRO No. Nil as per details given therein.";
                worksheet.Cells["A15:M15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["F20:I20"].Merge = true;
                worksheet.Cells["F20:I20"].Value = "Signature and Rank of Officer maintaining cash A/c";
                worksheet.Cells["F20:I20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L20:M20"].Merge = true;
                worksheet.Cells["L20:M20"].Value = "(" + logisticsOfficer + ")";
                worksheet.Cells["L20:M20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L21:M21"].Merge = true;
                worksheet.Cells["L21:M21"].Value = "Lieutenant Commander";
                worksheet.Cells["L21:M21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L22:M22"].Merge = true;
                worksheet.Cells["L22:M22"].Value = "Logistics Officer";
                worksheet.Cells["L22:M22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["B24:E24"].Merge = true;
                worksheet.Cells["A13:M13"].Style.Font.Bold = true;
                worksheet.Cells["B24:E24"].Value = "APPROVED AND CERTIFIED THAT :-";
                worksheet.Cells["B24:E24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["B26"].Value = "(i)";
                worksheet.Cells["B27"].Value = "(ii)";
                worksheet.Cells["B28"].Value = "(iii)";
                worksheet.Cells["B29"].Value = "(iv)";
                worksheet.Cells["B30"].Value = "(v)";

                worksheet.Cells["C26:K26"].Merge = true;
                worksheet.Cells["C26:K26"].Value = "The extra issues shown on page 9 have been made under Commanding Officer's authority.";
                worksheet.Cells["C26:K26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["C27:K27"].Merge = true;
                worksheet.Cells["C27:K27"].Value = "The quantity of packing material charged off for sanitary purpooses were issued under Commanding Officer's authority.";
                worksheet.Cells["C27:K27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["C28:K28"].Merge = true;
                worksheet.Cells["C28:K28"].Value = "The remains as shown in this account are correct to the best of my knowledge and belief .";
                worksheet.Cells["C28:K28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["C29:K29"].Merge = true;
                worksheet.Cells["C29:K29"].Value = "The quarterly muster of sailors has been carried out in accordance with Reg. No.0353 of Regs Navy.";
                worksheet.Cells["C29:K29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["C30:K30"].Merge = true;
                worksheet.Cells["C30:K30"].Value = "The periodicity of issue of fish has been restricted in accordance with NI 2/98 as ammended from time to time.";
                worksheet.Cells["C30:K30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["K32:M32"].Merge = true;
                worksheet.Cells["K32:M32"].Value = "Signature of the Commanding Officer";
                worksheet.Cells["K32:M32"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L36:M36"].Merge = true;
                worksheet.Cells["L36:M36"].Value = "(" + commandingOfficer + ")";
                worksheet.Cells["L36:M36"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L37:M37"].Merge = true;
                worksheet.Cells["L37:M37"].Value = "Commodore";
                worksheet.Cells["L37:M37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["L38:M38"].Merge = true;
                worksheet.Cells["L38:M38"].Value = "Commanding Officer";
                worksheet.Cells["L38:M38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["B37:C37"].Merge = true;
                worksheet.Cells["B37:C37"].Value = "INS Hamla";
                worksheet.Cells["B37:C37"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["B38:C38"].Merge = true;
                worksheet.Cells["B38:C38"].Value = "Marve Malad(W)";
                worksheet.Cells["B38:C38"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["B39:C39"].Merge = true;
                worksheet.Cells["B39:C39"].Value = "Mumbai-95";
                worksheet.Cells["B39:C39"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["F39"].Value = "Date:";

                // Adjust column widths
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save and download the Excel file
                string fileName = $"Page14_Report_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
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