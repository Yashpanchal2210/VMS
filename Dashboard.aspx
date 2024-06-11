<%@ Page Title="Dashboard" Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Dashboard.aspx.cs" Inherits="VMS_1.Dashboard" %>

<%@ Import Namespace="System.Web.Security" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .issue-row {
            color: red;
        }
    </style>
    <form id="form1" runat="server">

        <div class="container">
            <h2>Month Stock</h2>
            <div class="form-group row">
                <label class="col-sm-2 col-form-label" for="ddlMonths">Select Month:</label>
                <div class="col-sm-4">
                    <asp:DropDownList ID="ddlMonths" runat="server" CssClass="form-control">
                        <asp:ListItem Text="January" Value="1"></asp:ListItem>
                        <asp:ListItem Text="February" Value="2"></asp:ListItem>
                        <asp:ListItem Text="March" Value="3"></asp:ListItem>
                        <asp:ListItem Text="April" Value="4"></asp:ListItem>
                        <asp:ListItem Text="May" Value="5"></asp:ListItem>
                        <asp:ListItem Text="June" Value="6"></asp:ListItem>
                        <asp:ListItem Text="July" Value="7"></asp:ListItem>
                        <asp:ListItem Text="August" Value="8"></asp:ListItem>
                        <asp:ListItem Text="September" Value="9"></asp:ListItem>
                        <asp:ListItem Text="October" Value="10"></asp:ListItem>
                        <asp:ListItem Text="November" Value="11"></asp:ListItem>
                        <asp:ListItem Text="December" Value="12"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-sm-2">
                    <asp:Button ID="Button1" runat="server" Text="Filter" CssClass="btn btn-primary" OnClick="btnFilter_Click" />
                </div>
            </div>
            <br />
            <asp:GridView ID="GridViewMonthStock" runat="server" CssClass="table table-bordered table-striped" OnRowDataBound="GridViewMonthStock_RowDataBound">
            </asp:GridView>
        </div>

        <div class="container">
            <h2>Page 2 To 7</h2>
            <asp:GridView ID="GridViewP2to7" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>

        <div class="container">
            <h2>Present Stock</h2>
            <asp:GridView ID="GridViewPresentStock" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
            <asp:Button ID="ExportPresentStockButton" runat="server" Text="Export Present Stock to Excel" OnClick="ExportPresentStockButton_Click" CssClass="btn btn-primary" />
        </div>

        <div class="container">
            <h2 class="mt-4">Export Issue</h2>
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" />
            </div>
            <asp:Button ID="ExportToExcelButton" runat="server" Text="Export to Excel" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" />
        </div>
        <asp:Label ID="lblStatus" runat="server"></asp:Label>

        <div class="container mt-3">
            <h2>Divers</h2>
            <asp:Button ID="ExportDiversStockButton" runat="server" Text="Export to Excel" OnClick="ExportDiversStockButton_Click" CssClass="btn btn-primary" />
            <asp:GridView ID="GridViewExtraIssueDivers" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>

        <div class="container mt-3">
            <h2>Officers</h2>
            <asp:Button ID="Button2" runat="server" Text="Export to Excel" OnClick="ExportOfficersButton_Click" CssClass="btn btn-primary" />
            <asp:GridView ID="GridViewOfficersSheet" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>

        <div class="container mt-3">
            <h2>Extra Issue</h2>
            <asp:Button ID="Button3" runat="server" Text="Export to Excel" OnClick="ExportExtraIssueButton_Click" CssClass="btn btn-primary" />
            <asp:GridView ID="GridViewExtraIssue" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>
    </form>


    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function addAlternativeItem() {
            var tableBody = document.getElementById("Table2");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `<td><input type="text" class="form-control" name="alternateitemname" /></td>
                                <td><input type="number" class="form-control" name = "equivalentofficerScale" required min="0" step="0.001"/></td>
                                <td><input type="number" class="form-control" name="equivalentsailorScale" required min="0" step="0.001"/></td>
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
