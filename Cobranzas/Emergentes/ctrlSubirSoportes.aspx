<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeBehind="ctrlSubirSoportes.aspx.cs" Inherits="Cobranzas.Emergentes.ctrlSubirSoportes" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>
        <asp:Literal runat="server" ID="litTitulo" Text="<%$ Resources:Recursos, titSystem %>"></asp:Literal></title>
    <link rel="shorcut icon" type="image/x-icon" href="/favicon.ico" />
    <link rel="icon" type="image/x-icon" href="/favicon.ico" />
    <link href="/Estilos/Site.css" rel="stylesheet" type="text/css" />
    <link href="/Estilos/Estilos.css?ver=1" rel="stylesheet" type="text/css" />
    <script src="/Recursos.aspx?ver=1" type="text/javascript"></script>
    <script src="/Scripts/Conversiones.js"></script>
    <script src="/Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="ctrlSubirSoportes.aspx.js?v=4" type="text/javascript"></script>
    <script src="/Scripts/General.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(Inicializar);
    </script>
</head>
<body>
    <input type="hidden" runat="server" id="Tamano" />
    <h3>Seleccione un Archivo</h3>
    <input type="hidden" runat="server" id="ArchivoSubido" />
    <form action="ctrlSubirSoportes.aspx" method="post" id="Form" runat="server">
        <input type="file" runat="server" id="Archivo" name="Archivo" onchange="SubirArchivo();" />
        <input type="hidden" id="Permanente" name="Permanente" />
    </form>
    <span>Límite de Tamaño: </span><span id="Limite"></span><span>Mb</span>
    <br />
    <span id="mensaje" class="Error"></span>
</body>
</html>
