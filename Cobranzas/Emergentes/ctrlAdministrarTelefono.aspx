<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlAdministrarTelefono.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlAdministrarTelefono" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlAdministrarTelefono.aspx.js?ver=11" type="text/javascript"></script>
    <script src="/Scripts/Editor.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%;">
        <tr>
            <td>
                <h2>Plantillas</h2>
            </td>
            <td style="text-align: right;">
            </td>
        </tr>
    </table>
    <div id="Manejo">
        <div id="tabs">
            <ul>
                <li><a href="#tabListadoEliminar" id="pstListadoEliminar">
                    <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, PendingDelete%>" /></a></li>
                <li><a href="#tabListadoAgregar" id="pstListadoAgregar">
                    <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, PendingEntry%>" /></a></li>
            </ul>
            <div id="tabListadoEliminar">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <h3>
                                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
                        </td>
                    </tr>
                </table>
                <div id="pnlListadoEliminar">
                    <asp:Literal ID="Literal53" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
            </div>
            <div id="tabListadoAgregar">
                                <table style="width: 100%">
                    <tr>
                        <td>
                            <h3>
                                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
                        </td>
                    </tr>
                </table>
                <div id="pnlListadoAgregar">
                    <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
            </div>
        </div>
    </div>
</asp:Content>
