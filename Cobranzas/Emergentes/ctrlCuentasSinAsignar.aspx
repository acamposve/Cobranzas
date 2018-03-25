<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlCuentasSinAsignar.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlCuentasSinAsignar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlCuentasSinAsignar.aspx.js?ver=3" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" runat="server" id="idOperador" />
    <h2><asp:Literal ID="Literal23" runat="server" Text="<%$ Resources:Recursos, ListOfUnallocatedAccounts %>"></asp:Literal></h2>
    <div id="pnlCuentas">
        <asp:Literal ID="Literal26" runat="server" Text="<%$ Resources:Recursos, Loading%>"></asp:Literal>...
    </div>
    <h3><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, AssignToCampaign%>"></asp:Literal></h3>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Campaign%>"></asp:Literal>:
            </th>
            <td>
                <select id="cboCampana">
                </select>
            </td>
            <th>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Flow%>"></asp:Literal>:
            </th>
            <td>
                <select id="cboCFlujo">
                </select>
            </td>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Step%>"></asp:Literal>:
            </th>
            <td>
                <select id="cboCPaso">
                </select>
            </td>
            <td>
                <input type="button" onclick="AsignarCampana();" runat="server" value="<%$ Resources:Recursos, Assign%>" />
            </td>
        </tr>
    </table>

    <h3><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, AssignOperator %>"></asp:Literal></h3>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Operator%>"></asp:Literal>:
            </th>
            <td>
                <select id="cboOperador">
                </select>
            </td>
            <th>
                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Flow%>"></asp:Literal>:
            </th>
            <td>
                <select id="cboOFlujo">
                </select>
            </td>
            <th>
                <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, Step%>"></asp:Literal>:
            </th>
            <td>
                <select id="cboOPaso">
                </select>
            </td>
            <td>
                <input type="button" onclick="AsignarOperador();" runat="server" value="<%$ Resources:Recursos, Assign%>" />
            </td>
        </tr>
    </table>
</asp:Content>
