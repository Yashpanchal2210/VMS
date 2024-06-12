<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="UnapprovedUsers.aspx.cs" Inherits="VMS_1.UnapprovedUsers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Unapproved Users List</h2>

        <form id="usersForm" runat="server">
            <asp:GridView ID="GridViewUser" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False" OnRowCommand="GridViewUser_RowCommand" Width="100%">
                <Columns>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("name") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Rank">
                        <ItemTemplate>
                            <asp:Label ID="lblRank" runat="server" Text='<%# Eval("rank") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtRank" runat="server" Text='<%# Bind("rank") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Designation">
                        <ItemTemplate>
                            <asp:Label ID="lblDesignation" runat="server" Text='<%# Eval("designation") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDesignation" runat="server" Text='<%# Bind("designation") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Nuid">
                        <ItemTemplate>
                            <asp:Label ID="lblNuid" runat="server" Text='<%# Eval("NudId") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="txtNuid" runat="server" Text='<%# Bind("NudId") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Role">
                        <ItemTemplate>
                            <asp:Label ID="lblRole" runat="server" Text='<%# Eval("role") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                                <asp:ListItem Text="User" Value="User"></asp:ListItem>
                                <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                                <asp:ListItem Text="Logistic Officer" Value="Logistic Officer"></asp:ListItem>
                                <asp:ListItem Text="CO" Value="CO"></asp:ListItem>
                            </asp:DropDownList>
                        </EditItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Approval">
                        <ItemTemplate>
                            <asp:Button ID="btnApprove" runat="server" Text="Approve" CommandName="Approve" CommandArgument='<%# Eval("NudId") %>' CssClass="btn btn-success" />
                            <asp:Button ID="btnReject" runat="server" Text="Reject" CommandName="Reject" CommandArgument='<%# Eval("NudId") %>' CssClass="btn btn-danger" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </form>
    </div>

    <script type="text/javascript" src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script type="text/javascript" src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script type="text/javascript">
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
    </script>
</asp:Content>
