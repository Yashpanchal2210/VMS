<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VictuallingManagement.aspx.cs" Inherits="VMS_1.VictuallingManagement" MasterPageFile="~/Site.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .table-container {
            display: flex;
            overflow-x: auto;
            white-space: nowrap;
        }
    </style>
    <div class="container">
        <h2 class="mt-4">Victualling Account</h2>
        <form id="PageForm" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" required class="form-control date-picker" style="width: 21%" />
            </div>
            <asp:Button ID="ExportToExcelButton" runat="server" Text="View" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" BackColor="#009900" />
        </form>

        <div id="tablesContainer" runat="server">
            <!-- Tables will be displayed here -->
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
