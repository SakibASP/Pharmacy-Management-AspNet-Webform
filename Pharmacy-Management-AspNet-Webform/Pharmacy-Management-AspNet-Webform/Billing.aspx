<%@ Page Title="Billing" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Billing.aspx.cs" Inherits="Pharmacy_Management_AspNet_Webform.Billing" EnableEventValidation="false" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h2>Sales Billing</h2>
    </div>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <div class="billing-section">
            <h3>Invoice Header</h3>
            <div class="form-row">
                <div class="form-group">
                    <label>Invoice Number</label>
                    <asp:TextBox ID="txtInvoiceNumber" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Invoice Date</label>
                    <asp:TextBox ID="txtInvoiceDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtInvoiceDate"
                        CssClass="text-danger" ErrorMessage="Date is required." Display="Dynamic" ValidationGroup="BillForm" />
                </div>
            </div>
            <div class="form-row">
                <div class="form-group">
                    <label>Customer Name</label>
                    <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvCustomer" runat="server" ControlToValidate="txtCustomerName"
                        CssClass="text-danger" ErrorMessage="Customer name is required." Display="Dynamic" ValidationGroup="BillForm" />
                </div>
                <div class="form-group">
                    <label>Contact</label>
                    <asp:TextBox ID="txtContact" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
        </div>

        <div class="billing-section">
            <h3>Invoice Items</h3>
            <button type="button" class="btn btn-success btn-sm" onclick="addRow();">+ Add Item</button>
            <table class="detail-table" id="tblItems">
                <thead>
                    <tr>
                        <th style="width:25%">Medicine</th>
                        <th style="width:12%">Batch No</th>
                        <th style="width:13%">Expiry Date</th>
                        <th style="width:10%">Qty</th>
                        <th style="width:12%">Unit Price</th>
                        <th style="width:13%">Line Total</th>
                        <th style="width:8%">Action</th>
                    </tr>
                </thead>
                <tbody id="itemsBody">
                </tbody>
            </table>
        </div>

        <div class="totals-section">
            <div class="totals-box">
                <div class="totals-row">
                    <span>Sub Total:</span>
                    <span id="spanSubTotal">0.00</span>
                </div>
                <div class="totals-row">
                    <span>Discount:</span>
                    <input type="text" id="txtDiscount" value="0" onkeyup="calculateGrandTotal();" />
                </div>
                <div class="totals-row grand-total">
                    <span>Grand Total:</span>
                    <span id="spanGrandTotal">0.00</span>
                </div>
            </div>
        </div>

        <div class="modal-actions" style="margin-top:25px;">
            <asp:Button ID="btnSaveBill" runat="server" Text="Save Invoice" CssClass="btn btn-primary" OnClick="btnSaveBill_Click" ValidationGroup="BillForm" OnClientClick="return prepareSave();" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" CausesValidation="false" />
            <asp:Button ID="btnBackToBills" runat="server" Text="Back to Bills" CssClass="btn btn-secondary" OnClick="btnBackToBills_Click" CausesValidation="false" Visible="false" />
        </div>

        <asp:HiddenField ID="hfItemsData" runat="server" />
        <asp:HiddenField ID="hfEditId" runat="server" Value="0" />
    </div>

    <script type="text/javascript">
        var medicineData = [];
        var isEditMode = false;

        function loadMedicines(data) {
            medicineData = data;
        }

        function getMedicineOptions() {
            var options = '<option value="">-- Select Medicine --</option>';
            for (var i = 0; i < medicineData.length; i++) {
                options += '<option value="' + medicineData[i].Id + '" data-batch="' + medicineData[i].BatchNo + '" data-expiry="' + medicineData[i].ExpiryDate + '" data-price="' + medicineData[i].UnitPrice + '" data-stock="' + medicineData[i].Quantity + '">' + medicineData[i].Name + ' (Stock: ' + medicineData[i].Quantity + ')' + '</option>';
            }
            return options;
        }

        function addRow() {
            var tbody = document.getElementById('itemsBody');
            var row = document.createElement('tr');
            row.innerHTML = '<td><select class="med-select" onchange="onMedicineChange(this);">' + getMedicineOptions() + '</select></td>' +
                '<td><input type="text" class="batch-no" readonly /></td>' +
                '<td><input type="text" class="expiry-date" readonly /></td>' +
                '<td><input type="number" class="qty" min="1" value="1" onkeyup="calculateRow(this);" onchange="calculateRow(this);" /></td>' +
                '<td><input type="text" class="unit-price" readonly /></td>' +
                '<td><input type="text" class="line-total" readonly /></td>' +
                '<td><button type="button" class="btn btn-danger btn-sm" onclick="removeRow(this);">X</button></td>';
            tbody.appendChild(row);
        }

        function onMedicineChange(sel) {
            var row = sel.closest('tr');
            var opt = sel.options[sel.selectedIndex];
            if (sel.value !== '') {
                row.querySelector('.batch-no').value = opt.getAttribute('data-batch');
                row.querySelector('.expiry-date').value = opt.getAttribute('data-expiry');
                row.querySelector('.unit-price').value = parseFloat(opt.getAttribute('data-price')).toFixed(2);
                row.querySelector('.qty').value = 1;
                calculateRow(row.querySelector('.qty'));
            } else {
                row.querySelector('.batch-no').value = '';
                row.querySelector('.expiry-date').value = '';
                row.querySelector('.unit-price').value = '';
                row.querySelector('.line-total').value = '';
                calculateTotals();
            }
        }

        function calculateRow(input) {
            var row = input.closest('tr');
            var qty = parseInt(row.querySelector('.qty').value) || 0;
            var price = parseFloat(row.querySelector('.unit-price').value) || 0;
            var lineTotal = qty * price;
            row.querySelector('.line-total').value = lineTotal.toFixed(2);
            calculateTotals();
        }

        function calculateTotals() {
            var rows = document.querySelectorAll('#itemsBody tr');
            var subTotal = 0;
            for (var i = 0; i < rows.length; i++) {
                var lt = parseFloat(rows[i].querySelector('.line-total').value) || 0;
                subTotal += lt;
            }
            document.getElementById('spanSubTotal').innerText = subTotal.toFixed(2);
            calculateGrandTotal();
        }

        function calculateGrandTotal() {
            var subTotal = parseFloat(document.getElementById('spanSubTotal').innerText) || 0;
            var discount = parseFloat(document.getElementById('txtDiscount').value) || 0;
            var grandTotal = subTotal - discount;
            if (grandTotal < 0) grandTotal = 0;
            document.getElementById('spanGrandTotal').innerText = grandTotal.toFixed(2);
        }

        function removeRow(btn) {
            var row = btn.closest('tr');
            row.remove();
            calculateTotals();
        }

        function prepareSave() {
            var rows = document.querySelectorAll('#itemsBody tr');
            if (rows.length === 0) {
                alert('Please add at least one item.');
                return false;
            }

            var items = [];
            for (var i = 0; i < rows.length; i++) {
                var medId = rows[i].querySelector('.med-select').value;
                var qty = parseInt(rows[i].querySelector('.qty').value) || 0;
                var price = parseFloat(rows[i].querySelector('.unit-price').value) || 0;
                var lineTotal = parseFloat(rows[i].querySelector('.line-total').value) || 0;
                var batch = rows[i].querySelector('.batch-no').value;
                var expiry = rows[i].querySelector('.expiry-date').value;

                if (medId === '') {
                    alert('Please select a medicine for row ' + (i + 1));
                    return false;
                }
                if (qty <= 0) {
                    alert('Quantity must be greater than 0 for row ' + (i + 1));
                    return false;
                }

                if (!isEditMode) {
                    var sel = rows[i].querySelector('.med-select');
                    var opt = sel.options[sel.selectedIndex];
                    var stock = parseInt(opt.getAttribute('data-stock')) || 0;
                    if (qty > stock) {
                        alert('Insufficient stock for ' + opt.text + '. Available: ' + stock);
                        return false;
                    }
                }

                items.push({
                    MedicineId: parseInt(medId),
                    BatchNo: batch,
                    ExpiryDate: expiry,
                    Quantity: qty,
                    UnitPrice: price,
                    LineTotal: lineTotal
                });
            }

            var subTotal = parseFloat(document.getElementById('spanSubTotal').innerText) || 0;
            var discount = parseFloat(document.getElementById('txtDiscount').value) || 0;
            var grandTotal = parseFloat(document.getElementById('spanGrandTotal').innerText) || 0;

            var data = {
                Items: items,
                SubTotal: subTotal,
                Discount: discount,
                GrandTotal: grandTotal
            };

            document.getElementById('<%= hfItemsData.ClientID %>').value = JSON.stringify(data);
            return true;
        }

        function loadExistingItems(items, subTotal, discount, grandTotal) {
            isEditMode = true;
            var tbody = document.getElementById('itemsBody');
            tbody.innerHTML = '';
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var row = document.createElement('tr');
                row.innerHTML = '<td><select class="med-select" onchange="onMedicineChange(this);">' + getMedicineOptions() + '</select></td>' +
                    '<td><input type="text" class="batch-no" readonly /></td>' +
                    '<td><input type="text" class="expiry-date" readonly /></td>' +
                    '<td><input type="number" class="qty" min="1" value="' + item.Quantity + '" onkeyup="calculateRow(this);" onchange="calculateRow(this);" /></td>' +
                    '<td><input type="text" class="unit-price" readonly /></td>' +
                    '<td><input type="text" class="line-total" readonly /></td>' +
                    '<td><button type="button" class="btn btn-danger btn-sm" onclick="removeRow(this);">X</button></td>';
                tbody.appendChild(row);

                var sel = row.querySelector('.med-select');
                sel.value = item.MedicineId.toString();
                row.querySelector('.batch-no').value = item.BatchNo;
                row.querySelector('.expiry-date').value = item.ExpiryDate;
                row.querySelector('.unit-price').value = parseFloat(item.UnitPrice).toFixed(2);
                row.querySelector('.line-total').value = parseFloat(item.LineTotal).toFixed(2);
            }
            document.getElementById('spanSubTotal').innerText = subTotal.toFixed(2);
            document.getElementById('txtDiscount').value = discount;
            document.getElementById('spanGrandTotal').innerText = grandTotal.toFixed(2);
        }
    </script>
</asp:Content>
