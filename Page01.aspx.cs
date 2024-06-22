using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace VMS_1
{
    public partial class Page01 : System.Web.UI.Page
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
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Page1 Report");

                worksheet.Cells.Style.Font.Name = "Arial";

                // Title and content based on provided image
                worksheet.Cells["A1:M1"].Merge = true;
                worksheet.Cells["A1:M1"].Value = $"ACCOUNT OF VICTUALLING STORES FOR THE MONTH OF {formattedMonthYear}";
                worksheet.Cells["A1:M1"].Style.Font.Size = 12;
                worksheet.Cells["A1:M1"].Style.Font.Bold = true;
                worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                worksheet.Cells["A2"].Value = "Name and Rank of the Commanding Officer:";
                worksheet.Cells["C2"].Value = commandingOfficer;

                worksheet.Cells["A3"].Value = "Name and the Rank of the Logistics Officer:";
                worksheet.Cells["C3"].Value = logisticsOfficer;

                worksheet.Cells["A4"].Value = "Name and Rank of the officer directly responsible for keeping the Account:";
                worksheet.Cells["C4"].Value = accountingOfficer;

                // Instructions
                worksheet.Cells["A6"].Value = "INSTRUCTIONS";
                worksheet.Cells["A6"].Style.Font.Bold = true;
                worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                worksheet.Cells["A7"].Value = "1. The working or rough copy (form IN - 213) of this account is to be written up in ink daily. On completion of the account the following action is to be taken:-";
                worksheet.Cells["A7:M7"].Merge = true;

                worksheet.Cells["A8"].Value = "On completion of the account the following action is to be taken:-";
                worksheet.Cells["A8:M8"].Merge = true;

                worksheet.Cells["A9"].Value = "Ships and establishments carrying their own accounts are to check the working or rough copy and prepare a fair copy for audit. All relevant vouchers and certificates are to accompany the account.";
                worksheet.Cells["A9:M9"].Merge = true;

                worksheet.Cells["A10"].Value = "All relevant vouchers and certificates are to accompany the account.";
                worksheet.Cells["A10:M10"].Merge = true;

                worksheet.Cells["A11"].Value = "Ships and establishments NOT carrying their own accounts are to forward the working OR rough copy of the account to BLOGO together with all relevant vouchers. ";
                worksheet.Cells["A11:M11"].Merge = true;

                worksheet.Cells["A12"].Value = "After checking and completion of the account the BLOGO is to prepare a fair copy for audit. All relevant vouchers and certificates are to accompany the account.";
                worksheet.Cells["A12:M12"].Merge = true;

                worksheet.Cells["A13"].Value = "All relevant vouchers and certificates are to accompany the account.";
                worksheet.Cells["A13:M13"].Merge = true;

                worksheet.Cells["A14"].Value = "2. The quantities remaining as per muster, at the end of the each month in line 28 to be carried forward to the new account in line 4.";
                worksheet.Cells["A14:M14"].Merge = true;

                worksheet.Cells["A15"].Value = "Differences discovered between the actual quantities after muster and the balance as per account are to be entered on line 29, Pages 2 to 7. The certificates on";
                worksheet.Cells["A15:M15"].Merge = true;

                // Details of Enclosures
                worksheet.Cells["A16"].Value = "DETAILS OF ENCLOSURES";
                worksheet.Cells["A16"].Style.Font.Bold = true;
                worksheet.Cells["A16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A17"].Value = "AS PER ATTACHED LIST";
                worksheet.Cells["A17:M17"].Merge = true;

                // Footer
                worksheet.Cells["A15"].Value = "Account written up by (Name):";
                worksheet.Cells["C15"].Value = Session["UserName"];
                worksheet.Cells["E15"].Value = "Rank:";
                worksheet.Cells["F15"].Value = Session["Rank"];
                worksheet.Cells["G15"].Value = "No:";
                worksheet.Cells["H15"].Value = Session["NudId"];

                worksheet.Cells["A16"].Value = "Account Checked up by (Name):";
                worksheet.Cells["E16"].Value = "Rank:";
                worksheet.Cells["G16"].Value = "No:";

                // Adjust column widths
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save and download the Excel file
                string fileName = $"Page1_Report_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
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

        //private string GetOfficerNameByRole(string role)
        //{
        //    string query = "SELECT UserName FROM usermaster WHERE Role = @Role";

        //    using (SqlConnection connection = new SqlConnection(connStr))
        //    {
        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@Role", role);
        //        connection.Open();
        //        return command.ExecuteScalar()?.ToString();
        //    }
        //}

        //private string GetUserRankByUserName(string userName)
        //{
        //    string query = "SELECT Rank FROM usermaster WHERE UserName = @UserName";

        //    using (SqlConnection connection = new SqlConnection(connStr))
        //    {
        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@UserName", userName);
        //        connection.Open();
        //        return command.ExecuteScalar()?.ToString();
        //    }
        //}

        //private string GetUserNudIDByUserName(string userName)
        //{
        //    string query = "SELECT NudID FROM usermaster WHERE UserName = @UserName";

        //    using (SqlConnection connection = new SqlConnection(connStr))
        //    {
        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@UserName", userName);
        //        connection.Open();
        //        return command.ExecuteScalar()?.ToString();
        //    }
        //}
    }
}