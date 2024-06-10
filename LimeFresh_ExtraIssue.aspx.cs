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
    public partial class LimeFresh_ExtraIssue : System.Web.UI.Page
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
                string[] lime = Request.Form.GetValues("lime");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < date.Length; i++)
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO ExtraIssueCategory (Date, Strength, Tea, Milk, Sugar, LimeFresh, LimeJuice, Type) VALUES (@Date, @Strength, @Tea, @Milk, @Sugar, @LimeFresh, @LimeJuice, @Type)", conn);

                        cmd.Parameters.AddWithValue("@Date", date[i]);
                        cmd.Parameters.AddWithValue("@Strength", strength[i]);
                        cmd.Parameters.AddWithValue("@Tea", "0");
                        cmd.Parameters.AddWithValue("@Milk", "0");
                        cmd.Parameters.AddWithValue("@Sugar", "0");
                        cmd.Parameters.AddWithValue("@LimeFresh", lime[i]);
                        cmd.Parameters.AddWithValue("@LimeJuice", "0");
                        cmd.Parameters.AddWithValue("@Type", "LimeFresh");

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

                    GridViewExtraIssueCategory4.DataSource = dt;
                    GridViewExtraIssueCategory4.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }
    }
}