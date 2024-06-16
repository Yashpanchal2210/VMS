using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class RationScale : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                GetItemNames();
                BindGridView();
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                // Get data from the form
                string[] itemname = Request.Form.GetValues("itemname");
                string[] rate = Request.Form.GetValues("rate");

                // Connect to the database
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    for (int i = 0; i < itemname.Length; i++)
                    {
                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM RationScale WHERE ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@ItemName", itemname[i]);

                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // Update the existing item
                            SqlCommand updateCmd = new SqlCommand("UPDATE RationScale SET ItemName = @ItemName, Rate = @Rate WHERE ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@ItemName", itemname[i]);
                            updateCmd.Parameters.AddWithValue("@Rate", decimal.Parse(rate[i]));

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand("INSERT INTO RationScale (ItemId, ItemName, Rate) VALUES (@ItemId, @ItemName, @Rate)", conn);
                            cmd.CommandType = CommandType.Text;

                            int itemId = GetItemIdByName(itemname[i]); // Implement this method to get the ItemId by ItemName

                            cmd.Parameters.AddWithValue("@ItemId", itemId);
                            cmd.Parameters.AddWithValue("@ItemName", itemname[i]);
                            cmd.Parameters.AddWithValue("@Rate", decimal.Parse(rate[i]));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                lblStatus.Text = "Data entered successfully.";

                // Optionally, you can refresh the GridView after data insertion here
                BindGridView();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred: " + ex.Message;
            }
        }

        private int GetItemIdByName(string itemName)
        {
            int itemId = 0;

            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = "SELECT AltItemID FROM AlternateItem WHERE AltItemName = @ItemName";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ItemName", itemName);

                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    itemId = Convert.ToInt32(result);
                }
            }

            return itemId;
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

        private void BindGridView()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM RationScale", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewRationScale.DataSource = dt;
                    GridViewRationScale.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        protected void GridViewRationScale_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridViewRationScale.EditIndex = e.NewEditIndex;
            BindGridView();

            try
            {
                DropDownList ddlItemName = (DropDownList)GridViewRationScale.Rows[e.NewEditIndex].FindControl("ddlItemName");
                if (ddlItemName != null)
                {
                    List<object> items = GetItemNames();
                    ddlItemName.DataSource = items;
                    ddlItemName.DataTextField = "Text";
                    ddlItemName.DataValueField = "Value";
                    ddlItemName.DataBind();

                    GridViewRow row = GridViewRationScale.Rows[e.NewEditIndex];
                    if (row != null && row.DataItem != null)
                    {
                        string currentItemName = ((DataRowView)row.DataItem)["ItemName"].ToString();
                        if (ddlItemName.Items.FindByText(currentItemName) != null)
                        {
                            ddlItemName.SelectedValue = currentItemName;
                        }
                    }
                }
                else
                {
                    lblStatus.Text = "DropDownList ddlItemName not found.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while editing: " + ex.Message;
            }
        }

        protected void GridViewRationScale_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            int id = (int)e.Keys["ID"];
            DropDownList ddlItemName = (DropDownList)GridViewRationScale.Rows[e.RowIndex].FindControl("ddlItemName");
            string itemname = ddlItemName.SelectedItem.Text;
            string rate = (string)e.NewValues["Rate"];

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "UPDATE RationScale SET ItemName=@ItemName, Rate=@Rate WHERE Id=@ID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ItemName", itemname);
                cmd.Parameters.AddWithValue("@Rate", rate);
                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            GridViewRationScale.EditIndex = -1;
            BindGridView();
        }

        protected void GridViewRationScale_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridViewRationScale.EditIndex = -1;
            BindGridView();
        }

        protected void GridViewRationScale_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            int id = (int)e.Keys["ID"];

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "DELETE FROM RationScale WHERE Id=@ID";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            BindGridView();
        }
    }
}
