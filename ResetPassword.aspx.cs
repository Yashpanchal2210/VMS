using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace VMS_1
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }
        protected void btnReset_Click(object sender, EventArgs e)
        {
            string NUDID = Request.QueryString["NUDID"];
            string newPassword = txtNewPassword.Text.Trim();

            // Connection string to your database
            string connectionString = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=SSPI;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE usermaster SET Password = @NewPassword WHERE NudId = @NUDID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NewPassword", newPassword);
                    command.Parameters.AddWithValue("@NUDID", NUDID);

                    try
                    {
                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ErrorLabel.Text = "Password reset Successfully";
                            ErrorLabel.Visible = true;
                            // Wait for 5 seconds before redirecting to the login page
                            Response.AddHeader("REFRESH", "5;URL=LOGIN.aspx");
                            
                        }
                        else
                        {
                            ErrorLabel.Text = "Failed to reset password. Please try again.";
                            ErrorLabel.Visible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions
                        ErrorLabel.Text = "An error occurred: " + ex.Message;
                        ErrorLabel.Visible = true;
                    }
                }
            }
        }
    }
}
