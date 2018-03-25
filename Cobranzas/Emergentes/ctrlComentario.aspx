<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="ctrlComentario.aspx.cs" Inherits="Cobranzas.ctrlComentario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="ctrlComentario.aspx.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
    <h2>Comentario para el Cliente:</h2>
    <asp:TextBox runat="server" ID="txtComentario" TextMode="MultiLine" Rows="10" Style="width: 90%;" ClientIDMode="Static" ></asp:TextBox><br />
    <asp:Button runat="server" ID="btnGuardar" CssClass="btnGuardar32" onclick="btnGuardar_Click" />
</asp:Content>
