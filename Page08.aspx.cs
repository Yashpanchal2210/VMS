using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace VMS_1
{
    public partial class Page08 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Page 08 Report");

                worksheet.Cells.Style.Font.Name = "Arial";

                // Title and content based on provided image
                worksheet.Cells["A1:M1"].Merge = true;
                worksheet.Cells["A1:M1"].Value = $"PAGE - 8";
                worksheet.Cells["A1:M1"].Style.Font.Size = 12;
                worksheet.Cells["A1:M1"].Style.Font.Bold = true;
                worksheet.Cells["A1:M1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A2:M2"].Value = "PACKING MATERIAL ACCOUNT";
                worksheet.Cells["A2:M2"].Style.Font.Size = 12;
                worksheet.Cells["A2:M2"].Style.Font.Bold = true;
                worksheet.Cells["A2:M2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:M2"].Merge = true;

                worksheet.Cells["E3"].Value = "BAGS";
                worksheet.Cells["F3"].Value = "BAGS";
                worksheet.Cells["G3"].Value = "BAGS";
                worksheet.Cells["H3"].Value = "BAGS";
                worksheet.Cells["I3"].Value = "TINS";
                worksheet.Cells["J3"].Value = "TINS";
                worksheet.Cells["K3"].Value = "TEA";
                worksheet.Cells["L3"].Value = "CARTON";
                worksheet.Cells["M3"].Value = "PLASTIC";

                worksheet.Cells["A4"].Value = "LINE";
                worksheet.Cells["E4"].Value = "2MD";
                worksheet.Cells["F4"].Value = "2MD";
                worksheet.Cells["G4"].Value = "ASSOR-";
                worksheet.Cells["H4"].Value = "1 MD";
                worksheet.Cells["I4"].Value = "Misc";
                worksheet.Cells["J4"].Value = "15 KG";
                worksheet.Cells["K4"].Value = "BAGS";
                worksheet.Cells["M4"].Value = "BAGS";

                worksheet.Cells["A5"].Value = "NO.";
                worksheet.Cells["E5"].Value = "BTSA";
                worksheet.Cells["F5"].Value = "ATSA";
                worksheet.Cells["G5"].Value = "TED";
                worksheet.Cells["H5"].Value = "ATSA";

                worksheet.Cells["B6"].Value = "DENOM->";
                worksheet.Cells["E6"].Value = "NOS";
                worksheet.Cells["F6"].Value = "NOS";
                worksheet.Cells["G6"].Value = "NOS";
                worksheet.Cells["H6"].Value = "NOS";
                worksheet.Cells["I6"].Value = "NOS";
                worksheet.Cells["J6"].Value = "NOS";
                worksheet.Cells["K6"].Value = "NOS";
                worksheet.Cells["M6"].Value = "NOS";

                for (int i = 1; i <= 29; i++)
                {
                    worksheet.Cells[$"A{6 + i}"].Value = i;
                }

                worksheet.Cells["B7"].Value = "BALANCE  B/F FROM LAST A/C";
                worksheet.Cells["B7"].Style.Font.Bold = true;

                worksheet.Cells["B8"].Value = "RECEIPTS (VR. NOS.)";

                worksheet.Cells["E7"].Value = "RECEIPTS (VR. NOS.)";
                worksheet.Cells["F7"].Value = "RECEIPTS (VR. NOS.)";
                worksheet.Cells["G7"].Value = "RECEIPTS (VR. NOS.)";
                worksheet.Cells["J7"].Value = "RECEIPTS (VR. NOS.)";

                worksheet.Cells["B22"].Value = "TOTAL RECEIPTS";
                worksheet.Cells["B22"].Style.Font.Bold = true;

                worksheet.Cells["B23"].Value = "ISSUE TO OTHER SHIPS";

                worksheet.Cells["E22"].Value = "0";
                worksheet.Cells["E22"].Style.Font.Bold = true;
                worksheet.Cells["F22"].Value = "0";
                worksheet.Cells["F22"].Style.Font.Bold = true;
                worksheet.Cells["G22"].Value = "0";
                worksheet.Cells["G22"].Style.Font.Bold = true;
                worksheet.Cells["H22"].Value = "0";
                worksheet.Cells["H22"].Style.Font.Bold = true;
                worksheet.Cells["I22"].Value = "0";
                worksheet.Cells["I22"].Style.Font.Bold = true;
                worksheet.Cells["J22"].Value = "0";
                worksheet.Cells["J22"].Style.Font.Bold = true;
                worksheet.Cells["K22"].Value = "0";
                worksheet.Cells["K22"].Style.Font.Bold = true;
                worksheet.Cells["L22"].Value = "0";
                worksheet.Cells["L22"].Style.Font.Bold = true;
                worksheet.Cells["M22"].Value = "0";
                worksheet.Cells["M22"].Style.Font.Bold = true;

                worksheet.Cells["B31"].Value = "USED FOR SANITARY PURPOSE";
                worksheet.Cells["B32"].Value = "ISSUES GRAND TOTAL";
                worksheet.Cells["B32"].Style.Font.Bold = true;
                worksheet.Cells["B33"].Value = "BALANCE AS PER ACOUNT";
                worksheet.Cells["B34"].Value = "QTY AS PER MUSTER";
                worksheet.Cells["B35"].Value = "DIFFERENCES";

                worksheet.Cells["F31"].Value = "0";
                worksheet.Cells["G31"].Value = "0";

                worksheet.Cells["E32"].Formula = string.Format("SUM(E23:E31)");
                worksheet.Cells["F32"].Formula = string.Format("SUM(F23:F31)");
                worksheet.Cells["G32"].Formula = string.Format("SUM(G23:G31)");
                worksheet.Cells["H32"].Formula = string.Format("SUM(H23:H31)");
                worksheet.Cells["I32"].Formula = string.Format("SUM(I23:I31)");
                worksheet.Cells["J32"].Formula = string.Format("SUM(J23:J31)");
                worksheet.Cells["K32"].Formula = string.Format("SUM(K23:K31)");
                worksheet.Cells["L32"].Formula = string.Format("SUM(L23:L31)");
                worksheet.Cells["M32"].Formula = string.Format("SUM(M23:M31)");

                worksheet.Cells["E33"].Formula = string.Format("SUM(E22-E32)");
                worksheet.Cells["F33"].Formula = string.Format("SUM(F22-F32)");
                worksheet.Cells["G33"].Formula = string.Format("SUM(G22-G32)");
                worksheet.Cells["H33"].Formula = string.Format("SUM(H22-H32)");
                worksheet.Cells["I33"].Formula = string.Format("SUM(I22-I32)");
                worksheet.Cells["J33"].Formula = string.Format("SUM(J22-J32)");
                worksheet.Cells["K33"].Formula = string.Format("SUM(K22-K32)");
                worksheet.Cells["L33"].Formula = string.Format("SUM(L22-L32)");
                worksheet.Cells["M33"].Formula = string.Format("SUM(M22-M32)");

                worksheet.Cells["E34"].Formula = string.Format("+E33");
                worksheet.Cells["F34"].Formula = string.Format("+F33");
                worksheet.Cells["G34"].Formula = string.Format("+G33");
                worksheet.Cells["H34"].Formula = string.Format("+H33");
                worksheet.Cells["I34"].Formula = string.Format("+I33");
                worksheet.Cells["J34"].Formula = string.Format("+J33");
                worksheet.Cells["K34"].Formula = string.Format("+K33");
                worksheet.Cells["L34"].Formula = string.Format("+L33");
                worksheet.Cells["M34"].Formula = string.Format("+M33");

                using (ExcelRange range = worksheet.Cells["A1:M37"])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Medium;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                }

                // Adjust column widths
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Save and download the Excel file
                string fileName = $"Page08_Report_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
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