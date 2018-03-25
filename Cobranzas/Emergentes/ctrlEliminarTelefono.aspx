<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlEliminarTelefono.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlEliminarTelefono" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlEliminarTelefono.aspx.js?ver=20" type="text/javascript"></script>
    <script src="/Scripts/Editor.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%;">
        <tr>
            <td>
                <h2>Telefonos</h2>
            </td>
            <td style="text-align: right;">
                <!--input type="button" value="Ejecutar Metas del Día" onclick="EjecutarMetas()" /-->
            </td>
        </tr>
    </table>
    <div id="Manejo">
        <div id="tabs">
            <ul>
                <li><a href="#tabListado" id="pstListado">
                    <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Listing%>" /><input type="hidden" id="idOperador" /></a></li>
            </ul>
            <div id="tabListado">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <h3>
                                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Listing%>"></asp:Literal></h3>
                        </td>
                    </tr>
                </table>
                <div id="pnlListado">
                    <asp:Literal ID="Literal53" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
            </div>
        </div>
    </div>
</asp:Content>
