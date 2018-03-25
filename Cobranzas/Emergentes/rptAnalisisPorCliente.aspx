<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptAnalisisPorCliente.aspx.cs" Inherits="Cobranzas.rptAnalisisPorCliente" ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptAnalisisPorCliente.aspx.js?ver=9" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server" ClientIDMode="Static">
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Country%>" />:
            </th>
            <td>
                <select id="cboPais" runat="server" name="cboPais" onclick="FiltrarPorPais();" onblur="SeleccionarPais();">
                </select>
                <input type="hidden" id="idPais" runat="server" />
                <div id="lstPais">
                </div>
            </td>
            <th>
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, client%>" />:
            </th>
            <td>
                <select id="cboCliente" runat="server" name="cboCliente">
                </select>
            </td>
            <td rowspan="2">
                <input type="button" runat="server" onclick="Ejecutar()" value="<%$ Resources:Recursos, Consult %>" />
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" Text="<%$ Resources:Recursos, Export %>" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, StartDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaIni" runat="server" />
            </td>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, EndDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaFin" runat="server" />
            </td>
        </tr>
        <tr id="filaOperador">
            <th>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Operator%>" />:
            </th>
            <td colspan="3">
                <select id="cboOperador" runat="server">
                </select>
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
