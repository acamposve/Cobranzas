<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlMetaDetalle.aspx.cs" Inherits="Cobranzas.ctrlMetaDetalle" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlMetaDetalle.aspx.js?ver=3" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%;">
        <tr>
            <td>
                <h2>Detalles de la Meta</h2>
            </td>
            <td style="text-align: right;">
                <input type="hidden" runat="server" id="idOperador" />
                <input type="hidden" runat="server" id="idMeta" />
                <input type="hidden" runat="server" id="Fecha" />
                <asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="Exportar"/>
            </td>
        </tr>
    </table>

    <div id="tabs">
        <ul>
            <li><a href="#tabCuentas" id="pstCuentas"><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Accounts%>" /></a></li>
            <li><a href="#tabExclusiones" id="pstExclusiones"><asp:Literal ID="Literal6" runat="server" Text="Exclusiones" /></a></li>
            <li><a href="#tabInclusiones" id="pstInclusiones"><asp:Literal ID="Literal7" runat="server" Text="Inclusiones" /></a></li>
            <li><a href="#tabRanking" id="pstRanking"><asp:Literal ID="Literal8" runat="server" Text="Ranking" /></a></li>
        </ul>
        <div id="tabCuentas">
            <h2><asp:Literal ID="Literal23" runat="server" Text="Cuentas Incluidas en la Meta"></asp:Literal></h2>
            <div id="pnlCuentas">
                <asp:Literal ID="Literal27" runat="server" Text="<%$ Resources:Recursos, PleaseWait%>"></asp:Literal>
            </div>
            Ajuste Manual: <input type="text" id="txtAjusteOperadorDet" disabled="disabled" />
        </div>
        <div id="tabExclusiones">
            <h2><asp:Literal ID="Literal3" runat="server" Text="Cuentas Excluidas en la Meta"></asp:Literal></h2>
            <div id="pnlExclusiones">
                <asp:Literal ID="Literal15" runat="server" Text="<%$ Resources:Recursos, PleaseWait%>"></asp:Literal>
            </div>
        </div>
        <div id="tabInclusiones">
            <h2><asp:Literal ID="Literal16" runat="server" Text="Cuentas Incluidas Manualmente en la Meta"></asp:Literal></h2>
            <div id="pnlInclusiones">
                <asp:Literal ID="Literal17" runat="server" Text="<%$ Resources:Recursos, PleaseWait%>"></asp:Literal>
            </div>
        </div>
        <div id="tabRanking">
            <h2><asp:Literal ID="Literal1" runat="server" Text="Pagos Incluidos en la Meta"></asp:Literal></h2>
            <div id="pnlPagosMeta">
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, PleaseWait%>"></asp:Literal>
            </div>
        </div>
    </div>
</asp:Content>
