<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ReportsGenerate.aspx.cs" Inherits="VMS_1.ReportsGenerate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Reports Generate</h2>
        <h5 class="mt-4">Export Issue</h5>
        <div class="form-group">
            <label for="monthYearPicker">Select Month and Year:</label>
            <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" />
        </div>
        <asp:Button ID="ExportToExcelButton" runat="server" Text="Export to Excel" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" />
    </div>
</asp:Content>
