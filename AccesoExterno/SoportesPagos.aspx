<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SoportesPagos.aspx.cs" Inherits="Cobranzas.SoportesPagos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Visualización de Pagos</title>
    <link href="/Estilos.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <h1>Pagos del Sistema de Cobranzas</h1>
        Seleccione el Pais:
        <asp:DropDownList runat="server" ID="cboPais">
            <asp:ListItem Selected="True" Text="Venezuela" Value="VEN"></asp:ListItem>
            <asp:ListItem Text="Estados Unidos" Value="USA"></asp:ListItem>
            <asp:ListItem Text="Guyana" Value="GUY"></asp:ListItem>
            <asp:ListItem Text="Sint Maarten" Value="SXM"></asp:ListItem>
        </asp:DropDownList><br />
        Introduzca el código del Pago Temporal si no ha sido aprobado o Real si ya ha sido Aprobado:
        <asp:TextBox runat="server" ID="txtCodigo"></asp:TextBox>
        <asp:Button runat="server" ID="btnBuscar" OnClick="btnBuscar_Click" Text="Buscar" />
        <br />



        <table class="TablaDatos">
            <tr>
                <th>
                    Tipo de Pago:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblTipoPago"></asp:Label>
                </td>
                <th>
                    Fecha:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblFechaNuevoPago"></asp:Label>
                </td>
            </tr>
            <tr>
                <th>
                    Referencia:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblReferencia"></asp:Label>
                </td>
                <th>
                    Banco Destino:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblBancoDestino"></asp:Label>
                </td>
            </tr>
            <tr>
                <th>
                    Moneda:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblMoneda"></asp:Label>
                </td>
                <th>
                    Efectivo:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblEfectivo"></asp:Label>
                </td>
            </tr>
            <tr>
                <th>
                    Operador:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblOperador"></asp:Label>
                </td>
                <th>
                    Soportes:
                </th>
                <td>
                    <div id="Adjuntos">
                        <asp:HiddenField runat="server" ID="idPago" />
                        <div runat="server" id="pnlResultados">
                        </div>
                    </div>
                </td>
            </tr>
            <tr runat="server" id="trTransferencia">
                <th>
                    Banco Origen:
                </th>
                <td>
                    <asp:Label runat="server" ID="lblBancoOrigen"></asp:Label>
                </td>
                <th>
                </th>
                <td>
                </td>
            </tr>
            <tr runat="server" id="trCheques">
                <td colspan="4">
                    <h3>Cheques</h3>
                    <div runat="server" ID="pnlCheques">
                    </div>
                    Total Cheques: <asp:Label runat="server" ID="lblTotalCheques"></asp:Label>
                </td>
            </tr>
            <tr>
                <th colspan="4">
                    Descripcion:
                </th>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Label runat="server" ID="lblDescripcion"></asp:Label>
                </td>
            </tr>
            <tr>
                <th colspan="4">
                    Facturas:
                </th>
            </tr>
            <tr>
                <td colspan="4">
                    <h3>Facturas Seleccionadas:</h3>
                    <div runat="server" id="pnlFacturasSelecionadas"></div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    Total Pago: 
                <asp:Label runat="server" ID="lblTotalPago"></asp:Label>

                    Total Aplicaciones:
                <asp:Label runat="server" ID="lblTotalAplicacion"></asp:Label>

                    Retenciones: 
                <asp:Label runat="server" ID="lblTotalRetenciones"></asp:Label>

                    Restante: 
                <asp:Label runat="server" ID="lblMontoRestante"></asp:Label>
                </td>
            </tr>
        </table>

        <br />
        <asp:Panel runat="server" ID="SCI" Visible="false">
            <h3>Acciones:</h3>
            <asp:Button runat="server" ID="btnLlevarPago" Text="Llevar Pago al SCI" OnClick="btnLlevarPago_Click" /><br />
            <asp:TextBox runat="server" ID="txtRechazo"></asp:TextBox><asp:Button runat="server" ID="btnRechazarPAgo" Text="Rechazar el Pago" OnClick="btnRechazarPago_Click" />
        </asp:Panel>
    </div>
    </form>
</body>
</html>
