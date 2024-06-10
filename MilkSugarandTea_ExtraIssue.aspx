<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="MilkSugarandTea_ExtraIssue.aspx.cs" Inherits="VMS_1.MilkSugarandTea_ExtraIssue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <h2 class="">Pest Control - Extra Issue</h2>
    </div>
    <form id="milkSugarTeaForm" runat="server">
        <div class="table-responsive">
            <table class="table" id="myTable">
                <thead>
                    <tr>
                        <th class="heading">Date</th>
                        <th class="heading">Strength</th>
                        <th class="heading">Tea</th>
                        <th class="heading">Milk Fresh</th>
                        <th class="heading">Sugar</th>
                        <th class="heading">Action</th>
                    </tr>
                </thead>
                <tbody id="tableBody" runat="server">
                    <tr>
                        <td>
                            <input type="date" name="date" class="form-control" />
                        </td>
                        <td>
                            <input type="number" name="strength" id="strength" class="form-control" />
                        </td>
                        <td>
                            <input type="number" name="tea" id="tea" class="form-control" readonly />
                        </td>
                        <td>
                            <input type="number" name="milk" id="milk" class="form-control" readonly />
                        </td>
                        <td>
                            <input type="number" name="sugar" id="sugar" class="form-control" readonly />
                        </td>
                        <td>
                            <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="text-center">
            <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
            <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
        </div>
        <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>

        <div>
            <h2 class="mt-4">Entered Data</h2>
        </div>
        <div>
            <asp:GridView ID="GridViewExtraIssueCategory5" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </div>
    </form>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script type="text/javascript">

        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `
                 <td>
                    <input type="date" name="date" class="form-control" />
                </td>
                <td>
                    <input type="number" name="strength" id="strength_${rowSequence}" class="form-control" />
                </td>
                <td>
                    <input type="number" name="tea" id="tea_${rowSequence}" class="form-control" readonly />
                </td>
                <td>
                    <input type="number" name="milk" id="milk_${rowSequence}" class="form-control" readonly />
                </td>
                <td>
                    <input type="number" name="sugar" id="sugar_${rowSequence}" class="form-control" readonly />
                </td>
                <td>
                    <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                </td>`;
            tableBody.appendChild(newRow);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        $(document).on('input', 'input[name="strength"]', function () {

            var strengthValue = parseFloat($(this).val());

            if (!isNaN(strengthValue)) {
                var calTea = (strengthValue * 0.007).toFixed(3);
                var calMilk = (strengthValue * 0.070).toFixed(3);
                var calSugar = (strengthValue * 0.030).toFixed(3);

                if (rowSequence == 0) {
                    $('#tea').val(calTea);
                    $('#milk').val(calMilk);
                    $('#sugar').val(calSugar);
                }
                if (rowSequence > 0) {
                    var rowInt = rowSequence - 1;
                    $('#tea_' + rowInt).val(calTea);
                    $('#milk_' + rowInt).val(calMilk);
                    $('#sugar_' + rowInt).val(calSugar);
                }

            }
        });

    </script>
</asp:Content>
