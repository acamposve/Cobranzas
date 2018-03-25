<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlCampanas.aspx.cs" Inherits="Cobranzas.ctrlCampanas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlCampanas.aspx.js?ver=6" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
    <style type="text/css">
        .fecha
        {
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal ID="Literal59" runat="server" Text="<%$ Resources:Recursos, CampaignManagement %>"></asp:Literal></h2>
    <div id="Manejo">
        <div id="tabs">
            <ul>
                <li><a href="#tabListado" id="pstListado"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Listing%>" /></a></li>
                <li><a href="#tabDetalles" id="pstDetalles"><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Details%>" /></a></li>
            </ul>
            <div id="tabListado">
                <h3><asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
                <div id="pnlDistCampanas">
                    <asp:Literal ID="Literal53" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
                <input type="button" runat="server" value="<%$ Resources:Recursos, NewCampaign%>" onclick="Nuevo();" />
            </div>
            <div id="tabDetalles">
                <input type="hidden" id="idCampana" />
                <table style="width: 100%">
                    <tr>
                        <td style="text-align: right;">
                            <input type="button" id="btnGuardar" onclick="GuardarCampana();" class="btnGuardar32" title="Guardar" /><br />
                        </td>
                    </tr>
                </table>
                <div id="tabs2">
                    <ul>
                        <li><a href="#tabDatos" id="pstDatos"><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Data%>" /></a></li>
                        <li><a href="#tabCuentas" id="pstCuentas"><asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Accounts%>" /></a></li>
                        <li><a href="#tabCuentasInactivas" id="A1"><asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, InactiveAccounts%>" /></a></li>
                        <li><a href="#tabCuentasSinAsignar" id="pstCuentasSinAsignar"><asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, UnassignedAccounts%>" /></a></li>
                        <li><a href="#tabEstadisticas" id="pstEstadisticas"><asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, Statistics%>" /></a></li>
                        <li><a href="#tabOperaciones" id="pstOperaciones"><asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Recursos, Operations%>" /></a></li>
                    </ul>
                    <div id="tabDatos">
                        <table class="TablaDatos">
                            <tr>
                                <th>
                                    <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:Recursos, Name %>"></asp:Literal>
                                </th>
                                <td colspan="5">
                                    <input type="text" id="txtmNombreCampana" style="width: 90%;" />
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:Recursos, StartDate %>"></asp:Literal>
                                </th>
                                <td>
                                    <input type="text" id="txtmFechaInicio" class="fecha" />
                                </td>
                                <th>
                                    <asp:Literal ID="Literal12" runat="server" Text="<%$ Resources:Recursos, EstimatedEndDate %>"></asp:Literal>
                                </th>
                                <td>
                                    <input type="text" id="txtmFechaEstimadaFin" class="fecha" />
                                </td>
                                <th>
                                    <asp:Literal ID="Literal13" runat="server" Text="<%$ Resources:Recursos, EndDate %>"></asp:Literal>
                                </th>
                                <td style="min-width: 80px;">
                                    <span id="txtmFechaFin" />
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <asp:Literal ID="Literal14" runat="server" Text="<%$ Resources:Recursos, Flows %>"></asp:Literal>
                                </th>
                                <td colspan="2">
                                    <select id="cbomFlujo">
                                    </select>
                                </td>
                                <th>
                                    <asp:Literal ID="Literal15" runat="server" Text="<%$ Resources:Recursos, Step %>"></asp:Literal>
                                </th>
                                <td colspan="2">
                                    <select id="cbomPaso">
                                    </select>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <asp:Literal ID="Literal16" runat="server" Text="<%$ Resources:Recursos, TargetAmount %>"></asp:Literal>
                                </th>
                                <td>
                                    <input type="text" id="txtmMontoMeta" />
                                </td>
                                <th>
                                    <asp:Literal ID="Literal17" runat="server" Text="<%$ Resources:Recursos, Weight %>"></asp:Literal>
                                </th>
                                <td>
                                    <input type="text" id="txtmPeso" />
                                </td>
                                <th>
                                    <asp:Literal ID="Literal18" runat="server" Text="<%$ Resources:Recursos, TypeOfAssignment %>"></asp:Literal>
                                </th>
                                <td>
                                    <select id="cbomTipoAsignacion">
                                        <option value="0">Sin Asignación</option>
                                        <option value="1">Asignación Automática</option>
                                        <option value="2">Gestión Libre</option>
                                    </select>
                                </td>
                            </tr>
                            <tr>
                                <th>
                                    <asp:Literal ID="Literal19" runat="server" Text="<%$ Resources:Recursos, Owner%>"></asp:Literal>
                                </th>
                                <td>
                                    <select id="cbomDueno">
                                    </select>
                                </td>
                                <th>
                                    <asp:Literal ID="Literal20" runat="server" Text="<%$ Resources:Recursos, RuleOut%>"></asp:Literal>
                                </th>
                                <td>
                                    <select id="cbomReglaSalida">
                                    </select>
                                </td>
                                <th>
                                    <asp:Literal ID="Literal21" runat="server" Text="<%$ Resources:Recursos, Active%>"></asp:Literal>
                                </th>
                                <td>
                                    <input type="checkbox" id="chkmActiva" />
                                </td>
                            </tr>
                        </table>
                        <h2><asp:Literal ID="Literal22" runat="server" Text="<%$ Resources:Recursos, OperatorsAssignedToCampaign %>"></asp:Literal></h2>
                        <div id="pnlOperadoresCampana">
                            <asp:Literal ID="Literal54" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                        </div>
                        <select id="cbomOperadores">
                        </select><input type="button" class="btnAgregar32" onclick="AgregarOperador();" />
                    </div>
                    <div id="tabCuentas">
                        <h2><asp:Literal ID="Literal23" runat="server" Text="<%$ Resources:Recursos, ActiveAccountsInTheCampaign %>"></asp:Literal></h2>
                        <asp:Literal ID="Literal24" runat="server" Text="<%$ Resources:Recursos, Operator %>"></asp:Literal>:<select id="cboOperador"></select><input type="button" value="Consultar" onclick="ConsultarActivas();" />
                        <div id="pnlCuentas">
                            <asp:Literal ID="Literal27" runat="server" Text="<%$ Resources:Recursos, DoASearchToDisplayData%>"></asp:Literal>
                        </div>
                        <input type="button" value="Eliminar Cuentas Seleccionadas" onclick="EliminarCuentas();" />
                        <h3><asp:Literal ID="Literal25" runat="server" Text="<%$ Resources:Recursos, AssignOperator %>"></asp:Literal></h3>
                        <table class="TablaDatos">
                            <tr>
                                <th>
                                    <asp:Literal ID="Literal26" runat="server" Text="<%$ Resources:Recursos, Operator%>"></asp:Literal>
                                </th>
                                <td>
                                    <select id="cboOperadorAsignar">
                                    </select>
                                </td>
                                <td>
                                    <input type="button" onclick="AsignarOperador();" value="Asignar" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabCuentasInactivas">
                        <h2><asp:Literal ID="Literal28" runat="server" Text="<%$ Resources:Recursos, DormantAccountsInCampaign %>"></asp:Literal></h2>
                        <asp:Literal ID="Literal29" runat="server" Text="<%$ Resources:Recursos, StartDate%>"></asp:Literal>:<input type="text" id="dtpFechaIni" class="fecha" /> <asp:Literal ID="Literal38" runat="server" Text="<%$ Resources:Recursos, EndDate%>"></asp:Literal>:<input type="text" id="dtpFechaFin" class="fecha" /><input type="button" value="Consultar" onclick="ConsultarInactivas();" />
                        <div id="pnlCuentasInactivas">
                            <asp:Literal ID="Literal55" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                        </div>
                    </div>
                    <div id="tabCuentasSinAsignar">
                        <h2><asp:Literal ID="Literal30" runat="server" Text="<%$ Resources:Recursos, UnassignedAccounts %>"></asp:Literal></h2>
                        <div id="pnlCuentasSinAsignar">
                            <asp:Literal ID="Literal56" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                        </div>
                        <input type="button" value="Asignar a la Campaña" onclick="AsignarCuentas();" />
                    </div>
                    <div id="tabEstadisticas">
                        <asp:Literal ID="Literal31" runat="server" Text="<%$ Resources:Recursos, CampaignStatistics%>"></asp:Literal>
                    </div>
                    <div id="tabOperaciones">
                        <asp:Literal ID="Literal32" runat="server" Text="<%$ Resources:Recursos, OperationsOfTheCampaign%>"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="Wizard">
        <div id="Paso1" style="display: none;" class="Paso">
            <h3><asp:Literal ID="Literal33" runat="server" Text="<%$ Resources:Recursos, Step1 %>"></asp:Literal></h3>
            <asp:Literal ID="Literal34" runat="server" Text="<%$ Resources:Recursos, Name%>"></asp:Literal> <input type="text" id="txtNombreCampana" /><br />
            <h3><asp:Literal ID="Literal35" runat="server" Text="<%$ Resources:Recursos, Step2 %>"></asp:Literal></h3>
            <asp:Literal ID="Literal36" runat="server" Text="<%$ Resources:Recursos, StartDate%>"></asp:Literal> <input type="text" id="dtpFechaInicio" class="fecha" /> <asp:Literal ID="Literal37" runat="server" Text="<%$ Resources:Recursos, EstimatedEndDate%>"></asp:Literal> <input type="text" id="dtpFechaEstimadaFin" class="fecha" />
            <h3><asp:Literal ID="Literal39" runat="server" Text="<%$ Resources:Recursos, Step3 %>"></asp:Literal></h3>
            <select id="cboReglas">
            </select>
            <h3><asp:Literal ID="Literal40" runat="server" Text="<%$ Resources:Recursos, Step4%>"></asp:Literal></h3>
            <div id="pnlOperadores">
            </div>
            <div style="display: none;">
                <select id="cboOperadores" multiple="multiple">
                </select>
                <select id="cboOperadores2">
                </select>
            </div>
            <h3><asp:Literal ID="Literal41" runat="server" Text="<%$ Resources:Recursos, Step5%>"></asp:Literal></h3>
            <select id="cboModosDistribucion">
            </select>
            <br />
            <input type="button" runat="server" value="<%$ Resources:Recursos, Previous %>" onclick="$('#Wizard').hide();$('#Manejo').fadeIn();" /> <input type="button" runat="server" value="<%$ Resources:Recursos, Next %>" onclick="CambiarPaso(1,2);" />
        </div>
        <div id="Paso2" style="display: none;" class="Paso">
            <h3><asp:Literal ID="Literal42" runat="server" Text="<%$ Resources:Recursos, Step6%>"></asp:Literal></h3>
            <asp:Literal ID="Literal43" runat="server" Text="<%$ Resources:Recursos, Flows%>"></asp:Literal>:<select id="cboFlujo"></select>
            <asp:Literal ID="Literal44" runat="server" Text="<%$ Resources:Recursos, Step%>"></asp:Literal>:<select id="cboPaso"></select>
            <h3><asp:Literal ID="Literal45" runat="server" Text="<%$ Resources:Recursos, Step7%>"></asp:Literal></h3>
            <div id="pnlDistCuentasReglas" class="contTabla">
            </div>
            <h3><asp:Literal ID="Literal46" runat="server" Text="<%$ Resources:Recursos, Step8%>"></asp:Literal></h3>
            <asp:Literal ID="Literal47" runat="server" Text="<%$ Resources:Recursos, Goal%>"></asp:Literal>: <input type="text" id="txtMontoMeta" />
            <h3><asp:Literal ID="Literal48" runat="server" Text="<%$ Resources:Recursos, Step9%>"></asp:Literal></h3>
            <select id="cboTipoAsignacion">
                <option value="0"><asp:Literal ID="Literal49" runat="server" Text="<%$ Resources:Recursos, NotAssign%>"></asp:Literal></option>
                <option value="1"><asp:Literal ID="Literal50" runat="server" Text="<%$ Resources:Recursos, AutomaticAssignment%>"></asp:Literal></option>
                <option value="2"><asp:Literal ID="Literal51" runat="server" Text="<%$ Resources:Recursos, FreeManagement%>"></asp:Literal></option>
            </select>
            <h3><asp:Literal ID="Literal52" runat="server" Text="<%$ Resources:Recursos, Step10%>"></asp:Literal></h3>
            <select id="cboReglaSalida">
            </select>
            <br />
            <input type="button" runat="server" value="<%$ Resources:Recursos, Previous %>" onclick="CambiarPaso(2,1);" /> <input type="button" runat="server" value="<%$ Resources:Recursos, Finish %>" onclick="return Finalizar();" />
        </div>
    </div>
</asp:Content>
