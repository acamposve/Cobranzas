<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Gestion.aspx.cs" Inherits="Cobranzas.Gestion" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="Server">
    <script src="/Scripts/Editor.js" type="text/javascript"></script>
    <script src="Gestion.aspx.js?ver=14" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="body" runat="Server">
    <header>
        <div id="Encabezado">
            <input type="hidden" id="idOperadorAct" runat="server" />
            <input type="hidden" id="idOperadorLog" runat="server" />
            <input type="hidden" id="idOperador" runat="server" />
            <input type="hidden" id="TipoOperador" runat="server" />
            <input type="hidden" id="Hora" />
        </div>
        <div id="Salir">
            <a class="btnSalir" href="Default.aspx?salir=1">
                <asp:Literal ID="Literal39" runat="server" Text="<%$ Resources:Recursos, Exit %>"></asp:Literal></a>
        </div>
        <div id="Basicos">
            <table>
                <tr>
                    <td style="text-align: right;">
                        <span id="Contador" class="Temporizador"></span>
                        <input type="button" onclick="PlayGestion()" class="btnPlay" id="btnPlay" title="Iniciar Gestión Automática" /><input type="button" onclick="    PauseGestion()" class="btnPause" id="btnPause" style="display: none;" title="Pausar Gestión Automática" /><input type="button" onclick="    StopGestion();" class="btnStop" id="btnStop" style="display: none;" title="Detener Gestión Automática" />
                    </td>
                    <td>
                        <div id="pnlErrores" style="display: none;">
                            Nro de Errores: <span id="nroErrores"></span>
                            <br />
                            <span class="Vinculo" onclick="EnviarReporte();">Enviar Reporte de Errores</span>
                        </div>
                    </td>
                    <td>
                        <div id="minipnlSuplantacion" style="display: none;">
                            <asp:Literal ID="litSuplantando" runat="server" Text="<%$ Resources:Recursos, Supplanting %>"></asp:Literal>: <span id="lblSuplantando"></span>
                        </div>
                        <div id="minipnlSupervision" style="display: none;">
                            <asp:Literal ID="litSupervisando" runat="server" Text="<%$ Resources:Recursos, Supervising %>"></asp:Literal>: <span id="lblSupervisando"></span>
                        </div>
                    </td>
                    <td>
                        <asp:Literal ID="litUser" runat="server" Text="<%$ Resources:Recursos, User %>"></asp:Literal>:
                        <asp:Label ID="lblUser" runat="server" Text=""></asp:Label><br />
                        <asp:Literal ID="litDate" runat="server" Text="<%$ Resources:Recursos, Date %>"></asp:Literal>:
                        <asp:Label ID="lblDate" runat="server" Text=""></asp:Label><br />
                        <asp:Literal ID="litTime" runat="server" Text="<%$ Resources:Recursos, Time %>"></asp:Literal>:
                        <asp:Label ID="lblTime" runat="server" Text=""></asp:Label><br />
                        <asp:DropDownList ID="ddlIdioma" runat="server" AutoPostBack="True" onchange="setCookieCulture();">
                            <asp:ListItem Text="<%$ Resources:Recursos, Spanish %>" Value="es-VE"></asp:ListItem>
                            <asp:ListItem Text="<%$ Resources:Recursos, English %>" Value="en-US"></asp:ListItem>
                            <asp:ListItem Text="<%$ Resources:Recursos, Portuguese %>" Value="pt-BR"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
    </header>
    <div id="Avisos">
        <table style="width: 100%">
            <tr>
                <td>
                    <h2>
                        <asp:Literal ID="Literal55" runat="server" Text="<%$ Resources:Recursos, Reminders %>"></asp:Literal></h2>
                </td>
                <td style="text-align: right;">
                    <input type="button" value="Refrescar" onclick="Avisos_Actualizar();" />
                </td>
            </tr>
        </table>
        <input type="hidden" id="idAviso" value="0" />
        <div style="position: absolute; left: 0; top: 40px; bottom: 0px; right: 0px; overflow: auto;">
            <table id="tblAvisos" class="Tabla tblAvisos">
                <tr>
                    <th>
                        <asp:Literal ID="Literal40" runat="server" Text="<%$ Resources:Recursos, Time %>"></asp:Literal>:
                    </th>
                    <th>
                        <asp:Literal ID="Literal41" runat="server" Text="<%$ Resources:Recursos, Reminder %>"></asp:Literal>:
                    </th>
                    <th>
                        <asp:Literal ID="Literal42" runat="server" Text="<%$ Resources:Recursos, User %>"></asp:Literal>:
                    </th>
                    <th>
                        <asp:Literal ID="Literal43" runat="server" Text="<%$ Resources:Recursos, Person %>"></asp:Literal>:
                    </th>
                </tr>
            </table>
            <input type="button" id="btnNuevoAviso" runat="server" onclick="Emergente({ url: '/Emergentes/ctrlNuevoAviso.aspx' });" value="<%$ Resources:Recursos, NewReminder %>" />
        </div>
    </div>
    <div id="Cuerpo">
        <div id="pnlGestion">
            <ul>
                <li runat="server" id="liSistema">

                    <a href="#tabSistema" id="pstSistema">
                        <asp:Literal ID="Literal76" runat="server" Text="<%$ Resources:Recursos, System %>"></asp:Literal></a>




                </li>
                <li runat="server" id="liSupervision"><a href="#tabSupervision" id="pstSupervision">
                    <asp:Literal ID="Literal53" runat="server" Text="<%$ Resources:Recursos, Supervision %>"></asp:Literal></a></li>
                <li runat="server" id="liDistribucion"><a href="#tabDistribucion" id="pstDistribucion">
                    <asp:Literal ID="Literal54" runat="server" Text="<%$ Resources:Recursos, Distribution %>"></asp:Literal></a></li>
                <li><a href="#tabIndicadores" id="pstIndicadores">
                    <asp:Literal runat="server" Text="<%$ Resources:Recursos, DashBoard %>"></asp:Literal></a></li>
                <li><a href="#tabBandejaEntrada" id="pstBandejaEntrada">
                    <asp:Literal runat="server" Text="<%$ Resources:Recursos, InBox %>"></asp:Literal></a></li>
                <li><a href="#tabCartera" id="pstCartera">
                    <asp:Literal runat="server" Text="<%$ Resources:Recursos, Portfolio %>"></asp:Literal></a></li>
                <li><a href="#tabGestion" id="pstGestion">
                    <asp:Literal runat="server" Text="<%$ Resources:Recursos, Management %>"></asp:Literal></a></li>
            </ul>
            <div id="tabSistema" class="PanelGestion" runat="server">
                <!-- [-----------------------------------------------------------------------------------------------------------------------------------------Sistema]-->
                <h2>Herramientas para diagnóstico del sistema</h2>
                <table style="width: 100%">
                    <tr>
                        <td>
                            <input type="button" value="Ejecutar Pruebas... " onclick="EjecutarPruebas();" />
                            <h2>Servidor Web</h2>
                            <table>
                                <tr>
                                    <th>TiempoRespuesta:
                                    </th>
                                    <td>
                                        <span id="SWTiempoRespuesta"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>TiempoSubida (50K)
                                    </th>
                                    <td>
                                        <span id="SWTiempoSubida"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>TiempoBajada (50K)
                                    </th>
                                    <td>
                                        <span id="SWTiempoBajada"></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <h2>Base de Datos</h2>
                            <table>
                                <tr>
                                    <th>Ping:
                                    </th>
                                    <td>
                                        <span id="BDPing"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <span onclick="ConsultarBloqueos();">Bloqueos: </span>
                                    </th>
                                    <td>
                                        <div id="BDBloqueos">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Tiempo de Conexión:
                                    </th>
                                    <td>
                                        <span id="BDTiempoConexion"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Tiempo de Transporte: (5K + 5K)
                                    </th>
                                    <td>
                                        <span id="BDTiempoTransporte"></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <h2>Correo:</h2>
                            <table>
                                <tr>
                                    <th>Último Proceso Ejecutado:
                                    </th>
                                    <td>
                                        <span id="CoUltimpoProceso"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Último Correo Entrante:
                                    </th>
                                    <td>
                                        <span id="CoUltimoCorreoEntrante"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Cantidad de Correos Pendientes Por Enviar:
                                    </th>
                                    <td>
                                        <span id="CoPendientes"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Último Correo Enviado:
                                    </th>
                                    <td>
                                        <span id="CoUltimoCorreoEnviado"></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <h2>Transporte</h2>
                            <table>
                                <tr>
                                    <th>Actualizar:
                                    </th>
                                    <td>
                                        <span id="TrActualizar"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Traer:
                                    </th>
                                    <td>
                                        <span id="TrTraer"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Llevar:
                                    </th>
                                    <td>
                                        <span id="TrLlevar"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Distribuir:
                                    </th>
                                    <td>
                                        <span id="TrDistribuir"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Avanzar:
                                    </th>
                                    <td>
                                        <span id="TrAvanzar"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th>Procesar:
                                    </th>
                                    <td>
                                        <span id="TrProcesar"></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <h2>Orígenes:</h2>
                            <div id="pnlOrigenes">
                            </div>
                        </td>
                        <td colspan="2">
                            <h2>Centrales Ip:</h2>
                            <div id="pnlResultadoCentralesIp">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <h2>Ejecutar Comandos En Base de Datos:</h2>
                            <textarea rows="20" cols="100" id="txtBDComando"></textarea>
                            <input type="button" value="Ejecutar" onclick="EjecutarComando()" />
                            <div id="pnlResultadoBDComando">
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabSupervision" class="PanelGestion">
                <!-- [-----------------------------------------------------------------------------------------------------------------------------------------Supervisión]-->
                <table style="width: 100%">
                    <tr>
                        <td style="vertical-align: top;">
                            <h2>
                                <asp:Literal ID="Literal56" runat="server" Text="<%$ Resources:Recursos, StaffUnderSupervision %>"></asp:Literal></h2>
                            <div id="pnlSupervision">
                            </div>
                            <h2>
                                <asp:Literal ID="Literal57" runat="server" Text="<%$ Resources:Recursos, OperatorsThatICanImpersonate %>"></asp:Literal></h2>
                            <div id="pnlOperadoresYoSuplantar">
                            </div>
                            <input id="Button5" type="button" runat="server" onclick="Revertir()" value="<%$ Resources:Recursos, Revert %>" />
                            <h2>
                                <asp:Literal ID="Literal83" runat="server" Text="<%$ Resources:Recursos, OperatorsThatCanImpersonateMe %>"></asp:Literal></h2>
                            <div id="pnlOperadoresSuplantarme">
                            </div>
                            <input type="button" runat="server" value="Agregar Suplente" onclick="Emergente({ url: '/Emergentes/ctrlSuplente.aspx?idOperador=' + idOpL() });" />
                            <h2>
                                <asp:Literal ID="Literal91" runat="server" Text="<%$ Resources:Recursos, ActivityForSupervisors %>"></asp:Literal></h2>
                            <div id="pnlActividades">
                                <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlOperadores.aspx?idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal69" runat="server" Text="<%$ Resources:Recursos, Operators %>" />
                                </div>
                                <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlMetas.aspx?idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal84" runat="server" Text="<%$ Resources:Recursos, Goals %>" />
                                </div>
                                <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlExclusiones.aspx?idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal88" runat="server" Text="<%$ Resources:Recursos, Exclusions %>" />
                                </div>
                                <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlPlantillas.aspx?idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal90" runat="server" Text="<%$ Resources:Recursos, Templates %>" />
                                </div>
                                <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlAdministrarTelefono.aspx?idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal92" runat="server" Text="<%$ Resources:Recursos, PhoneManager %>" />
                                </div>
                            </div>
                            <h2>
                                <asp:Literal ID="Literal58" runat="server" Text="<%$ Resources:Recursos, ReportsForSupervisors %>"></asp:Literal></h2>
                            <div id="pnlReportes">
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptGestiones.aspx?Supervisados=1&idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal34" runat="server" Text="<%$ Resources:Recursos, Managements %>" />
                                </div>
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptAnalisisPorCliente.aspx?Supervisados=1&idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal38" runat="server" Text="<%$ Resources:Recursos, OutstandingAccounts %>" />
                                </div>
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptLogins.aspx?Supervisados=1&idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal48" runat="server" Text="Logins" />
                                </div>
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptPersonasGestionadas.aspx?Supervisados=1&idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal49" runat="server" Text="<%$ Resources:Recursos, PeopleManaged %>" />
                                </div>
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptLlamadas.aspx?Supervisados=1&idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal74" runat="server" Text="<%$ Resources:Recursos, Calls %>" />
                                </div>
                                
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptCompromisos.aspx?Supervisados=1&idOperador='+idOpA(), modal:false});">
                                    <asp:Literal ID="Literal93" runat="server" Text="<%$ Resources:Recursos, Commitments %>" />
                                </div>
                                                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptRankingEjecutivo.aspx?idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal95" runat="server" Text="Ranking por Ejecutivo" />
                            </div>

                                
                            </div>
                            <h2>
                                <asp:Literal ID="Literal59" runat="server" Text="<%$ Resources:Recursos, GeneralReports %>"></asp:Literal></h2>
                            <div id="pnlReportesGenerales">
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptAnalisisPorCliente.aspx?General=1&idOperador='+idOpA()});">
                                    <asp:Literal ID="Literal51" runat="server" Text="<%$ Resources:Recursos, OutstandingAccounts %>" />
                                </div>
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptCorreosAdministrador.aspx'});">
                                    <asp:Literal ID="Literal73" runat="server" Text="<%$ Resources:Recursos, MailFromAdministrator %>" />
                                </div>
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptGestionesCuentas.aspx'});">
                                    <asp:Literal ID="Literal77" runat="server" Text="Gestiones por Cuentas" />
                                </div>
                                <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptClienteCuentas.aspx'});">
                                    <asp:Literal ID="Literal78" runat="server" Text="Cuentas del Cliente" />
                                </div>
                            </div>
                        </td>
                        <td style="vertical-align: top;">
                            <h2>
                                <asp:Literal ID="Literal60" runat="server" Text="<%$ Resources:Recursos, ManagementsOfTheDay %>"></asp:Literal></h2>
                            <asp:Image runat="server" ID="chrtGestionesSupervision" ImageUrl="/Img/Temp/Nuevo.png" Width="600px" Height="300px" />
                            <h2>
                                <asp:Literal ID="Literal61" runat="server" Text="<%$ Resources:Recursos, ManagementsPerWeek %>"></asp:Literal></h2>
                            <asp:Image runat="server" ID="chrtGestionesPorSemana" ImageUrl="/Img/Temp/Nuevo.png" Width="600px" Height="400px" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabDistribucion" class="PanelGestion">
                <!-- [-----------------------------------------------------------------------------------------------------------------------------------------Distribución]-->
                <div class="divSeccion">
                    <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlReglas.aspx?idOperador='+idOpA(), width:600,height:500});">
                        <asp:Literal ID="Literal63" runat="server" Text="<%$ Resources:Recursos, RulesOfDistribution %>" />
                    </div>
                    <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlFlujos.aspx?idOperador='+idOpA(),minWidth:720,minHeight:600});">
                        <asp:Literal ID="Literal64" runat="server" Text="<%$ Resources:Recursos, Workflows %>" />
                    </div>
                    <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlCampanas.aspx?idOperador='+idOpA()});">
                        <asp:Literal ID="Literal65" runat="server" Text="<%$ Resources:Recursos, Campaigns %>" />
                    </div>
                    <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlDistOperador.aspx?idOperador='+idOpA()});">
                        <asp:Literal ID="Literal66" runat="server" Text="<%$ Resources:Recursos, AutomaticDistributionOfAccountsToOperators %>" />
                    </div>
                    <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlDistCampanas.aspx?idOperador='+idOpA()});">
                        <asp:Literal ID="Literal67" runat="server" Text="<%$ Resources:Recursos, AutomaticDistributionOfAccountsToCampaign %>" />
                    </div>
                    <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlCuentasSinAsignar.aspx?idOperador='+idOpA()});">
                        <asp:Literal ID="Literal68" runat="server" Text="<%$ Resources:Recursos, UnassignedAccounts %>" />
                    </div>
                    <div class="btnConfig" onclick="Emergente({url:'/Emergentes/ctrlStatus.aspx?idOperador='+idOpA()});">
                        <asp:Literal ID="Literal70" runat="server" Text="<%$ Resources:Recursos, Status %>" />
                    </div>
                </div>
            </div>
            <div id="tabIndicadores" class="PanelGestion">
                <!-- [-----------------------------------------------------------------------------------------------------------------------------------------Indicadores]-->
                <table style="width: 100%; border-collapse: collapse;">
                    <tr>
                        <td style="vertical-align: top;">
                            <h2>
                                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Managements_Day%>" /></h2>
                            <asp:Image runat="server" ID="chrtGestiones" ImageUrl="/Img/Temp/Nuevo.png" Width="500px" Height="300px" />
                        </td>
                        <td style="vertical-align: top;">
                            <h2>
                                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Indicators%>" /></h2>
                            <table class="TablaDatos">
                                <tr>
                                    <th>
                                        <asp:Literal ID="litIndicador" runat="server" Text="<%$ Resources:Recursos, Indicator%>" />
                                    </th>
                                    <th>
                                        <asp:Literal ID="litMeta" runat="server" Text="<%$ Resources:Recursos, Goal%>" />
                                    </th>
                                    <th>
                                        <asp:Literal ID="litReal" runat="server" Text="<%$ Resources:Recursos, Real%>" />
                                    </th>
                                    <td style="width: 122px">
                                        <div style="width: 100px; background-color: #0000FF; color: Gray;">
                                            %
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <asp:Literal ID="litManagements" runat="server" Text="<%$ Resources:Recursos, Managements%>" />
                                    </th>
                                    <td>&gt;<span id="lblMetaGestiones"></span>
                                    </td>
                                    <td>
                                        <span id="lblRealGestiones"></span>
                                    </td>
                                    <td>
                                        <div id="barGestiones" style="width: 0px; color: Gray;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <asp:Literal ID="litPersonManagements" runat="server" Text="<%$ Resources:Recursos, PersonManagements%>" />
                                    </th>
                                    <td>&gt;<span id="lblMetaGestionesPersonas"></span>
                                    </td>
                                    <td>
                                        <span id="lblRealGestionesPersonas"></span>
                                    </td>
                                    <td>
                                        <div id="barGestionesPersonas" style="width: 0px; color: Gray;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <asp:Literal ID="litMonto" runat="server" Text="<%$ Resources:Recursos, Amount_Collect%>" />
                                    </th>
                                    <td>&gt;<span id="lblMetaMonto"></span> USD
                                    </td>
                                    <td>
                                        <span id="lblRealMonto"></span>USD
                                    </td>
                                    <td>
                                        <div id="barMonto" style="width: 0px; color: Gray;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <asp:Literal ID="litTimeManagements" runat="server" Text="<%$ Resources:Recursos, Time_Managements %>"></asp:Literal>
                                    </th>
                                    <td>&gt;<span id="lblMetaTiempo"></span>
                                    </td>
                                    <td>
                                        <span id="lblRealTiempo"></span>
                                    </td>
                                    <td>
                                        <div id="barTiempo" style="width: 0px; color: Gray;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <asp:Literal ID="litTiempoInactivo" runat="server" Text="<%$ Resources:Recursos, Idle_Time %>"></asp:Literal>
                                    </th>
                                    <td>&lt;<span id="lblMetaTiempoInactivo"></span>
                                    </td>
                                    <td>
                                        <span id="lblRealTiempoInactivo"></span>
                                    </td>
                                    <td>
                                        <div id="barInactivo" style="width: 0px; color: Gray;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th>
                                        <asp:Literal ID="litMTO" runat="server" Text="<%$ Resources:Recursos, MTO %>"></asp:Literal>
                                    </th>
                                    <td>&lt;<span id="lblMetaTMO"></span>
                                    </td>
                                    <td>
                                        <span id="lblRealTMO"></span>
                                    </td>
                                    <td>
                                        <div id="barTMO" style="width: 0px; color: Gray;">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="2">
                                        <asp:Literal ID="litCuentas" runat="server" Text="<%$ Resources:Recursos, AssignedAccounts %>"></asp:Literal>
                                    </th>
                                    <td colspan="2">
                                        <span id="lblCuentasAsignadas"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="2">
                                        <asp:Literal ID="litPersonas" runat="server" Text="<%$ Resources:Recursos, AssignedPersons %>"></asp:Literal>
                                    </th>
                                    <td colspan="2">
                                        <span id="lblPersonasAsignadas"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="2">
                                        <asp:Literal ID="Literal79" runat="server" Text="Tiempo en Llamada"></asp:Literal>
                                    </th>
                                    <td colspan="2">
                                        <span id="lblTiempoEnLlamada"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="2">
                                        <asp:Literal ID="Literal80" runat="server" Text="Llamadas Realizadas"></asp:Literal>
                                    </th>
                                    <td colspan="2">
                                        <span id="lblLlamadasRealizadas"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="2">
                                        <asp:Literal ID="Literal81" runat="server" Text="Llamadas Contestadas"></asp:Literal>
                                    </th>
                                    <td colspan="2">
                                        <span id="lblLlamadasContestadas"></span>
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="2">
                                        <asp:Literal ID="Literal82" runat="server" Text="Llamadas No Contestadas"></asp:Literal>
                                    </th>
                                    <td colspan="2">
                                        <span id="lblLlamadasNoContestadas"></span>
                                    </td>
                                </tr>
                            </table>
                            <h2>
                                <asp:Literal ID="Literal85" runat="server" Text="<%$ Resources:Recursos, Goals %>"></asp:Literal></h2>
                            <div id="pnlMetas">
                            </div>
                            <h2><asp:Literal ID="Literal62" runat="server" Text="<%$ Resources:Recursos, Reports %>"></asp:Literal></h2>
                             <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptGestiones.aspx?Supervisados=0&idOperador='+idOp()});">
                                <asp:Literal ID="Literal33" runat="server" Text="<%$ Resources:Recursos, Managements %>" />
                            </div>
                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptAvisos.aspx?Supervisados=0&idOperador='+idOp()});">
                                <asp:Literal ID="Literal37" runat="server" Text="<%$ Resources:Recursos, Reminders %>" />
                            </div>
                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptPagosCobrador.aspx?idOperador='+idOp()});">
                                <asp:Literal ID="Literal45" runat="server" Text="<%$ Resources:Recursos, PaymentsToOperator %>" />
                            </div>
                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptLogins.aspx?Supervisados=0&idOperador='+idOp()});">
                                <asp:Literal ID="Literal46" runat="server" Text="Logins" />
                            </div>
                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptPersonasGestionadas.aspx?Supervisados=0&idOperador='+idOp()});">
                                <asp:Literal ID="Literal47" runat="server" Text="<%$ Resources:Recursos, PeopleManaged %>" />
                            </div>
                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptCompromisos.aspx?Supervisados=0&idOperador='+idOp(), modal:false});">
                                <asp:Literal ID="Literal50" runat="server" Text="<%$ Resources:Recursos, Commitments %>" />
                            </div>
                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptAnalisisPorCliente.aspx?Supervisados=0&idOperador='+idOp()});">
                                <asp:Literal ID="Literal52" runat="server" Text="<%$ Resources:Recursos, OutstandingAccounts %>" />
                            </div>
                            <div class="btnReporte" onclick="Emergente({url:'/Emergentes/rptLlamadas.aspx?Supervisados=0&idOperador='+idOpA()});">
                                <asp:Literal ID="Literal75" runat="server" Text="<%$ Resources:Recursos, Calls %>" />
                            </div>





                        </td>
                    </tr>
                </table>
            </div>
            <div id="tabBandejaEntrada" class="PanelGestion">
                <!-- [-----------------------------------------------------------------------------------------------------------------------------------------Bandeja de Entrada]-->
                <table style="width: 100%; border-collapse: collapse;">
                    <tr>
                        <td>
                            <h2>
                                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, UnclassifiedMail%>" />
                                (<span id="Cantidad"></span>)</h2>
                        </td>
                        <td style="text-align: right;">
                            <input type="hidden" id="idCorreo" />
                            <input type="button" runat="server" value="<%$ Resources:Recursos, Update%>" onclick="RefrescarCorreos();" />
                            <input type="button" id="idFiltro" runat="server" value="<%$ Resources:Recursos, Filters%>" onclick="    AdministrarFiltros();" />
                            <input id="Button4" type="button" runat="server" value="Actualizar Contraseña de Correo" onclick="    ActualizarContrasena(true);" />
                        </td>
                    </tr>
                </table>
                <div id="pnlCorreos">
                </div>
                <table class="TablaDatos">
                    <tr>
                        <th>
                            <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, AssignPerson%>" />:
                        </th>
                        <td>Filtro:<input type="text" id="txtFiltroPersona" onkeyup="FiltrarPersona()" />
                            <select id="cboaPersona">
                            </select>
                        </td>
                        <td>
                            <input type="button" runat="server" value="<%$ Resources:Recursos, Assign%>" onclick="AsignarCorreosAPersona(false);" />
                            <input type="button" runat="server" value="<%$ Resources:Recursos, CreateRule%>" onclick="    AsignarCorreosAPersona(true);" style="display: none;" />
                            <input type="button" runat="server" value="<%$ Resources:Recursos, MarkAsPersonal %>" onclick="    MarcarComoPersonal();" />
                        </td>
                    </tr>
                </table>
                <h2>
                    <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, ClassifiedButNotRead%>"></asp:Literal>
                    (<span id="Cantidad2"></span>)</h2>
                <div id="pnlCorreosClasificadosNoLeidos">
                </div>
            </div>
            <div id="tabCartera" class="PanelGestion">
                <!-- [-----------------------------------------------------------------------------------------------------------------------------------------Cartera]-->
                <input id="btnIniciarGestion" runat="server" type="button" class="btnAceptar32Texto" onclick="FinalizarGestion();" value="<%$ Resources:Recursos, BeginManagement %>" />
                <table style="width: 100%;">
                    <tr>
                        <td>
                            <h2>
                                <asp:Literal ID="Literal32" Text="<%$ Resources:Recursos, AssignedPersons %>" runat="server" /></h2>
                        </td>
                        <td style="text-align: right">
                            <input type="checkbox" id="chkIncluyeAutomatico" runat="server" onclick="ActualizarCartera();" />
                            Incluir Gestiones Automáticas
                            <br />
                            <input type="button" value="Otras Personas" onclick="Emergente({ url: 'Emergentes/extPersonas.aspx' })" /><input id="Button3" type="button" runat="server" value="<%$ Resources:Recursos, Download %>" onclick="    Cartera_Descargar();" />
                            <input type="button" runat="server" value="<%$ Resources:Recursos, Update %>" onclick="ActualizarCartera();" />
