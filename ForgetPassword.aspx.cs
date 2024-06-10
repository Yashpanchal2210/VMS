using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace VMS_1
{
    public partial class ForgetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string NUDID = txtNUDID.Text.Trim();
            string secretQuestion = ddlSecretQuestion.SelectedItem.Text;
            string answer = txtAnswer.Text.Trim();

            // Connection string to your database
            string connectionString = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM usermaster WHERE NudId = @NUDID AND SecretQuestion = @SecretQuestion AND Answer = @Answer";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NUDID", NUDID);
                    command.Parameters.AddWithValue("@SecretQuestion", secretQuestion);
                    command.Parameters.AddWithValue("@Answer", answer);

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            // If the provided information matches, redirect to ResetPassword page with NUDID parameter
                            Response.Redirect($"ResetPassword.aspx?NUDID={NUDID}");
                        }
                        else
                        {
                            // If no match found, display an error message
                            ErrorLabel.Text = "Invalid NUD ID, secret question, or answer. Please try again.";
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
