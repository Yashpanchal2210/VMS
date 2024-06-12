using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class UnapprovedUsers : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            if (!IsPostBack)
            {
                LoadGridView();
            }
        }

        private void LoadGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT name, rank, Designation, NudId, Password, Role, IsApproved FROM usermaster WHERE NudId <> 'admin' AND (IsApproved IS NULL OR IsApproved = 0)";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridViewUser.DataSource = reader;
                GridViewUser.DataBind();
            }
        }

        protected void GridViewUser_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Approve")
            {
                string nudId = e.CommandArgument.ToString();
                ApproveUser(nudId);
            }
            else if (e.CommandName == "Reject")
            {
                string nudId = e.CommandArgument.ToString();
                RejectUser(nudId);
            }
        }

        private void ApproveUser(string nudId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE usermaster SET IsApproved = 1 WHERE NudId = @NudId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NudId", nudId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadGridView();
        }

        private void RejectUser(string nudId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM usermaster WHERE NudId = @NudId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NudId", nudId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadGridView();
        }
    }
}