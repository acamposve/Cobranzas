<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptPersonasGestionadas.aspx.cs" Inherits="Cobranzas.Emergentes.rptPersonasGestionadas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptPersonasGestionadas.aspx.js?ver=8" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h3>Personas Gestionadas</h3>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, StartDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaDesde" runat="server" />
            </td>
            <th>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, DateUntil%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaHasta" runat="server" />
            </td>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, EndDate%>" />:
            </th>
            <td>
                <input type="text" class="fecha" id="dtpFechaFin" runat="server" />
            </td>
            <td rowspan="5">
                <input type="button" runat="server" onclick="Ejecutar()" value="<%$ Resources:Recursos, Execute%>" />
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" Text="<%$ Resources:Recursos, Export %>" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Country%>" />:
            </th>
            <td colspan="5">
                <select id="cboPais" style="width: 200px" runat="server" onchange="SeleccionarPais()">
                </select>
                <input type="hidden" id="idPais" runat="server" />
                <div id="lstPais">
                </div>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Client%>" />:
            </th>
            <td colspan="5">
                <select id="cboCliente" runat="server" name="cboCliente">
                </select>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Operator%>" />:
            </th>
            <td colspan="5">
                <select id="cboOperador" runat="server" name="cboOperador">
                </select>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, TypeOfReport%>" />:
            </th>
            <td colspan="5">
                <select id="cboTipoReporte" runat="server" name="cboTipoReporte">
                    <option value="1">Gestionados</option>
                    <option value="0">No Gestionados</option>
                    <option value="null">Todos</option>
                </select>
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
