<%@ Page Title="Inventory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Inventory.aspx.cs" Inherits="Pharmacy_Management_AspNet_Webform.Inventory" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h2>Inventory Management</h2>
    </div>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false">
        <asp:Label ID="lblMessage" runat="server"></asp:Label>
    </asp:Panel>

    <div class="card">
        <div class="toolbar">
            <asp:Button ID="btnAddNew" runat="server" Text="+ Add Medicine" CssClass="btn btn-primary" OnClick="btnAddNew_Click" />
            <div class="toolbar-right">
                <asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" CssClass="btn btn-success btn-sm" OnClick="btnExportExcel_Click" />
                <asp:Button ID="btnExportPdf" runat="server" Text="Export PDF" CssClass="btn btn-warning btn-sm" OnClick="btnExportPdf_Click" />
            </div>
        </div>

        <asp:GridView ID="gvMedicines" runat="server" AutoGenerateColumns="false" CssClass="data-table"
            OnRowCommand="gvMedicines_RowCommand" EmptyDataText="No medicines found." DataKeyNames="Id">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="ID" />
                <asp:BoundField DataField="Name" HeaderText="Name" />
                <asp:BoundField DataField="GenericName" HeaderText="Generic Name" />
                <asp:BoundField DataField="Category" HeaderText="Category" />
                <asp:BoundField DataField="BatchNo" HeaderText="Batch No" />
                <asp:BoundField DataField="ExpiryDate" HeaderText="Expiry Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit Price" DataFormatString="{0:N2}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnView" runat="server" CommandName="ViewItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-primary">View</asp:LinkButton>
                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-warning">Edit</asp:LinkButton>
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="DeleteItem" CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this medicine?');">Delete</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <asp:Panel ID="pnlForm" runat="server" Visible="false" CssClass="modal-overlay active">
        <div class="modal-box">
            <h3><asp:Label ID="lblFormTitle" runat="server" Text="Add Medicine"></asp:Label></h3>
            <asp:HiddenField ID="hfMedicineId" runat="server" />
            <div class="form-row">
                <div class="form-group">
                    <label>Medicine Name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtName"
                        CssClass="text-danger" ErrorMessage="Name is required." Display="Dynamic" ValidationGroup="MedForm" />
                </div>
                <div class="form-group">
                    <label>Generic Name</label>
                    <asp:TextBox ID="txtGenericName" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group">
                    <label>Category</label>
                    <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Batch No</label>
                    <asp:TextBox ID="txtBatchNo" runat="server" CssClass="form-control"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvBatchNo" runat="server" ControlToValidate="txtBatchNo"
                        CssClass="text-danger" ErrorMessage="Batch No is required." Display="Dynamic" ValidationGroup="MedForm" />
                </div>
            </div>
            <div class="form-row">
                <div class="form-group">
                    <label>Expiry Date</label>
                    <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvExpiry" runat="server" ControlToValidate="txtExpiryDate"
                        CssClass="text-danger" ErrorMessage="Expiry date is required." Display="Dynamic" ValidationGroup="MedForm" />
                </div>
                <div class="form-group">
                    <label>Quantity</label>
                    <asp:TextBox ID="txtQuantity" runat="server" CssClass="form-control" TextMode="Number" Text="0"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvQty" runat="server" ControlToValidate="txtQuantity"
                        CssClass="text-danger" ErrorMessage="Quantity is required." Display="Dynamic" ValidationGroup="MedForm" />
                </div>
            </div>
            <div class="form-group">
                <label>Unit Price</label>
                <asp:TextBox ID="txtUnitPrice" runat="server" CssClass="form-control" Text="0.00"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPrice" runat="server" ControlToValidate="txtUnitPrice"
                    CssClass="text-danger" ErrorMessage="Unit price is required." Display="Dynamic" ValidationGroup="MedForm" />
            </div>
            <div class="modal-actions">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" ValidationGroup="MedForm" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlView" runat="server" Visible="false" CssClass="modal-overlay active">
        <div class="modal-box">
            <h3>Medicine Details</h3>
            <table class="view-modal-table">
                <tr><td>ID:</td><td><asp:Label ID="lblViewId" runat="server" /></td></tr>
                <tr><td>Name:</td><td><asp:Label ID="lblViewName" runat="server" /></td></tr>
                <tr><td>Generic Name:</td><td><asp:Label ID="lblViewGeneric" runat="server" /></td></tr>
                <tr><td>Category:</td><td><asp:Label ID="lblViewCategory" runat="server" /></td></tr>
                <tr><td>Batch No:</td><td><asp:Label ID="lblViewBatch" runat="server" /></td></tr>
                <tr><td>Expiry Date:</td><td><asp:Label ID="lblViewExpiry" runat="server" /></td></tr>
                <tr><td>Quantity:</td><td><asp:Label ID="lblViewQty" runat="server" /></td></tr>
                <tr><td>Unit Price:</td><td><asp:Label ID="lblViewPrice" runat="server" /></td></tr>
            </table>
            <div class="modal-actions">
                <asp:Button ID="btnCloseView" runat="server" Text="Close" CssClass="btn btn-secondary" OnClick="btnCloseView_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>
