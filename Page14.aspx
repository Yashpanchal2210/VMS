<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Page14.aspx.cs" Inherits="VMS_1.Page14" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Page-14 Report</h2>
        <form id="PageForm" runat="server">
            <div class="form-group">
                <label for="monthYearPicker">Select Month and Year:</label>
                <input type="month" id="monthYearPicker" runat="server" required class="form-control date-picker" style="width: 21%" />

                <label class="col-form-label">Commanding Officer:</label>
                <select class="form-control" id="coVal" name="coVal">
                    <option value="">Select</option>
                </select>

                <label class="col-form-label">Logistic Officer:</label>
                <select class="form-control" id="loVal" name="loVal">
                    <option value="">Select</option>
                </select>

                <label class="col-form-label">Accounting Officer:</label>
                <select class="form-control" id="aoVal" name="aoVal">
                    <option value="">Select</option>
                </select>
            </div>
            <asp:Button ID="ExportToExcelButton" runat="server" Text="Download Report" OnClick="ExportToExcelButton_Click" CssClass="btn btn-primary" BackColor="#009900" />
        </form>
    </div>

    <script>
        window.onload = function () {
            getCOData();
        };

        function getCOData() {
            fetch('Page14.aspx/GetCONames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        var itemSelect = document.getElementById('coVal');

                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item;
                            option.textContent = item;
                            itemSelect.appendChild(option);
                        });
                        getLOData();
                    }
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }

        function getLOData() {
            fetch('Page14.aspx/GetLONames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        var itemSelect = document.getElementById('loVal');

                        data.d.forEach(function (item) {
                            var option = document.createElement('option');
                            option.value = item;
                            option.textContent = item;
                            itemSelect.appendChild(option);
                        });
                        getAOData();
                    }
                })
                .catch(error => {
                    console.error('Error fetching item names:', error);
                });
        }

        function getAOData() {
            fetch('Page14.aspx/GetAONames', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.d && data.d.length) {
                        var itemSelect = document.getElementById('aoVal');

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
    </script>
</asp:Content>
