<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlReglas.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlReglas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlReglas.aspx.js?ver=5" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%">
        <tr>
            <td>
                <h2><asp:Literal ID="Literal59" runat="server" Text="<%$ Resources:Recursos, SelectionRulesOfAuditors %>"></asp:Literal></h2>
            </td>
            <td id="pnlSeleccion" style="text-align: right;">
                <input type="button" id="btnSeleccionar" onclick="Seleccionar();" class="btnAceptar32" /> <input type="button" id="btnEliminar" onclick="Eliminar();" class="btnEliminar32" />
            </td>
        </tr>
    </table>
    <div id="tabs">
        <ul>
            <li><a href="#tabListado" runat="server" id="pstListado"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Listing%>" /></a></li>
            <li><a href="#tabDetalles" runat="server" id="pstDetalles"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Details%>" /></a></li>
        </ul>
        <div id="tabListado">
            <h3><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, NewRule %>"></asp:Literal></h3>
            <div id="pnlDistReglas">
            </div>
            <input type="button" runat="server" value="<%$ Resources:Recursos, NewRule %>" onclick="Nuevo();" />
        </div>
        <div id="tabDetalles">
            <input type="hidden" id="idRegla" />
            <table style="width: 100%">
                <tr>
                    <td style="text-align: right;">
                        <input type="button" runat="server" id="btnGuardar" onclick="GuardarReglas();" class="btnGuardar32" title="<%$ Resources:Recursos, Save %>" /><br />
                    </td>
                </tr>
            </table>
            <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Name%>" />:<input type="text" id="txtNombreRegla" style="width: 200px;" /><br />
            <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, Type%>" />:
            <select id="cboTipo">
                <option value="F">Regla para Flujos</option>
                <option value="D">Regla para Distribución</option>
                <option value="C">Regla para Campañas</option>
                <option value="M">Regla para Metas</option>
            </select>
            <h3><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, RuleCriteria %>"></asp:Literal></h3>
            <table class="TablaDatos">
                <tr>
                    <th>
                        <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:Recursos, Field %>"></asp:Literal> <input type="hidden" id="nroCampo" />
                    </th>
                    <td>
                        <select id="cboCampo" onchange="LlenarComboOperadores();">
                        </select>
                    </td>
                    <td rowspan="3" style="width: 0px;">
                        <input type="button" id="btnAgregarCriterio" class="btnAgregar32" onclick="AgregarCriterio();" /> <input type="button" id="btnEliminarCriterio" class="btnEliminar32" onclick="EliminarCriterio();" />
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Operator %>"></asp:Literal>
                    </th>
                    <td>
                        <select id="cboOperador" onchange="SeleccionTipoValor();">
                        </select>
                    </td>
                </tr>
                <tr>
                    <th>
                        <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Value %>"></asp:Literal>
                    </th>
                    <td>
                        <div id="Valores">
                        </div>
                        <input type="text" id="txtValor" style="display: none;" />
                        <select id="cboStatus" style="display: none;">
                        </select>
                        <select id="cboPersonas" style="display: none;">
                        </select>
                        <select id="cboClientes" style="display: none;">
                        </select>
                        <select id="cboProductos" style="display: none;">
                        </select>
                        <select id="cboPasos" style="display: none;">
                        </select>
                        <input type="checkbox" id="chkValor" style="display: none;" />
                    </td>
                </tr>
            </table>
            <div id="pnlReglaCriterios">
            </div>
        </div>
    </div>
</asp:Content>
