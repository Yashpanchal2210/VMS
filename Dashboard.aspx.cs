using System;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Configuration;
using System.Web.Security;
using System.Web;
using System.Web.UI.WebControls;
using System.Threading;
using DocumentFormat.OpenXml.Bibliography;
using System.Web.UI;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using OfficeOpenXml.Drawing.Vml;
using ClosedXML.Excel;
using System.Linq;

namespace VMS_1
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private DataTable dtMonthStock;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            LoadGridViewPresentStock();
            LoadGridViewPage2to7();
            LoadGridViewMonthStock();
            LoadGridViewDivers();
            LoadOfficerSheet();
            LoadGridViewExtraIssue();
            FilterDataByMonth(Convert.ToString(DateTime.Now.Month));
        }

        private void LoadGridViewMonthStock()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM MonthEndStockMaster", conn);
                    dtMonthStock = new DataTable();
                    da.Fill(dtMonthStock);

                    GridViewMonthStock.DataSource = dtMonthStock;
                    GridViewMonthStock.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void GridViewMonthStock_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int dateColumnIndex = 1;
                foreach (DataControlField field in GridViewMonthStock.Columns)
                {
                    if (field.HeaderText == "Date") // Replace "Date" with the actual header text of your date column
                    {
                        dateColumnIndex = GridViewMonthStock.Columns.IndexOf(field);
                        break;
                    }
                }

                if (dateColumnIndex >= 0)
                {
                    string dateText = e.Row.Cells[dateColumnIndex].Text;
                    if (!string.IsNullOrEmpty(dateText))
                    {
                        DateTime dateValue;
                        if (DateTime.TryParse(dateText, out dateValue))
                        {
                            e.Row.Cells[dateColumnIndex].Text = dateValue.ToString("MMMM yyyy");
                        }
                    }
                }

                int typeColumnIndex = 4;
                string type = e.Row.Cells[typeColumnIndex].Text;
                if (type == "Issue")
                {
                    e.Row.CssClass = "issue-row";
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedMonth = ddlMonths.SelectedValue;

            DataTable filteredData = FilterDataByMonth(selectedMonth);

            GridViewMonthStock.DataSource = filteredData;
            GridViewMonthStock.DataBind();
        }

        private DataTable FilterDataByMonth(string month)
        {
            DataTable filteredData = dtMonthStock.Clone();
            foreach (DataRow row in dtMonthStock.Rows)
            {
                DateTime dateValue;
                if (DateTime.TryParse(row["Date"].ToString(), out dateValue))
                {
                    if (dateValue.Month == int.Parse(month))
                    {
                        filteredData.ImportRow(row);
                    }
                }
            }

            return filteredData;
        }

        private void LoadGridViewPresentStock()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM PresentStockMaster", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewPresentStock.DataSource = dt;
                    GridViewPresentStock.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        private void LoadGridViewPage2to7()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(@"SELECT 
                                                PS.*
                                            FROM 
                                                PresentStockMaster PS 
                                            LEFT JOIN 
                                                IssueMaster ISM ON PS.ItemName = ISM.ItemName", conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewP2to7.DataSource = dt;
                    GridViewP2to7.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void ExportPresentStockButton_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                DataTable dt = new DataTable();
                foreach (TableCell cell in GridViewPresentStock.HeaderRow.Cells)
                {
                    dt.Columns.Add(cell.Text);
                }
                foreach (GridViewRow row in GridViewPresentStock.Rows)
                {
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < row.Cells.Count; i++)
                    {
                        dr[i] = row.Cells[i].Text;
                    }
                    dt.Rows.Add(dr);
                }

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PresentStock");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);

                    // Save the file
                    string fileName = $"PresentStock_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
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
            catch (ThreadAbortException)
            {
                // Catch the ThreadAbortException to prevent it from propagating
                // This exception is expected when using Response.End()
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while exporting data: " + ex.Message;
            }
        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            string monthYear = monthYearPicker.Value; // Assuming monthYearPicker.Value holds the selected month and year in YYYY-MM format
            DataTable dt = FetchStrengthData(monthYear);
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

        private DataTable FetchStrengthData(string monthYear)
        {
            //string connStr = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=True;Encrypt=False";
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT * FROM Strength WHERE ISDATE(CONVERT(VARCHAR(10), Date, 120)) = 1 AND CONVERT(VARCHAR(7), Date, 120) = @Month";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", monthYear);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }

            return dt;
        }

        private void LoadGridViewDivers()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ExtraIssue", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssueDivers.DataSource = dt;
                    GridViewExtraIssueDivers.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
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

        protected void ExportOfficersButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                DataTable strengthData = new DataTable();
                int sumVegOfficer = 0;
                int sumNonVegOfficer = 0;
                int sumRikOfficer = 0;
                int sumNonEntitledOfficer = 0;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Strength", conn);
                    dataAdapter.Fill(strengthData);

                    sumVegOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("Veg_Officer"));
                    sumNonVegOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("NonVeg_Officer"));
                    sumRikOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("RIK_Officer"));
                    sumNonEntitledOfficer = strengthData.AsEnumerable().Sum(row => row.Field<int>("NonEntitled_Officer"));

                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("WKSHT OFFICERS -DEC 23");

                    // Set the title

                    worksheet.Cells["A1"].Value = $"WKSHT OFFICERS - {DateTime.Now.ToString("MMMM yyyy")}";
                    worksheet.Cells["A1:I1"].Merge = true;
                    worksheet.Cells["A1:I1"].Style.Font.Bold = true;
                    worksheet.Cells["A1:I1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Set the 'S', 'V', and Total columns
                    worksheet.Cells["C2"].Value = "'S'";
                    worksheet.Cells["C2"].Style.Font.Bold = true;
                    worksheet.Cells["E2"].Value = "'V'";
                    worksheet.Cells["E2"].Style.Font.Bold = true;
                    worksheet.Cells["G2"].Value = "Total";
                    worksheet.Cells["G2"].Style.Font.Bold = true;
                    worksheet.Cells["C2:G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    // Set the values for 'S', 'V', and Total columns
                    worksheet.Cells["B3"].Value = "GEN";
                    worksheet.Cells["B4"].Value = "RIK + HON. OFFICER";
                    worksheet.Cells["B5"].Value = "NON ENTITLED MESSING";
                    worksheet.Cells["B6"].Value = "Total";
                    worksheet.Cells["B6"].Style.Font.Bold = true;

                    worksheet.Cells["C3"].Value = sumVegOfficer;
                    worksheet.Cells["E3"].Value = sumNonVegOfficer;
                    worksheet.Cells["F3"].Value = "=";
                    worksheet.Cells["C3:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G3"].Formula = "C3 + E3";
                    worksheet.Cells["G3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["C4"].Value = "";
                    worksheet.Cells["E4"].Value = "";
                    worksheet.Cells["F4"].Value = "=";
                    worksheet.Cells["C4:F4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G4"].Value = sumRikOfficer;
                    worksheet.Cells["G4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["C5"].Value = "";
                    worksheet.Cells["E5"].Value = "";
                    worksheet.Cells["F5"].Value = "=";
                    worksheet.Cells["C5:F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G5"].Value = sumNonEntitledOfficer;
                    worksheet.Cells["G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells["C6"].Formula = "SUM(C3:C5)";
                    worksheet.Cells["E6"].Formula = "SUM(E3:E5)";
                    worksheet.Cells["F6"].Value = "=";
                    worksheet.Cells["C6:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells["G6"].Formula = "SUM(G3:G5)";
                    worksheet.Cells["G6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    worksheet.Cells["G3:G6"].Style.Font.Bold = true;

                    DataTable gridViewData = new DataTable();
                    gridViewData.Columns.Add("Item");
                    gridViewData.Columns.Add("Strength");
                    gridViewData.Columns.Add("Scale");

                    foreach (GridViewRow row in GridViewOfficersSheet.Rows)
                    {
                        DataRow dataRow = gridViewData.NewRow();
                        dataRow["Item"] = row.Cells[0].Text;
                        dataRow["Strength"] = row.Cells[1].Text;
                        dataRow["Scale"] = row.Cells[2].Text;
                        gridViewData.Rows.Add(dataRow);
                    }

                    // Group by ItemName and process data
                    var groupedData = gridViewData.AsEnumerable()
                        .GroupBy(row => row.Field<string>("Item"))
                        .Select(group => new
                        {
                            ItemName = Convert.ToString(group.Key),
                            Strength = Convert.ToString(group.Sum(row => Convert.ToInt32(row.Field<string>("Strength")))),
                            Scale = Convert.ToString(group.Average(row => Convert.ToDecimal(row.Field<string>("Scale")))),
                            QtyEntitled = Convert.ToString(group.Sum(row => Convert.ToDecimal(row.Field<string>("QtyEntitled")))),
                            QtyIssued = Convert.ToString(group.Sum(row => Convert.ToDecimal(row.Field<string>("QtyIssued")))),
                            AlternateItems = Convert.ToString(group.Select(row => row.Field<string>("AlternateItemName"))).Distinct().ToList()
                        });

                    int currentRow = 9; // Start after the headers
                    int serNumber = 1;
                    foreach (var group in groupedData)
                    {
                        worksheet.Cells[currentRow, 1].Value = Convert.ToString(serNumber++);
                        worksheet.Cells[currentRow, 2].Value = Convert.ToString(group.ItemName);
                        worksheet.Cells[currentRow, 3].Value = Convert.ToString(group.Strength);
                        worksheet.Cells[currentRow, 4].Value = "x";
                        worksheet.Cells[currentRow, 5].Value = Convert.ToString(group.Scale);
                        worksheet.Cells[currentRow, 6].Value = "=";
                        worksheet.Cells[currentRow, 7].Value = Convert.ToString(group.QtyEntitled);
                        worksheet.Cells[currentRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        currentRow++;

                        // Alternate items
                        foreach (var altItem in group.AlternateItems)
                        {
                            worksheet.Cells[currentRow, 2].Value = ""; // Blank for alternate item rows
                            worksheet.Cells[currentRow, 3].Value = ""; // Blank for alternate item rows
                            worksheet.Cells[currentRow, 4].Value = ""; // Blank for alternate item rows
                            worksheet.Cells[currentRow, 5].Value = ""; // Blank for alternate item rows
                            worksheet.Cells[currentRow, 6].Value = "=";
                            worksheet.Cells[currentRow, 7].Value = 0; // Example value, replace with actual logic if needed
                            worksheet.Cells[currentRow, 8].Value = altItem;
                            currentRow++;
                        }

                        // Total row for the group
                        worksheet.Cells[currentRow, 2].Value = "Total";
                        worksheet.Cells[currentRow, 2].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 6].Value = "=";
                        worksheet.Cells[currentRow, 7].Value = group.QtyEntitled;
                        worksheet.Cells[currentRow, 7].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                        currentRow++;

                        // Add a blank row for spacing
                        currentRow++;
                    }

                    // Save the file
                    string fileName = $"Officers_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
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
            catch (ThreadAbortException)
            {
                // Catch the ThreadAbortException to prevent it from propagating
                // This exception is expected when using Response.End()
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while exporting data: " + ex.Message;
            }
        }

        protected void ExportDiversStockButton_Click(object sender, EventArgs e)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                DataTable dt = new DataTable();

                // Add only the columns you want to include in the Excel file
                dt.Columns.Add("Name");
                dt.Columns.Add("Rank");
                dt.Columns.Add("P.No.");
                dt.Columns.Add("NO. OF DAYS ENTILED");
                dt.Columns.Add("CHOCOLATE");
                dt.Columns.Add("HORLICKS");
                dt.Columns.Add("EGGS");
                dt.Columns.Add("MILK");
                dt.Columns.Add("G/NUT");
                dt.Columns.Add("BUTTER");
                dt.Columns.Add("SUGAR");

                foreach (GridViewRow row in GridViewExtraIssueDivers.Rows)
                {
                    DataRow dr = dt.NewRow();

                    // Populate the DataRow with the values from the GridView, excluding Id and Date columns
                    dr["Name"] = row.Cells[1].Text;
                    dr["Rank"] = row.Cells[2].Text;
                    dr["P.No."] = row.Cells[3].Text;
                    dr["NO. OF DAYS ENTILED"] = row.Cells[4].Text;
                    dr["CHOCOLATE"] = row.Cells[5].Text;
                    dr["HORLICKS"] = row.Cells[6].Text;
                    dr["EGGS"] = row.Cells[7].Text;
                    dr["MILK"] = row.Cells[8].Text;
                    dr["G/NUT"] = row.Cells[9].Text;
                    dr["BUTTER"] = row.Cells[10].Text;
                    dr["SUGAR"] = row.Cells[11].Text;

                    dt.Rows.Add(dr);
                }

                DataRow totalRow = dt.NewRow();
                totalRow[0] = "Total"; // Assuming the first column is for row labels

                for (int i = 3; i <= 10; i++)
                {
                    decimal total = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (decimal.TryParse(dr[i].ToString(), out decimal value))
                        {
                            total += value;
                        }
                    }
                    totalRow[i] = total;
                }

                dt.Rows.Add(totalRow);

                DataRow qtyRow = dt.NewRow();
                qtyRow[0] = "QTY ISSUED"; // Assuming the first column is for row labels

                for (int i = 4; i <= 10; i++)
                {
                    decimal total = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[0].ToString() == "Total")
                            continue;

                        if (decimal.TryParse(dr[i].ToString(), out decimal value))
                        {
                            total += value;
                        }
                    }
                    qtyRow[i] = total;
                }
                dt.Rows.Add(qtyRow);

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Divers");
                    // Set the title
                    worksheet.Cells["A1"].Value = $"SUMMARY OF RATION ISSUED TO DIVER FOR THE MONTH OF {DateTime.Now.ToString("MMMM yyyy")}";

                    worksheet.Cells["A1:K1"].Merge = true; // Merge cells for the title
                    worksheet.Cells["A1:K1"].Style.Font.Bold = true; // Bold font for the title

                    // Add empty row
                    worksheet.InsertRow(2, 1);

                    // Add authority
                    worksheet.Cells["A3"].Value = "AUTHORITY: GOI LETTER DD/1655/RATION/IHQ MOD (NAVY)/1396/D(N-IV)08 DATED 07 OCT 08";
                    worksheet.Cells["A3:K3"].Merge = true; // Merge cells for the authority
                    worksheet.Cells["A3:K3"].Style.Font.Bold = true; // Bold font for the authority

                    // Add empty row
                    worksheet.InsertRow(4, 1);

                    // Add the DataTable
                    worksheet.Cells["A5"].LoadFromDataTable(dt, true);

                    // Set borders for the entire table
                    var borderStyle = ExcelBorderStyle.Thin;
                    var borderColor = System.Drawing.Color.Black;

                    var range = worksheet.Cells[5, 1, dt.Rows.Count + 5, dt.Columns.Count];
                    range.Style.Border.Top.Style = borderStyle;
                    range.Style.Border.Bottom.Style = borderStyle;
                    range.Style.Border.Left.Style = borderStyle;
                    range.Style.Border.Right.Style = borderStyle;
                    range.Style.Border.Top.Color.SetColor(borderColor);
                    range.Style.Border.Bottom.Color.SetColor(borderColor);
                    range.Style.Border.Left.Color.SetColor(borderColor);
                    range.Style.Border.Right.Color.SetColor(borderColor);

                    // Set bold font for the total row
                    worksheet.Cells[dt.Rows.Count + 6, 4, dt.Rows.Count + 6, 10].Style.Font.Bold = true;

                    // Check user role and add watermark if the user role is "user"
                    //if (Session["Role"] != null && Session["Role"].ToString() == "User")
                    //{
                    //    var watermark = worksheet.Drawings.AddShape("Watermark", eShapeStyle.Rect);
                    //    watermark.Text = "User";
                    //    watermark.Font.Size = 40;
                    //    watermark.Font.Bold = true;
                    //    watermark.Fill.Style = eFillStyle.SolidFill;

                    //    watermark.Fill.Color.SetColor(System.Drawing.Color.FromArgb(150, 255, 255, 255));

                    //    watermark.SetPosition(5, 5);
                    //    watermark.SetSize(800, 200);

                    //    watermark.TextAlignment = OfficeOpenXml.Drawing.eTextAlignment.Center;
                    //    watermark.TextVertical = OfficeOpenXml.Drawing.eTextVerticalType.Vertical;
                    //    watermark.Rotation = -45;
                    //}

                    // Save the file
                    string fileName = $"Divers_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
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
            catch (ThreadAbortException)
            {
                // Catch the ThreadAbortException to prevent it from propagating
                // This exception is expected when using Response.End()
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while exporting data: " + ex.Message;
            }
        }

        protected void ExportExtraIssueButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                DataTable strengthData = new DataTable();
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM ExtraIssueCategory", conn);
                    dataAdapter.Fill(strengthData);
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage excelPackage = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("STATEMENT OF EXTRA ISSUE");

                    // Set the title

                    worksheet.Cells["A2"].Value = $"STATEMENT OF EXTRA ISSUE - {DateTime.Now.ToString("MMMM yyyy")}";
                    worksheet.Cells["A2:N2"].Merge = true;
                    worksheet.Cells["A2:N2"].Style.Font.Bold = true;
                    worksheet.Cells["A2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;


                    worksheet.Cells["A5"].Value = "DENOM- >";
                    worksheet.Cells["A6"].Value = "SCALE- >";
                    worksheet.Cells["A7"].Value = "DATE";

                    worksheet.Cells["B3"].Value = "NO. OF PERSONS";
                    worksheet.Cells["B3"].Style.Font.Bold = true;
                    worksheet.Cells["C3"].Value = "TEA";
                    worksheet.Cells["C3"].Style.Font.Bold = true;
                    worksheet.Cells["D3"].Value = "MILK FRESH";
                    worksheet.Cells["D3"].Style.Font.Bold = true;
                    worksheet.Cells["E3"].Value = "SUGAR";
                    worksheet.Cells["E3"].Style.Font.Bold = true;

                    worksheet.Cells["F3"].Value = "NO. OF PERSONS";
                    worksheet.Cells["F3"].Style.Font.Bold = true;
                    worksheet.Cells["G3"].Value = "LIME FRESH";
                    worksheet.Cells["G3"].Style.Font.Bold = true;

                    worksheet.Cells["H3"].Value = "NO. OF PERSONS";
                    worksheet.Cells["H3"].Style.Font.Bold = true;
                    worksheet.Cells["I3"].Value = "LIME JUICE";
                    worksheet.Cells["I3"].Style.Font.Bold = true;
                    worksheet.Cells["J3"].Value = "SUGAR";
                    worksheet.Cells["J3"].Style.Font.Bold = true;

                    worksheet.Cells["K3"].Value = "NO. OF PERSONS PEST CONTROL";
                    worksheet.Cells["K3"].Style.Font.Bold = true;
                    worksheet.Cells["L3"].Value = "MILK FRESH";
                    worksheet.Cells["L3"].Style.Font.Bold = true;

                    worksheet.Cells["M3"].Value = "NO. OF PERSONS LEAD POISIONING";
                    worksheet.Cells["M3"].Style.Font.Bold = true;
                    worksheet.Cells["N3"].Value = "MILK FRESH";
                    worksheet.Cells["N3"].Style.Font.Bold = true;


                    worksheet.Cells["B2:N2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells["C5"].Value = "KGS";
                    worksheet.Cells["C6"].Value = "0.007";
                    worksheet.Cells["D5"].Value = "LTRS";
                    worksheet.Cells["D6"].Value = "0.070";
                    worksheet.Cells["E5"].Value = "KGS";
                    worksheet.Cells["E6"].Value = "0.030";
                    worksheet.Cells["G5"].Value = "KGS";
                    worksheet.Cells["G6"].Value = "0.070";
                    worksheet.Cells["I5"].Value = "KGS";
                    worksheet.Cells["I6"].Value = "0.028";
                    worksheet.Cells["J5"].Value = "KGS";
                    worksheet.Cells["J6"].Value = "0.040";
                    worksheet.Cells["L5"].Value = "LTRS";
                    worksheet.Cells["L6"].Value = "0.250";
                    worksheet.Cells["N5"].Value = "LTRS";
                    worksheet.Cells["N6"].Value = "0.280";

                    int currentRow = 8;
                    double totalStrengthTeaMilkSugar = 0;
                    double totalTea = 0;
                    double totalMilkTeaMilkSugar = 0;
                    double totalSugarTeaMilkSugar = 0;

                    double totalStrengthLimeFresh = 0;
                    double totalLimeFresh = 0;

                    double totalStrengthLimeAndSugar = 0;
                    double totalLimeJuice = 0;
                    double totalSugarLimeAndSugar = 0;

                    double totalStrengthPestControl = 0;
                    double totalMilkPestControl = 0;

                    double totalStrengthLeadPoisoning = 0;
                    double totalMilkLeadPoisoning = 0;

                    foreach (DataRow row in strengthData.Rows)
                    {
                        worksheet.Cells[$"A{currentRow}"].Value = Convert.ToDateTime(row["Date"]).ToString("MM/dd/yyyy");

                        if (row["Type"].ToString() == "TeaMilk&Sugar")
                        {
                            double strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);
                            double tea = row["Tea"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Tea"]);
                            double milk = row["Milk"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Milk"]);
                            double sugar = row["Sugar"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Sugar"]);

                            worksheet.Cells[$"B{currentRow}"].Value = strength;
                            worksheet.Cells[$"C{currentRow}"].Value = tea;
                            worksheet.Cells[$"D{currentRow}"].Value = milk;
                            worksheet.Cells[$"E{currentRow}"].Value = sugar;

                            totalStrengthTeaMilkSugar += strength;
                            totalTea += tea;
                            totalMilkTeaMilkSugar += milk;
                            totalSugarTeaMilkSugar += sugar;
                        }
                        else if (row["Type"].ToString() == "LimeFresh")
                        {
                            double strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);
                            double limeFresh = row["LimeFresh"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["LimeFresh"]);

                            worksheet.Cells[$"F{currentRow}"].Value = strength;
                            worksheet.Cells[$"G{currentRow}"].Value = limeFresh;

                            totalStrengthLimeFresh += strength;
                            totalLimeFresh += limeFresh;
                        }
                        else if (row["Type"].ToString() == "LimeandSugar")
                        {
                            double strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);
                            double limeJuice = row["LimeJuice"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["LimeJuice"]);
                            double sugar = row["Sugar"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Sugar"]);

                            worksheet.Cells[$"H{currentRow}"].Value = strength;
                            worksheet.Cells[$"I{currentRow}"].Value = limeJuice;
                            worksheet.Cells[$"J{currentRow}"].Value = sugar;

                            totalStrengthLimeAndSugar += strength;
                            totalLimeJuice += limeJuice;
                            totalSugarLimeAndSugar += sugar;
                        }
                        else if (row["Type"].ToString() == "PestControl")
                        {
                            double strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);
                            double milk = row["Milk"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Milk"]);

                            worksheet.Cells[$"K{currentRow}"].Value = strength;
                            worksheet.Cells[$"L{currentRow}"].Value = milk;

                            totalStrengthPestControl += strength;
                            totalMilkPestControl += milk;
                        }
                        else if (row["Type"].ToString() == "LeadPoisioning")
                        {
                            double strength = row["Strength"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Strength"]);
                            double milk = row["Milk"] == DBNull.Value ? 0.00 : Convert.ToDouble(row["Milk"]);

                            worksheet.Cells[$"M{currentRow}"].Value = strength;
                            worksheet.Cells[$"N{currentRow}"].Value = milk;

                            totalStrengthLeadPoisoning += strength;
                            totalMilkLeadPoisoning += milk;
                        }

                        currentRow++;
                    }

                    // Add the totals row
                    worksheet.Cells[$"A{currentRow}"].Value = "TOTAL";
                    worksheet.Cells[$"A{currentRow}"].Style.Font.Bold = true;

                    worksheet.Cells[$"B{currentRow}"].Value = totalStrengthTeaMilkSugar;
                    worksheet.Cells[$"C{currentRow}"].Value = totalTea;
                    worksheet.Cells[$"D{currentRow}"].Value = totalMilkTeaMilkSugar;
                    worksheet.Cells[$"E{currentRow}"].Value = totalSugarTeaMilkSugar;

                    worksheet.Cells[$"F{currentRow}"].Value = totalStrengthLimeFresh;
                    worksheet.Cells[$"G{currentRow}"].Value = totalLimeFresh;

                    worksheet.Cells[$"H{currentRow}"].Value = totalStrengthLimeAndSugar;
                    worksheet.Cells[$"I{currentRow}"].Value = totalLimeJuice;
                    worksheet.Cells[$"J{currentRow}"].Value = totalSugarLimeAndSugar;

                    worksheet.Cells[$"K{currentRow}"].Value = totalStrengthPestControl;
                    worksheet.Cells[$"L{currentRow}"].Value = totalMilkPestControl;

                    worksheet.Cells[$"M{currentRow}"].Value = totalStrengthLeadPoisoning;
                    worksheet.Cells[$"N{currentRow}"].Value = totalMilkLeadPoisoning;

                    // Make the totals row bold
                    worksheet.Cells[$"A{currentRow}:N{currentRow}"].Style.Font.Bold = true;

                    currentRow += 2;

                    // Add the new table headers
                    worksheet.Cells[$"A{currentRow}"].Value = "S.NO.";
                    worksheet.Cells[$"B{currentRow}"].Value = "ITEM";
                    worksheet.Cells[$"C{currentRow}"].Value = "ENTITLED";
                    worksheet.Cells[$"D{currentRow}"].Value = "ISSUED";

                    worksheet.Cells[$"E{currentRow}"].Value = "S.NO.";
                    worksheet.Cells[$"F{currentRow}"].Value = "ITEM";
                    worksheet.Cells[$"G{currentRow}"].Value = "ENTITLED";
                    worksheet.Cells[$"H{currentRow}"].Value = "ISSUED";

                    worksheet.Cells[$"C{currentRow}:D{currentRow}"].Merge = true;
                    worksheet.Cells[$"G{currentRow}:H{currentRow}"].Merge = true;

                    currentRow++;

                    // Add the new table data
                    worksheet.Cells[$"A{currentRow}"].Value = 1;
                    worksheet.Cells[$"B{currentRow}"].Value = "TEA";
                    worksheet.Cells[$"C{currentRow}"].Value = totalTea;
                    worksheet.Cells[$"D{currentRow}"].Value = "";

                    worksheet.Cells[$"E{currentRow}"].Value = 4;
                    worksheet.Cells[$"F{currentRow}"].Value = "LIME FRESH";
                    worksheet.Cells[$"G{currentRow}"].Value = totalLimeFresh;
                    worksheet.Cells[$"H{currentRow}"].Value = "";

                    currentRow++;

                    worksheet.Cells[$"A{currentRow}"].Value = 2;
                    worksheet.Cells[$"B{currentRow}"].Value = "SUGAR";
                    worksheet.Cells[$"C{currentRow}"].Value = totalSugarTeaMilkSugar;
                    worksheet.Cells[$"D{currentRow}"].Value = "";

                    worksheet.Cells[$"E{currentRow}"].Value = 5;
                    worksheet.Cells[$"F{currentRow}"].Value = "LIME JUICE";
                    worksheet.Cells[$"G{currentRow}"].Value = totalLimeJuice;
                    worksheet.Cells[$"H{currentRow}"].Value = "";

                    currentRow++;

                    worksheet.Cells[$"A{currentRow}"].Value = 3;
                    worksheet.Cells[$"B{currentRow}"].Value = "MILK FRESH";
                    worksheet.Cells[$"C{currentRow}"].Value = totalMilkTeaMilkSugar;
                    worksheet.Cells[$"D{currentRow}"].Value = "";

                    //worksheet.Cells[$"C{currentRow}:D{currentRow}"].Merge = true;

                    worksheet.Cells[$"E{currentRow}:H{currentRow}"].Merge = true;

                    // Apply borders to the entire range
                    var borderRange = worksheet.Cells[$"A2:N{currentRow}"];
                    borderRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    borderRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                    // Save the file
                    string fileName = $"ExtraIssue_{DateTime.Now.ToString("MMMM_yyyy")}.xlsx";
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
            catch (ThreadAbortException)
            {
                // Catch the ThreadAbortException to prevent it from propagating
                // This exception is expected when using Response.End()
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while exporting data: " + ex.Message;
            }
        }

        private void LoadGridViewExtraIssue()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("Select * from ExtraIssueCategory", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssue.DataSource = dt;
                    GridViewExtraIssue.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}
