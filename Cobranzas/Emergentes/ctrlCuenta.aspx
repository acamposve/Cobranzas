<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlCuenta.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlCuenta" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Datos:</h2>
    <div runat="server" id="pnlDatos">
        
    </div>
    <h2>Reportes de la Cuenta</h2>
    <div id="pnlReportes" runat="server"></div>
    <h2>Soportes de la Cuenta:</h2>
    <div runat="server" id="cont">
    </div>
</asp:Content>
