<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="rptRanking.aspx.cs" Inherits="Cobranzas.Emergentes.rptRanking" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="rptRanking.aspx.js?ver=10" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="idOperador" runat="server" />
    <h2><asp:Literal ID="Literal59" runat="server" Text="Ranking"></asp:Literal></h2>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Date%>" />:
            </th>
            <td>
                <input type="text" id="dtpFecha" class="fecha" runat="server" />
            </td>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Goals%>" />:
            </th>
            <td>
                <!--select id="lstMetas" multiple="multiple" style="height:100px;"></!--select-->
                <div id="lstMetas"></div>
                <input type="hidden" id="selMetas" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="4" style="text-align: center">
                <asp:Button runat="server" ID="btnExportar" OnClientClick="Preparar()" OnClick="btnExportar_Click" Text="<%$ Resources:Recursos, Export%>" />
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
