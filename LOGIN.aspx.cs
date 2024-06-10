using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace VMS_1
{
    public partial class LOGIN : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                FormsAuthentication.SignOut();
            }
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            Response.AppendHeader("Pragma", "no-cache");
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }


        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = UserName.Text;
            string password = Password.Text;
            string role;
            string name;

            if (ValidateUser(username, password, out role, out name))
            {
                Session["UserName"] = name;
                Session["Role"] = role;
                FormsAuthentication.SetAuthCookie(username, false);
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                // If user is not valid, display error message
                ErrorLabel.Visible = true;
                ErrorLabel.Text = "Invalid username or password";
                UserName.Focus();
            }
        }

        private bool ValidateUser(string username, string password, out string role, out string name)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT COUNT(*) FROM usermaster WHERE NudId = @Username AND Password = @Password";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Password", password);

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    role = null;
                    name = null;

                    string roleQuery = "SELECT Role FROM usermaster WHERE NudId = @Username AND Password = @Password";
                    string nameQuery = "SELECT name FROM usermaster WHERE NudId = @Username AND Password = @Password";

                    using (SqlCommand roleCmd = new SqlCommand(roleQuery, con))
                    using (SqlCommand nameCmd = new SqlCommand(nameQuery, con))
                    {
                        roleCmd.Parameters.AddWithValue("@Username", username);
                        roleCmd.Parameters.AddWithValue("@Password", password);

                        object roleObj = roleCmd.ExecuteScalar();
                        if (roleObj != null)
                        {
                            role = roleObj.ToString();
                        }

                        nameCmd.Parameters.AddWithValue("@Username", username);
                        nameCmd.Parameters.AddWithValue("@Password", password);

                        object nameObj = nameCmd.ExecuteScalar();
                        if (nameObj != null)
                        {
                            name = nameObj.ToString();
                        }

                        return true;
                    }
                }
            }

            role = null;
            name = null;
            return false;
        }



    }
}

