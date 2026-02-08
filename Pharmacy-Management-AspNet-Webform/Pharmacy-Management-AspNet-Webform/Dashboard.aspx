<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Pharmacy_Management_AspNet_Webform.Dashboard" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <h2>Dashboard</h2>
    </div>
    <p class="welcome-text">Welcome to the Pharmacy Management System. Select a module below to get started.</p>
    <div class="dashboard-grid">
        <div class="dashboard-card card-inventory">
            <h3>Inventory Management</h3>
            <p>Manage medicine stocks, add, edit, delete and export data.</p>
            <a href="Inventory.aspx">Go to Inventory</a>
        </div>
        <div class="dashboard-card card-billing">
            <h3>Billing System</h3>
            <p>Create sales invoices with multiple medicines.</p>
            <a href="Billing.aspx">Generate Bill</a>
        </div>
        <div class="dashboard-card card-bills">
            <h3>Bill List</h3>
            <p>View, edit, delete invoices and export reports.</p>
            <a href="BillList.aspx">Go to Bills</a>
        </div>
    </div>
</asp:Content>