<!--<br />                            
                            <input type="button" runat="server" value="Asignar de Primero" onclick="InsertarColas(1);" /> -->
                            
                            
                         
                        </td>
                    </tr>
                </table>
                <div id="pnlCartera">
                    Presione Actualizar para Mostrar la Cartera...
                </div>
            </div>
            <div id="tabGestion" class="PanelGestion">
            
                <!-- [-----------------------------------------------------------------------------------------------------------------------------------------Gestión]-->
                <asp:HiddenField ID="idPersona" runat="server" Value="" />
                <input type="hidden" id="idAvisoPersona" />
                <div class="divSeccion">
                    <table style="width: 100%">
                        <tr>
                            <td style="text-align: right;">
                                <input runat="server" type="button" class="btnAceptar32Texto" onclick="FinalizarGestion()" value="<%$ Resources:Recursos, EndManagement %>" />
                            </td>
                            <!--td style="text-align: right; width: 100px;">
                            <img src="Img/Cancelar48.png" />
                        </td-->
                        </tr>
                    </table>
                    <div id="_datosPersona" class="EncSeccion">
                        <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, PersonData %>"></asp:Literal>
                    </div>
                    <div id="datosPersona" class="Seccion">
                        <table style="width: 100%; border-collapse: collapse;">
                            <tr>
                                <td style="vertical-align: top;">
                                    <table class="TablaDatos">
                                        <tr>
                                            <th>
                                                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Code %>"></asp:Literal>
                                            </th>
                                            <td>
                                                <asp:Label ID="lblPeCodigo" runat="server" Text=""></asp:Label>
                                            </td>
                                            <th>
                                                <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, Rif %>"></asp:Literal>
                                            </th>
                                            <td>
                                                <asp:Label ID="lblPeRif" runat="server" Text=""></asp:Label>
                                                <!--Warning-->
                                                <input type="button" onclick="BuscarRNC();" value="RNC" />
                                                <input type="button" onclick="    Seniat();" value="Seniat" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Recursos, Name %>"></asp:Literal>
                                            </th>
                                            <td colspan="3">
                                                <asp:Label ID="lblPeNombre" runat="server" Text=""></asp:Label>
                                                <input type="button" onclick="BuscarGoogle();" value="Google" />
                                                <input type="button" onclick="    BuscarPAC();" value="Páginas Amarillas" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:Recursos, TaxAddress %>"></asp:Literal>
                                            </th>
                                            <td colspan="3">
                                                <asp:Label ID="lblPeDireccion" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <asp:Literal ID="Literal35" runat="server" Text="<%$ Resources:Recursos, DeliveryAddress %>"></asp:Literal>
                                            </th>
                                            <td colspan="3">
                                                <asp:Label ID="lblPeDireccionEntrega" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>URL:
                                            </th>
                                            <td colspan="3">
                                                <asp:HyperLink ID="lnkPeURL" runat="server" NavigateUrl="about:blank" Text="" Target="_blank"></asp:HyperLink>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:Recursos, Email %>"></asp:Literal>
                                            </th>
                                            <td>
                                                <asp:Label ID="lblPeEmail" runat="server" CssClass="Correo" Text=""></asp:Label>
                                            </td>
                                            <th>
                                                <asp:Literal ID="Literal12" runat="server" Text="<%$ Resources:Recursos, Country %>"></asp:Literal>
                                                <input type="hidden" id="idPais" />
                                            </th>
                                            <td>
                                                <asp:Label ID="lblPePais" runat="server" Text=""></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <th>
                                                <asp:Literal ID="Literal36" runat="server" Text="<%$ Resources:Recursos, Contact %>"></asp:Literal>
                                            </th>
                                            <td>
                                                <asp:Label ID="lblPeContacto" runat="server" Text=""></asp:Label>
                                            </td>
                                            <td colspan="2" rowspan="3">
                                                <div id="divPeDatos">
                                                </div>
                                                <h2>
                                                    <asp:Literal ID="Literal72" runat="server" Text="<%$ Resources:Recursos, Comments %>"></asp:Literal><input type="button" class="btnAgregar32" onclick="EditarComentario();" /></h2>
                                                <span id="lblPeComentarios"></span>
                                            </td>
                                        </tr>
                                        <tr style="background-color: transparent;">
                                            <th>
                                                <asp:Literal ID="Literal14" runat="server" Text="<%$ Resources:Recursos, Phones %>"></asp:Literal>
                                                <a href="sip:#113" class="Telefono">113</a>
                                                <input type="button" class="btnAgregar32" onclick="AgregarTelefono()" /><input type="button" class="btnEliminar32" onclick="    EliminarTelefono()" />
                                            </th>
                                            <td colspan="1" id="lstPeTelefonos"></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <h2>
                                                    <asp:Literal ID="Literal13" runat="server" Text="<%$ Resources:Recursos, Documents %>"></asp:Literal></h2>
                                                <div class="Soportes" id="pnlPeSoportes">
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="4">
                                                <h2>
                                                    <asp:Literal ID="Literal44" runat="server" Text="<%$ Resources:Recursos, Reminders %>"></asp:Literal></h2>
                                                <div id="pnlPeAvisos">
                                                </div>
                                                <h2>
                                                    <asp:Literal ID="Literal71" runat="server" Text="<%$ Resources:Recursos, Observations %>"></asp:Literal></h2>
                                                <div id="pnlPeObservaciones">
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td style="vertical-align: top;">
                                    <table class="Tabla">
                                        <tr>
                                            <th>
                                                <asp:Literal ID="Literal15" runat="server" Text="<%$ Resources:Recursos, Contact %>" />
                                            </th>
                                        </tr>
                                        <tr>
                                            <td id="lstPeContactos"></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <input id="Button2" type="button" runat="server" class="btnAgregar32" onclick="AgregarContacto()" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="_datosDeuda" class="EncSeccion">
                        <asp:Literal ID="Literal16" runat="server" Text="<%$ Resources:Recursos, ActiveAccounts %>" />
                    </div>
                    <div id="datosDeuda" class="Seccion">
                        <div id="pnlPeCuentas">
                        </div>
                    </div>
                    <div id="_datosGestion" class="EncSeccion">
                        <asp:Literal ID="Literal18" runat="server" Text="<%$ Resources:Recursos, CurrentManagement %>" />
                    </div>
                    <div id="datosGestion" class="Seccion">
                        <div id="pnlPeDatosGestion">
                            <h2>
                                <asp:Literal ID="Literal22" runat="server" Text="<%$ Resources:Recursos, CurrentManagement %>" /></h2>
                            <div id="tabsOtrasActividades">
                                <!--div id="tabs"-->
                                <ul>
                                    <li><a href="#tabNuevaGestion" id="pstNuevaGestion">
                                        <asp:Literal ID="Literal31" runat="server" Text="<%$ Resources:Recursos, NewManagement %>"></asp:Literal></a></li>
                                    <li><a href="#tabReclamos" id="pstReclamos">
                                        <asp:Literal ID="Literal23" runat="server" Text="<%$ Resources:Recursos, Claims %>"></asp:Literal></a></li>
                                    <li><a href="#tabCompromisos" id="pstCompromisos">
                                        <asp:Literal ID="Literal24" runat="server" Text="<%$ Resources:Recursos, PaymentCommitments %>"></asp:Literal></a></li>
                                    <li><a href="#tabPagos" id="pstPagos">
                                        <asp:Literal ID="Literal25" runat="server" Text="<%$ Resources:Recursos, Payments %>"></asp:Literal></a></li>
                                    <li><a href="#tabMetas" id="pstMetas">
                                        <asp:Literal ID="Literal86" runat="server" Text="<%$ Resources:Recursos, Goals %>"></asp:Literal></a></li>
                                </ul>
                                <div id="tabNuevaGestion">
                                    <table class="Tabla">
                                        <tr>
                                            <td style="vertical-align: top;">
                                                <h3>
                                                    <asp:Literal ID="Literal21" runat="server" Text="<%$ Resources:Recursos, NewManagement %>" /></h3>
                                                <table class="Tabla">
                                                    <tr>
                                                        <td>
                                                            <asp:Literal ID="Literal19" runat="server" Text="<%$ Resources:Recursos, Status %>" />
                                                        </td>
                                                        <td>
                                                            <select id="cboGestStatus" onchange="SelecionGestion()" style="width: 300px;">
                                                            </select>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Literal ID="Literal20" runat="server" Text="<%$ Resources:Recursos, Description %>" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <textarea id="txtGestDescripcion" rows="3" cols="80"></textarea>
                                                            <br />
                                                            Adjuntar Archivo a la Gestión:
                                                            <input id="Button8" type="button" runat="server" value="Archivo desde el Equipo..." onclick="Adjuntar();" />
                                                            <div id="Adjuntos">
                                                            </div>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <input id="Button1" type="button" runat="server" value="<%$ Resources:Recursos, SaveManagement %>" onclick="ConfirmarGestion()" />
                                                
                                                
                                                
                                                
                                                <input id="Button9" type="button" runat="server" value="Guardar Gestión a Grupo de Empresas" onclick="ConfirmarGestionGrupoEmpresa();" />





                                                <br />
                                                <input type="button" id="btnEnviarNuevoCorreo" runat="server" value="<%$ Resources:Recursos, SendMail %>" onclick="btnEnviarNuevoCorreo_Click();" />
                                            </td>
                                            <td style="vertical-align: top;">
                                                <h3>Crear Aviso</h3>
                                                <table class="Tabla">
                                                    <tr>
                                                        <td>Fecha:
                                                            <input type="text" class="fecha" id="dtpFechaAviso" />
                                                            Hora:
                                                            <select id="cboHoraAviso">
                                                                <option value="07">7 am</option>
                                                                <option value="08">8 am</option>
                                                                <option value="09">9 am</option>
                                                                <option value="10">10 am</option>
                                                                <option value="11">11 am</option>
                                                                <option value="12">12 pm</option>
                                                                <option value="13">1 pm</option>
                                                                <option value="14">2 pm</option>
                                                                <option value="15">3 pm</option>
                                                                <option value="16">4 pm</option>
                                                                <option value="17">5 pm</option>
                                                                <option value="18">6 pm</option>
                                                            </select>:
                                                            <select id="cboMinutoAviso">
                                                                <option value="00">00</option>
                                                                <option value="05">05</option>
                                                                <option value="10">10</option>
                                                                <option value="15">15</option>
                                                                <option value="20">20</option>
                                                                <option value="25">25</option>
                                                                <option value="30">30</option>
                                                                <option value="35">35</option>
                                                                <option value="40">40</option>
                                                                <option value="45">45</option>
                                                                <option value="50">50</option>
                                                                <option value="55">55</option>
                                                            </select>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Mensaje:<br />
                                                            <textarea id="txtNuevoAviso" cols="80" rows="3" style="width: 90%;" runat="server"></textarea>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>Operador:<select id="cboOperadorAviso"></select>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td><table><tr>
                                                            <td><input type="button" value="Crear" onclick="CrearAviso();" /></td>
                                                            <td>&nbsp;</td>
                                                            <td>Asignar Prioritario</td>
                                                            <td><input type="checkbox" id="chkAvisoPrioritario" /></td>


                                                                   </tr></table>
                                                            
                                                        </td>   
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div id="tabReclamos">
                                    <h3>
                                        <asp:Literal ID="Literal28" runat="server" Text="<%$ Resources:Recursos, Claims %>"></asp:Literal></h3>
                                    <div id="pnlPeReclamos">
                                    </div>
                                    <input type="button" id="btnNuevoReclamo" value="Nuevo Reclamo" onclick="NuevoReclamo()" />
                                </div>
                                <div id="tabCompromisos">
                                    <h3>
                                        <asp:Literal ID="Literal29" runat="server" Text="<%$ Resources:Recursos, PaymentCommitments %>"></asp:Literal></h3>
                                    <div id="pnlPeCompromisos">
                                    </div>
                                    <input type="button" id="btnNuevoCompromiso" value="Nuevo Compromiso de Pago" onclick="NuevoCompromiso()" />
                                </div>
                                <div id="tabPagos">
                                    <h3>
                                        <asp:Literal ID="Literal30" runat="server" Text="<%$ Resources:Recursos, Payments %>"></asp:Literal></h3>
                                    <div id="pnlPePagos">
                                    </div>
                                    <input type="button" id="btnNuevoPago" value="Registrar Nuevo Pago" onclick="NuevoPago()" />
                                </div>
                                <div id="tabMetas">
                                    <h3>
                                        <asp:Literal ID="Literal87" runat="server" Text="<%$ Resources:Recursos, Goals %>"></asp:Literal></h3>
                                    <div id="Div2">
                                    </div>
                                    Escoja un Motivo Para Excluir:
                                    <select id="cboExclusionMotivo"></select><br />
                                    <input type="button" id="Button6" value="Excluir cuentas seleccionadas de las metas" onclick="ExcluirMetasCuentas();" />
                                    <br />
                                    <input type="button" id="Button7" value="Excluir esta persona de las metas" onclick="ExcluirMetasPersona();" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div id="_historialGestion" class="EncSeccion">
                        <asp:Literal ID="Literal17" runat="server" Text="<%$ Resources:Recursos, ManagementHistory %>" />
                    </div>
                    <div id="historialGestion" class="Seccion">
                        <div id="pnlPeGestiones">
                        </div>
                    </div>
                    <div id="_datosReportes" class="EncSeccion">
                        <asp:Literal ID="Literal26" runat="server" Text="<%$ Resources:Recursos, Reports %>" />
                    </div>
                    <div id="datosReportes" class="Seccion">
                        <div id="pnlPeDatosReportes">
                            <!--<div class="btnReporte" onclick="Emergente({url:'/Emergentes/repAnalisis.aspx?idPersona='+$('#idPersona').val() +'&idOperador='+idOp()});">-->
                            <div class="btnReporte" onclick="location.href='/Emergentes/repAnalisis.aspx?idPersona='+$('#idPersona').val() +'&idOperador='+idOp();">
                                <asp:Literal ID="Literal27" runat="server" Text="<%$ Resources:Recursos, OutstandingAccounts %>" />
                            </div>
                            <div style="display: table-cell;" id="pnlPeReportes">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--/div-->
            <br />
            <br />
        </div>
    </div>
    <footer>Copyright (c) 2014 Veconinter C.A. <asp:Label runat="server" id="lblAmbiente" style="background-color:Yellow" Visible="false"></asp:Label><span id="NoRecibeCorreos" style="background-color: Red; color: Yellow; display: none; cursor: pointer;" title="Por favor actualizar Contraseña de Correo" onclick="ActualizarContrasena(true);"> - No recibe Correos</span> </footer>
</asp:Content>
