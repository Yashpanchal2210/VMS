<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="OtherShips.aspx.cs" Inherits="VMS_1.OtherShips" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="">Other Ships</h2>
        <form id="otherShipsForm" runat="server">
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Item Name</th>
                            <th class="heading">Denom</th>
                            <th class="heading">Qty</th>
                            <th class="heading">Reference</th>
                            <th class="heading">Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" name="date" class="form-control" />
                            </td>
                            <td>
                                <select class="form-control" name="itemname" id="itemname" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control" name="denom" id="denom">
                                    <option value="">Select</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Other">Other</option>
                                </select>
                            </td>
                            <td>
                                <input type="number" name="qty" id="qty" class="form-control" />
                            </td>
                            <td>
                                <input type="text" name="refno" id="refno" class="form-control" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="text-left">
                <h5>Upload CRV</h5>
                <label class="col-form-label" for="fileDate">Date</label>
                <input type="month" class="form-control " name="fileDate" id="fileDate" style="width: auto;" />

                <label class="col-form-label" for="FileUpload1">File</label>
                <asp:FileUpload CssClass="form-control mt-2" Width="20%" ID="FileUpload1" runat="server" ToolTip="Select Only Excel File" />

                <asp:Button CssClass="btn btn-dark mt-2" Width="10%" ID="Button1" runat="server" Text="Upload" OnClick="UploadFileButton_Click" />
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
                <asp:GridView ID="GridViewOtherShips" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>
            </div>
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script type="text/javascript">

        $(document).ready(function () {
            fetchItems('');
        });

        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `
             <td>
                <input type="date" name="date" class="form-control" />
            </td>
             <td>
                <select class="form-control itemname" name="itemname" id="itemname_${rowSequence}" required>
                    <option value="">Select</option>
                </select>
            </td>
            <td>
                <select class="form-control" name="denom" id="denom">
                    <option value="">Select</option>
                    <option value="Kgs">Kgs</option>
                    <option value="Ltr">Ltr</option>
                    <option value="Nos">Nos</option>
                    <option value="Other">Other</option>
                </select>
            </td>
            <td>
                <input type="number" name="qty" id="qty" class="form-control" />
            </td>
            <td>
                <input type="text" name="refno" id="refno" class="form-control" />
            </td>
            <td>
                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
            </td>`;
            tableBody.appendChild(newRow);
            fetchItems(newRow);
            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function fetchItems(row) {
            fetch('Divers_ExtraIssue.aspx/GetItemNames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        if (row) {
                            itemSelect = row.querySelector('.itemname');
                        } else {
                            itemSelect = document.getElementById('itemname');
                        }


                        // Clear existing options
                        itemSelect.innerHTML = '<option value="">Select</option>';

                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item.Value;
                            option.textContent = item.Text;
                            itemSelect.appendChild(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }

    </script>
</asp:Content>
