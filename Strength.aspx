<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Strength.aspx.cs" Inherits="VMS_1.Strength" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Strength Module</h2>

        <form id="strengthForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
            </div>--%>
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading date">Date</th>
                            <th class="heading veg-officer">Veg Officer</th>
                            <th class="heading non-veg-officer">Non-Veg Officer</th>
                            <th class="heading rik-officer">RIK Officer</th>
                            <th class="heading veg-staff">Veg Staff</th>
                            <th class="heading non-veg-staff">Non-Veg Staff</th>
                            <th class="heading rik-staff">RIK Staff</th>
                            <th class="heading non-entitled-officer">Non-Entitled Officer</th>
                            <th class="heading non-entitled-staff">Non-Entitled Staff</th>
                            <th class="heading civilian">Civilian</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" class="form-control" name="date" required /></td>
                            <td>
                                <input type="number" class="form-control" name="vegOfficer" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="nonVegOfficer" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="rikOfficer" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="vegStaff" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="nonVegStaff" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="rikStaff" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="nonEntitledOfficer" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="nonEntitledStaff" required min="0" /></td>
                            <td>
                                <input type="number" class="form-control" name="civilian" required min="0" /></td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />

            </div>
            <div>
                <h2 class="mt-4">Strength Data</h2>

            </div>
            <div>
                <asp:GridView ID="GridViewStrength" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>
            </div>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function setTheme(theme) {
            var gridView = document.getElementById("GridView2");

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

        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td><input type="date" class="form-control" name="date" required /></td>
                                <td><input type="number" class="form-control" name="vegOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonVegOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="rikOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="vegStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonVegStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="rikStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonEntitledOfficer" required min="0" /></td>
                                <td><input type="number" class="form-control" name="nonEntitledStaff" required min="0" /></td>
                                <td><input type="number" class="form-control" name="civilian" required min="0" /></td>
                                <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
        }

        function deleteRow(btn) {
            var row = btn.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }
    </script>
</asp:Content>
