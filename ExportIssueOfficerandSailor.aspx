<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ExportIssueOfficerandSailor.aspx.cs" Inherits="VMS_1.ExportIssueOfficer_Sailor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" runat="server">
        <div class="container">
            <h2 class="mt-4">Issue Master Officer and Sailor</h2>
            <p class="mt-4">&nbsp;</p>
            <p class="mt-4">&nbsp;</p>
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" class="form-control date-picker" />
            </div>
            <asp:Button ID="ExportToExcelButton" runat="server" Text="Export to Excel" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" />
        </div>
        <div>
            <asp:Label ID="lblStatus" runat="server"></asp:Label>
        </div>
    </form>
</asp:Content>
