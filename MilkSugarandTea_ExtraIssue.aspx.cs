using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class MilkSugarandTea_ExtraIssue : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            LoadGridView();
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                string[] strengths = Request.Form.GetValues("strength");
                string[] tea = Request.Form.GetValues("tea");
                string[] milk = Request.Form.GetValues("milk");
                string[] sugar = Request.Form.GetValues("sugar");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < date.Length; i++)
                    {
                        // Insert Lime Fresh data
                        InsertItemData(conn, date[i], strengths[i], "Milk", tea[i]);

                        // Insert Sugar data
                        InsertItemData(conn, date[i], strengths[i], "Sugar", milk[i]);
                        
                        InsertItemData(conn, date[i], strengths[i], "Tea", sugar[i]);
                    }
                }
                lblStatus.Text = "Data entered successfully.";

                LoadGridView();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        private void InsertItemData(SqlConnection conn, string date, string strength, string itemName, string qty)
        {
            int itemId = 0;
            string query = "SELECT Id FROM BasicLieuItems WHERE IlueItem = @ItemName";

            using (SqlCommand cmd1 = new SqlCommand(query, conn))
            {
                cmd1.Parameters.AddWithValue("@ItemName", itemName);
                using (SqlDataReader reader = cmd1.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        itemId = Convert.ToInt32(reader["Id"]);
                    }
                }
            }

            SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssueCategory (Date, Strength, ItemId, ItemName, Type, Qty) VALUES (@Date, @Strength, @ItemId, @ItemName, @Type, @Qty)", conn);

            cmd.Parameters.AddWithValue("@Date", date);
            cmd.Parameters.AddWithValue("@Strength", strength);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            cmd.Parameters.AddWithValue("@ItemName", itemName);
            cmd.Parameters.AddWithValue("@Type", "MilkSugarAndTea");
            cmd.Parameters.AddWithValue("@Qty", qty);

            cmd.ExecuteNonQuery();

            // Update PresentStockMaster table if ItemName exists
            SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
            updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemName);
            updatePresentStockCmd.Parameters.AddWithValue("@Quantity", qty);
            updatePresentStockCmd.Parameters.AddWithValue("@Denos", "Ltr");
            updatePresentStockCmd.ExecuteNonQuery();

            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM MonthEndStockMaster WHERE Date = @Date AND ItemName = @ItemName", conn);
            checkCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date)); // Use current date
            checkCmd.Parameters.AddWithValue("@ItemName", itemName);
            int count = (int)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                // If data exists, update the existing row
                SqlCommand updateCmd = new SqlCommand("UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END WHERE Date = @Date AND ItemName = @ItemName", conn);
                updateCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date)); // Use current date
                updateCmd.Parameters.AddWithValue("@ItemName", itemName);
                updateCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty));

                updateCmd.ExecuteNonQuery();
            }
            else
            {
                // If no data exists, insert a new row
                SqlCommand insertCmd = new SqlCommand("INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                insertCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date)); // Use current date
                insertCmd.Parameters.AddWithValue("@ItemName", itemName);
                insertCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty));
                insertCmd.Parameters.AddWithValue("@Type", "Issue"); // Set the correct parameter name for Type

                insertCmd.ExecuteNonQuery();
            }
        }

        private void LoadGridView()
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ExtraIssueCategory", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewExtraIssueCategory5.DataSource = dt;
                    GridViewExtraIssueCategory5.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}