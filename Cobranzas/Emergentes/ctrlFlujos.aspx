<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlFlujos.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlFlujos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="/Scripts/Graficos.js" type="text/javascript"></script>
    <script src="ctrlFlujos.aspx.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
    <style>
        .cajas
        {
            width: 120px;
            height: 30px; /*padding: 0.5em;*/
            float: left; /*margin: 0 10px 10px 0;*/
            background-color: #CCCCFF;
            border: 1px solid black;
            cursor: pointer;
            text-align: center;
            vertical-align: middle;
            color: #000040;
            border-radius: 5px;
            position: absolute;
        }
        .cajas:hover
        {
            background-color: #400000;
            color: White;
        }
        .reglas
        {
            width: 80px;
            display: inline-block;
            height: 15px;
            font-size: x-small;
            background-color: #FFFFCC; /*transparent;*/
            border: 1px solid black;
            cursor: pointer;
            text-align: center;
            vertical-align: middle;
            color: #000040;
            border-radius: 3px;
            position: absolute;
            overflow: hidden;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal ID="Literal59" runat="server" Text="<%$ Resources:Recursos, Flows %>"></asp:Literal></h2>
    <div id="tabs">
        <ul>
            <li><a href="#tabListado" runat="server" id="pstListado"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Listing%>" /></a></li>
            <li><a href="#tabDetalles" id="pstDetalles"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Details%>" /></a></li>
        </ul>
        <div id="tabListado">
            <table style="width: 100%">
                <tr>
                    <td>
                        <h3><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
                    </td>
                    <td style="text-align: right;">
                        <input type="button" class="btnAgregar32" id="btnNuevoFlujo" onclick="Nuevo();" />
                    </td>
                </tr>
            </table>
            <div id="pnlDistFlujos">
            </div>
        </div>
        <div id="tabDetalles">
            <input type="hidden" id="idFlujo" />
            <table style="width: 100%;">
                <tr>
                    <td>
                        <h3><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Details %>"></asp:Literal></h3>
                    </td>
                    <td style="text-align: right;">
                        <input type="button" class="btnGuardar32" onclick="Guardar();" title="Guardar Flujo" />
                        <input type="button" class="btnFlecha32" onclick="CrearReglas();" title="Crear Reglas de Transición" />
                        <input type="button" class="btnMano32" onclick="ModificarDiagrama();" title="Modificar Diagrama" />
                    </td>
                </tr>
            </table>
            <table>
                <tr>
                    <th>
                        <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Name %>"></asp:Literal>
                    </th>
                    <td>
                        <input type="text" id="txtNombreFlujo" />
                    </td>
                    <th>
                        <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, ClientType %>"></asp:Literal>
                    </th>
                    <td>
                        <select id="cboTipoCliente">
                            <option value="1">TipoCliente Cableado</option>
                        </select>
                    </td>
                    <th>
                        <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, RuleOut %>"></asp:Literal>
                    </th>
                    <td>
                        <select id="cboReglaSalida">
                            <option value="1">Regla Cableada</option>
                        </select>
                    </td>
                </tr>
            </table>
            <div style="width: 700px; height: 350px;">
                <canvas id="lienzo" width="700px" height="350px" style="border: 1px solid green; position: absolute;" onclick="CanvasClick();">
                    <div style="width: 100%; height: 100%;">
                        Su navegador no soporta HTML5, por lo tanto no podrá usar esta función. Actualice su navegador.
                    </div>
                </canvas>
                <div id="ContenedorPasos" style="width: 700px; height: 350px; position: absolute; border: 1px solid blue;">
                </div>
            </div>
            <select id="cboPasos">
            </select><input type="button" class="btnAgregar32" onclick="AgregarPaso();" />
        </div>
    </div>
</asp:Content>
