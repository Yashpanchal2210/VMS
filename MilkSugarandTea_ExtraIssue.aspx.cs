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
                string[] strength = Request.Form.GetValues("strength");
                string[] tea = Request.Form.GetValues("tea");
                string[] milk = Request.Form.GetValues("milk");
                string[] sugar = Request.Form.GetValues("sugar");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < date.Length; i++)
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssueCategory (Date, Strength, Tea, Milk, Sugar, LimeFresh, LimeJuice, Type) VALUES (@Date, @Strength, @Tea, @Milk, @Sugar, @LimeFresh, @LimeJuice, @Type)", conn);

                        cmd.Parameters.AddWithValue("@Date", date[i]);
                        cmd.Parameters.AddWithValue("@Strength", strength[i]);
                        cmd.Parameters.AddWithValue("@Tea", tea[i]);
                        cmd.Parameters.AddWithValue("@Milk", milk[i]);
                        cmd.Parameters.AddWithValue("@Sugar", sugar[i]);
                        cmd.Parameters.AddWithValue("@LimeFresh", "0");
                        cmd.Parameters.AddWithValue("@LimeJuice", "0");
                        cmd.Parameters.AddWithValue("@Type", "TeaMilk&Sugar");

                        cmd.ExecuteNonQuery();
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