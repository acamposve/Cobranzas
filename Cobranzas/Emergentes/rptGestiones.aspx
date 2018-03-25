<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptGestiones.aspx.cs" Inherits="Cobranzas.Emergentes.rptGestiones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptGestiones.aspx.js?ver=66" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="idOperador" runat="server" />
    <h2>
        <asp:Literal ID="Literal59" runat="server" Text="<%$ Resources:Recursos, PerformedManagements %>"></asp:Literal></h2>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, StartDate%>" />:
            </th>
            <td>
                <input type="text" id="dtpFechaDesde" class="fecha" runat="server" />
            </td>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, EndDate%>" />:
            </th>
            <td>
                <input type="text" id="dtpFechaHasta" class="fecha" runat="server" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Country%>" />:
            </th>
            <td colspan="3">
                <select id="cboPais" style="width: 200px" runat="server" onchange="SeleccionarPais()">
                </select>
                <input type="hidden" id="idPais" runat="server" />
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
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Person%>" />:
            </th>
            <td colspan="3">
                <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Filter%>" />:<input type="text" id="txtFiltroPersona" onkeyup="FiltrarPersona()" />
                <input type="hidden" id="idPersona" runat="server" />
                <select id="cboPersona" style="width: 600px" runat="server">
                </select>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Status%>" />:
            </th>
            <td colspan="3">
                <input type="hidden" id="idStatus" runat="server" />
                <select id="cboStatus" style="width: 600px" onchange="SeleccionarStatus();">
                </select>
                <div id="lstStatus">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: center">
                <input type="button" runat="server" value="<%$ Resources:Recursos, Consult %>" onclick="ConsultarGestiones();" />
                <div id="btnExpor"></div>
                <%--<asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" OnClientClick="Preparar();" Text="<%$ Resources:Recursos, Export %>" />--%>
            </td>
        </tr>
    </table>
    <div id="pnlGestiones">
    </div>
</asp:Content>
