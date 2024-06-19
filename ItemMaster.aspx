<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ItemMaster.aspx.cs" Inherits="VMS_1.ItemMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mb-4">Enter Scale of Items</h2>

        <form id="itemMasterForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
                <asp:LinkButton ID="Receipt" runat="server" Text="Go to Receipt Module" CssClass="btn btn-info" PostBackUrl="~/ItemReceipt.aspx"></asp:LinkButton>
            </div>--%>

            <h3>Basic Items</h3>
            <div class="table-responsive">
                <input type="hidden" id="categoryVal" />
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Basic Item</th>
                            <th class="heading">Category</th>
                            <th class="heading">Denomination</th>
                            <th class="heading">Veg Scale</th>
                            <th class="heading">NonVeg Scale</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <%--<input type="text" class="form-control" name="itemname" required />--%>
                                <asp:DropDownList ID="basicItem" runat="server" CssClass="form-control" onchange="fetchBasicDenom()" required>
                                    <asp:ListItem Text="Select" Value="" />
                                </asp:DropDownList>
                            </td>
                            <td>
                                <%--<input type="number" class="form-control" name="officerScale" required min="0" step="0.001" />--%>
                                <select class="form-control" name="category" id="CategoryBasic" onchange="changeCategory(this.value)" required>
                                    <option value="">Select</option>
                                    <option value="Officer">Officer</option>
                                    <option value="Sailor">Sailor</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denoms" readonly />
                                <%-- <select class="form-control" name="denoms" required>
                                    <option value="">Select</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Other">Other</option>
                                </select>--%>
                            </td>
                            <td>
                                <input type="number" class="form-control" name="veg" required min="0" step="0.00000001" /></td>
                            <td>
                                <input type="number" class="form-control" name="nonveg" required min="0" step="0.00000001" /></td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <h3>In-lieu Items Details</h3>
            <div class="table-responsive">
                <table class="table" id="Table2">
                    <thead>
                        <tr>
                            <th class="heading">In-lieu Item</th>
                            <th class="heading">Category</th>
                            <th class="heading">Denomination</th>
                            <th class="heading">Veg Scale</th>
                            <th class="heading">NonVeg Scale</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody id="Tbody1" runat="server">
                        <tr>
                            <td>
                                <select class="form-control" name="inlieuItem" id="inlieuItemVal">
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="categoryIlue" id="categoryIlue" readonly /></td>
                            <td>
                                <input type="text" class="form-control" name="denoms" readonly />
                            </td>
                            <td>
                                <input type="number" class="form-control" name="vegscaleIlue" min="0" step="0.00000001" />
                            </td>
                            <td>
                                <input type="number" class="form-control" name="nonvegscaleIlue" min="0" step="0.00000001" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <!-- Submit Button -->
            <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
            <button type="button" class="btn btn-primary" onclick="addAlternativeItem()">Add Alternative Item</button>

            <!-- Alternative Item Fields Container -->
            <div id="alternateItemContainer"></div>
            <asp:Label ID="lblMessage" runat="server" ForeColor="Green"></asp:Label>
            <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            <div class="mt-2">
                <asp:GridView ID="GridView1" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" OnRowEditing="GridView1_RowEditing" OnRowCancelingEdit="GridView1_RowCancelingEdit" OnRowDeleting="GridView1_RowDeleting">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" InsertVisible="false" Visible="false" />
                        <asp:BoundField DataField="BasicItem" HeaderText="Basic Item" ReadOnly="True" />
                        <asp:BoundField DataField="Category" HeaderText="Category" ReadOnly="True" />
                        <asp:BoundField DataField="Denomination" HeaderText="Denomination" ReadOnly="True" />
                        <asp:BoundField DataField="VegScale" HeaderText="Veg Scale" ReadOnly="True" />
                        <asp:BoundField DataField="NonVegScale" HeaderText="NonVeg Scale" ReadOnly="True" />
                        <asp:BoundField DataField="InLieuItem" HeaderText="InLieu Item" ReadOnly="True" />
                        <asp:BoundField DataField="Category" HeaderText="Category" ReadOnly="True" />
                        <%--<asp:BoundField DataField="Denomination" HeaderText="Denomination" ReadOnly="True" />--%>
                        <asp:BoundField DataField="VegScale" HeaderText="VegScale" ReadOnly="True" />
                        <asp:BoundField DataField="NonVegScale" HeaderText="NonVeg Scale" ReadOnly="True" />
                        <%--<asp:CommandField HeaderText="Action" ShowDeleteButton="true" />--%>
                    </Columns>
                </asp:GridView>
            </div>

        </form>
    </div>


    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        var rowSequence = 1;
        function addAlternativeItem() {
            var tableBody = document.getElementById("MainContent_Tbody1");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td>
                                    <select class="form-control" name="inlieuItem" id="inlieuItemVal_${rowSequence}">
                                        <option value="">Select</option>
                                    </select>
                                </td>
                                <td>
                                    <input type="text" class="form-control" name="categoryIlue" id="categoryIlue_${rowSequence}" readonly /></td>
                                <td>
                                    <input type="text" class="form-control" name="denoms" readonly/>
                                </td>
                                <td>
                                    <input type="number" class="form-control" name="vegscaleIlue" min="0" step="0.00000001" />
                                </td>
                                <td>
                                    <input type="number" class="form-control" name="nonvegscaleIlue" min="0" step="0.00000001" />
                                </td>
                                <td>
                                    <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
            fetchInLieuItemsForRow(rowSequence);

            var hiddenCategory = document.getElementById("categoryVal");
            var caregoryField = document.getElementById("categoryIlue_" + rowSequence);
            caregoryField.value = hiddenCategory.value;


            rowSequence++;
        }

        function fetchBasicDenom() {
            var basicItemValue = document.getElementById('<%= basicItem.ClientID %>').value;

            fetch('ItemMaster.aspx/GetBasicDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ basicItem: basicItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {
                        var denomsDropdown = document.getElementById("denomsVal");
                        denomsDropdown.value = data.d;
                    }
                    fetchInLieuItemsForRow('');
                })
                .catch(error => {
                    console.error('Error fetching basic denomination:', error);
                });
        }

        function fetchInLieuDenom(val) {
            fetch('ItemMaster.aspx/GetDenominationForInLieuItem', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ inlieuItem: val })
            })
                .then(response => response.json())
                .then(data => {
                    var idVal = "";
                    if (Id == null || Id == '') {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    } else {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal_${rowSequence}`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    }

                    if (data.d && Array.isArray(data.d)) {
                        data.d.forEach(function (item) {
                            var option = document.createElement("option");
                            option.value = item;
                            option.text = item;
                            inlieuItemDropdown.add(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching in-lieu items:', error);
                });
        }

        function changeCategory(val) {
            var hiddenCategory = document.getElementById("categoryVal");
            hiddenCategory.value = val;
            var caregoryField = document.getElementById("categoryIlue");
            caregoryField.value = val;
        }

        function fetchInLieuItemsForRow(rowSequence) {
            var basicItemValue = document.getElementById('<%= basicItem.ClientID %>').value;

            fetch('ItemMaster.aspx/GetInLieuItems', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ basicItem: basicItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    var inlieuItemDropdown = "";
                    if (rowSequence == null || rowSequence == '') {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    } else {
                        inlieuItemDropdown = document.getElementById(`inlieuItemVal_${rowSequence}`);
                        inlieuItemDropdown.innerHTML = '<option value="">Select</option>';
                    }

                    if (data.d && Array.isArray(data.d)) {
                        data.d.forEach(function (item) {
                            var option = document.createElement("option");
                            option.value = item;
                            option.text = item;
                            inlieuItemDropdown.add(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching in-lieu items:', error);
                });
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
