<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptCorreosAdministrador.aspx.cs" Inherits="Cobranzas.Emergentes.rptCorreosAdministrador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptCorreosAdministrador.aspx.js?ver=0" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h3>Correos de Administrador</h3>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, StartDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaDesde" runat="server" />
            </td>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, EndDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaHasta" runat="server" />
            </td>
            <td>
                <input type="button" runat="server" onclick="Ejecutar()" value="<%$ Resources:Recursos, Execute%>" />
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" Text="<%$ Resources:Recursos, Export %>" />
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
