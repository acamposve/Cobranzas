<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlLlamada.aspx.cs" Inherits="Cobranzas.ctrlLlamada" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlLlamada.aspx.js?ver=5" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <input type="hidden" id="CanalPropio" /><input type="hidden" id="CanalCliente" />
    
    <h3>Llamada Telefónica en curso:</h3>
    <table>
        <tr>
            <td>
                Número: <span id="Llamada" class="Telefono"></span>
                <br />
                Status: <span id="Status" class="Telefono">Iniciando...</span>
                <br />
                <input type="text" id="txtExtension" style="width:80px" runat="server" /> <input type="button" id="btnTransferir" onclick="Transferir()" value="Transferir" />
            </td>
            <td>
                <span id="Temporizador" class="Temporizador"></span>
                <br />
                <input type="button" id="btnColgar" onclick="Colgar()" value="Colgar" />
            </td>
        </tr>
    </table>
</asp:Content>
