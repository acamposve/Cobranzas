<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlDistOperador.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlDistOperador" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlDistOperador.aspx.js?ver=2" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal ID="Literal23" runat="server" Text="<%$ Resources:Recursos, AutomaticDistributionOfAccountsOperators %>"></asp:Literal></h2>
    <div id="tabs">
        <ul>
            <li><a href="#tabListado" id="pstListado"><asp:Literal ID="Literal24" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></a></li>
            <li><a href="#tabDetalles" id="pstDetalles"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Details %>"></asp:Literal></a></li>
        </ul>
        <div id="tabListado">
            <h3><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
            <div id="pnlDistCuentasOperadores">
            </div>
            <input type="button" runat="server" value="<%$ Resources:Recursos, New %>" onclick="Nuevo()" />
        </div>
        <div id="tabDetalles">
            <input type="hidden" id="idDistribucion" />
            <table style="width: 100%">
                <tr>
                    <td>
                        <h3><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Details %>"></asp:Literal></h3>
                    </td>
                    <td style="text-align: right;">
                        <input type="button" id="btnGuardar" runat="server" onclick="Guardar();" class="btnGuardar32" title="<%$ Resources:Recursos, Save %>" /><br />
                        <input type="button" id="btnForzar" runat="server" onclick="Forzar();" value="Distribuir Ahora" />
                    </td>
                </tr>
            </table>
            <table class="TablaDatos">
                <tr>
                    <th>
                        <asp:Literal ID="Literal26" runat="server" Text="<%$ Resources:Recursos, Name%>"></asp:Literal>:
                    </th>
                    <td colspan="3">
                        <input type="text" id="txtNombre" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Rule%>"></asp:Literal>:
                    </th>
                    <td>
                        <select id="cboRegla">
                        </select>
                    </td>
                    <th>
                        <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Operator%>"></asp:Literal>:
                    </th>
                    <td>
                        <select id="cboOperador">
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Flow%>"></asp:Literal>:
                    </th>
                    <td>
                        <select id="cboFlujo">
                        </select>
                    </td>
                    <th>
                        <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Step%>"></asp:Literal>:
                    </th>
                    <td>
                        <select id="cboPaso">
                        </select>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
