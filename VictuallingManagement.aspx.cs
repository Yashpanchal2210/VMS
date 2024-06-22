using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class VictuallingManagement : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ExportToExcelButton_Click(object sender, EventArgs e)
        {
            // Your existing code to fetch data from the database and create the Excel package
            DataTable inLieuItemsTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connStr))
            {
                string query = "SELECT inlieuitem FROM InLieuItems";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(inLieuItemsTable);
                }
            }

            int itemCount = inLieuItemsTable.Rows.Count;
            // Generate HTML tables
            string htmlTables = "<div class='table-container'>";
            int itemIndex = 0;


            for (int i = 2; i < 9; i++) // Change the loop condition to 9 for 7 pages
            {
                htmlTables += "<table border='1' width='100%' style='text-align:center; display:inline-block; margin-right:10px;'>";
                htmlTables += $"<tr><th colspan='11'>Page {i.ToString().PadLeft(2, '0')}</th></tr>";
                htmlTables += "<tr><th style='width:40px;'>Line No.</th>";

                if (i == 2) // Set width only for the first page
                {
                    htmlTables += "<th style='width: 360px;'>Column 1</th>";
                }

                for (int h = 1; h < (i == 2 ? 10 : 11); h++)
                {
                    string itemHeader = itemIndex < itemCount ? inLieuItemsTable.Rows[itemIndex]["inlieuitem"].ToString() : $"Column {h}";
                    htmlTables += $"<th>{itemHeader}</th>";
                    itemIndex++;
                }
                htmlTables += "</tr>";

                for (int j = 0; j < 40; j++)
                {
                    htmlTables += "<tr>";
                    int lineNumber = j + 1;
                    htmlTables += $"<td>{lineNumber}</td>";
                    for (int k = 0; k < 10; k++)
                    {
                        htmlTables += "<td></td>";
                    }
                    htmlTables += "</tr>";
                }

                htmlTables += "</table>";
            }
            htmlTables += "</div>";

            tablesContainer.InnerHtml = htmlTables;
        }




    }
}