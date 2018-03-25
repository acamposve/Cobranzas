<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlStatus.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlStatus.aspx.js?ver=4" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal Text="<%$ Resources:Recursos, Status %>" runat="server" /></h2>
    <input type="hidden" id="idOperador" />
    <table class="TablaDatos">
        <tr>
            <th>Tipo Cliente:</th>
            <td>
                <select id="cboTipoCliente" onclick="ActualizarPais();ActualizarListado();" onblur="ActualizarPais();ActualizarListado();"></select>
            </td>
            <th>País</th>
            <td>
                <select id="cboPais" onclick="ActualizarListado();" onblur="ActualizarListado();"></select>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal14" runat="server" Text="<%$ Resources:Recursos, Level%>"></asp:Literal>:
            </th>
            <td>
                <input type="text" id="txtNivel" />
            </td>
            <td rowspan="4" colspan="2" style="width: 0px;">
                <input type="button" id="btnAgregar" class="btnAgregar32" onclick="Nuevo();" /> <input type="button" id="Button1" class="btnGuardar32" onclick="    Guardar();" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Type%>"></asp:Literal>:
            </th>
            <td>
                <select id="cboTipo">
                    <option value="Positivo"><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Positive%>"></asp:Literal></option>
                    <option value="Neutral"><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Neutral%>"></asp:Literal></option>
                    <option value="Negativo"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Negative%>"></asp:Literal></option>
                </select>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Name%>"></asp:Literal>:
            </th>
            <td>
                <input type="text" id="txtNombre" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Active1%>"></asp:Literal>:
            </th>
            <td>
                <input type="checkbox" id="chkActivo" />
            </td>
        </tr>
    </table>
    <div id="pnlResultados">
    </div>
</asp:Content>
