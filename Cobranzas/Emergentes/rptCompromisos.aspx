<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptCompromisos.aspx.cs" Inherits="Cobranzas.Emergentes.rptCompromisos" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptCompromisos.aspx.js?ver=8" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="idOperador" runat="server"  />
    <input type="hidden" id="Supervisado" runat="server"  />
    <h2>Compromisos</h2>
    <table class="TablaDatos">
        <tr>
            <th>
                Fecha Desde:
            </th>
            <td>
                <input type="text" id="dtpFechaDesde" class="fecha" runat="server" />
            </td>
            <th>
                Fecha Hasta:
            </th>
            <td>
                <input type="text" id="dtpFechaHasta" class="fecha" runat="server" />
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
        <tr>
            <td colspan="4" style="text-align: center">
                <input type="button" value="Consultar" onclick="ConsultarCompromisos();" /> 
                <asp:Button runat="server" ID="btnExportar" OnClick="btnExportar_Click" Text="Exportar" />
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
