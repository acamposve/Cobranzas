function Dos(Texto)
{
    Texto = Texto.toString();
    return Texto.length == 2 ? Texto : (Texto.length == 1 ? ("0" + Texto) : "00");
}
function AFechaMuyCorta(fecha)
{
    try
    {
        if (typeof (fecha) == "string") fecha = new Date(eval(fecha.substr(6, 13)));
        var dia = fecha.getDate();
        var mes = fecha.getMonth() + 1;
        var ano = fecha.getFullYear();
        return Dos(dia) + "/" + Dos(mes) + "/" + ano;
    }
    catch (e) { return ""; }
}
function deJSON(fecha)
{
    return new Date(eval(fecha.substr(6, 13)));
}
function AFechaHora(fecha)
{
    try
    {
        if (typeof (fecha) == "string") fecha = new Date(eval(fecha.substr(6, 13)));

        var dia = fecha.getDate();
        var mes = fecha.getMonth() + 1;
        var ano = fecha.getFullYear();
        var hora = fecha.getHours();
        var minuto = fecha.getMinutes();
        var ampm = hora >= 12 ? "PM" : "AM";
        hora = hora > 12 ? hora - 12 : hora;
        return Dos(dia) + "/" + Dos(mes) + "/" + ano + " " + Dos(hora) + ":" + Dos(minuto) + ampm;
    }
    catch (e) { return ""; }
}

function ConvertirAFecha(Texto)
{
    var ano = Texto.substr(0, 4);
    var mes = Texto.substr(4, 2);
    var dia = Texto.substr(6, 2);
    var hora = Texto.substr(8, 2);
    var minuto = Texto.substr(10, 2);
    var tfecha = dia + "/" + mes + "/" + ano;
    var thora = (hora > 12 ? hora - 12 : hora) + ":" + minuto + (hora > 12 ? "PM" : "AM");
    return tfecha + " " + thora;
}
function ConvertirADate(Texto)
{
    var ano = Texto.substr(0, 4);
    var mes = Texto.substr(4, 2);
    var dia = Texto.substr(6, 2);
    var hora = Texto.substr(8, 2);
    var minuto = Texto.substr(10, 2);
    var fecha = new Date(ano, mes - 1, dia, hora, minuto, 0);
    return fecha;
}
function FechaJaponesa(Texto)
{
    var ano = Texto.substr(6, 4);
    var mes = Texto.substr(3, 2);
    var dia = Texto.substr(0, 2);
    return ano + "-" + Dos(mes) + "-" + Dos(dia);
}
function aJSON(a)
{
    if (a == undefined) return null;
    if (a == null) return null;
    if (a == "") return null;
    if (a == 0) return null;
    if (a == NaN) return null;
    try
    {
        return Convert.ToDateTime(a).ToString("JSON"); //"\/Date(" + Date.parse(FechaJaponesa(a)) + ")\/";
    }
    catch (e) { return undefined; }
}
function ConvertirDeFecha(Texto)
{
    var ano = Texto.substr(6, 4);
    var mes = Texto.substr(3, 2);
    var dia = Texto.substr(0, 2);
    var hora = Texto.substr(11, 2);
    var minuto = Texto.substr(14, 2);
    var AP = Texto.substr(16, 2);
    return ano + mes + dia + (AP = "AM" ? hora : (hora + 12)) + minuto;
}
function FormatDecimal(monto)
{
    return monto.ToString();
}
function right(cadena, longitud)
{
    if (longitud > cadena.length) return "";
    return cadena.substr(cadena.length - longitud, longitud);
}
//Función realizada para codificar correctamente los objetos para pasarlos por el get. Evita problemas con acentos y caracteres especiales.
function toGET(obj)
{
    return encodeURIComponent(JSON.stringify(obj));
}
//--------------------------------------
//Obtiene un Objeto JSON a partir de los parámetros resultantes de document.location.search.replace
function searchToJSON()
{
    var rep = { '?': '{"', '=': '":"', '&': '","' };
    var s = document.location.search.replace(
		/[\?\=\&]/g, function (r) { return rep[r]; }
    );
    return JSON.parse(s.length ? s + '"}' : "{}");
}
function LlenarCombo_old(combo, datos, id, nombre)
{
    if (typeof (combo) == "string")
    {
        combo = $("#" + combo)[0];
    }
    $(combo).empty();
    var i;
    for (i = 0; i < datos.length; i++)
    {
        var opt = document.createElement("option");
        $(opt).val(eval("datos[i]." + id));
        $(opt).text(eval("datos[i]." + nombre));
        combo.appendChild(opt);
    }
}
function SeleccionarColor(a, ColorBase)
{
    var AColor = ColorHexToArray(ColorBase);
    AColor[0] += a;
    var Result = ColorArrayToHex(AColor);
    return Result;
}
function HEXDIGTODEC(a)
{
    Valores = Array();
    Valores["0"] = 0;
    Valores["1"] = 1;
    Valores["2"] = 2;
    Valores["3"] = 3;
    Valores["4"] = 4;
    Valores["5"] = 5;
    Valores["6"] = 6;
    Valores["7"] = 7;
    Valores["8"] = 8;
    Valores["9"] = 9;
    Valores["A"] = 10;
    Valores["B"] = 11;
    Valores["C"] = 12;
    Valores["D"] = 13;
    Valores["E"] = 14;
    Valores["F"] = 15;
    return Valores[a];
}
function DECDIGTOHEX(a)
{
    Valores = Array();
    Valores[0] = '0';
    Valores[1] = '1';
    Valores[2] = '2';
    Valores[3] = '3';
    Valores[4] = '4';
    Valores[5] = '5';
    Valores[6] = '6';
    Valores[7] = '7';
    Valores[8] = '8';
    Valores[9] = '9';
    Valores[10] = 'A';
    Valores[11] = 'B';
    Valores[12] = 'C';
    Valores[13] = 'D';
    Valores[14] = 'E';
    Valores[15] = 'F';
    return Valores[a];
}
function HexToDec(a)
{
    var result = 0;
    var long = a.length;
    for (var i = 0; i < long ; i++)
    {
        result = result * 16 + HEXDIGTODEC(a[i]);
    }
    return result;
    //return HEXDIGTODEC(a.substr(0, 1)) * 16 + HEXDIGTODEC(a.substr(1, 1));
}
function ColorHexToArray(a)
{
    var Result = Array();
    Result[0] = a.substr(1, 2);
    Result[1] = a.substr(3, 2);
    Result[2] = a.substr(5, 2);
    return Result;
}
function DecToHex(a)
{
    var R2 = a % 16;
    var R1 = (a - a % 16) / 16;
    return DECDIGTOHEX(R1) + DECDIGTOHEX(R2);
}
function ColorArrayToHex(a)
{
    return "#" + DecToHex(a[0]) + DecToHex(a[1]) + DecToHex(a[2]);
}

