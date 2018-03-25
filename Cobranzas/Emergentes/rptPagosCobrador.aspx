<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptPagosCobrador.aspx.cs" Inherits="Cobranzas.Emergentes.rptPagosCobrador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptPagosCobrador.aspx.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal ID="Literal59" runat="server" Text="<%$ Resources:Recursos, PaymentsToCollector%>"></asp:Literal></h2>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, StartDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaIni" runat="server" />
            </td>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, EndDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaFin" runat="server" />
            </td>
            <td rowspan="2">
                <input type="button" onclick="Ejecutar()" runat="server" value="<%$ Resources:Recursos, Execute%>" />
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" Text="<%$ Resources:Recursos, Export%>" />
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
