using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.Services;
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
                b.Denomination AS Denomination, 
                b.VegScale AS VegScale, 
                b.NonVegScale AS NonVegScale, 
                i.InLieuItem AS InLieuItem, 
                i.Category AS InLieuItemCategory, 
                i.Denomination AS InLieuItemDenomination, 
                i.VegScale AS InLieuItemVegScale, 
                i.NonVegScale AS InLieuItemNonVegScale
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

                    SqlCommand mainItemCmd = new SqlCommand("UpsertItemWithAlternates", conn);
                    mainItemCmd.CommandType = CommandType.StoredProcedure;
                    mainItemCmd.Parameters.AddWithValue("@ItemName", itemName);
                    mainItemCmd.Parameters.AddWithValue("@Category", category);
                    mainItemCmd.Parameters.AddWithValue("@Denomination", denomsVal);
                    mainItemCmd.Parameters.AddWithValue("@VegScale", VegScale);
                    mainItemCmd.Parameters.AddWithValue("@NonVegScale", NonVegScale);
                    mainItemCmd.ExecuteNonQuery();


                    object itemID;
                    using (SqlCommand getIdCmd = new SqlCommand("SELECT TOP 1 Id FROM BasicItems ORDER BY Id DESC", conn))
                    {
                        itemID = getIdCmd.ExecuteScalar();
                    }
                    if (inlieuItem != null)
                    {
                        for (int i = 0; i < inlieuItem.Length; i++)
                        {
                            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM PresentStockMaster WHERE ItemName = @AltItemName", conn);
                            checkCmd.Parameters.AddWithValue("@AltItemName", inlieuItem[i]);
                            int Presentcount = (int)checkCmd.ExecuteScalar();

                            SqlCommand checkCmdIssue = new SqlCommand("SELECT COUNT(*) FROM InLieuItems WHERE InLieuItem = @AltItemName", conn);
                            checkCmdIssue.Parameters.AddWithValue("@AltItemName", inlieuItem[i]);
                            int Issuecount = (int)checkCmdIssue.ExecuteScalar();


                            if (Presentcount == 0)
                            {
                                SqlCommand insertCmd = new SqlCommand("INSERT INTO PresentStockMaster (ItemName, Qty) VALUES (@AltItemName, @Qty)", conn);
                                insertCmd.Parameters.AddWithValue("@AltItemName", inlieuItem[i]);
                                insertCmd.Parameters.AddWithValue("@Qty", "0");
                                insertCmd.ExecuteNonQuery();
                            }

                            if (Issuecount == 0)
                            {
                                SqlCommand getDenomCmd = new SqlCommand("SELECT iLueDenom FROM BasicLieuItems WHERE iLueItem = @ItemName", conn);
                                getDenomCmd.Parameters.AddWithValue("@ItemName", inlieuItem[i]);
                                string denomination = (string)getDenomCmd.ExecuteScalar();

                                SqlCommand altItemCmd = new SqlCommand("INSERT INTO InLieuItems (BasicItemId, InLieuItem, Category, Denomination, VegScale, NonVegScale) VALUES (@IlueId, @IlieuName, @CategoryIlieu, @Denomilieu, @VegScale, @NonVegScale)", conn);
                                altItemCmd.Parameters.AddWithValue("@IlueId", itemID);
                                altItemCmd.Parameters.AddWithValue("@IlieuName", inlieuItem[i] ?? "");
                                altItemCmd.Parameters.AddWithValue("@CategoryIlieu", categoryIlue[i] ?? "");
                                altItemCmd.Parameters.AddWithValue("@Denomilieu", denomination ?? "");
                                altItemCmd.Parameters.AddWithValue("@VegScale", vegscaleIlue[i]);
                                altItemCmd.Parameters.AddWithValue("@NonVegScale", nonvegscaleIlue[i]);
                                altItemCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                lblStatus.Text = "Data entered successfully.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = "An error occurred: " + ex.Message;
            }

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

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int rowIndex = e.RowIndex;

                int rowCount = GridView1.Rows.Count;

                int dataKeyCount = GridView1.DataKeys.Count;

                var dataKeys = GridView1.DataKeys;

                int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Value);
                DeleteItem(id);
                LoadGridView();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void DeleteItem(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                SqlCommand deleteInLieuCmd = new SqlCommand("DELETE FROM InLieuItems WHERE BasicItemId = @Id", conn);
                deleteInLieuCmd.Parameters.AddWithValue("@Id", id);
                deleteInLieuCmd.ExecuteNonQuery();

                SqlCommand deleteCmd = new SqlCommand("DELETE FROM BasicItems WHERE Id = @Id", conn);
                deleteCmd.Parameters.AddWithValue("@Id", id);
                deleteCmd.ExecuteNonQuery();
            }
        }
    }
}