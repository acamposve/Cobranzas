/// <reference path="/Scripts/jquery-1.10.2-vsdoc.js"/>
/// <reference path="/Scripts/General.js"/>
/// <reference path="/Scripts/Servicios.js"/>
/// <reference path="/Scripts/InterfazGrafica.js"/>
/// <reference path="/Scripts/DOM.js"/>
/// <reference path="/Scripts/Conversiones.js"/>
function Inicializar()
{
    try
    {
        Argumentos = searchToJSON();
        $("#Permanente").val(Argumentos.Permanente);
        if ($("#ArchivoSubido").val() != "")
        {
            window.parent.AgregarAdjunto($("#ArchivoSubido").val());
            window.parent.CerrarEmergente();
        }
        $("#Limite").text(Math.round(Convert.ToDecimal($("#Tamano").val()) / 1024).toString());
    } catch (e) { }
}
function SubirArchivo()
{
    var F = $("#Archivo")[0];
    var Archivo = F.value;
    if (Archivo == "") return;
    Archivo = Archivo.split("\\");
    Archivo = Archivo[Archivo.length - 1];
    Archivo = Archivo.split("/");
    Archivo = Archivo[Archivo.length - 1];
    lstAdjuntos = window.parent.lstAdjuntos;
    if (lstAdjuntos != undefined)
    {
        for (var i = 0; i < lstAdjuntos.length; i++)
        {
            var pos = lstAdjuntos[i].indexOf("_");
            var Parte = lstAdjuntos[i].substr(pos + 1);
            if (Parte == Archivo)
            {
                $("#mensaje").text("Ya ha adjuntado un archivo con el mismo nombre");
                return;
            }
        }

    }
    var Maximo = Convert.ToInt32($("#Tamano").val()) * 1024;
    var tamano = 0;
    if (window.File && window.FileReader && window.FileList && window.Blob)
    {
        tamano = F.files[0].size;
    } else
    {
        // solo IE, no funciona con archivos multiples
        var Fs = new ActiveXObject("Scripting.FileSystemObject");
        var ruta = F.value;
        var archivo = Fs.getFile(ruta);
        tamano = archivo.size;
    }
    if (tamano > Maximo)
    {
        $("#mensaje").text("El archivo es muy pesado, favor subir un archivo de menos de " + $("#Limite").text() + "Mb");
        return;
    }
    Extension = Archivo.split(".");
    Extension = Extension[Extension.length - 1].toLowerCase();
    if (".xls.xlsx.doc.docx.msg.eml.txt.pdf.jpg.jpeg.gif.bmp.png.".indexOf("." + Extension + ".") == -1)
    {
        $("#mensaje").text("La extensión del archivo seleccionado no es válida");
        return;
    }
    if (Archivo.length > 100)
    {
        $("#mensaje").text("El nombre de archivo es demasiado largo (100 Caracteres máximo)");
        return;
    }
    if (Archivo.indexOf(',') != -1 || Archivo.indexOf(';') != -1)
    {
        $("#mensaje").text("El nombre del archivo no debe contener los siguientes caracteres: ',' ';' ");
        return;
    }
    document.forms[0].submit();
}
