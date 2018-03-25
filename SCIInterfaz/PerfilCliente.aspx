<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PerfilCliente.aspx.cs" Inherits="Cobranzas.PerfilCliente" %>

<!DOCTYPE Html>
<html>
<head>
    <title>Perfil del Cliente</title>
    <!--link href="/Estilos.css" rel="stylesheet" /-->
    <style>
        body {
            font-family: Arial;
            font-size: small;
        }

        table {
            border-collapse: collapse;
        }

            table, table td, table th {
                border: 1px solid black;
            }

        .Monto {
            text-align: right;
        }

        .Contenido {
            font-family: Arial;
            font-size: small;
        }

        .auto-style1 {
            width: 12%;
        }
    </style>
</head>
<body>
    <h2>Perfil del Cliente</h2>
    <table style="width: 100%">
        <tr>
            <td>
                <img src="/Img/Logo.jpg" /></td>
            <td colspan="3" class="Contenido"><span align="center"><b>Período</b></span><br />
                <b>Desde:</b><asp:Label runat="server" ID="lblFechaDesde"></asp:Label><br />
                <b>Hasta:</b><asp:Label runat="server" ID="lblFechaHasta"></asp:Label>

            </td>
            <td colspan="4" class="Contenido"><b>Línea Naviera:</b><asp:Label runat="server" ID="lblNaviera"></asp:Label></td>
        </tr>
        <tr>
            <td class="Contenido">
                <b>Nombre:</b><asp:Label runat="server" ID="lblNombre"></asp:Label><br />
                <b>Rif:</b><asp:Label runat="server" ID="lblRif"></asp:Label><br />
                <b>Codigo:</b><asp:Label runat="server" ID="lblCodigo"></asp:Label><br />
                <b>Pais:</b><asp:Label runat="server" ID="lblPais"></asp:Label>
            </td>
            <td colspan="7" class="Contenido">
                <b>Dirección Fiscal:</b>
                <asp:Label runat="server" ID="lblDireccionFiscal"></asp:Label><br />
                <b>Dirección De Entrega:</b>
                <asp:Label runat="server" ID="lblDireccionEntrega"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <fieldset>
        <legend>Resumen  de Pagos realizado en el periodo por el cliente</legend>
        <table style="width: 100%">
            <tr>
                <th colspan="2"></th>
                <th>0- 15 días</th>
                <th>16 a 30 días</th>
                <th>31 a 45 días</th>
                <th>45 a 60 días</th>
                <th>61 a 90 días</th>
                <th>más 90 días</th>
            </tr>
            <tr>
                <th rowspan="2" class="auto-style1">Tiempo de recuperación Total (incluyendo Taquilla)</th>
                <th style="width: 10%">Cantidad de Facturas:</th>
                <td style="width: 5%" class="Monto">
                    <asp:Label runat="server" ID="lblTF1"></asp:Label></td>
                <td style="width: 5%" class="Monto">
                    <asp:Label runat="server" ID="lblTF2"></asp:Label></td>
                <td style="width: 5%" class="Monto">
                    <asp:Label runat="server" ID="lblTF3"></asp:Label></td>
                <td style="width: 5%" class="Monto">
                    <asp:Label runat="server" ID="lblTF4"></asp:Label></td>
                <td style="width: 5%" class="Monto">
                    <asp:Label runat="server" ID="lblTF5"></asp:Label></td>
                <td style="width: 5%" class="Monto">
                    <asp:Label runat="server" ID="lblTF6"></asp:Label></td>
            </tr>
            <tr>
                <th>Monto:</th>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblTM1"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblTM2"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblTM3"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblTM4"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblTM5"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblTM6"></asp:Label></td>
            </tr>
            <tr>
                <th rowspan="2" class="auto-style1">Tiempo de Recuperación de cartera al cobro (solo por cobranzas)</th>
                <th>Cantidad de Facturas:</th>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCF1"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCF2"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCF3"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCF4"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCF5"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCF6"></asp:Label></td>
            </tr>
            <tr>
                <th>Monto:</th>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCM1"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCM2"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCM3"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCM4"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCM5"></asp:Label></td>
                <td class="Monto">
                    <asp:Label runat="server" ID="lblCM6"></asp:Label></td>
            </tr>
        </table>
    </fieldset>

    <br />
    <!--- Gestión del Cliente --->
    <br />
    <fieldset>
        <legend>Gestiones realizadas al Cliente</legend>
        <table style="width: 100%">
            <tr>
                <td class="Contenido">
                    <b>Cantidad de Facturas pendientes de pago:</b>
                    <asp:Label runat="server" ID="lblCantidadFacturas"></asp:Label>
                </td>
                <td colspan="3" class="Contenido">
                    <b>Deuda Actual:</b><asp:Label runat="server" ID="lblDeudaDolar"></asp:Label>
                </td>
                <td colspan="4" class="Contenido">
                    <b>Deuda Actual(Local):</b><asp:Label runat="server" ID="lblDeudaLocal"></asp:Label>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <fieldset>

        <legend>Condiciones del Cliente:</legend>
        <table style="width: 100%">
            <tr>
                <td><b>Paga DG:</b>
                    <asp:Label runat="server" ID="lblPagaDG"></asp:Label></td>
                <td colspan="3"><b>Paga Seguro:</b>
                    <asp:Label runat="server" ID="lblPagaSeguro"></asp:Label></td>
                <td colspan="4"><b>Pasa por Taquilla:</b>
                    <asp:Label runat="server" ID="lblTaquilla"></asp:Label></td>
            </tr>
            <tr>
                <td>
                    <b>Última Fecha de Pago:</b>
                    <asp:Label runat="server" ID="lblUltimoPago"></asp:Label>
                </td>
                <td colspan="3">
                    <b>Cantidad de Pagos en el Período:</b>
                    <asp:Label runat="server" ID="lblCantidadPagos"></asp:Label>
                </td>
                <td colspan="4">
                    <b>Cheques Devueltos: </b>
                    <asp:Label runat="server" ID="lblChequesDevueltos"></asp:Label>
                </td>
            </tr>
        </table>
    </fieldset>


    <br />
    <fieldset>
        <legend><b>Status actual del cliente de acuerdo al Sistema:</b>&nbsp;<asp:Label runat="server" ID="lblStatus"></asp:Label></legend>

        <table style="width: 100%">
            <tr>
                <td><b>Gestiones Totales: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblGestionesTotales"></asp:Label></td>
                <td>&nbsp;</td>
                <td><b>Llamadas Contestadas: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblLlamadasContestadas"></asp:Label></td>
                <td>&nbsp;</td>
                <td><b>Llamadas No Contestadas: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblLlamadasNoContestadas"></asp:Label></td>
            </tr>
            <tr>
                <td><b>Envíos automáticos: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblEnviosAutomaticos"></asp:Label></td>
                <td>&nbsp;</td>
                <td><b>Envíos de correos por el Operador: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblCorreos"></asp:Label></td>
                <td>&nbsp;</td>
                <td><b>Correos asignados por el Operador: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblCorreosAsignados"></asp:Label></td>

            </tr>
            <tr>
                <td><b>Pagos Procesados por el Operador: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblPagosProcesados"></asp:Label></td>
                <td>&nbsp;</td>
                <td><b>Reclamos Procesados por el Operador: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblReclamos"></asp:Label></td>
                <td>&nbsp;</td>
                <td><b>Compromisos Cargados por el Operador: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblCompromisos"></asp:Label></td>
            </tr>
            <tr>
                <td><b>Avisos Creados y Cerrados: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblAvisos"></asp:Label></td>
                <td>&nbsp;</td>
                <td><b>Visitas realizadas: </b></td>
                <td>
                    <asp:Label runat="server" ID="lblVisitas"></asp:Label></td>

            </tr>
        </table>
    </fieldset>


    <!--- Fin Gestion Cliente-->
    <br />

    <fieldset>
        <legend><b>Descripción de la ultima gestión cargada por el operador:</b></legend>

        <table style="width: 100%">
            <tr>
                <td colspan="8">
                    <br />
                    <asp:Label runat="server" ID="lblGestion"></asp:Label>
                </td>
            </tr>
        </table>
    </fieldset>
    <asp:Panel runat="server" ID="pnlExportar">
        <form runat="server">
            <asp:Button runat="server" ID="btnExportarExcel" Text="Exportar a Excel" OnClick="btnExportarExcel_Click" />
            <asp:Button runat="server" ID="btnExportarWord" Text="Exportar a Word" OnClick="btnExportarWord_Click" />
            <input type="button" value="Imprimir" onclick="this.parentNode.style.display = 'none'; window.print();" />
        </form>
    </asp:Panel>
</body>
</html>
