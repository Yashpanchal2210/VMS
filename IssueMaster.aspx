<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="IssueMaster.aspx.cs" Inherits="VMS_1.IssueMaster" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">

        <h2 class="mt-4">Issue Module - Officers</h2>

        <form id="issueForm" runat="server">
            <div>
                <label for="userType">Select User Type:</label>
                <select id="userType" onchange="toggleUserType()" name="userrole" class="form-control" style="width: 100px;">
                    <option value="officer" selected>Officer</option>
                    <option value="sailor">Sailor</option>
                </select>
            </div>
            <input type="hidden" id="ScalAmount_Val" />
            <input type="hidden" id="ItemCategory_Val" />
            <input type="hidden" id="entitledStrength" name="entitledstrength" />
            <div class="table-responsive">
                <table class="table" id="issueTable">
                    <thead>
                        <tr>
                            <th class="heading date">Date</th>
                            <th class="heading category">Item Category</th>
                            <th class="heading itemname">Item Name</th>
                            <th class="heading issuedto">Enter Strength</th>
                            <th class="heading qtyentitled">Qty Entitled</th>
                            <th class="heading qty">Qty Issued</th>
                            <th class="heading denom">Denomination</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" class="form-control" name="date" required /></td>
                            <td>
                                <%--<asp:DropDownList runat="server" CssClass="form-control itemcategory" AutoPostBack="False" onchange="itemcategory_SelectedIndexChanged()" Width="130px">
                                    <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                </asp:DropDownList>--%>
                                <select class="form-control itemcategory" id="itemcategory" name="itemcategory" onchange="itemcategory_SelectedIndexChanged(this)" width="130px" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <%--<asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control" AutoPostBack="False" Width="130px">
                                    <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                </asp:DropDownList>--%>
                                <select class="form-control" id="DropDownList1" name="itemname" width="130px" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="Strength" />
                            </td>
                            <td>
                                <input type="text" id="EntitledStrength" class="form-control" readonly />
                                <%--<asp:Label ID="EntitledStrength" runat="server" Text="" class="form-control"></asp:Label>--%>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="Qtyissued" /></td>
                            <td>
                                <select class="form-control" name="denom" required>
                                    <option value="">Select Denom</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Others">Others</option>
                                </select>
                            </td>
                            <%--<td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>--%>
                        </tr>
                    </tbody>
                </table>
            </div>
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
            <div>
                <label for="ddlMonth">Select Month:</label>
                <asp:DropDownList ID="ddlMonth" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged">
                    <asp:ListItem Text="January" Value="1"></asp:ListItem>
                    <asp:ListItem Text="February" Value="2"></asp:ListItem>
                    <asp:ListItem Text="March" Value="3"></asp:ListItem>
                    <asp:ListItem Text="April" Value="4"></asp:ListItem>
                    <asp:ListItem Text="May" Value="5"></asp:ListItem>
                    <asp:ListItem Text="June" Value="6"></asp:ListItem>
                    <asp:ListItem Text="July" Value="7"></asp:ListItem>
                    <asp:ListItem Text="August" Value="8"></asp:ListItem>
                    <asp:ListItem Text="September" Value="9"></asp:ListItem>
                    <asp:ListItem Text="Octomber" Value="10"></asp:ListItem>
                    <asp:ListItem Text="November" Value="11"></asp:ListItem>
                    <asp:ListItem Text="December" Value="12"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div>
                <a class="btn" href="ExportIssueOfficerandSailor.aspx">Export Issue</a>
            </div>
            <div>
                <asp:GridView ID="GridViewIssue" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>
            </div>

        </form>

    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.1/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.1/js/bootstrap.min.js"></script>
    <script>
        $(document).ready(function () {
            fetchItemCategories();
            //if ($('#userRole').val() == null || $('#userRole').val() == "") {
            //    $('#userRole').val("officer")
            //}
        });
        function toggleUserType() {
            var userType = document.getElementById("userType").value;
            var heading = document.querySelector('.container h2');

            if (userType === "officer") {
                $('#userRole').val("officer");
                heading.textContent = "Issue Module - Officers";
            } else if (userType === "sailor") {
                $('#userRole').val("sailor");
                heading.textContent = "Issue Module - Sailors";
            }
        }

        $(document).on('input', 'input[name="Strength"]', function () {
            var scaleAmount = $('#ScalAmount_Val').val();
            var strengthValue = parseFloat($(this).val());

            if (!isNaN(strengthValue)) {
                var entitledStrength = strengthValue * scaleAmount;

                $(this).closest('tr').find('.EntitledStrength').val(entitledStrength);
            } else {
                $(this).closest('tr').find('.EntitledStrength').val('');
            }
        });

        document.querySelector('input[name="Strength"]').addEventListener('input', function () {

            var scaleAmount = $('#ScalAmount_Val').val();
            var strengthValue = parseFloat(document.querySelector('input[name="Strength"]').value);

            var strengthValue = parseFloat(this.value);

            if (!isNaN(strengthValue)) {
                var entitledStrength = strengthValue * scaleAmount;
                var id = 'EntitledStrength';
                $('#entitledStrength').val(entitledStrength);

                if (id != 'EntitledStrength' + '_' + rowSequence) {
                    document.getElementById('EntitledStrength').textContent = entitledStrength;
                    $('#EntitledStrength').val(entitledStrength);
                } else {
                    document.getElementById('EntitledStrength' + '_' + rowSequence).textContent = entitledStrength;
                }
            } else {
                document.getElementById('EntitledStrength').textContent = '';
            }
        });

        function setTheme(theme) {
            var gridView = document.getElementById("GridView3");
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

        // Global variable to keep track of sequence number
        var rowSequence = 1;

        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");

            var selectedDate = document.querySelector('input[type="date"][name="date"]').value;
            newRow.innerHTML = `
                            <td>
                                <input type="date" class="form-control" name="date" value="${selectedDate}" disabled required />
                            </td>
                            <td>
                                 <select class="form-control itemcategory" id="itemcategory_${rowSequence}" onchange="itemcategory_SelectedIndexChanged(this)" disabled required>
                                </select>
                            </td>
                            <td>
                                <select id="DropDownList1_${rowSequence}" class="form-control" name="itemname" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control strength-input" name="Strength" id="Strength_${rowSequence}" />
                            </td>
                            <td>
                                <input class="form-control entitled_strength" type="text" name="EntitledStrength" id="EntitledStrength_${rowSequence}" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control" name="Qtyissued" id="Qtyissued_${rowSequence}" />
                            </td>
                            <td>
                                <select class="form-control" name="denom" required id="denom_${rowSequence}">
                                    <option value="">Select Denom</option>
                                    <option value="Kgs">Kgs</option>
                                    <option value="Ltr">Ltr</option>
                                    <option value="Nos">Nos</option>
                                    <option value="Others">Others</option>
                                </select>
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                            </td>`;

            tableBody.appendChild(newRow);

            var strengthInput = document.getElementById("Strength_" + rowSequence);
            var strengthID = "Strength_" + rowSequence;
            strengthInput.addEventListener('input', function () {
                var scaleAmount = parseFloat($('#ScalAmount_Val').val());
                var strengthValue = parseFloat(this.value);

                if (!isNaN(strengthValue)) {
                    var entitledStrength = strengthValue * scaleAmount;
                    var entitledStrengthElement = document.getElementById(strengthID);
                    var int = rowSequence - 1;
                    document.getElementById('EntitledStrength_' + int).value = entitledStrength;
                } else {
                    document.getElementById('EntitledStrength_' + rowSequence).value = '';
                }
            });

            //fetchItemCategories();
            //fetchItemCategories(newRow);
            $("#itemcategory_" + rowSequence).append($('#itemcategory').html())

            var getCategory = $("#ItemCategory_Val").val();
            $('.itemcategory').val(getCategory);
            fetchItemNames(getCategory, rowSequence);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        document.addEventListener("DOMContentLoaded", function () {
            var rows = document.querySelectorAll("#tableBody tr");
            rows.forEach(fetchItemCategories);
        });

        function checkReceivedFrom(selectElement) {
            var parentTd = selectElement.parentNode;
            var othersInput = parentTd.querySelector('input[name="otherReceivedFrom"]');
            if (selectElement.value === 'Others') {
                if (!othersInput) {
                    othersInput = document.createElement('input');
                    othersInput.type = 'text';
                    othersInput.name = 'otherReceivedFrom';
                    othersInput.className = 'form-control mt-2';
                    othersInput.placeholder = 'Specify other source';
                    parentTd.appendChild(othersInput);
                }
            } else {
                if (othersInput) {
                    parentTd.removeChild(othersInput);
                }
            }
        }

        //function fetchItemCategories(row) {
        //    fetch('IssueMaster.aspx/GetItemCategories', {
        //        method: 'POST',
        //        headers: {
        //            'Content-Type': 'application/json'
        //        }
        //    })
        //        .then(response => response.json())
        //        .then(data => {
        //            if (data.d && data.d.length) {
        //                var itemSelect = row.querySelector('.itemcategory');

        //                data.d.forEach(function (item) {
        //                    var categoryVal = document.getElementById('ItemCategory_Val').value;
        //                    var option = document.createElement('option');
        //                    option.value = item.Value;
        //                    option.textContent = item.Text;
        //                    option.setAttribute('data-scaleamount', item.ScaleAmount);
        //                    if (item.Value === categoryVal) {
        //                        option.selected = true;
        //                    }
        //                    itemSelect.appendChild(option);

        //                    scaleAmountsByCategory[item.Value] = parseFloat(item.ScaleAmount);
        //                });
        //            }
        //        })
        //        .catch(error => {
        //            console.error('Error fetching item categories:', error);
        //        });
        //}
        var scaleAmountsByCategory = {};
        function fetchItemCategories() {
            fetch('IssueMaster.aspx/GetItemCategories', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {

                        var dropdown = document.getElementById('itemcategory');

                        // Clear existing options
                        dropdown.innerHTML = '<option value="">Select</option>';

                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item.Value;
                            option.textContent = item.Text;
                            option.setAttribute('data-scaleamount', item.ScaleAmount);
                            dropdown.appendChild(option);

                            scaleAmountsByCategory[item.Value] = parseFloat(item.ScaleAmount);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error fetching item categories:', error);
                });
        }


        function populateDropDown(data) {
            var dropdown = $('#itemcategory');
            dropdown.empty();
            $.each(data, function (key, entry) {
                dropdown.append($('<option></option>').attr('value', entry.Value).text(entry.Text));
            });
        }
        var selectedCategoryValue = '';

        function itemcategory_SelectedIndexChanged(element) {
            selectedCategoryValue = element.value;
            document.querySelector('input[name="Strength"]').value = '';
            document.getElementById('EntitledStrength').textContent = '';
            var selectedCategory = document.getElementById('itemcategory').value;
            //var selectedCategory = document.querySelector('.itemcategory').value;
            var ScalAmount = document.getElementById('ScalAmount_Val');
            $('#ItemCategory_Val').val(selectedCategory);
            fetchItemNames(selectedCategory, '');

            var scaleAmount = scaleAmountsByCategory[selectedCategory];
            ScalAmount.value = scaleAmount;
        }

        function fetchItemNames(category, val) {
            fetch('IssueMaster.aspx/GetItemNamesByCategory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ category: category })
            })
                .then(response => response.json())
                .then(data => {
                    if (val == '' || val == null) {
                        var dropdown = document.getElementById('DropDownList1');
                    } else {
                        var dropdown = document.getElementById('DropDownList1' + '_' + val);
                    }
                    dropdown.innerHTML = '';
                    var itemNames = JSON.parse(data.d);
                    itemNames.forEach(function (itemName) {
                        var option = document.createElement('option');
                        option.value = itemName;
                        option.textContent = itemName;
                        dropdown.appendChild(option);
                    });
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }
    </script>
</asp:Content>
