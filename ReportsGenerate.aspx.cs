using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class ReportsGenerate : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");

        }
        protected void ViewToExcelButton_Click(object sender, EventArgs e)
        {
            string selectedMonthYear = monthYearPicker.Value;
            DataTable dt = FetchStrengthData(selectedMonthYear);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindGridView();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGridView();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GridView1.Rows[e.RowIndex];
            string id = GridView1.DataKeys[e.RowIndex].Value.ToString();
            string updatedValue1 = ((TextBox)row.FindControl("txtVegOfficers")).Text;
            string updatedValue2 = ((TextBox)row.FindControl("txtNonVegOfficers")).Text;
            string updatedValue3 = ((TextBox)row.FindControl("txtVegSailor")).Text;
            string updatedValue4 = ((TextBox)row.FindControl("txtNonVegSailor")).Text;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE Strength SET vegOfficers = @VegOfficers, nonVegOfficers = @NonVegOfficers, vegSailor = @VegSailor, nonVegSailor = @NonVegSailor WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@VegOfficers", updatedValue1);
                cmd.Parameters.AddWithValue("@NonVegOfficers", updatedValue2);
                cmd.Parameters.AddWithValue("@VegSailor", updatedValue3);
                cmd.Parameters.AddWithValue("@NonVegSailor", updatedValue4);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            GridView1.EditIndex = -1;
            BindGridView();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string id = GridView1.DataKeys[e.RowIndex].Value.ToString();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Strength WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            BindGridView(); // Rebind GridView after deletion
        }

        private void BindGridView()
        {
            string selectedMonthYear = monthYearPicker.Value;
            DataTable dt = FetchStrengthData(selectedMonthYear);
            GridView1.DataSource = dt;
            GridView1.DataBind();
        }
        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            string monthYear = monthYearPicker.Value; 
            DataTable dt = FetchStrengthData(monthYear);

            
            dt.Columns.Add("rikofficer", typeof(int));
            dt.Columns.Add("riksailor", typeof(int));
            dt.Columns.Add("nonentitledofficer", typeof(int));
            dt.Columns.Add("nonentitledsailor", typeof(int));

            foreach (DataRow row in dt.Rows)
            {
                row["rikofficer"] = Convert.ToInt32(row["vegrikOfficers"]) + Convert.ToInt32(row["nonVegrikOfficers"]);
                row["riksailor"] = Convert.ToInt32(row["vegSailorRik"]) + Convert.ToInt32(row["nonVegSailorRik"]);
                row["nonentitledofficer"] = Convert.ToInt32(row["vegNonEntitledOfficer"]) + Convert.ToInt32(row["nonVegNonEntitledOfficer"]);
                row["nonentitledsailor"] = Convert.ToInt32(row["vegNonEntitledSailor"]) + Convert.ToInt32(row["NonVegNonEntitledSailor"]);
            }

            
            object sumNonEntitledOfficer = dt.Compute("SUM(nonentitledofficer)", "");
            object sumRIKOfficer = dt.Compute("SUM(rikofficer)", "");
            object sumNonEntitledSailor = dt.Compute("SUM(nonentitledsailor)", "");
            object sumCivilian = dt.Compute("SUM(civilians)", "");

            dt.Columns.Remove("vegNonEntitledOfficer");
            dt.Columns.Remove("nonVegNonEntitledOfficer");
            dt.Columns.Remove("vegNonEntitledSailor");
            dt.Columns.Remove("NonVegNonEntitledSailor");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Strength Report");
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells.Style.Font.Size = 12;

                worksheet.Column(1).Width = 15;
                worksheet.Cells["A1:J1"].Merge = true;
                worksheet.Cells["A1:J1"].Value = "PAGE 13";
                worksheet.Cells["A1:J1"].Style.Font.Bold = true;
                worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:J1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A2:J2"].Merge = true;
                DateTime parsedMonthYear = DateTime.Parse(monthYear); // Corrected parsing
                worksheet.Cells["A2:J2"].Value = parsedMonthYear.ToString("MMMM yyyy").ToUpper();
                worksheet.Cells["A2:J2"].Style.Font.Bold = true;
                worksheet.Cells["A2:J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:J2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A3:J3"].Merge = true;
                worksheet.Cells["A3:J3"].Value = "NUMBER VICTUALLED";
                worksheet.Cells["A3:J3"].Style.Font.Bold = true;
                worksheet.Cells["A3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:J3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["A4"].Value = "Date";
                worksheet.Cells["B4:F4"].Merge = true;
                worksheet.Cells["B4:F4"].Value = "OFFICERS";
                worksheet.Cells["G4:J4"].Merge = true;
                worksheet.Cells["G4:J4"].Value = "SAILORS";

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

                int rowIndex = 6;
                foreach (DataRow row in dt.Rows)
                {
                    DateTime dateValue = (DateTime)row["dates"];
                    worksheet.Cells[rowIndex, 1].Value = dateValue.ToString("dd MMM yyyy");
                    worksheet.Cells[rowIndex, 2].Value = row["vegOfficers"];
                    worksheet.Cells[rowIndex, 3].Value = row["nonVegOfficers"];
                    worksheet.Cells[rowIndex, 4].Value = row["rikofficer"];
                    worksheet.Cells[rowIndex, 6].Value = Convert.ToInt32(row["vegOfficers"]) + Convert.ToInt32(row["nonVegOfficers"]) + Convert.ToInt32(row["rikofficer"]);
                    worksheet.Cells[rowIndex, 6].Style.Font.Bold = true;
                    worksheet.Cells[rowIndex, 7].Value = row["vegSailor"];
                    worksheet.Cells[rowIndex, 8].Value = row["nonVegSailor"];
                    worksheet.Cells[rowIndex, 9].Value = row["riksailor"];
                    worksheet.Cells[rowIndex, 10].Value = Convert.ToInt32(row["vegSailor"]) + Convert.ToInt32(row["nonVegSailor"]) + Convert.ToInt32(row["riksailor"]);
                    worksheet.Cells[rowIndex, 10].Style.Font.Bold = true;
                    rowIndex++;
                }

                // Totals
                worksheet.Cells["F37"].Value = sumNonEntitledOfficer;
                worksheet.Cells["F37"].Style.Font.Bold = true;
                worksheet.Cells["J37"].Value = sumNonEntitledSailor;
                worksheet.Cells["J37"].Style.Font.Bold = true;
                worksheet.Cells["J38"].Value = sumCivilian;
                worksheet.Cells["J38"].Style.Font.Bold = true;

                // Set formulas for totals
                worksheet.Cells["B39"].Formula = $"SUM(B6:B{rowIndex - 1})";
                worksheet.Cells["C39"].Formula = $"SUM(C6:C{rowIndex - 1})";
                worksheet.Cells["D39"].Formula = $"SUM(D6:D{rowIndex - 1})";
                worksheet.Cells["F39"].Formula = $"SUM(F6:F{rowIndex - 1})";
                worksheet.Cells["G39"].Formula = $"SUM(G6:G{rowIndex - 1})";
                worksheet.Cells["H39"].Formula = $"SUM(H6:H{rowIndex - 1})";
                worksheet.Cells["I39"].Formula = $"SUM(I6:I{rowIndex - 1})";
                worksheet.Cells["J39"].Formula = $"SUM(J6:J{rowIndex - 1})";

                // Style adjustments
                worksheet.Cells["A1:J39"].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                worksheet.Cells["A1:J5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A1:J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["A1:J5"].Style.Font.Bold = true;

                // Save and download
                string fileName = $"Strength_{parsedMonthYear.ToString("MMMM_yyyy")}.xlsx";
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

        private DataTable FetchStrengthData(string monthYear)
        {
            
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                
                conn.Open();

                string query = "SELECT * FROM Strength WHERE ISDATE(CONVERT(VARCHAR(10), dates, 120)) = 1 AND CONVERT(VARCHAR(7), dates, 120) = @Month ORDER BY dates ASC;";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", monthYear);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                conn.Close();

            }

            return dt;
        }

    }
}
