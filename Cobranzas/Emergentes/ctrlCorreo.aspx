<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlCorreo.aspx.cs" Inherits="Cobranzas.ctrlCorreo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlCorreo.aspx.js?v=6" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <table style="width: 100%">
        <tr>
            <td style="text-align: left;">
                <a href="" runat="server" id="lnkAnterior" class="btnAnterior32" title="Anterior"></a>
            </td>
            <td>
                <h2><asp:Literal ID="Literal6" runat="server" Text="<%$ Resources:Recursos, Email %>" /></h2>
            </td>
            <td style="text-align: right;">
                <input type="button" runat="server" value="<%$ Resources:Recursos,Reply %>" onclick="Respuesta('Responder');" /> <input type="button" runat="server" value="<%$ Resources:Recursos,ReplyAll %>" onclick="Respuesta('ResponderTodos');" /> <input type="button" runat="server" value="<%$ Resources:Recursos,Forward %>" onclick="Respuesta('Reenviar');" />
            </td>
            <td style="text-align: right;">
                <a href="" runat="server" id="lnkSiguiente" class="btnSiguiente32" title="Siguiente"></a>
            </td>
        </tr>
    </table>
    <input type="hidden" runat="server" id="idOperador" /> <input type="hidden" runat="server" id="idCorreo" />
    <table style="width:100%">
        <tr>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Subject %>" />:
            </th>
            <td>
                <asp:Label ID="lblAsunto" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, MailFrom %>" />:
            </th>
            <td>
                <asp:Label ID="lblRemitente" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, MailTo %>" />:
            </th>
            <td>
                <asp:Label ID="lblDestinatarios" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, MailCC %>" />:
            </th>
            <td>
                <asp:Label ID="lblDestinatariosCopia" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal5" runat="server" Text="<%$ Resources:Recursos, MailBCC %>" />:
            </th>
            <td>
                <asp:Label ID="lblDestinatariosCopiaOculta" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal7" runat="server" Text="<%$ Resources:Recursos, Date %>" />:
            </th>
            <td>
                <asp:Label ID="lblFecha" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal8" runat="server" Text="<%$ Resources:Recursos, Attachments %>" />:
            </th>
            <td>
                <asp:Label runat="server" ID="lblAdjuntos">
                </asp:Label>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal9" runat="server" Text="<%$ Resources:Recursos, Person %>" />:
            </th>
            <td>
                Filtro:<input type="text" id="txtFiltroPersona" onkeyup="FiltrarPersona()" />
                <select id="cboaPersona">
                </select>
                <input type="button" runat="server" value="<%$ Resources:Recursos, Assign %>" onclick="AsignarCorreosAPersona(false);" enableviewstate="True" visible="True" /> <input type="button" runat="server" value="<%$ Resources:Recursos, CreateRule %>" onclick="AsignarCorreosAPersona(true);" style="display: none;" /><input type="button" runat="server" value="Asignar a Grupo de Empresa" onclick="AsignarCorreosGrupodeEmpresa(false);" enableviewstate="True" visible="True" />    <input id="Button3" type="button" runat="server" value="<%$ Resources:Recursos, MarkAsPersonal %>" onclick="    MarcarComoPersonal();" />
                <div id="pnlPersonas">
                </div>
            </td>
        </tr>
    </table>
    <iframe runat="server" id="iframe" src="" width="100%" height="310px" style="background-color: White;" frameborder="0"></iframe>
</asp:Content>
