<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptGestionesCuentas.aspx.cs" Inherits="Cobranzas.Emergentes.rptGestionesCuentas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptGestionesCuentas.aspx.js?ver=9" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h3>Gestiones por Cuenta</h3>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Country%>" />:
            </th>
            <td colspan="4">
                <input type="hidden" id="idPais" runat="server" />
                <select id="cboPais" style="width: 600px" runat="server" onchange="SeleccionarPais()">
                </select>
                <div id="lstPais">
                </div>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Client%>" />:
            </th>
            <td colspan="3">
                <input type="hidden" id="idCliente" runat="server" />
                <select id="cboCliente" style="width: 600px" runat="server">
                </select>
            </td>
            <td>
                <input type="button" runat="server" onclick="Ejecutar()" value="<%$ Resources:Recursos, Execute%>" />
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" OnClientClick="Preparar()" Text="<%$ Resources:Recursos, Export %>" />
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
