<%@ Page Title="Bill List" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BillList.aspx.cs" Inherits="Pharmacy_Management_AspNet_Webform.BillList" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h2>Bill List</h2>
    </div>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <div class="toolbar">
            <asp:Button ID="btnNewBill" runat="server" Text="+ New Bill" CssClass="btn btn-primary" OnClick="btnNewBill_Click" CausesValidation="false" />
            <div class="toolbar-right">
                <asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" CssClass="btn btn-success btn-sm" OnClick="btnExportExcel_Click" CausesValidation="false" />
                <asp:Button ID="btnExportPdf" runat="server" Text="Export PDF" CssClass="btn btn-warning btn-sm" OnClick="btnExportPdf_Click" CausesValidation="false" />
            </div>
        </div>

        <asp:GridView ID="gvBills" runat="server" AutoGenerateColumns="false" CssClass="data-table"
            OnRowCommand="gvBills_RowCommand" EmptyDataText="No bills found." DataKeyNames="Id">
            <Columns>
                <asp:BoundField DataField="InvoiceNumber" HeaderText="Invoice #" />
                <asp:BoundField DataField="InvoiceDate" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                <asp:BoundField DataField="SubTotal" HeaderText="Sub Total" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="Discount" HeaderText="Discount" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="GrandTotal" HeaderText="Grand Total" DataFormatString="{0:N2}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnView" runat="server" CommandName="ViewItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-primary">View</asp:LinkButton>
                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-warning">Edit</asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this bill? Stock will be restored.');">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <asp:Panel ID="pnlView" runat="server" Visible="false" CssClass="modal-overlay active">
        <div class="modal-box" style="width:650px;">
            <h3>Invoice Details</h3>
            <table class="view-modal-table">
                <tr><td>Invoice #:</td><td><asp:Label ID="lblViewInvoiceNo" runat="server" /></td></tr>
                <tr><td>Date:</td><td><asp:Label ID="lblViewDate" runat="server" /></td></tr>
                <tr><td>Customer:</td><td><asp:Label ID="lblViewCustomer" runat="server" /></td></tr>
                <tr><td>Contact:</td><td><asp:Label ID="lblViewContact" runat="server" /></td></tr>
            </table>
            <h4 style="margin-top:15px;font-size:14px;">Items</h4>
            <asp:GridView ID="gvViewDetails" runat="server" AutoGenerateColumns="false" CssClass="data-table">
                <Columns>
                    <asp:BoundField DataField="MedicineName" HeaderText="Medicine" />
                    <asp:BoundField DataField="BatchNo" HeaderText="Batch No" />
                    <asp:BoundField DataField="ExpiryDate" HeaderText="Expiry" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                    <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="LineTotal" HeaderText="Line Total" DataFormatString="{0:N2}" />
                </Columns>
            </asp:GridView>
            <table class="view-modal-table" style="margin-top:10px;">
                <tr><td>Sub Total:</td><td><asp:Label ID="lblViewSubTotal" runat="server" /></td></tr>
                <tr><td>Discount:</td><td><asp:Label ID="lblViewDiscount" runat="server" /></td></tr>
                <tr style="font-weight:bold;"><td>Grand Total:</td><td><asp:Label ID="lblViewGrandTotal" runat="server" /></td></tr>
            </table>
            <div class="modal-actions">
                <asp:Button ID="btnCloseView" runat="server" Text="Close" CssClass="btn btn-secondary" OnClick="btnCloseView_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
