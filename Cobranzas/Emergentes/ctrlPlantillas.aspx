<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlPlantillas.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlPlantillas" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlPlantillas.aspx.js?ver=72" type="text/javascript"></script>
    <script src="/Scripts/Editor.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%;">
        <tr>
            <td>
                <h2>Plantillas</h2>
            </td>
            <td style="text-align: right;">
                <!--input type="button" value="Ejecutar Metas del Día" onclick="EjecutarMetas()" /-->
            </td>
        </tr>
    </table>
    <div id="Manejo">
        <div id="tabs">
            <ul>
                <li><a href="#tabListado" id="pstListado">
                    <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Listing%>" /></a></li>
                <li><a href="#tabDetalles" id="pstDetalles">
                    <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, Details%>" /></a></li>
            </ul>
            <div id="tabListado">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <h3>
                                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Listing %>"></asp:Literal></h3>
                        </td>
                        <td style="text-align: right;">
                            <input id="Button2" type="button" runat="server" class="btnAgregar32" title="<%$ Resources:Recursos, NewTemplate %>" onclick="Nuevo();" />
                        </td>
                    </tr>
                </table>
                <div id="pnlListado">
                    <asp:Literal ID="Literal53" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
            </div>
            <div id="tabDetalles">
                <table style="width: 100%">
                    <tr>
                        <td>
                            <h2>
                                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, NewTemplate %>" />
                            </h2>
                        </td>
                        <td style="text-align: right;">
                            <input type="button" id="Button1" runat="server" value="<%$ Resources:Recursos, Save %>" onclick="Guardar();" />
                            <input type="button" id="Button3" runat="server" value="<%$ Resources:Recursos, Delete %>" onclick="Eliminar();" />
                        </td>
                    </tr>
                </table>
                <table class="TablaDatos">
                    <tr>
                        <th>
                            <input type="hidden" id="idPlantilla" runat="server" />
                            <asp:Literal ID="Literal11" runat="server" Text="<%$ Resources:Recursos, Name %>" />
                        </th>
                        <td colspan="2">
                            <input type="text" style="width: 90%;" id="txtNombre" runat="server" autocomplete="on" name="email" onkeypress="SalvarAlTiempo();" />
                        </td>
                        <td>
                            <input runat="server" type="checkbox" id="chkPrivate" /><asp:Literal ID="Literal14" runat="server" Text="<%$ Resources:Recursos, Private %>" /><br />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Recursos, AssignedOperator %>" />
                        </th>
                        <td>
                            <select id="cboOperador"></select>
                        </td>
                        <th>
                            <asp:Literal ID="Literal10" runat="server" Text="<%$ Resources:Recursos, Date %>" />
                        </th>
                        <td>
                            <span id="lblFecha"></span>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Literal ID="Literal12" runat="server" Text="<%$ Resources:Recursos, Country %>" />
                        </th>
                        <td>
                            <select id="cboPais"></select>
                        </td>
                        <th>
                            <asp:Literal ID="Literal13" runat="server" Text="<%$ Resources:Recursos, ClientType %>" />
                        </th>
                        <td>
                            <select id="cboTipoCliente"></select>
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, MailCC %>" />
                        </th>
                        <td colspan="3">
                            <input type="text" style="width: 90%;" id="txtCorreoCC" runat="server" autocomplete="on" name="email" onkeypress="SalvarAlTiempo();" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, MailBCC %>" />
                        </th>
                        <td colspan="3">
                            <input type="text" style="width: 90%;" id="txtCorreoCCO" runat="server" autocomplete="on" name="email" onkeypress="SalvarAlTiempo();" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Subject %>" />
                        </th>
                        <td colspan="3">
                            <input type="text" style="width: 90%;" id="txtCorreoAsunto" runat="server" onkeypress="SalvarAlTiempo();" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: center;" colspan="3">
                            <div style="width: 605px; display: inline-block; background-color: White; text-align: left;">
                                <!--table style="width:100%"-->
                                <textarea id="txtMensaje" name="txtMensaje" rows="20" cols="80" style="width: 600px;" runat="server" onkeypress="SalvarAlTiempo();"></textarea>
                                <!--/table-->
                            </div>
                        </td>
                        <td style="text-align: left;">
                            <span id="lblLeyenda">Las siguientes etiquetas puede ser usadas para ingresar datos dinamicos:<br />
                                <strong>{analisis}</strong>: Mostrar Análisis de Antigüedad<br />
                                <strong>{persona}</strong>: Mostrar nombre de la persona<br />
                                <strong>{fecha}</strong>: Mostrar la fecha actual<br />
                                <strong>{rif}</strong>: Mostrar el rif de la persona<br />
                                <strong>{codpersona}</strong>: Mostrar el código de la persona<br />
                                <strong>{contacto}</strong>: Mostrar la persona de contacto<br />
                                <strong>{deudalocal}</strong>: Mostrar el monto total de la deuda en moneda local<br />
                                <strong>{deudadolar}</strong>: Mostrar el monto total de la deuda en Dólares<br />
                                <strong>{minimaantiguedad}</strong>: Mostrar la antiguedad más pequeña entre las deudas de la persona<br />
                                <strong>{maximaantiguedad}</strong>: Mostrar la antiguedad más grande entre las deudas de la persona<br />
                                <strong>{facturas}</strong>: Mostrar el total de facturas<br />
                                <strong>{pais}</strong>: Mostrar el pais de la persona contacto<br />
                                <strong>{codpais}</strong>: Mostrar el pais de la persona en formato codigo de 3 letras<br />
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <h2>
                                <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, MailAttach %>" /><br />
                            </h2>
                            <input type="button" runat="server" value="Archivo desde el Equipo..." onclick="Adjuntar();" />
                            <div id="Adjuntos">
                            </div>
                            <div runat="server" id="pnlDocumentos">
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
