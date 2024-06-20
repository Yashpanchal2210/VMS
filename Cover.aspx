<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="COVER.aspx.cs" Inherits="VMS_1.Page1" MasterPageFile="~/Site.master"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="container">
        <h2 class="mt-4">Cover Page</h2>
        <form id="PageForm" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" required class="form-control date-picker" style="width: 21%" />
            </div>
            <asp:Button ID="ExportToExcelButton" runat="server" Text="Download Report" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" BackColor="#009900" />
        </form>
    </div>
</asp:Content>