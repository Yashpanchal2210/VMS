<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddBasicandLieuItems.aspx.cs" Inherits="VMS_1.AddBasicandLieuItems" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mb-4">Add Items</h2>
        <form id="itemMasterForm" runat="server">
            <h3>Basic Items</h3>
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Basic Item</th>
                            <th class="heading">Denomination</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="text" class="form-control" name="basicitem" required />
                            </td>
                            <td>
                                <select class="form-control" name="basicdenom" required>
                                    <option value="">Select</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Other">Other</option>
                                </select>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <h3>In-lieu Items</h3>
            <div class="table-responsive">
                <table class="table" id="Table2">
                    <thead>
                        <tr>
                            <th class="heading">In-lieu Item</th>
                            <th class="heading">Denomination</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="Tbody1" runat="server">
                        <tr>
                            <td>
                                <input type="text" class="form-control" name="ilieuitem" required />
                            </td>
                            <td>
                                <select class="form-control" name="ilieudenom" required>
                                    <option value="">Select</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Other">Other</option>
                                </select>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Submit Button -->
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="SubmitButton_Click" />
            <button type="button" class="btn btn-primary" onclick="addNewItem()">Add In-lieu Item</button>

            <!-- Alternative Item Fields Container -->
            <div id="alternateItemContainer"></div>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            <div class="mt-2">
                <asp:GridView ID="GridViewBasicInlieuItems" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="false" DataKeyNames="ID" OnRowEditing="GridViewBasicInlieuItems_RowEditing" OnRowUpdating="GridViewBasicInlieuItems_RowUpdating" OnRowCancelingEdit="GridViewBasicInlieuItems_RowCancelingEdit" OnRowDeleting="GridViewBasicInlieuItems_RowDeleting">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="BasicItem" HeaderText="Basic Item" />
                        <asp:BoundField DataField="BasicDenom" HeaderText="Denomination" />
                        <asp:BoundField DataField="iLueItem" HeaderText="In-lieu Item" />
                        <asp:BoundField DataField="iLueDenom" HeaderText="Denomination" />
                        <asp:CommandField HeaderText="Action" ShowEditButton="true" ShowDeleteButton="true" />
                    </Columns>
                </asp:GridView>
            </div>
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>

    <script type="text/javascript">
        function addNewItem() {
            var tableBody = document.getElementById("MainContent_Tbody1");
            var newRow = document.createElement("tr");
            newRow.innerHTML = ` <td>
                                     <input type="text" class="form-control" name="ilieuitem" required />
                                 </td>
                                 <td>
                                     <select class="form-control" name="ilieudenom" required>
                                         <option value="">Select</option>
                                         <option value="Kgs">Kgs</option>
                                         <option value="Ltr">Ltr</option>
                                         <option value="Nos">Nos</option>
                                         <option value="Other">Other</option>
                                     </select>
                                 </td>
                                 <td>
                                     <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                                 </td>`;
            tableBody.appendChild(newRow);
        }

        function setTheme(theme) {
            var gridView = document.getElementById("GridView1");

            if (theme === "blue") {
                document.body.style.backgroundColor = '#3498db';
                document.body.classList.remove('dark-theme');
                gridView.classList.remove('dark-theme-text');
                document.querySelectorAll('.heading').forEach(function (element) {
                    element.classList.remove('dark-theme');
                });
                document.querySelector('.table').classList.remove('dark-theme');
            } else if (theme === "dark") {
                document.body.style.backgroundColor = '#333';
                document.body.classList.add('dark-theme');
                gridView.classList.add('dark-theme-text');
                document.querySelectorAll('.heading').forEach(function (element) {
                    element.classList.add('dark-theme');
                });
                document.querySelector('.table').classList.add('dark-theme');
            }
        }

        function deleteRow(btn) {
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }
    </script>
</asp:Content>
