using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace VMS_1
{
    public partial class UserRegister : System.Web.UI.Page
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

            //string cs = "Data Source=PIYUSH-JHA\\SQLEXPRESS;Initial Catalog=InsProj;Integrated Security=SSPI;Encrypt=False";
            string cs = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                // Check if user with the same NUD ID already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM usermaster WHERE NudId = @NudID", con);
                checkCmd.Parameters.AddWithValue("@NudID", NudID.Text);
                con.Open();
                int existingUserCount = (int)checkCmd.ExecuteScalar();
                if (existingUserCount > 0)
                {
                    Label1.Visible = true;
                    Label1.Text = "User with this NUD ID already exists.";
                    return;
                }

                // Insert new user record
                SqlCommand cmd = new SqlCommand("INSERT INTO usermaster(Name, Rank, Designation, NudId, Password,SecretQuestion, Answer, Role) VALUES(@Name, @Rank, @Designation, @NudID, @Password, @SecretQuestion, @Answer, @Role)", con);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Name", Name.Text);
                cmd.Parameters.AddWithValue("@Rank", Rank.SelectedValue);
                cmd.Parameters.Add("@Designation", System.Data.SqlDbType.NVarChar).Value = Designation.Text;
                cmd.Parameters.AddWithValue("@NudID", NudID.Text);
                cmd.Parameters.AddWithValue("@Password", Password.Text);
                cmd.Parameters.AddWithValue("@SecretQuestion", SecretQuestion.SelectedValue);
                cmd.Parameters.AddWithValue("@Role", Role.SelectedValue);
                cmd.Parameters.AddWithValue("@Answer", Answer.Text);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    Label1.Visible = true;
                    Label1.Text = "Record saved successfully.";

                    // Wait for 5 seconds before redirecting to the login page
                    Response.AddHeader("REFRESH", "5;URL=LOGIN.aspx");
                }
                else
                {
                    Label1.Visible = true;
                    Label1.Text = "Record not saved, please try again.";
                }
            }
        }
    }
}
