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
    public partial class LimeandSugar_ExtraIssue : System.Web.UI.Page
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

                string[] dates = Request.Form.GetValues("date");
                string[] strengths = Request.Form.GetValues("strength");
                string[] limes = Request.Form.GetValues("lime");
                string[] sugars = Request.Form.GetValues("sugar");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    for (int i = 0; i < limes.Length; i++)
                    {
                        int itemId = 0;
                        string limeQuery = "SELECT Id FROM BasicLieuItems WHERE IlueItem = @ItemName";

                        using (SqlCommand cmd1 = new SqlCommand(limeQuery, conn))
                        {
                            cmd1.Parameters.AddWithValue("@ItemName", "Lime Fresh");
                            using (SqlDataReader reader = cmd1.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    itemId = Convert.ToInt32(reader["Id"]);
                                }
                            }
                        }

                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssueCategory (Date, Strength, ItemId, ItemName, Type, Qty) VALUES (@Date, @Strength, @ItemId, @ItemName, @Type, @Qty)", conn);

                        for (int j = 0; j < dates.Length; j++)
                        {
                            cmd.Parameters.Clear(); // Clear parameters before reusing

                            cmd.Parameters.AddWithValue("@Date", dates[j]);
                            cmd.Parameters.AddWithValue("@Strength", strengths[j]);
                            cmd.Parameters.AddWithValue("@ItemId", itemId);
                            cmd.Parameters.AddWithValue("@ItemName", "Lime Fresh");
                            cmd.Parameters.AddWithValue("@Type", "LimeandSugar");
                            cmd.Parameters.AddWithValue("@Qty", limes[i]);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Second loop for sugar
                    for (int i = 0; i < sugars.Length; i++)
                    {
                        int itemId = 0;
                        string sugarQuery = "SELECT Id FROM BasicLieuItems WHERE IlueItem = @ItemName";

                        using (SqlCommand cmd1 = new SqlCommand(sugarQuery, conn))
                        {
                            cmd1.Parameters.AddWithValue("@ItemName", "Sugar");
                            using (SqlDataReader reader = cmd1.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    itemId = Convert.ToInt32(reader["Id"]);
                                }
                            }
                        }

                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssueCategory (Date, Strength, ItemId, ItemName, Type, Qty) VALUES (@Date, @Strength, @ItemId, @ItemName, @Type, @Qty)", conn);

                        for (int j = 0; j < dates.Length; j++)
                        {
                            cmd.Parameters.Clear(); // Clear parameters before reusing

                            cmd.Parameters.AddWithValue("@Date", dates[j]);
                            cmd.Parameters.AddWithValue("@Strength", strengths[j]);
                            cmd.Parameters.AddWithValue("@ItemId", itemId);
                            cmd.Parameters.AddWithValue("@ItemName", "Sugar");
                            cmd.Parameters.AddWithValue("@Type", "LimeandSugar");
                            cmd.Parameters.AddWithValue("@Qty", sugars[i]);

                            cmd.ExecuteNonQuery();
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

                    GridViewExtraIssueCategory3.DataSource = dt;
                    GridViewExtraIssueCategory3.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}