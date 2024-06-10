<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Divers_ExtraIssue.aspx.cs" Inherits="VMS_1.Divers_ExtraIssue" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <h2 class="mt-4">Extra Issue - Divers</h2>
        <form id="extraIssueForm" runat="server">
            <div class="table-responsive">
                <table class="table" id="extraissueTable">
                    <thead>
                        <tr>
                            <th class="heading">Date</th>
                            <th class="heading">Name</th>
                            <th class="heading">Rank</th>
                            <th class="heading">P.No.</th>
                            <th class="heading">NO. OF DAYS ENTILED</th>
                            <th class="heading">CHOCOLATE</th>
                            <th class="heading">HORLICKS</th>
                            <th class="heading">EGGS</th>
                            <th class="heading">MILK</th>
                            <th class="heading">G/NUT</th>
                            <th class="heading">BUTTER</th>
                            <th class="heading">SUGAR</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody" runat="server">
                        <tr>
                            <td>
                                <input type="date" class="form-control" name="date" required />
                            </td>
                            <td style="width: 10%;">
                                <input type="text" class="form-control" name="name" required style="text-transform: capitalize;" />
                            </td>
                            <td>
                                <select class="form-control rank" id="rank" name="rank" width="200px" required>
                                    <option value="" selected>Select</option>
                                    <option value="Sub Lieutenant">Sub Lieutenant</option>
                                    <option value="Lieutenant">Lieutenant</option>
                                    <option value="Lieutenant Commander">Lieutenant Commander</option>
                                    <option value="Commander">Commander</option>
                                    <option value="Captain">Captain</option>
                                    <option value="Commodore">Commodore</option>
                                    <option value="Rear Admiral">Rear Admiral</option>
                                    <option value="Vice Admiral">Vice Admiral</option>
                                    <option value="Admiral">Admiral</option>
                                    <option value="Seaman 2nd Class">Seaman 2nd Class</option>
                                    <option value="Seaman Ist Class">Seaman Ist Class</option>
                                    <option value="Leading Rate">Leading</option>
                                    <option value="Petty Officer">Petty Officer</option>
                                    <option value="Chief Petty Officer">Chief Petty Officer</option>
                                    <option value="Master Chief Petty Officer IInd Class">Master Chief Petty Officer IInd Class</option>
                                    <option value="Master Chief Petty Officer Ist Class">Master Chief Petty Officer Ist Class</option>
                                </select>
                            </td>
                            <td>
                                <input type="text" class="form-control pno" name="pno" required style="text-transform: capitalize;" />
                            </td>
                            <td>
                                <input type="text" class="form-control days" id="days" name="days" required />
                            </td>
                            <td>
                                <select class="form-control chocolate" id="chocolate" name="chocolate" required>
                                    <option value="" selected>Select</option>
                                    <option value="Chocolate (50 gms)">Chocolate (50 gms)</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control horlicks" id="horlicks" name="horlicks" required>
                                    <option value="" selected>Select</option>
                                    <option value="Complan/ Horlicks (50 gms)" selected>Complan/ Horlicks (50 gms)</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control eggs" id="eggs" name="eggs" required>
                                    <option value="" selected>Select</option>
                                    <option value="Eggs (2 Nos)">Eggs (2 Nos)</option>
                                    <option value="Milk Fresh (150 ml)">Milk Fresh (150 ml)</option>
                                    <option value="Milk Tinned (55 gms)">Milk Tinned (55 gms)</option>
                                    <option value="Milk Powder (20 gms)">Milk Powder (20 gms)</option>
                                    <option value="Cheese Tinned (50 gms)">Cheese Tinned (50 gms)</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control milk" id="milk" name="milk" required>
                                    <option value="" selected>Select</option>
                                    <option value="Milk Fresh (200 ml)">Milk Fresh (200 ml)</option>
                                    <option value="Milk Tinned (80 gms)">Milk Tinned (80 gms)</option>
                                    <option value="Milk Powder (28 gms)">Milk Powder (28 gms)</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control gnut" id="gnut" name="gnut" required>
                                    <option value="" selected>Select</option>
                                    <option value="Ground-nut (50 gins)">Ground-nut (50 gins)</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control butter" id="butter" name="butter" required>
                                    <option value="" selected>Select</option>
                                    <option value="Butter Fresh/Tinned (50 gms)">Butter Fresh/Tinned 50 gms</option>
                                </select>
                            </td>
                            <td>
                                <select class="form-control sugar" id="sugar" name="sugar" required>
                                    <option value="" selected>Select</option>
                                    <option value="Sugar (50 gms)">Sugar (50 gms)</option>
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
            <div class="mt-3">
                <asp:GridView ID="GridViewExtraIssueDivers" runat="server" CssClass="table table-bordered table-striped">
                </asp:GridView>
            </div>
        </form>
    </div>
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        var rowSequence = 0;
        function addRow() {
            var tableBody = document.getElementById("MainContent_tableBody");
            var newRow = document.createElement("tr");
            newRow.innerHTML = `
                        <td>
                            <input type="date" class="form-control" name="date" required />
                        </td>
                         <td>
                             <input type="text" class="form-control" name="name" required style="text-transform: capitalize;" />
                         </td>
                         <td>
                             <select class="form-control rank" id="rank" name="rank" width="130px" required>
                                 <option value="">Select</option>
                                 <option value="Sub Lieutenant">Sub Lieutenant</option>
                                 <option value="Lieutenant">Lieutenant</option>
                                 <option value="Lieutenant Commander">Lieutenant Commander</option>
                                 <option value="Commander">Commander</option>
                                 <option value="Captain">Captain</option>
                                 <option value="Commodore">Commodore</option>
                                 <option value="Rear Admiral">Rear Admiral</option>
                                 <option value="Vice Admiral">Vice Admiral</option>
                                 <option value="Admiral">Admiral</option>
                                 <option value="Seaman 2nd Class">Seaman 2nd Class</option>
                                 <option value="Seaman Ist Class">Seaman Ist Class</option>
                                 <option value="Leading Rate">Leading</option>
                                 <option value="Petty Officer">Petty Officer</option>
                                 <option value="Chief Petty Officer">Chief Petty Officer</option>
                                 <option value="Master Chief Petty Officer IInd Class">Master Chief Petty Officer IInd Class</option>
                                 <option value="Master Chief Petty Officer Ist Class">Master Chief Petty Officer Ist Class</option>
                             </select>
                         </td>
                         <td>
                             <input type="text" class="form-control pno" name="pno" required style="text-transform: capitalize;" />
                         </td>
                         <td>
                            <input type="text" class="form-control days" id="days" name="days" required />
                        </td>
                          <td>
                             <select class="form-control chocolate" id="chocolate_${rowSequence}" name="chocolate" required>
                                 <option value="" selected>Select</option>
                                 <option value="Chocolate (50 gms)">Chocolate (50 gms)</option>
                             </select>
                         </td>
                         <td>
                             <select class="form-control horlicks" id="horlicks_${rowSequence}" name="horlicks" required>
                                 <option value="" selected>Select</option>
                                 <option value="Complan/ Horlicks (50 gms)" selected>Complan/ Horlicks (50 gms)</option>
                             </select>
                         </td>
                         <td>
                             <select class="form-control eggs" id="eggs_${rowSequence}" name="eggs" required>
                                 <option value="" selected>Select</option>
                                 <option value="Eggs (2 Nos)">Eggs (2 Nos)</option>
                                 <option value="Milk Fresh (150 ml)">Milk Fresh (150 ml)</option>
                                 <option value="Milk Tinned (55 gms)">Milk Tinned (55 gms)</option>
                                 <option value="Milk Powder (20 gms)">Milk Powder (20 gms)</option>
                                 <option value="Cheese Tinned (50 gms)">Cheese Tinned (50 gms)</option>
                             </select>
                         </td>
                         <td>
                             <select class="form-control milk" id="milk_${rowSequence}" name="milk" required>
                                 <option value="" selected>Select</option>
                                 <option value="Milk Fresh (200 ml)">Milk Fresh (200 ml)</option>
                                 <option value="Milk Tinned (80 gms)">Milk Tinned (80 gms)</option>
                                 <option value="Milk Powder (28 gms)">Milk Powder (28 gms)</option>
                             </select>
                         </td>
                         <td>
                             <select class="form-control gnut" id="gnut_${rowSequence}" name="gnut" required>
                                 <option value="" selected>Select</option>
                                 <option value="Ground-nut (50 gins)">Ground-nut (50 gins)</option>
                             </select>
                         </td>
                         <td>
                             <select class="form-control butter" id="butter_${rowSequence}" name="butter" required>
                                 <option value="" selected>Select</option>
                                 <option value="Butter Fresh/Tinned (50 gms)">Butter Fresh/Tinned 50 gms</option>
                             </select>
                         </td>
                         <td>
                             <select class="form-control sugar" id="sugar_${rowSequence}" name="sugar" required>
                                 <option value="" selected>Select</option>
                                 <option value="Sugar (50 gms)">Sugar (50 gms)</option>
                             </select>
                         </td>
                        <td><button type="button" class="btn btn-danger" onclick="deleteRow(this)">Delete</button></td>`;
            tableBody.appendChild(newRow);

            rowSequence++;
        }

        function deleteRow(button) {
            var row = button.parentNode.parentNode;
            row.parentNode.removeChild(row);
        }


    </script>
</asp:Content>

