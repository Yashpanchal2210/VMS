<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Page08.aspx.cs" MasterPageFile="~/Site.Master" Inherits="VMS_1.Page08" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Page-08 Report</h2>
        <form id="PageForm" runat="server">
            <asp:Button ID="ExportToExcelButton" runat="server" Text="Download Report" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" BackColor="#009900" />
        </form>
    </div>
</asp:Content>