function lambda(l)
{
    var fn = l.match(/\((.*)\)\s*=>\s*(.*)/);  //Busca una expresión lambda del tipo (algo algo)=>algo
    var p = [];
    var b = "";

    if (fn.length > 0) fn.shift(); //olvido la expresión completa
    if (fn.length > 0) b = fn.pop(); //tomo el cuerpo
    if (fn.length > 0) p = fn.pop()
    .replace(/^\s*|\s(?=\s)|\s*$|,/g, '').split(' '); //tomo los parámetros como arreglo.

    // colocar un return, si es ue ya no lo tiene
    fn = ((!/\s*return\s+/.test(b)) ? "return " : "") + b;

    p.push(fn); //coloco el cuerpo de la función al final del arreglo

    try
    {
        return Function.apply({}, p);
    }
    catch (e)
    {
        return null;
    }
}
Array.prototype.select = function (campos)
{
    var res = [];
    if (campos.length == 0) return this;
    for (var i = 0; i < this.length; i++)
    {
        if (typeof (campos) == "string")
        {
            res.push(this[i][campos]);
        }
        else
        {
            var obj = {};
            for (var propiedad in campos)
            {
                with (this[i])
                {
                    obj[propiedad] = eval(campos[propiedad]);
                }
            }
            res.push(obj);
        }
    }
    return res;
}
Array.prototype.distinct = function ()
{
    var res = [];
    for (var i = 0; i < this.length; i++)
    {
        var Encontrado = false;
        for (var j = 0; j < res.length; j++)
        {
            if (JSON.stringify(this[i]) == JSON.stringify(res[j]))
            {
                Encontrado = true;
                break;
            }
        }
        if (!Encontrado)
        {
            res.push(this[i]);
        }
    }
    return res;
}
Array.prototype.sum = function (f)
{
    var Result = 0;
    if (f == undefined)
    {
        for (var i = 0; i < this.length; i++)
        {
            Result += Convert.ToDecimal(this[i]);
        }
        return Result;
    }

    var fn = f;
    // si el parámetro es un string
    if (typeof (f) == "string")
    {
        // lambdifícalo...
        if ((fn = lambda(fn)) == null)
        {
            // si falla, se jode la vaina
            throw "Error de sintaxis en la función lambda: " + f;
        }
    }
    var l = this.length;
    for (var i = 0; i < l; i++)
    {
        Result += fn.call(null, this[i]);
    }
    return Result;
}
Array.prototype.where = function (f)
{
    var fn = f;
    // si el parámetro es un string
    if (typeof (f) == "string")
    {
        // lambdifícalo...
        if ((fn = lambda(fn)) == null)
        {
            // si falla, se jode la vaina
            throw "Error de sintaxis en la función lambda: " + f;
        }
    }
    // inicializa el arreglo de resultados
    var res = [];
    var l = this.length;
    // configura los parámetros para la función de filtrado
    var p = [0, 0, res];
    // Agrega cualquier parámetro que haga falta...
    for (var i = 1; i < arguments.length; i++) p.push(arguments[i]);
    // pásale la función a cada elemento del arreglo
    for (var i = 0; i < l; i++)
    {
        // salta los elementos indefinidos
        if (typeof this[i] == "undefined") continue;
        // param1 = elemento
        p[0] = this[i];
        // param2 = índice actual
        p[1] = i;
        // llama a la función filtro. si devuelve true copia elemento a resultados
        if (!!fn.apply(this, p)) res.push(this[i]);
    }
    // retorna los resultados filtrados.
    return res;
}
function Clonar(from)
{
    if (from == null || typeof (from) != "object")
        return from;
    if (from.constructor != Object &&
	   from.constructor != Array)
        return from;
    if (from.constructor == Date ||
	   from.constructor == RegExp ||
	   from.constructor == Function ||
	   from.constructor == String ||
	   from.constructor == Number ||
	   from.constructor == Boolean)
        return new from.constructor(from);
    var to = {};
    to = to || new from.constructor();
    for (var name in from)
    {
        to[name] = typeof to[name] == "undefined" ?
			this.clone(from[name]) :
			to[name];
    }
    return to;
}
function SuperClonar(orgNode)
{

    var orgNodeEvents = orgNode.getElementsByTagName('*');
    var cloneNode = orgNode.cloneNode(true);
    var cloneNodeEvents = cloneNode.getElementsByTagName('*');

    var allEvents = new Array('onabort', 'onbeforecopy', 'onbeforecut', 'onbeforepaste', 'onblur', 'onchange', 'onclick',
'oncontextmenu', 'oncopy', 'ondblclick', 'ondrag', 'ondragend', 'ondragenter', 'ondragleave',
'ondragover', 'ondragstart', 'ondrop', 'onerror', 'onfocus', 'oninput', 'oninvalid', 'onkeydown',
'onkeypress', 'onkeyup', 'onload', 'onmousedown', 'onmousemove', 'onmouseout',
'onmouseover', 'onmouseup', 'onmousewheel', 'onpaste', 'onreset', 'onresize', 'onscroll', 'onsearch', 'onselect', 'onselectstart', 'onsubmit', 'onunload');


    // The node root
    for (var j = 0; j < allEvents.length; j++)
    {
        eval('if( orgNode.' + allEvents[j] + ' ) cloneNode.' + allEvents[j] + ' = orgNode.' + allEvents[j]);
    }

    // Node descendants
    for (var i = 0; i < orgNodeEvents.length; i++)
    {
        for (var j = 0; j < allEvents.length; j++)
        {
            eval('if( orgNodeEvents[i].' + allEvents[j] + ' ) cloneNodeEvents[i].' + allEvents[j] + ' = orgNodeEvents[i].' + allEvents[j]);
        }
    }

    return cloneNode;

}

function setCookie(Nombre, Valor, DiasExpiracion)
{
    var expires = "";
    if (DiasExpiracion != undefined)
    {
        var d = new Date();
        d.setTime(d.getTime() + (DiasExpiracion * 24 * 60 * 60 * 1000));
        expires = ";expires=" + d.toGMTString();
    }
    document.cookie = Nombre + "=" + Valor.replace(/[;]/g, "¬") + expires;
}

function getCookie(Nombre)
{
    var nombre = Nombre + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++)
    {
        var c = ca[i].trim();
        if (c.indexOf(nombre) == 0) return c.substring(nombre.length, c.length).replace(/[¬]/g, ";");
    }
    return "";
}

Array.prototype.contains = function (obj) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}