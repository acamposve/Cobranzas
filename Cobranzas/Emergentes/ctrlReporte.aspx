<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlReporte.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlReporte" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlReporte.aspx.js?ver=4" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2 runat="server" id="lbltitulo"></h2>
    <input type="hidden" id="PideParametros" runat="server" /> <input type="hidden" id="Origen" runat="server" />
    <div id="pnlResultado" runat="server">
        <h3>Esperando datos por parte de aplicación externa...</h3>
    </div>
    <asp:Button runat="server" ID="btnEjecutar" OnClick="btnEjecutar_Click" Text="Ejecutar" Visible="false" OnClientClick="$('#Cargando').show();"></asp:Button>
</asp:Content>
