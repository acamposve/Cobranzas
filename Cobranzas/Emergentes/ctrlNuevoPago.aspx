<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlNuevoPago.aspx.cs" Inherits="Cobranzas.ctrlNuegoPago" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlNuevoPago.aspx.js?ver=20" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, NewPayment %>"></asp:Literal></h2>
    <div style="display:none;">
        <select id="porcRetIva">
            <option value="0" selected="selected" >0%</option>
            <option value="75">75%</option>
            <option value="100">100%</option>
        </select>
        <select id="porcRetISLR">
            <option value="0" selected="selected">0%</option>
            <option value="1">1%</option>
            <option value="2">2%</option>
            <option value="3">3%</option>
            <option value="4">4%</option>
            <option value="5">5%</option>
        </select>
    </div>
    <table class="TablaDatos">
        <tr>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Type %>"></asp:Literal>
            </th>
            <td>
                <select id="cboTipoPago" onclick="CambiarModo();">
                    <option value="1"><asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, Deposit %>"></asp:Literal></option>
                    <option value="2"><asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Transfer %>"></asp:Literal></option>
                </select>
            </td>
            <th>
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, Date %>"></asp:Literal>:
            </th>
            <td>
                <input id="txtFechaNuevoPago" type="text" style="width: 80px;" runat="server" class="fecha" maxlength="10" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, Reference %>"></asp:Literal>:
            </th>
            <td>
                <input id="txtReferenciaPago" type="text" style="width: 200px;" runat="server"/>
            </td>
            <th>
                <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Bank %>"></asp:Literal> Destino:
            </th>
            <td>
                <select id="cboBancos">
                </select>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Currency %>"></asp:Literal>:
            </th>
            <td>
                <select id="cboMonedasPago" onchange="RefrescarCuentas();">
                </select>
            </td>
            <th>
                <asp:Literal ID="Literal13" runat="server" Text="<%$ Resources:Recursos, PaymentCash %>"></asp:Literal>:
            </th>
            <td>
                <input id="txtMontoEfectivo" type="text" style="width: 200px;" runat="server" maxlength="12" onblur="blurDec(this);ValidarSumaPagos(this);" onfocus="return focusDec(this);" onkeydown="return kdDec(event,this);" onkeypress="return kpDec(event, this);" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal15" runat="server" Text="<%$ Resources:Recursos, Operator %>"></asp:Literal>:
            </th>
            <td>
                <select id="cboOperadorPago">
                </select>
            </td>
            <th>
                <asp:Literal ID="Literal16" runat="server" Text="<%$ Resources:Recursos, Attachments %>"></asp:Literal>:
            </th>
            <td>
                <input id="Button1" type="button" runat="server" value="<%$ Resources:Recursos, FileFromPC %>" onclick="Adjuntar();" />
                <div id="Adjuntos">
                </div>
            </td>
        </tr>
        <tr id="trTransferencia">
            <th>
                <asp:Literal ID="Literal14" runat="server" Text="<%$ Resources:Recursos, Bank %>"></asp:Literal> Origen:
            </th>
            <td>
                <select id="cboBancoOrigen">
                </select>
            </td>
            <th>
            </th>
            <td>
            </td>
        </tr>
        <tr id="trCheques">
            <td colspan="4">
                <h3>Cheques</h3>
                <input type="button" value="Agregar Cheque" onclick="AgregarCheque();" />
                <table class="TablaDatos" id="Cheques">
                    <tr>
                        <th>
                            Nro Cheque
                        </th>
                        <th>
                            Banco Cheque
                        </th>
                        <th>
                            Monto Cheque
                        </th>
                        <th>
                        </th>
                    </tr>
                    <tr id="modeloCheque" style="display: none;">
                        <td>
                            <input type="text" maxlength="12" />
                        </td>
                        <td>
                            <select id="cboBancoCheque">
                            </select>
                        </td>
                        <td>
                            <input type="text" maxlength="12" onblur="blurDec(this);ValidarSumaPagos(this);" onfocus="return focusDec(this);" onkeydown="return kdDec(event);" onkeypress="return kpDec(event, this);" />
                        </td>
                        <td>
                            <input type="button" value="Eliminar" onclick="EliminarCheque(this);" />
                        </td>
                    </tr>
                </table>
                <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Recursos, PaymentCheck %>"></asp:Literal>:<input id="txtTotalCheques" disabled="disabled" type="text" style="width: 90px;" /> 
            </td>
        </tr>
        <tr>
            <th colspan="4">
                <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:Recursos, Description %>"></asp:Literal>:
            </th>
        </tr>
        <tr>
            <td colspan="4">
                <textarea id="txtNuevoPago" cols="80" name="S1" rows="3" style="width: 90%;" runat="server" maxlength="500" onblur="return blurText(this);" onfocus="return focusText(this);" onkeydown="return kdText(event);" onkeypress="return kpText(event);"></textarea>
            </td>
        </tr>
        <tr>
            <th colspan="4">
                <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:Recursos, Accounts %>"></asp:Literal>:
            </th>
        </tr>
        <tr>
            <td colspan="4">
                <div id="pnlPeFacturasPago">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                Total Pago: 
                <input id="txtTotalPago" disabled="disabled" type="text" style="width: 90px;" /> 
                
                Total Aplicaciones:
                <input id="txtTotalAplicacion" disabled="disabled" type="text" style="width: 90px;" /> 
                
                Retenciones: 
                <input id="txtTotalRetenciones" disabled="disabled" type="text" style="width: 90px;" />
                
                <asp:Literal ID="Literal12" runat="server" Text="<%$ Resources:Recursos, Remaining %>"></asp:Literal>: 
                <input id="txtMontoRestante" disabled="disabled" type="text" style="width: 90px;" runat="server" />
            </td>
        </tr>
    </table>
    <span class="Error" id="lblError"></span>
    <p>
        <input type="button" id="btnCrear" runat="server" value="<%$ Resources:Recursos, Save %>" onclick="Preguntar({mensaje:'¿Está seguro de querer guardar el Pago?',funcion:InsertarPago});" />
    </p>
</asp:Content>
