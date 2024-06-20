using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace VMS_1
{
    public partial class OtherShips : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FormsAuthentication.RedirectToLoginPage();
            }
            GetItems();
            LoadGridView();
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

                string[] date = Request.Form.GetValues("date");
                string[] itemnameId = Request.Form.GetValues("itemname");
                string[] denom = Request.Form.GetValues("denom");
                string[] qty = Request.Form.GetValues("qty");
                string[] refno = Request.Form.GetValues("refno");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Iterate through each row and insert data into the database
                    for (int i = 0; i < date.Length; i++)
                    {
                        string itemname = GetItemNameById(conn, itemnameId[i]);

                        SqlCommand cmd = new SqlCommand("INSERT INTO otherships (Date, Itemname, Denom, Qty, Reference) VALUES (@Date, @Itemname, @Denom, @Qty, @Reference)", conn);

                        cmd.Parameters.AddWithValue("@Date", date[i]);
                        cmd.Parameters.AddWithValue("@Itemname", itemname);
                        cmd.Parameters.AddWithValue("@Denom", denom[i]);
                        cmd.Parameters.AddWithValue("@Qty", qty[i]);
                        cmd.Parameters.AddWithValue("@Reference", refno[i]);

                        cmd.ExecuteNonQuery();

                        // Update PresentStockMaster table if ItemName exists
                        SqlCommand updatePresentStockCmd = new SqlCommand("UPDATE PresentStockMaster SET Qty = Qty - @Quantity WHERE ItemName = @ItemName", conn);
                        updatePresentStockCmd.Parameters.AddWithValue("@ItemName", itemname);
                        updatePresentStockCmd.Parameters.AddWithValue("@Quantity", qty[i]);
                        updatePresentStockCmd.Parameters.AddWithValue("@Denos", denom[i]);
                        updatePresentStockCmd.ExecuteNonQuery();

                        SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM MonthEndStockMaster WHERE Date = @Date AND ItemName = @ItemName", conn);
                        checkCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                        checkCmd.Parameters.AddWithValue("@ItemName", itemname);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            // If data exists, update the existing row
                            SqlCommand updateCmd = new SqlCommand("UPDATE MonthEndStockMaster SET Qty = CASE WHEN Qty - @Quantity >= 0 THEN Qty - @Quantity ELSE Qty END WHERE Date = @Date AND ItemName = @ItemName", conn);
                            updateCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            updateCmd.Parameters.AddWithValue("@ItemName", itemname);
                            updateCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));

                            updateCmd.ExecuteNonQuery();
                        }
                        else
                        {
                            // If no data exists, insert a new row
                            SqlCommand insertCmd = new SqlCommand("INSERT INTO MonthEndStockMaster (Date, ItemName, Qty, Type) VALUES (@Date, @ItemName, @Quantity, @Type)", conn);
                            insertCmd.Parameters.AddWithValue("@Date", DateTime.Parse(date[i])); // Use current date
                            insertCmd.Parameters.AddWithValue("@ItemName", itemname);
                            insertCmd.Parameters.AddWithValue("@Quantity", decimal.Parse(qty[i]));
                            insertCmd.Parameters.AddWithValue("@Type", "Issue"); // Set the correct parameter name for Type

                            insertCmd.ExecuteNonQuery();
                        }
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

        private string GetItemNameById(SqlConnection conn, string itemId)
        {
            string itemName = string.Empty;
            string query = "SELECT ILueItem FROM BasicLieuItems WHERE Id = @ItemId";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ItemId", itemId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        itemName = reader["ILueItem"].ToString();
                    }
                }
            }

            return itemName;
        }

        protected void UploadFileButton_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string fileName = Path.GetFileName(FileUpload1.FileName);
                string fileDateString = Request.Form["fileDate"];  // Assuming fileDateTextBox is the ID of your input element for date selection
                DateTime fileDate;

                if (DateTime.TryParse(fileDateString, out fileDate))
                {
                    // Successfully parsed the date
                    // Proceed with your logic
                    lblStatus.Text = "File date: " + fileDate.ToString("MMMM yyyy");
                }
                else
                {
                    // Handle invalid date format
                    lblStatus.Text = "Invalid date format.";
                }

                // Save the file to the database
                SaveFileToDatabase(FileUpload1.PostedFile.InputStream, fileName, fileDate);
            }
            else
            {
                lblStatus.Text = "Please select a file to upload.";
            }
        }

        private void SaveFileToDatabase(Stream fileStream, string fileName, DateTime fileDate)
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    BinaryReader br = new BinaryReader(fileStream);
                    Byte[] bytes = br.ReadBytes((Int32)fileStream.Length);

                    string query = "INSERT INTO PDFFiles (Name, type, Date, Data) VALUES (@Name, @type, @FileDate, @Data)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = fileName;
                    cmd.Parameters.Add("@type", "OtherShips");
                    cmd.Parameters.Add("@FileDate", SqlDbType.VarChar).Value = fileDate;
                    cmd.Parameters.Add("@Data", SqlDbType.Binary).Value = bytes;

                    cmd.ExecuteNonQuery();
                }
                lblStatus.Text = "File uploaded successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while uploading the file: " + ex.Message;
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

                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM otherships", conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    GridViewOtherShips.DataSource = dt;
                    GridViewOtherShips.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "An error occurred while binding the grid view: " + ex.Message;
            }
        }

        [WebMethod]
        public static List<object> GetItems()
        {
            string connStr = ConfigurationManager.ConnectionStrings["InsProjConnectionString"].ConnectionString;
            string query = "SELECT * FROM RationScale";
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
                    Text = row["ItemName"].ToString(),
                    Value = row["ItemId"].ToString(),
                });
            }

            return items;
        }
    }
}