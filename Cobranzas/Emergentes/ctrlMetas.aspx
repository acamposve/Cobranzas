<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlMetas.aspx.cs" Inherits="Cobranzas.ctrlMetas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlMetas.aspx.js?ver=18" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
    <style type="text/css">
        .fecha {
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%;">
        <tr>
            <td>
                <h2>Metas</h2>
            </td>
            <td style="text-align: right;">
                <!--input type="button" value="Ejecutar Metas del Día" onclick="EjecutarMetas()" /-->
            </td>
        </tr>
    </table>
    <div id="Manejo">
        <div id="tabs">
            <ul>
                <li><a href="#tabListado" id="pstListado"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Listing%>" /></a></li>
                <li><a href="#tabDetalles" id="pstDetalles"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Details%>" /></a></li>
                <li><a href="#tabConsultar" id="pstConsultar"><asp:Literal ID="Literal9" runat="server" Text="Consultar" /></a></li>
                <li><a href="#tabRanking" id="pstRanking"><asp:Literal ID="Literal8" runat="server" Text="Ranking" /></a></li>
            </ul>
            <div id="tabListado">
                <h3><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
                <div id="pnlListado">
                    <asp:Literal ID="Literal53" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
                <input type="button" runat="server" value="Nueva Meta" onclick="Nuevo();" />
            </div>
            <div id="tabDetalles">
                <input type="hidden" id="idMeta" runat="server" />
                <table style="width: 100%">
                    <tr>
                        <td style="text-align: right;">
                            <input type="button" id="btnGuardar" onclick="Guardar();" class="btnGuardar32" title="Guardar" /><br />
                        </td>
                    </tr>
                </table>
                <table class="TablaDatos">
                    <tr>
                        <th>
                            <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:Recursos, Name %>"></asp:Literal>
                        </th>
                        <td colspan="5">
                            <input type="text" id="txtNombre" style="width: 90%;" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:Recursos, StartDate %>"></asp:Literal>
                        </th>
                        <td>
                            <input type="text" id="txtFechaInicio" class="fecha" />
                        </td>
                        <th>
                            <asp:Literal ID="Literal12" runat="server" Text="Frecuencia"></asp:Literal>
                        </th>
                        <td>
                            <select id="cboFrecuencia">
                                <option value="0">Una Sola Vez</option>
                                <option value="1">Semanal</option>
                                <option value="2">Mensual</option>
                                <option value="3">Anual</option>
                            </select>
                        </td>
                        <th>
                            <asp:Literal ID="Literal13" runat="server" Text="<%$ Resources:Recursos, EndDate %>"></asp:Literal>
                        </th>
                        <td style="min-width: 80px;">
                            <span id="txtFechaFin" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Literal ID="Literal14" runat="server" Text="Regla"></asp:Literal>
                        </th>
                        <td colspan="2">
                            <select id="cboRegla">
                            </select>
                        </td>
                        <td colspan="3"><input type="checkbox" id="chkAplicaExclusiones"/>Aplica exclusiones Automáticas </td>
                    </tr>
                </table>
                <div id="pnlOperadores">
                    <h2><asp:Literal ID="Literal22" runat="server" Text="Operadores Asignados a la Meta"></asp:Literal></h2>
                    <div id="pnlOperadoresMetas">
                        <asp:Literal ID="Literal54" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                    </div>
                    <h3>Agregar Operador a la Meta</h3>
                    Operador:<select id="cboOperadores"></select>
                    Ajuste a la meta: <input type="text" id="txtOperadorAjuste" /><input type="button" class="btnAgregar32" onclick="InsertarOperador();" />
                </div>
            </div>
            <div id="tabConsultar">
                Escoja la meta a Consultar:
                <select id="cboMetas" onblur="ConsultarOperadores()">
                </select>
                Escoja al operador:
                <select id="cboOperadorMeta">
                </select>
                <input type="hidden" runat="server" id="idOperador" />
                <input type="hidden" runat="server" id="Fecha" />

                <input type="button" value="Consultar" onclick="Consultar()" /><asp:Button ID="btnExportar" runat="server" Text="Exportar" OnClick="Exportar" OnClientClick="Preparar();" />
                <div id="tabs2" style="display: none;">
                    <ul>
                        <li><a href="#tabCuentas" id="pstCuentas"><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Accounts%>" /></a></li>
                        <li><a href="#tabExclusiones" id="pstExclusiones"><asp:Literal ID="Literal6" runat="server" Text="Exclusiones" /></a></li>
                        <li><a href="#tabInclusiones" id="pstInclusiones"><asp:Literal ID="Literal7" runat="server" Text="Inclusiones" /></a></li>
                        <li><a href="#tabOpRanking" id="pstOpRanking"><asp:Literal ID="Literal19" runat="server" Text="Ranking" /></a></li>
                    </ul>
                    <div id="tabCuentas">
                        <h2><asp:Literal ID="Literal23" runat="server" Text="Cuentas Incluidas en la Meta"></asp:Literal></h2>
                        <div id="pnlCuentas">
                            <asp:Literal ID="Literal27" runat="server" Text="<%$ Resources:Recursos, DoASearchToDisplayData%>"></asp:Literal>
                        </div>
                        <input type="button" value="Eliminar Cuentas Seleccionadas" onclick="EliminarCuentas();" /> <br />
                        Ajuste Manual: <input type="text" id="txtAjusteOperadorDet" /><input type="button" value="Cambiar" onclick="CambiarAjuste()" />
                    </div>
                    <div id="tabExclusiones">
                        <h2><asp:Literal ID="Literal3" runat="server" Text="Cuentas Excluidas en la Meta"></asp:Literal></h2>
                        <div id="pnlExclusiones">
                            <asp:Literal ID="Literal15" runat="server" Text="<%$ Resources:Recursos, DoASearchToDisplayData%>"></asp:Literal>
                        </div>
                        <input type="button" value="Eliminar Exclusiones Seleccionadas" onclick="EliminarExclusiones();" />
                    </div>
                    <div id="tabInclusiones">
                        <h2><asp:Literal ID="Literal16" runat="server" Text="Cuentas Incluidas Manualmente en la Meta"></asp:Literal></h2>
                        <div id="pnlInclusiones">
                            <asp:Literal ID="Literal17" runat="server" Text="<%$ Resources:Recursos, DoASearchToDisplayData%>"></asp:Literal>
                        </div>
                        <input type="button" value="Eliminar Inclusiones Seleccionadas" onclick="EliminarInclusiones();" />
                        <br />
                        Código Persona:<input type="text" id="CodigoPersona" /><input type="button" onclick="AgregarCuentasDePersona();" value="Agregar Cuentas de esta Persona" /><br />
                        Código Cuenta: <input type="text" id="CodigoCuenta" /><input type="button" onclick="AgregarCuenta();" value="Agregar esta Cuenta" />
                    </div>
                    <div id="tabOpRanking">
                        <h2><asp:Literal ID="Literal20" runat="server" Text="Pagos Incluidos en la Meta"></asp:Literal></h2>
                        <div id="pnlPagosMeta">
                            <asp:Literal ID="Literal21" runat="server" Text="<%$ Resources:Recursos, PleaseWait%>"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>
            <div id="tabRanking">
                <h2><asp:Literal ID="Literal18" runat="server" Text="Informe de Cumplimiento de las Metas"></asp:Literal></h2>
                Escoja la meta a Consultar:
                <select id="cboMetaRanking">
                </select>
                <input type="hidden" runat="server" id="FechaRanking" />
                <input type="button" value="Consultar" onclick="ConsultarRanking()" />
                <asp:Button ID="btnExportarRanking" runat="server" Text="Exportar" OnClick="ExportarRanking" OnClientClick="Preparar();"></asp:Button>
                <div id="pnlRanking"></div>
            </div>
        </div>
    </div>
</asp:Content>
