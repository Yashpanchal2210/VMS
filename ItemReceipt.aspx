<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="ItemReceipt.aspx.cs" Inherits="VMS_1.ItemReceipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



    <div class="container">
        <h2 class="mt-4">Receipt Module</h2>

        <form id="receiptForm" runat="server">
            <%--<div class="text-right">
                <asp:LinkButton ID="DashboardButton" runat="server" Text="Go to Dashboard" CssClass="btn btn-info" PostBackUrl="~/Dashboard.aspx"></asp:LinkButton>
            </div>--%>
            <div class="table-responsive">
                <table class="table" id="myTable">
                    <thead>
                        <tr>
                            <th class="heading itemname">Item Name</th>
                            <th class="heading qty">Quantity</th>
                            <th class="heading denom">Denomination</th>
                            <th class="heading rcvdfrom">Received From</th>
                            <th class="heading ref">Reference/CRV No</th>
                            <th class="heading date">Date</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <select class="form-control itemname" id="itemname" name="itemname" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="qty" required pattern="^\d+(\.\d+)?$" />
                            </td>
                            <td>
                                <select class="form-control" name="denom" required>
                                    <option value="">Select Denom</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Others">Others</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control" name="rcvdfrom" required onchange="checkReceivedFrom(this)">
                                    <option value="">Select </option>
                                    <option value="BV Yard">BV Yard</option>
                                    <option value="Local Purchase">Local Purchase</option>
                                    <option value="Other Ship">Other Ship</option>
                                    <option value="Others">Others</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="ref" required />
                            </td>
                            <td>
                                <input type="date" class="form-control" name="date" required />
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <%--<div class="text-left">
                <asp:LinkButton ID="item" runat="server" Text="Go to Item Master" CssClass="btn btn-info" PostBackUrl="~/ItemMaster.aspx"></asp:LinkButton>
            </div>--%>
            <div>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </div>
            <div class="text-center">
                <button type="button" class="btn btn-primary mr-2" onclick="addRow()">Add Row</button>
                <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" CssClass="btn btn-success mr-2" Width="107px" Height="38px" />
            </div>
            <div>
                <h2 class="mt-4">Entered Data</h2>
            </div>
            <div class="form-group">
                <label for="monthYear">Select Month and Year:</label>
                <input type="month" id="monthYear" name="monthYear" class="form-control" onchange="filterData()" /><br />
                &nbsp;
            </div>
            <asp:GridView ID="GridView" runat="server" CssClass="table table-bordered table-striped">
            </asp:GridView>
        </form>
    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.1/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.1/js/bootstrap.min.js"></script>
    <script>
        $(document).ready(function () {
            loadItemNamesForRow('');
        });
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
        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `
                                 <td>
                                      <select class="form-control itemname" name="itemname" id="itemname_${rowSequence}" required>
                                     </select>
                                 </td>
                                <td><input type="text" class="form-control" name="qty" required pattern="^\\d+(\\.\\d+)?$" /></td>
                                <td><select class="form-control" name="denom" required>
                                    <option value="">Select Denom</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Others">Others</option>
                                </select></td>
                                <td><select class="form-control" name="rcvdfrom" required onchange="checkReceivedFrom(this)">
                                    <option value="">Select </option>
                                    <option value="BV Yard">BV Yard</option>
                                    <option value="Local Purchase">Local Purchase</option>
                                    <option value="Other Ship">Other Ship</option>
                                    <option value="Others">Others</option>
                                </select></td>
                                <td><input type="text" class="form-control" name="ref" required /></td>
                                <td><input type="date" class="form-control" name="date" required /></td>
                                <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);
            loadItemNamesForRow(newRow);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        function loadItemNamesForRow(row) {
            fetch('ItemReceipt.aspx/GetItemNames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        var itemSelect = '';
                        var int = rowSequence - 1;
                        if (rowSequence > 0) {
                            itemSelect = document.getElementById('itemname_' + int);
                        } else {
                            itemSelect = document.getElementById('itemname');
                        }
                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item;
                            option.textContent = item;
                            itemSelect.appendChild(option);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }

        function checkReceivedFrom(selectElement) {
            if (selectElement.value === 'Others') {
                var parentTd = selectElement.parentNode;
                var othersInput = document.createElement('input');
                othersInput.type = 'text';
                othersInput.name = 'otherReceivedFrom';
                othersInput.className = 'form-control mt-2';
                othersInput.placeholder = 'Specify other source';
                parentTd.appendChild(othersInput);
            } else {
                var parentTd = selectElement.parentNode;
                var othersInput = parentTd.querySelector('input[name="otherReceivedFrom"]');
                if (othersInput) {
                    parentTd.removeChild(othersInput);
                }
            }
        }

        document.addEventListener("DOMContentLoaded", function () {
            var rows = document.querySelectorAll("#tableBody tr");
            rows.forEach(loadItemNamesForRow);
        });

        function filterData() {
            var monthYearInput = document.getElementById('monthYear');
            var monthYearValue = monthYearInput.value;

            if (monthYearValue) {
                var form = document.getElementById('receiptForm');
                var formData = new FormData(form);

                fetch('ItemReceipt.aspx/GetFilteredData', {
                    method: 'POST',
                    body: JSON.stringify({ monthYear: monthYearValue }),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                    .then(response => response.json())
                    .then(data => {
                        if (data.d && data.d.length) {
                            var gridView = document.getElementById('<%= GridView.ClientID %>');
                            gridView.innerHTML = '';

                            var table = document.createElement('table');
                            table.className = 'table table-bordered table-striped';

                            var thead = document.createElement('thead');
                            var theadRow = document.createElement('tr');
                            ['Item Name', 'Quantity', 'Denomination', 'Received From', 'Reference No', 'Date'].forEach(function (heading) {
                                var th = document.createElement('th');
                                th.textContent = heading;
                                theadRow.appendChild(th);
                            });
                            thead.appendChild(theadRow);
                            table.appendChild(thead);

                            var tbody = document.createElement('tbody');
                            data.d.forEach(function (row) {
                                var tr = document.createElement('tr');
                                row.forEach(function (cell) {
                                    var td = document.createElement('td');
                                    td.textContent = cell;
                                    tr.appendChild(td);
                                });
                                tbody.appendChild(tr);
                            });
                            table.appendChild(tbody);

                            gridView.appendChild(table);
                        }
                    })
                    .catch(error => {
                        console.error('Error fetching filtered data:', error);
                    });
            }
        }
    </script>

</asp:Content>
