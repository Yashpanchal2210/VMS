using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class Divers_ExtraIssue : System.Web.UI.Page
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
                string[] name = Request.Form.GetValues("name");
                string[] rank = Request.Form.GetValues("rank");
                string[] pno = Request.Form.GetValues("pno");
                string[] days = Request.Form.GetValues("days");
                string[] itemnameId = Request.Form.GetValues("itemname");
                string[] qty = Request.Form.GetValues("qty");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < itemnameId.Length; i++)
                    {
                        (string itemName, string denomination) = GetItemNameById(conn, itemnameId[i]);

                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssue (Date, Name, Rank, PNo, Days, ItemId, ItemName, Qty, Type) VALUES (@Date, @Name, @Rank, @PNo, @Days, @ItemId, @ItemName, @Qty, @Type)", conn);

                        cmd.Parameters.AddWithValue("@Date", i < date.Length ? date[i] : date[0]);
                        cmd.Parameters.AddWithValue("@Name", i < name.Length ? name[i] : name[0]);
                        cmd.Parameters.AddWithValue("@Rank", i < rank.Length ? rank[i] : rank[0]);
                        cmd.Parameters.AddWithValue("@PNo", i < pno.Length ? pno[i] : pno[0]);
                        cmd.Parameters.AddWithValue("@Days", i < days.Length ? days[i] : days[0]);
                        cmd.Parameters.AddWithValue("@ItemName", itemName);
                        cmd.Parameters.AddWithValue("@ItemId", itemnameId[i]);
                        cmd.Parameters.AddWithValue("@Qty", qty[i]);
                        cmd.Parameters.AddWithValue("@Type", "DiversIssue");

                        cmd.ExecuteNonQuery();


                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", "Milk Fresh");
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", itemName);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denomination);
                        updatePresentStockCmd.ExecuteNonQuery();

                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM MonthEndStockMaster WHERE Date = @Date AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                        checkCmd.Parameters.AddWithValue("@ItemName", itemName);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand("UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END WHERE Date = @Date AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            updateCmd.Parameters.AddWithValue("@ItemName", itemName);
                            updateCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand("INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            insertCmd.Parameters.AddWithValue("@ItemName", itemName);
                            insertCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));
                            insertCmd.Parameters.AddWithValue("@Type", "Issue"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }
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

        private (string itemName, string denomination) GetItemNameById(SqlConnection conn, string itemId)
        {
            string itemName = string.Empty;
            string denomination = string.Empty;
            string query = "SELECT ILueItem, iLueDenom FROM BasicLieuItems WHERE Id = @ItemId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ItemId", itemId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        itemName = reader["ILueItem"].ToString();
                        denomination = reader["iLueDenom"].ToString();
                    }
                }
            }

            return (itemName, denomination);
        }

        [WebMethod]
        public static List<object> GetItemNames()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT iLueItem, Id FROM BasicLieuItems";
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            var items = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(new
                {
                    Text = row["iLueItem"].ToString(),
                    Value = row["Id"].ToString(),
                });
            }

            return items;
        }

        private void LoadGridView()
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

    }
}