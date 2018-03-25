<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="EstadoCuentaNaviera.aspx.cs" Inherits="Cobranzas.Emergentes.EstadoCeuentaNaviera" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h1>Lista de Clientes</h1>

    <asp:Panel ID="pnTablasClientes" runat="server"></asp:Panel>



<asp:Panel runat="server" ID="pnlExportar">
       
        <asp:Button runat="server" ID="btnExportarExcel" Text="Exportar a Excel" OnClick="btnExportarExcel_Click" />
        <asp:Button runat="server" ID="btnExportarWord" Text="Exportar a Word" OnClick="btnExportarWord_Click" />
        <input type="button" value="Imprimir" onclick="this.parentNode.style.display = 'none'; window.print();" />
        
    </asp:Panel>

</asp:Content>



