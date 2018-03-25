<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptLogins.aspx.cs" Inherits="Cobranzas.Emergentes.rptLogins" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptLogins.aspx.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="idOperador" runat="server" />
    <h2><asp:Literal ID="Literal59" runat="server" Text="<%$ Resources:Recursos, SystemInputsAndOutputs%>"></asp:Literal></h2>
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
            <td colspan="4" style="text-align: center">
                <input type="button" runat="server" value="<%$ Resources:Recursos, Consult%>" onclick="ConsultarLogins();" />
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" Text="<%$ Resources:Recursos, Export%>" />
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
