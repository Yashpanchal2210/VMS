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
    public partial class Page2_7Report : System.Web.UI.Page
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

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            // Get the current date and calculate the previous month
            string[] selectedDate = monthYearPicker.Value.Split('-');

            DataTable monthEndStockData = GetPreviousMonthData(selectedDate);

            // Fetch data from the PresentStockMaster table
            DataTable presentStockData = GetPresentStockData();

            //DataTable receiptData = GetReceiptData();
            DataTable presentReceiptData = GetReceiptData(selectedDate);

            //Fetch IssueData Officer
            DataTable presentIssueOfficer = GetOfficerIssueData(selectedDate);

            //Fetch IssueData Sailor
            DataTable presentIssueSailor = GetSailorIssueData(selectedDate);

            //Fetch Wastage
            DataTable presentWastage = GetWastageData(selectedDate);

            //Fetch Divers Data
            DataTable presentDivers = GetDiversIssueData(selectedDate);

            //Fetch OtherShips
            DataTable presentOtherShips = GetOtherShipData(selectedDate);

            //Fetch Patients
            DataTable presentPatients = GetPatientsData(selectedDate);

            //Fetch ExtraIssue
            DataTable presentExtraIssue = GetExtraIssueData(selectedDate);


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Page2-7 Report");
                worksheet.Cells.Style.Font.Name = "Arial";
                worksheet.Cells.Style.Font.Size = 12;
                worksheet.PrinterSettings.Orientation = eOrientation.Landscape;

                // Set column widths
                worksheet.Column(1).Width = 0;
                worksheet.Column(2).Width = 11;
                worksheet.Column(3).Width = 62.71;
                worksheet.Row(1).Height = 0;

                // Apply borders to all cells
                using (ExcelRange range = worksheet.Cells["B2:CI34"])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }

                // Add header rows and merge cells
                worksheet.Cells["B2:L2"].Merge = true;
                worksheet.Cells["B2:L2"].Value = "PAGE 02";
                worksheet.Cells["B2:L2"].Style.Font.Bold = true;
                worksheet.Cells["B2:L2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["B2:L2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells["B4"].Value = "LINE";
                worksheet.Cells["B5"].Value = "NO.";

                worksheet.Cells["N2:AA2"].Merge = true;
                worksheet.Cells["N2:AA2"].Value = "PAGE 03";
                worksheet.Cells["N2:AA2"].Style.Font.Bold = true;
                worksheet.Cells["N2:AA2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["N2:AA2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["AC2:AP2"].Merge = true;
                worksheet.Cells["AC2:AP2"].Value = "PAGE 04";
                worksheet.Cells["AC2:AP2"].Style.Font.Bold = true;
                worksheet.Cells["AC2:AP2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["AC2:AP2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["AR2:BE2"].Merge = true;
                worksheet.Cells["AR2:BE2"].Value = "PAGE 05";
                worksheet.Cells["AR2:BE2"].Style.Font.Bold = true;
                worksheet.Cells["AR2:BE2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["AR2:BE2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["BG2:BT2"].Merge = true;
                worksheet.Cells["BG2:BT2"].Value = "PAGE 06";
                worksheet.Cells["BG2:BT2"].Style.Font.Bold = true;
                worksheet.Cells["BG2:BT2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["BG2:BT2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells["BV2:CI2"].Merge = true;
                worksheet.Cells["BV2:CI2"].Value = "PAGE 07";
                worksheet.Cells["BV2:CI2"].Style.Font.Bold = true;
                worksheet.Cells["BV2:CI2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["BV2:CI2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                // Add numbering from 1 to 28 in column B (B7 to B34)
                int number = 1;
                for (int row = 7; row <= 34; row++)
                {
                    worksheet.Cells[row, 2].Value = number;
                    number++;
                }

                // Add static labels to cells
                worksheet.Cells["C7"].Value = "DENOS OF QTY ->";
                worksheet.Cells["C8"].Value = "FULL STOWAGE";
                worksheet.Cells["C9"].Value = "STOCK NOT TO FALL BELOW";
                worksheet.Cells["C9"].Style.Font.Bold = true;
                worksheet.Cells["C10"].Value = "BALANCE AS PER ACCOUNT";
                worksheet.Cells["C10"].Style.Font.Bold = true;

                int dataStartRowReceiptCRV = 11;
                foreach (DataRow rowReceiptCRV in presentReceiptData.Rows)
                {
                    string CellNo = "C" + dataStartRowReceiptCRV;
                    worksheet.Cells[CellNo].Value = rowReceiptCRV["referenceNos"];
                    int dataStartRowReceiptCRVNOS1 = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        if (row["InLieuItem"].Equals(rowReceiptCRV["itemnames"]))
                        {
                            worksheet.Cells[dataStartRowReceiptCRV, dataStartRowReceiptCRVNOS1].Value = rowReceiptCRV["quantities"]; // Start from column C
                            break;
                        }
                        dataStartRowReceiptCRVNOS1++;
                    }

                    dataStartRowReceiptCRV++;
                }

                //ISSUE OFFICER
                //int dataStartRowIssueOfficer = 23;
                foreach (DataRow rowIssueOfficer in presentIssueOfficer.Rows)
                {
                    int dataStartRowIssueOfficer1 = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        bool matchfound = false;
                        if (row["InLieuItem"].Equals(rowIssueOfficer["ItemName"]))
                        {
                            worksheet.Cells[23, dataStartRowIssueOfficer1].Value = Convert.ToDouble(rowIssueOfficer["QtyIssued"]); // Start from column C
                            matchfound = true;
                            //break;
                        }
                        if (!matchfound)
                        {
                            worksheet.Cells[23, dataStartRowIssueOfficer1].Value = 0.00;
                        }
                        dataStartRowIssueOfficer1++;
                    }
                }

                //ISSUE SAILOR
                foreach (DataRow rowIssueSailor in presentIssueSailor.Rows)
                {
                    int dataStartRowIssueSailor = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        bool matchFound = false;
                        if (row["InLieuItem"].Equals(rowIssueSailor["ItemName"]))
                        {
                            worksheet.Cells[22, dataStartRowIssueSailor].Value = Convert.ToDouble(rowIssueSailor["QtyIssued"]); // Start from column C
                            matchFound = true;
                            //break;
                        }
                        if (!matchFound)
                        {
                            worksheet.Cells[22, dataStartRowIssueSailor].Value = 0.00;
                        }
                        dataStartRowIssueSailor++;
                    }
                }

                //Wastage
                foreach (DataRow rowWastage in presentWastage.Rows)
                {
                    int dataStartRowWastage = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        bool matchFound = false;
                        if (row["InLieuItem"].Equals(rowWastage["ItemName"]))
                        {
                            worksheet.Cells[30, dataStartRowWastage].Value = Convert.ToDouble(rowWastage["Qty"]); // Start from column C
                            worksheet.Cells[30, dataStartRowWastage].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            matchFound = true;
                            //break;
                        }
                        if (!matchFound)
                        {
                            worksheet.Cells[30, dataStartRowWastage].Value = 0.00;
                            worksheet.Cells[30, dataStartRowWastage].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        dataStartRowWastage++;
                    }
                }

                //Divers
                foreach (DataRow rowDivers in presentDivers.Rows)
                {
                    int dataStartRowDivers = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        bool matchFound = false;
                        if (row["InLieuItem"].Equals(rowDivers["ItemName"]))
                        {
                            worksheet.Cells[29, dataStartRowDivers].Value = Convert.ToDouble(rowDivers["Qty"]); // Start from column C
                            worksheet.Cells[29, dataStartRowDivers].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            matchFound = true;
                            //break;
                        }
                        if (!matchFound)
                        {
                            worksheet.Cells[29, dataStartRowDivers].Value = 0.00;
                            worksheet.Cells[29, dataStartRowDivers].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        dataStartRowDivers++;
                    }
                }

                //OtherShips
                foreach (DataRow rowShips in presentOtherShips.Rows)
                {
                    int dataStartRowShips = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        bool matchFound = false;
                        if (row["InLieuItem"].Equals(rowShips["ItemName"]))
                        {
                            worksheet.Cells[27, dataStartRowShips].Value = Convert.ToDouble(rowShips["Qty"]); // Start from column C
                            worksheet.Cells[27, dataStartRowShips].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            matchFound = true;
                            //break;
                        }
                        if (!matchFound)
                        {
                            worksheet.Cells[27, dataStartRowShips].Value = 0.00;
                            worksheet.Cells[27, dataStartRowShips].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        dataStartRowShips++;
                    }
                }

                //Patients
                foreach (DataRow rowPatients in presentPatients.Rows)
                {
                    int dataStartRowPatients = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        bool matchFound = false;
                        if (row["InLieuItem"].Equals(rowPatients["ItemName"]))
                        {
                            worksheet.Cells[26, dataStartRowPatients].Value = Convert.ToDouble(rowPatients["Qty"]); // Start from column C
                            worksheet.Cells[26, dataStartRowPatients].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            matchFound = true;
                            //break;
                        }
                        if (!matchFound)
                        {
                            worksheet.Cells[26, dataStartRowPatients].Value = 0.00;
                            worksheet.Cells[26, dataStartRowPatients].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        dataStartRowPatients++;
                    }
                }

                //Extra Issue
                foreach (DataRow rowExtraIssue in presentExtraIssue.Rows)
                {
                    int dataStartRowExtraIssue = 4;
                    foreach (DataRow row in presentStockData.Rows)
                    {
                        bool matchFound = false;
                        if (row["InLieuItem"].Equals(rowExtraIssue["ItemName"]))
                        {
                            worksheet.Cells[24, dataStartRowExtraIssue].Value = Convert.ToDouble(rowExtraIssue["Qty"]); // Start from column C
                            worksheet.Cells[24, dataStartRowExtraIssue].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            matchFound = true;
                            //break;
                        }
                        if (!matchFound)
                        {
                            worksheet.Cells[24, dataStartRowExtraIssue].Value = 0.00;
                            worksheet.Cells[24, dataStartRowExtraIssue].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        dataStartRowExtraIssue++;
                    }
                }

                int dataStartColumnSum = 4;
                int dataEndColumn = dataStartColumnSum + presentStockData.Rows.Count;
                for (int col = dataStartColumnSum; col < dataEndColumn; col++)
                {
                    string startCell = ExcelCellBase.GetAddress(11, col);
                    string endCell = ExcelCellBase.GetAddress(20, col);
                    string sumCell = ExcelCellBase.GetAddress(21, col);
                    worksheet.Cells[sumCell].Formula = $"SUM({startCell}:{endCell})";
                }

                int dataStartColumnEndSum = 4;
                int dataEndColumnEnd = dataStartColumnEndSum + presentStockData.Rows.Count;
                for (int col = dataStartColumnEndSum; col < dataEndColumnEnd; col++)
                {
                    string startCell = ExcelCellBase.GetAddress(22, col);
                    string endCell = ExcelCellBase.GetAddress(30, col);
                    string sumCell = ExcelCellBase.GetAddress(31, col);
                    worksheet.Cells[sumCell].Formula = $"SUM({startCell}:{endCell})";
                }

                int startColumnSub = 4;
                int dataEndColumnSub = startColumnSub + presentStockData.Rows.Count - 1;
                int endColumnSub = dataEndColumnSub; // Assuming dataEndColumnSub is the last column index
                int resultRowSub1 = 32; // Row to display the first result
                int resultRowSub2 = 33; // Row to display the second result

                for (int col = startColumnSub; col <= endColumnSub; col++)
                {
                    string startCell = ExcelCellBase.GetAddress(21, col); // D21
                    string endCell = ExcelCellBase.GetAddress(31, col); // D31
                    string resultCell1 = ExcelCellBase.GetAddress(resultRowSub1, col); // Result in D32
                    string resultCell2 = ExcelCellBase.GetAddress(resultRowSub2, col); // Result in D33

                    // Set the formula to subtract D31 from D21 and place the result in the result cells
                    worksheet.Cells[resultCell1].Formula = $"{startCell}-{endCell}";
                    worksheet.Cells[resultCell2].Formula = resultCell1;
                }




                worksheet.Cells["C20"].Value = "FRESH RECEIPTS FROM  Pg - 12";
                worksheet.Cells["C21"].Value = "TOTAL RECEIPTS";
                worksheet.Cells["C21"].Style.Font.Bold = true;
                worksheet.Cells["C22"].Value = "ISSUE TO SHIP'S COY";
                worksheet.Cells["C23"].Value = "ISSUE TO OFFICERS";
                worksheet.Cells["C24"].Value = "EXTRA ISSUES from  Pg - 09";
                worksheet.Cells["C25"].Value = "PAYMENT ISSUE from Pg.10/11";
                worksheet.Cells["C26"].Value = "ISSUE TO PATIENTS";
                worksheet.Cells["C27"].Value = "ISSUE TO OTHER SHIPS";
                //worksheet.Cells["C28"].Value = "ISSUE TO CAT SCHOOL";
                worksheet.Cells["C29"].Value = "ISSUE TO DIVERS";
                worksheet.Cells["C30"].Value = "WASTAGE";
                worksheet.Cells["C31"].Value = "ISSUE GRAND TOTAL";
                worksheet.Cells["C31"].Style.Font.Bold = true;
                worksheet.Cells["C32"].Value = "BALANCE AS PER ACCOUNT";
                worksheet.Cells["C33"].Value = "BALANCE AS PER MUSTER";
                worksheet.Cells["C34"].Value = "CRV / LOSS STATEMENT";


                // Bind data from the PresentStockMaster table
                int dataStartColumn = 4; // Column D
                foreach (DataRow row in presentStockData.Rows)
                {
                    worksheet.Cells[4, dataStartColumn].Value = row["InLieuItem"];
                    worksheet.Cells[7, dataStartColumn].Value = row["Denomination"];
                    dataStartColumn++;

                    int dataStartRowReceipt = 4;
                    foreach (DataRow rowReceipt in monthEndStockData.Rows)
                    {
                        bool matchfound = false;
                        if (row["InLieuItem"].Equals(rowReceipt["ItemName"]))
                        {
                            worksheet.Cells[10, dataStartRowReceipt].Value = rowReceipt["Qty"]; // Start from column C
                            matchfound = true;
                        }
                        if (!matchfound)
                        {
                            worksheet.Cells[10, dataStartRowReceipt].Value = 0.00;
                        }
                        dataStartRowReceipt++;
                    }

                    //int dataStartRowReceiptCRVNOS = 4;
                    //foreach (DataRow rowReceiptCRV in presentReceiptData.Rows)
                    //{
                    //    if (row["ItemName"].Equals(rowReceiptCRV["itemnames"]))
                    //    {
                    //        worksheet.Cells[10, dataStartRowReceiptCRVNOS].Value = rowReceiptCRV["quantities"]; // Start from column C
                    //    }

                    //    dataStartRowReceiptCRVNOS++;
                    //}
                }

                //int startRow = 11; // Starting from B11 downwards
                //foreach (DataRow row in receiptData.Rows)
                //{
                //    // Assuming ReferenceNos is the column name from ReceiptMaster table
                //    string referenceNos = row["ReferenceNos"].ToString();
                //    worksheet.Cells[startRow, 2].Value = referenceNos; // Populate in column B (2)
                //    startRow++;
                //}

                // Save and download

                var DateNow = DateTime.Now.Month + "-" + DateTime.Now.Year;
                string fileName = $"PAGE2-7_{DateNow}.xlsx";
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

        private DataTable GetPreviousMonthData(string[] previousMonthDate)
        {
            int preveiousMonth = DateTime.Now.Month;
            int previousYear = DateTime.Now.Year;
            if (previousMonthDate != null && previousMonthDate.Length > 1)
            {
                int month = Convert.ToInt32(previousMonthDate[1]);
                if (month > 1)
                {
                    preveiousMonth = month - 1;
                    previousYear = Convert.ToInt32(previousMonthDate[0]);
                }
                else
                {
                    preveiousMonth = 12;
                    previousYear = previousYear - 1;
                }
            }


            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM MonthEndStockMaster WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", Convert.ToString(preveiousMonth));
                    cmd.Parameters.AddWithValue("@Year", Convert.ToString(previousYear));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetPresentStockData()
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT InLieuItem, Denomination FROM InLieuItems", conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetOfficerIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM IssueMaster Where Role = @role AND MONTH(Date) = @Month AND Year(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    cmd.Parameters.AddWithValue("@role", "Wardroom");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetSailorIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM IssueMaster Where Role = @role AND MONTH(Date) = @Month AND Year(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    cmd.Parameters.AddWithValue("@role", "Galley");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetReceiptData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ReceiptMaster WHERE MONTH(Dates) = @Month AND YEAR(Dates) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetWastageData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Wastage WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetRationPaymentIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Wastage WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetDiversIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM ExtraIssue WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetOtherShipData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM OtherShips WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetPatientsData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM patients WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        private DataTable GetExtraIssueData(string[] selectedDate)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ItemName, SUM(CONVERT(decimal(18, 3), Qty)) AS Qty FROM ExtraIssueCategory WHERE MONTH(Date) = @Month AND YEAR(Date) = @Year GROUP BY ItemName", conn))
                {
                    cmd.Parameters.AddWithValue("@Month", selectedDate[1]);
                    cmd.Parameters.AddWithValue("@Year", selectedDate[0]);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }
    }
}
