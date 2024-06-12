using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
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
            string nuid;

            if (ValidateUser(username, password, out role, out name, out nuid))
            {
                Session["UserName"] = name;
                Session["Role"] = role;
                Session["NudId"] = nuid;
                FormsAuthentication.SetAuthCookie(username, false);
                Response.Redirect("Dashboard.aspx");
            }
            else
            {
                ErrorLabel.Visible = true;
                ErrorLabel.ForeColor = Color.Red;
                ErrorLabel.Text = "Invalid username or password Or You are not approved yet";
                UserName.Focus();
            }
        }

        private bool ValidateUser(string username, string password, out string role, out string name, out string nuid)
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
                    string approvalQuery = "SELECT IsApproved FROM usermaster WHERE NudId = @Username AND Password = @Password";
                    using (SqlCommand approvalCmd = new SqlCommand(approvalQuery, con))
                    {
                        approvalCmd.Parameters.AddWithValue("@Username", username);
                        approvalCmd.Parameters.AddWithValue("@Password", password);

                        object isApprovedObj = approvalCmd.ExecuteScalar();
                        if (isApprovedObj != null && isApprovedObj != DBNull.Value && Convert.ToBoolean(isApprovedObj) == true)
                        {
                            role = null;
                            name = null;
                            nuid = null;

                            string roleQuery = "SELECT Role FROM usermaster WHERE NudId = @Username AND Password = @Password";
                            string nameQuery = "SELECT name FROM usermaster WHERE NudId = @Username AND Password = @Password";
                            string nuidQuery = "SELECT NudId FROM usermaster WHERE NudId = @Username AND Password = @Password";

                            using (SqlCommand roleCmd = new SqlCommand(roleQuery, con))
                            using (SqlCommand nudidCmd = new SqlCommand(nuidQuery, con))
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

                                nudidCmd.Parameters.AddWithValue("@Username", username);
                                nudidCmd.Parameters.AddWithValue("@Password", password);

                                object nudidObj = nudidCmd.ExecuteScalar();
                                if (nudidObj != null)
                                {
                                    nuid = nudidObj.ToString();
                                }

                                return true;
                            }
                        }
                        else
                        {
                            role = null;
                            name = null;
                            nuid = null;
                            return false;
                        }
                    }
                }
            }

            role = null;
            name = null;
            nuid = null;
            return false;
        }




    }
}

