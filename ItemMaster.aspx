<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ItemMaster.aspx.cs" Inherits="VMS_1.ItemMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Item Master</h2>
        <p class="auto-style1"><strong>Disclaimer: Write all scales in Kg only</strong></p>

        <form id="itemMasterForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
                <asp:LinkButton ID="Receipt" runat="server" Text="Go to Receipt Module" CssClass="btn btn-info" PostBackUrl="~/ItemReceipt.aspx"></asp:LinkButton>
            </div>--%>

            <h2>Item Details</h2>
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading name">Item Category</th>
                            <th class="heading officerScale">Officer Scale (in Kg)</th>
                            <th class="heading sailorScale">Sailor Scale (in Kg)</th>
                            <th class="heading">Officer Veg</th>
                            <th class="heading">OfficerNonVeg</th>

                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="text" class="form-control" name="itemname" required /></td>
                            <td>
                                <input type="number" class="form-control" name="officerScale" required min="0" step="0.001" /></td>
                            <td>
                                <input type="number" class="form-control" name="sailorScale" required min="0" step="0.001" /></td>
                            <td>
                                <input type="number" class="form-control" name="veg" required min="0" step="0.001" /></td>
                            <td>
                                <input type="number" class="form-control" name="nonveg" required min="0" step="0.001" /></td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <h2>Equivalent Alternate Item Details</h2>
            <div class="table-responsive">
                <table class="table" id="Table2">
                    <thead>
                        <tr>
                            <th class="heading alternateitem">Alternate Item Name</th>
                            <th class="heading equivalentofficerScale">Equivalent Scale Officer (in Kg)</th>
                            <th class="heading equivalentsailorScale">Equivalent Scale Sailor (in Kg)</th>
                            <th class="heading denomination">Denomination</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="Tbody1" runat="server">
                        <tr>
                            <td>
                                <input type="text" class="form-control" name="alternateitemname" /></td>
                            <td>
                                <input type="number" class="form-control" name="equivalentofficerScale" required min="0" step="0.001" /></td>
                            <td>
                                <input type="number" class="form-control" name="equivalentsailorScale" required min="0" step="0.001" /></td>
                            <td>
                                <select class="form-control" name="denoms">
                                    <option value="">Select</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltrs">Ltrs</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Other">Other</option>
                                </select>
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Submit Button -->
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_Click" />
            <button type="button" class="btn btn-success" onclick="addAlternativeItem()">Add Alternative Item</button>

            <!-- Alternative Item Fields Container -->
            <div id="alternateItemContainer"></div>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
    </div>

    </div>
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>

    <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered table-striped">
    </asp:GridView>
    </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function addAlternativeItem() {
            var tableBody = document.getElementById("MainContent_Tbody1");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td><input type="text" class="form-control" name="alternateitemname" /></td>
                                <td><input type="number" class="form-control" name = "equivalentofficerScale" required min="0" step="0.001"/></td>
                                <td><input type="number" class="form-control" name="equivalentsailorScale" required min="0" step="0.001"/></td>
                                <td>
                                    <select class="form-control" name="denoms">
                                        <option value="">Select</option>
                                        <option value="Kgs">Kgs</option>
                                        <option value="Ltrs">Ltrs</option>
                                        <option value="Nos">Nos</option>
                                        <option value="Other">Other</option>
                                    </select>
                                </td>
                                <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
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
