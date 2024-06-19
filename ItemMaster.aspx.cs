using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VMS_1
{
    public partial class ItemMaster : System.Web.UI.Page
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
                LoadBasicItems();
            }
        }

        private void LoadGridView()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
            SELECT 
                b.Id AS ID,
                b.BasicItem AS BasicItem, 
                b.Category AS Category, 
                REPLACE(b.Denomination, ',', '') AS Denomination, 
                FORMAT(b.VegScale, 'N4') AS VegScale, 
                FORMAT(b.NonVegScale, 'N4') AS NonVegScale, 
                i.Id AS InlIueId,
                i.InLieuItem AS InLieuItem, 
                i.Category AS InLieuItemCategory, 
                REPLACE(i.Denomination, ',', '') AS InLieuItemDenomination, 
                FORMAT(i.VegScale, 'N4') AS InLieuItemVegScale, 
                FORMAT(i.NonVegScale, 'N4') AS InLieuItemNonVegScale
            FROM 
                BasicItems b
            LEFT JOIN 
                InLieuItems i ON b.Id = i.BasicItemId
            ORDER BY 
                b.Id DESC";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                GridView1.DataSource = reader;
                GridView1.DataBind();
            }
        }

        private void LoadBasicItems()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT MIN(Id) AS Id, BasicItem, BasicDenom FROM BasicLieuItems GROUP BY BasicItem, BasicDenom";
                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                basicItem.DataSource = reader;
                basicItem.DataTextField = "BasicItem";
                basicItem.DataValueField = "Id";
                basicItem.DataBind();
            }
            basicItem.Items.Insert(0, new ListItem("Select", ""));
        }

        [WebMethod]
        public static List<string> GetInLieuItems(string basicItem)
        {
            List<string> items = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string idQuery = "SELECT BasicItem FROM BasicLieuItems WHERE Id = @Id";
                string basicItemValue = null;
                using (SqlCommand cmd = new SqlCommand(idQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", basicItem);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            basicItemValue = reader["BasicItem"].ToString();
                        }
                    }
                }

                if (basicItemValue != null)
                {
                    string query = "SELECT ilueItem, ilueDenom FROM BasicLieuItems WHERE BasicItem = @BasicItem";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BasicItem", basicItemValue);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                items.Add(reader["ilueItem"].ToString());
                            }
                        }
                    }
                }
            }

            return items;
        }

        [WebMethod]
        public static string GetBasicDenom(string basicItem)
        {
            string basicDenom = string.Empty;
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT BasicDenom FROM BasicLieuItems WHERE Id = @BasicItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BasicItem", basicItem);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    basicDenom = reader["BasicDenom"].ToString();
                }
            }

            return basicDenom;
        }

        [WebMethod]
        public static string GetDenominationForInLieuItem(string inlieuItem)
        {
            string ilueDenom = string.Empty;
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "select iLueDenom from BasicLieuItems where iLueItem = @ilueItem";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ilueItem", inlieuItem);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ilueDenom = reader["iLueDenom"].ToString();
                }
            }

            return ilueDenom;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                string itemName = basicItem.SelectedItem.Text;
                string category = Request.Form["category"];
                string denomsVal = Request.Form["denoms"];
                denomsVal = denomsVal.Replace(",", "");
                decimal VegScale = decimal.Parse(Request.Form["veg"]);
                decimal NonVegScale = decimal.Parse(Request.Form["nonveg"]);

                string[] inlieuItem = Request.Form.GetValues("inlieuItem");
                string[] categoryIlue = Request.Form.GetValues("categoryIlue");
                string[] vegscaleIlueStrings = Request.Form.GetValues("vegscaleIlue");
                string[] nonvegscaleIlueStrings = Request.Form.GetValues("nonvegscaleIlue");

                decimal[] vegscaleIlue = Array.ConvertAll(vegscaleIlueStrings, s => string.IsNullOrEmpty(s) ? 0.00m : decimal.Parse(s));
                decimal[] nonvegscaleIlue = Array.ConvertAll(nonvegscaleIlueStrings, s => string.IsNullOrEmpty(s) ? 0.00m : decimal.Parse(s));

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        int itemID;

                        // Check if the basic item exists
                        using (SqlCommand checkItemCmd = new SqlCommand("SELECT Id FROM BasicItems WHERE BasicItem = @ItemName AND Category = @Category", conn, transaction))
                        {
                            checkItemCmd.Parameters.AddWithValue("@ItemName", itemName);
                            checkItemCmd.Parameters.AddWithValue("@Category", category);
                            object result = checkItemCmd.ExecuteScalar();

                            if (result != null)
                            {
                                itemID = (int)result;

                                // Update existing item
                                using (SqlCommand updateItemCmd = new SqlCommand("UPDATE BasicItems SET VegScale = @VegScale, NonVegScale = @NonVegScale, Denomination = @Denomination, Category = @Category, updateDate = GETDATE() WHERE Id = @ItemID", conn, transaction))
                                {
                                    updateItemCmd.Parameters.AddWithValue("@VegScale", VegScale);
                                    updateItemCmd.Parameters.AddWithValue("@NonVegScale", NonVegScale);
                                    updateItemCmd.Parameters.AddWithValue("@Denomination", denomsVal);
                                    updateItemCmd.Parameters.AddWithValue("@Category", category);
                                    updateItemCmd.Parameters.AddWithValue("@ItemID", itemID);
                                    updateItemCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert new item
                                using (SqlCommand insertItemCmd = new SqlCommand("INSERT INTO BasicItems (BasicItem, Category, Denomination, VegScale, NonVegScale, AddDate, updateDate) VALUES (@ItemName, @Category, @Denomination, @VegScale, @NonVegScale, GETDATE(), NULL); SELECT SCOPE_IDENTITY();", conn, transaction))
                                {
                                    insertItemCmd.Parameters.AddWithValue("@ItemName", itemName);
                                    insertItemCmd.Parameters.AddWithValue("@Category", category);
                                    insertItemCmd.Parameters.AddWithValue("@Denomination", denomsVal);
                                    insertItemCmd.Parameters.AddWithValue("@VegScale", VegScale);
                                    insertItemCmd.Parameters.AddWithValue("@NonVegScale", NonVegScale);
                                    itemID = Convert.ToInt32(insertItemCmd.ExecuteScalar());
                                }
                            }
                        }

                        // Process alternate items
                        if (inlieuItem != null)
                        {
                            for (int i = 0; i < inlieuItem.Length; i++)
                            {
                                string altItemName = inlieuItem[i] ?? "";
                                string altCategory = categoryIlue[i] ?? "";
                                decimal altVegScale = vegscaleIlue[i];
                                decimal altNonVegScale = nonvegscaleIlue[i];

                                // Check if the alternate item exists for the same BasicItemId, InLieuItem, and Category
                                using (SqlCommand checkAltCmd = new SqlCommand("SELECT BasicItemId FROM InLieuItems WHERE InLieuItem = @AltItemName AND Category = @AltCategory", conn, transaction))
                                {
                                    checkAltCmd.Parameters.AddWithValue("@ItemID", itemID);
                                    checkAltCmd.Parameters.AddWithValue("@AltItemName", altItemName);
                                    checkAltCmd.Parameters.AddWithValue("@AltCategory", altCategory);
                                    object altResult = checkAltCmd.ExecuteScalar();

                                    if (altResult != null)
                                    {
                                        lblStatus.Text = "Item already exists. Cannot add duplicate item.";

                                        //using (SqlCommand updateAltCmd = new SqlCommand("UPDATE InLieuItems SET BasicItemId = @ItemID, InLieuItem = @AltItemName, Category = @AltCategory, Denomination = @Denomination, VegScale = @VegScale, NonVegScale = @NonVegScale WHERE BasicItemId = @ItemID", conn, transaction))
                                        //{
                                        //    updateAltCmd.Parameters.AddWithValue("@ItemID", itemID);
                                        //    updateAltCmd.Parameters.AddWithValue("@AltItemName", altItemName);
                                        //    updateAltCmd.Parameters.AddWithValue("@AltCategory", altCategory);
                                        //    updateAltCmd.Parameters.AddWithValue("@Denomination", denomsVal);
                                        //    updateAltCmd.Parameters.AddWithValue("@VegScale", altVegScale);
                                        //    updateAltCmd.Parameters.AddWithValue("@NonVegScale", altNonVegScale);
                                        //    updateAltCmd.ExecuteNonQuery();
                                        //}
                                    }
                                    else
                                    {
                                        using (SqlCommand insertAltCmd = new SqlCommand("INSERT INTO InLieuItems (BasicItemId, InLieuItem, Category, Denomination, VegScale, NonVegScale) VALUES (@ItemID, @AltItemName, @AltCategory, @Denomination, @VegScale, @NonVegScale)", conn, transaction))
                                        {
                                            insertAltCmd.Parameters.AddWithValue("@ItemID", itemID);
                                            insertAltCmd.Parameters.AddWithValue("@AltItemName", altItemName);
                                            insertAltCmd.Parameters.AddWithValue("@AltCategory", altCategory);
                                            insertAltCmd.Parameters.AddWithValue("@Denomination", denomsVal);
                                            insertAltCmd.Parameters.AddWithValue("@VegScale", altVegScale);
                                            insertAltCmd.Parameters.AddWithValue("@NonVegScale", altNonVegScale);
                                            insertAltCmd.ExecuteNonQuery();
                                        }
                                        lblStatus.Text = "Data entered successfully.";
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        lblMessage.Text = "An error occurred: " + ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred: " + ex.Message;
            }
            //ScriptManager.RegisterStartupScript(this, GetType(), "refreshPage", "window.location.href=window.location.href;", true);
            LoadGridView();
        }



        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            LoadGridView();
        }

        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            LoadGridView();
        }

        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int rowIndex = e.RowIndex;
                if (rowIndex >= 0 && rowIndex < GridView1.Rows.Count)
                {
                    GridViewRow row = GridView1.Rows[rowIndex];
                    int id = Convert.ToInt32(GridView1.DataKeys[rowIndex].Value);

                    decimal vegScale = decimal.Parse(((TextBox)row.FindControl("txtVegScale")).Text);
                    decimal nonVegScale = decimal.Parse(((TextBox)row.FindControl("txtNonVegScale")).Text);

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE InLieuItems SET VegScale = @VegScale, NonVegScale = @NonVegScale WHERE Id = @Id", conn))
                        {
                            cmd.Parameters.AddWithValue("@VegScale", vegScale);
                            cmd.Parameters.AddWithValue("@NonVegScale", nonVegScale);
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    GridView1.EditIndex = -1;
                    LoadGridView();
                }
            }
            catch (Exception ex)
            {
                // Log the error or handle it accordingly
                Response.Write("Error: " + ex.Message);
            }
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM InLieuItems WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }

            LoadGridView();
        }
    }
}
