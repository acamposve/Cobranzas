<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlEnvioCorreo.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlEnvioCorreo" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlEnvioCorreo.aspx.js?v=52" type="text/javascript"></script>
    <script src="/Scripts/Editor.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%">
        <tr>
            <td>
                <h2>
                    <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, SendMail %>" />
                </h2>
            </td>
            <td style="text-align: right;">
                <input type="button" id="Button1" runat="server" value="<%$ Resources:Recursos, Send %>" onclick="btnCorreoEnviar_Click();" />
            </td>
        </tr>
    </table>
    <table class="TablaDatos">
        <tr>

            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, MailTo %>" />
            </th>
            <td>
                <input type="radio" name="radpara" id="radCorreoPara" /><input type="text" style="width: 90%;" id="txtCorreoPara" runat="server" autocomplete="on" name="email" onkeypress="SalvarAlTiempo();" onfocus="$('#radCorreoPara')[0].checked=true;" />
            </td>
            <td rowspan="3" style="width: 300px;">
                <div id="pnlListadoContacto">
                    <asp:Literal ID="Literal12" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, MailCC %>" />
            </th>
            <td>
                <input type="radio" name="radpara" id="radCorreoCC" /><input type="text" style="width: 90%;" id="txtCorreoCC" runat="server" autocomplete="on" name="email" onkeypress="SalvarAlTiempo();" onfocus="$('#radCorreoCC')[0].checked=true;" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, MailBCC %>" />
            </th>
            <td>
                <input type="radio" name="radpara" id="radCorreoCCO" /><input type="text" style="width: 90%;" id="txtCorreoCCO" runat="server" autocomplete="on" name="email" onkeypress="SalvarAlTiempo();" onfocus="$('#radCorreoCCO')[0].checked=true;" />
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Subject %>" />
            </th>
            <td colspan="2">
                <input type="text" style="width: 90%;" id="txtCorreoAsunto" runat="server" onkeypress="SalvarAlTiempo();" />
            </td>
        </tr>
        <tr>
            <td style="text-align: center;" colspan="3">
                <div id="pnlListado">
                    <asp:Literal ID="Literal53" runat="server" Text="<%$ Resources:Recursos, Loading %>"></asp:Literal>...
                </div>
                <div style="width: 605px; display: inline-block; background-color: White; text-align: left;">
                    <!--table style="width:100%"-->
                    <textarea id="txtMensaje" name="txtMensaje" rows="20" cols="80" style="width: 600px;" runat="server" onkeypress="SalvarAlTiempo();"></textarea>
                    <!--/table-->
                </div>
                <div runat="server" id="pnlOriginal" style="width: 605px; display: inline-block; background-color: White; text-align: left;">
                    <h2>
                        <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, OriginalEmail %>" /></h2>
                    <input type="checkbox" id="chkOriginal" checked="checked" />
                    <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Recursos, AttachOriginalEmail %>" />
                    <iframe runat="server" id="iframe" width="600px" height="400px" frameborder="0"></iframe>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <h2>
                    <asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, MailAttach %>" /><br />
                </h2>
                <input type="button" runat="server" value="Archivo desde el Equipo..." onclick="Adjuntar();" />
                <div id="Adjuntos">
                </div>
                <input runat="server" type="checkbox" id="chkAnalisisAntiguedad" /><asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, OutstandingAccounts %>" /><br />
                <input runat="server" type="checkbox" id="chkAgrupado" />Agrupado por Cliente
                <div runat="server" id="pnlDocumentos">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="3" style="text-align: center;">
                <input type="button" id="btnCorreoEnviar" runat="server" value="<%$ Resources:Recursos, Send %>" onclick="btnCorreoEnviar_Click();" />
            </td>
        </tr>
    </table>
</asp:Content>
