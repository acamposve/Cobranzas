<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlNuevoAviso.aspx.cs"
    Inherits="Cobranzas.ctrlNuevoAviso" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlNuevoAviso.aspx.js?ver=1" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2><asp:Literal ID="Literal56" runat="server" Text="<%$ Resources:Recursos, CreateNewNotice %>"></asp:Literal></h2>
    <table style="width: 100%;">
        <tr>
            <th>
                <asp:Literal ID="Literal34" runat="server" Text="<%$ Resources:Recursos, Date %>" />
            </th>
            <td>
                <input id="dtpFechaAviso" type="text" style="width: 80px;" class="fecha" />
            </td>
            <th>
                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Recursos, Time %>" />
            </th>
            <td>
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
            <th colspan="4">
                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Recursos, Notice %>" />
            </th>
        </tr>
        <tr>
            <td colspan="4">
                <textarea id="txtNuevoAviso" cols="80" name="S1" rows="3" style="width: 90%;" runat="server"></textarea>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Recursos, User %>" />
            </th>
            <td colspan="3">
                <select id="cboOperadorAviso"></select>
            </td>
        </tr>
        <tr>
            <th>
                <asp:Literal ID="Literal4" runat="server" Text="<%$ Resources:Recursos, Person %>" />
            </th>
            <td colspan="3">
                Filtro:<input type="text" id="txtFiltroPersona" onkeyup="FiltrarPersona()" />
                <select id="cboaPersona" style="max-width:500px;">
                </select>
            </td>
        </tr>
    </table>
    <input type="button" onclick="Crear()" runat="server" value="<%$ Resources:Recursos, Create %>" />
</asp:Content>
