<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="IssueMaster.aspx.cs" Inherits="VMS_1.IssueMaster" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <style>
        .input-container {
            position: relative;
        }

            .input-container input {
                padding-right: 30px; /* Adjust padding to accommodate the icon */
            }

            .input-container .fa-info-circle {
                position: absolute;
                right: 10px;
                top: 50%;
                transform: translateY(-50%);
            }
    </style>
    <div class="container">

        <h2 class="mt-4">Daily Issue Register</h2>

        <form id="issueForm" runat="server">
            <div class="mb-3">
                <%--<label for="userType">Select User Type:</label>--%>
                <select id="userType" name="userrole" class="form-control" onchange="selectCategory(this.value, '')" style="width: 150px;">
                    <option value="" selected>Select</option>
                    <option value="Wardroom">Wardroom</option>
                    <option value="Galley">Galley</option>
                </select>
                <span class="text-danger">PLEASE SELECT TYPE FIRST</span>
            </div>
            <input type="hidden" id="ScalAmount_Val" />
            <input type="hidden" id="ItemCategory_Val" />
            <input type="hidden" id="entitledStrength" />
            <input type="hidden" id="EntitledStrength" name="EntitledStrength" />
            <input type="hidden" id="scaleAmount" />
            <input type="hidden" id="Selected_Category" name="Category" />

            <div class="table-responsive">
                <table class="table" id="issueTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Item Name</th>
                            <th class="heading">Denomination</th>
                            <th class="heading">Entitled Strength</th>
                            <th class="heading">Qty Issued</th>
                            <th class="heading"></th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" class="form-control" name="date" required /></td>
                            <td>
                                <select class="form-control js-states" id="DropDownList1" name="itemname" onchange="fetchBasicDenom(this.id)" width="130px" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal" name="denoms" readonly />
                            </td>
                            <td>
                                <div class="input-container">
                                    <input type="text" class="form-control" name="Strength" />
                                    <i class="fas fa-info-circle" data-toggle="modal" data-target="#infoModal"></i>
                                </div>
                            </td>
                            <td>
                                <input type="text" class="form-control" name="Qtyissued" /></td>

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
                <h2 class="mt-4">Issue Details</h2>
            </div>
            <%--<div>
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
            </div>--%>
            <div>
                <a class="btn" href="ExportIssueOfficerandSailor.aspx">Export Issue</a>
            </div>
            <div>
                <asp:GridView ID="GridViewIssue" runat="server" CssClass="table table-bordered table-striped" AutoGenerateColumns="False">
                    <Columns>
                        <asp:TemplateField HeaderText="Date">
                            <ItemTemplate>
                                <asp:Label ID="lblDate" runat="server" Text='<%# Eval("Date") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="lblDate" runat="server" Text='<%# Bind("Date") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <%--<asp:TemplateField HeaderText="Category">
                            <ItemTemplate>
                                <asp:Label ID="lblCategory" runat="server" Text='<%# Eval("CategoryItemName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="lblCategory" runat="server" Text='<%# Bind("CategoryItemName") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>--%>
                        <asp:TemplateField HeaderText="Item">
                            <ItemTemplate>
                                <asp:Label ID="lblItem" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtItem" runat="server" Text='<%# Bind("ItemName") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Qty Issued">
                            <ItemTemplate>
                                <asp:Label ID="lblQtyIssued" runat="server" Text='<%# Eval("QtyIssued") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtQtyIssued" runat="server" Text='<%# Bind("QtyIssued") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Denomination">
                            <ItemTemplate>
                                <asp:Label ID="lblDenomination" runat="server" Text='<%# Eval("Denomination") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDenomination" runat="server" Text='<%# Bind("Denomination") %>'></asp:TextBox>
                            </EditItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
            </div>


            <!-- Modal -->
            <div class="modal fade" id="entitledStrengthModal" tabindex="-1" role="dialog" aria-labelledby="entitledStrengthModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="entitledStrengthModalLabel">Entitled Strength</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">

                            <p><span class="font-weight-bolder" id="entitledStrengthValue"></span>Quantity is Entitled for given strength.</p>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="infoModal" tabindex="-1" role="dialog" aria-labelledby="infoModalLabel" aria-hidden="true">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="infoModalLabel">Entitled Strength Info</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Kindly enter approximate strength for which the item is to be issue * number of days 
           
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
            </div>


        </form>

    </div>

    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.1/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.1/js/bootstrap.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
    <script>
        $(document).ready(function () {
            $('#DropDownList1').select2({
                placeholder: 'Select an option',
                allowClear: true
            });
        });

        function selectCategory(val, row) {

            var categoryVal = document.getElementById("Selected_Category").value;

            if (categoryVal == null || categoryVal == "") {
                document.getElementById("Selected_Category").value = val;
            }

            fetch('IssueMaster.aspx/GetInLiueItemsByCategory', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ Category: document.getElementById("Selected_Category").value })
            })
                .then(response => response.json())
                .then(data => {
                    const items = JSON.parse(data.d);

                    if (row == "" || row == null) {
                        const selectElement = document.getElementById('DropDownList1');
                        selectElement.innerHTML = '<option value="">Select</option>';

                        for (let i = 0; i < items.length; i += 2) {
                            const optionText = items[i];
                            const optionValue = items[i + 1];
                            if (optionText && optionValue) {
                                const option = document.createElement('option');
                                option.text = optionText;
                                option.value = optionValue;
                                selectElement.appendChild(option);
                            }
                        }
                        selectElement.disabled = false;
                    } else {
                        const selectElement1 = document.getElementById('DropDownList1_' + row);
                        selectElement1.innerHTML = '<option value="">Select</option>';

                        for (let i = 0; i < items.length; i += 2) {
                            const optionText = items[i];
                            const optionValue = items[i + 1];
                            if (optionText && optionValue) {
                                const option = document.createElement('option');
                                option.text = optionText;
                                option.value = optionValue;
                                selectElement1.appendChild(option);
                            }
                        }
                        selectElement1.disabled = false;
                    }

                    // Enable the select element after populating it
                    
                })
                .catch(error => {
                    console.error('Error fetching Items:', error);
                });
        }

        function fetchBasicDenom(id) {
            var ItemValue = document.getElementById(id).value;
            document.getElementById("scaleAmount").value = "";

            if (id != null) {
                var value = id;  // Use 'var' instead of 'string'
                var parts = value.split('_');  // Use JavaScript's 'split' method
            }

            var part1 = parts[1];

            fetch('IssueMaster.aspx/GetItemDenom', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ ItemVal: ItemValue })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d) {

                        var result = JSON.parse(data.d);
                        var denomination = result.Denomination;

                        var vegScale = result.VegScale;
                        var nonvegeScale = result.NonVegScale;

                        var sumofScale = vegScale + nonvegeScale;
                        document.getElementById("scaleAmount").value = sumofScale;

                        if (rowSequence > 1) {
                            var denomsDropdown = document.getElementById("denomsVal_" + part1);
                            denomsDropdown.value = denomination;
                        } else {
                            var denomsDropdown = document.getElementById("denomsVal");
                            denomsDropdown.value = denomination;
                        }
                    }
                })
                .catch(error => {
                    console.error('Error fetching basic denomination:', error);
                });
        }

        $(document).on('change', 'input[name="Strength"]', function () {
            var scaleAmount = parseFloat($('#scaleAmount').val()).toFixed(4);
            var strengthValue = parseFloat($(this).val());

            if (!isNaN(strengthValue)) {
                var entitledStrength = strengthValue * scaleAmount;
                $('#entitledStrengthValue').text(entitledStrength);
                $('#entitledStrengthModal').modal('show');
                //$(this).closest('tr').find('.EntitledStrength').val(entitledStrength);
            } else {
                $('#entitledStrengthValue').text("Please enter a valid strength value.");
                $('#entitledStrengthModal').modal('show');
                //$(this).closest('tr').find('.EntitledStrength').val('');
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

            freezDiv();

            var selectedDate = document.querySelector('input[type="date"][name="date"]').value;
            newRow.innerHTML = `
                            <input type="hidden" id="EntitledStrength_${rowSequence}" name="EntitledStrength" />
                            <td>
                                <input type="date" class="form-control" name="date" value="${selectedDate}" disabled required />
                            </td>
                            <td>
                                <select id="DropDownList1_${rowSequence}" class="form-control" onchange="fetchBasicDenom(this.id)" name="itemname" required>
                                    <option value="">Select</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control" id="denomsVal_${rowSequence}" name="denoms" readonly />
                            </td>
                            <td>
                                <input type="text" class="form-control strength-input" name="Strength" id="Strength_${rowSequence}" />
                            </td>
                            <td>
                                <input type="text" class="form-control" name="Qtyissued" id="Qtyissued_${rowSequence}" />
                            </td>
                            <td>
                                <button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button>
                            </td>`;

            tableBody.appendChild(newRow);

            selectCategory('', rowSequence);

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
            //fetchItemNames(getCategory, rowSequence);

            $("#itemcategory_" + rowSequence).append($('#itemcategory').html())
            var getCategory = $("#ItemCategory_Val").val();
            $('.itemcategory').val(getCategory);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }

        //document.addEventListener("DOMContentLoaded", function () {
        //    var rows = document.querySelectorAll("#tableBody tr");
        //    rows.forEach(fetchItemCategories);
        //});

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

        var scaleAmountsByCategory = {};
        //function fetchItemCategories() {
        //    fetch('IssueMaster.aspx/GetItemCategories', {
        //        method: 'POST',
        //        headers: {
        //            'Content-Type': 'application/json'
        //        }
        //    })
        //        .then(response => response.json())
        //        .then(data => {
        //            if (data.d && data.d.length) {

        //                var dropdown = document.getElementById('itemcategory');

        //                // Clear existing options
        //                dropdown.innerHTML = '<option value="">Select</option>';

        //                data.d.forEach(function (item) {
        //                    var option = document.createElement('option');
        //                    option.value = item.Value;
        //                    option.textContent = item.Text;
        //                    option.setAttribute('data-scaleamount', item.ScaleAmount);
        //                    dropdown.appendChild(option);

        //                    scaleAmountsByCategory[item.Value] = parseFloat(item.ScaleAmount);
        //                });
        //            }
        //        })
        //        .catch(error => {
        //            console.error('Error fetching item categories:', error);
        //        });
        //}


        function populateDropDown(data) {
            var dropdown = $('#itemcategory');
            dropdown.empty();
            $.each(data, function (key, entry) {
                dropdown.append($('<option></option>').attr('value', entry.Value).text(entry.Text));
            });
        }
            var selectedCategoryValue = '';

        //function itemcategory_SelectedIndexChanged(element) {
        //    selectedCategoryValue = element.value;
        //    document.querySelector('input[name="Strength"]').value = '';
        //    document.getElementById('EntitledStrength').textContent = '';
        //    var selectedCategory = document.getElementById('itemcategory').value;
        //    //var selectedCategory = document.querySelector('.itemcategory').value;
        //    var ScalAmount = document.getElementById('ScalAmount_Val');
        //    $('#ItemCategory_Val').val(selectedCategory);
        //    fetchItemNames(selectedCategory, '');

        //    var scaleAmount = scaleAmountsByCategory[selectedCategory];
        //    ScalAmount.value = scaleAmount;
        //}

        //function fetchItemNames(category, val) {
        //    fetch('IssueMaster.aspx/GetItemNamesByCategory', {
        //        method: 'POST',
        //        headers: {
        //            'Content-Type': 'application/json'
        //        },
        //        body: JSON.stringify({ category: category })
        //    })
        //        .then(response => response.json())
        //        .then(data => {
        //            if (val == '' || val == null) {
        //                var dropdown = document.getElementById('DropDownList1');
        //            } else {
        //                var dropdown = document.getElementById('DropDownList1' + '_' + val);
        //            }
        //            dropdown.innerHTML = '';
        //            var itemNames = JSON.parse(data.d);
        //            itemNames.forEach(function (itemName) {
        //                var option = document.createElement('option');
        //                option.value = itemName;
        //                option.textContent = itemName;
        //                dropdown.appendChild(option);
        //            });
        //        })
        //        .catch(error => {
        //            console.error('Error fetching item names:', error);
        //        });
        //}

        function freezDiv() {
            var type = document.getElementById("userType");
            type.disabled = true;
        }
    </script>
</asp:Content>
