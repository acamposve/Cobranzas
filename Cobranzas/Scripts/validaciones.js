/** ----------------------------------------------------------------------------
*      Descripcion     : Funciones de validación.
*      @author  Miguel Berroteran.
*      Date created    : 27/06/2011            Date updated: 27/06/2011
*      Docs By         : Miguel Berroteran.
*      @Copyright      : (c) 2013
* -----------------------------------------------------------------------------
*/
Regional = {};
Regional.FormatoFecha = 'dd/MM/yyyy';
Regional.FormatoFechaHora = 'dd/MM/yyyy hh:mmtt';
Regional.SeparadorDecimal = ',';
Regional.SeparadorMiles = '.';


/**
* Determina codificación al presionar teclas.
* @param e.
*/
function kc(e)
{
    return e.keyCode ? e.keyCode : e.which ? e.which : e.charCode
}


/**
* Verificación de checkbox.
* @param e.
*/
function chk(e)
{
    var i;
    for (i = 0; i < e.childNodes.length; i++)
    {
        if (e.childNodes[i].type == "checkbox")
        {
            return e.childNodes[i].checked;
        }
    }
}


/**
* Verificación de checkbox actualmente no aplica en este proyecto.
* @param e.
* @param s.
*/
function setchk(e, s)
{
    var i;
    for (i = 0; i < e.childNodes.length; i++)
    {
        if (e.childNodes[i].type == "checkbox")
        {
            e.childNodes[i].checked = s;
        }
    }
}

function validez(t, val, msj)
{
    //t.attributes["valido"]=val?true:undefined; //estaba antes
    t.setAttribute("valido", val ? true : undefined); //estaba comentado antes
    //t.className=val?(t.attributes["requerido"]!=undefined?"requerido":""):"invalido"; //estaba antes
    t.className = val ? (t.getAttribute["requerido"] != undefined ? "requerido" : "") : "invalido"; //no estaba antes
    t.title = val ? "" : msj;
    return val;
}
function vacio(t)
{
    t.setAttribute("valido", undefined);
    t.className = t.getAttribute["requerido"] != undefined ? "requerido" : "";
    return true;
}
//---------------------------------Fechas
/**
* Funcion de validación de fecha
* @param e contiene fecha a verificar.
*/
function kpDate(e)
{
    var keyCode = kc(e);
    var keychar = String.fromCharCode(keyCode);
    var numcheck = /[1234567890\/]/;
    e.returnValue = numcheck.test(keychar) || keyCode == 9 || keyCode == 8 || keyCode == 13;
    return e.returnValue;
}
function kdDate(e)
{
    return true;
}

function focusDate(t)
{
    return vacio(t);
}

/**
* Valida que el formato de la fecha este correta de forma "DD/MM/AAAA".
* @param t contiene fecha a verificar.
* @return false|true puede regresar un valor boolean false|true.
*/
function blurDate(t)
{
    if (t.value == '') return vacio(t);
    var fecha = t.value;
    var Mensaje = 'La Fecha no tiene el formato correcto, debe introducir una fecha con el formato: "DD/MM/AAAA"';
    var ss = fecha.split('/');

    if (ss.length != 3)
    {
        return validez(t, false, Mensaje);
    }

    var ano = Number(ss[2]);
    if (ano < 50) ano += 2000;
    if (ano < 100) ano += 1900;
    if (ano < 1900 || ano > 2100)
    {
        return validez(t, false, Mensaje);
    }
    var bis = (ano % 4 == 0 && ano % 100 != 0) || ano % 400 == 0;
    var mes = Number(ss[1]);
    if (mes < 1 || mes > 12)
    {
        return validez(t, false, Mensaje);
    }
    var dia = Number(ss[0]);
    if (dia < 1 || dia > 31)
    {
        return validez(t, false, Mensaje);
    }
    switch (mes)
    {
        case 4:
        case 6:
        case 9:
        case 11:
            if (dia > 30)
            {
                return validez(t, false, Mensaje);
            }
            break;
        case 2:
            if (dia > (bis ? 29 : 28))
            {
                return validez(t, false, Mensaje);
            }
            break;
    }
    t.value = (dia < 10 ? '0' + dia.toString() : dia.toString()) + '/' + (mes < 10 ? '0' + mes.toString() : mes.toString()) + '/' + (ano.toString());
    return validez(t, true);
}


//------------------------------------------------------------------------------Enteros
/**
* Valida datos numericos enteros
* @param e.
* @return e.returnValue
*/
function kpInt(e)
{
    var keyCode = kc(e);
    var keychar = String.fromCharCode(keyCode);
    var numcheck = /[1234567890]/;
    e.returnValue = numcheck.test(keychar) || keyCode == 9 || keyCode == 8 || keyCode == 13;
    return e.returnValue;
}
function kdInt(e)
{
    return true;
}
function focusInt(t)
{
    return vacio(t);
}


/**
* Valida valor vacio en caso de ser true lo convierte en un integer.
* @param t.
* @return boolean retorna true cuando esta vacio.
*/
function blurInt(t)
{
    if (t.value == '') return vacio(t);
    try
    {
        t.value = parseInt(t.value).toString();
    }
    catch (e)
    {
        return validez(t, false, "Debe escribir un valor entero válido");
    }
    return validez(t, true);
}

/**
* Valida datos numericos enteros
* @param e.
* @return e.returnValue
*/
function kpNInt(e)
{
    var keyCode = kc(e);
    var keychar = String.fromCharCode(keyCode);
    var numcheck = /[1234567890-]/;
    e.returnValue = numcheck.test(keychar) || keyCode == 9 || keyCode == 8 || keyCode == 13;
    return e.returnValue;
}
function kdNInt(e)
{
    return true;
}
function focusNInt(t)
{
    return vacio(t);
}


/**
* Valida valor vacio en caso de ser true lo convierte en un integer.
* @param t.
* @return boolean retorna true cuando esta vacio.
*/
function blurNInt(t)
{
    if (t.value == '') return vacio(t);
    try
    {
        t.value = parseInt(t.value).toString();
    }
    catch (e)
    {
        return validez(t, false, "Debe escribir un valor entero válido");
    }
    return validez(t, true);
}

//------------------------------------------------------------------------------Decimales
/**
* Realiza validación de decimales capturando codificación del tecla presionada.
* @param e.
* @param s.
*/
function kpNDec(e, s)
{
    var keyCode = kc(e);
    var keychar = String.fromCharCode(keyCode);
    var numcheck = /[1234567890-]/;
    e.returnValue = numcheck.test(keychar) || keyCode == 9 || keyCode == 8 || keyCode == 13 || keychar==Regional.SeparadorDecimal;
    if (keychar == Regional.SeparadorDecimal && (s.value.indexOf(Regional.SeparadorDecimal) != -1))
    {
        e.returnValue = false;
    }
    if (keychar == '-' && s.value.length != 0)
    {
        e.returnValue = false;
    }
    return e.returnValue;
}
function kdNDec(e, s)
{
    return true;
}
function focusNDec(t)
{
    t.value = t.value.replace(RegExp("\\" + Regional.SeparadorMiles, "g"), "").replace(RegExp("\\" + Regional.SeparadorDecimal, "g"), ".");
    return vacio(t);
}

function blurNDec(t)
{
    t.value = t.value.replace(".", Regional.SeparadorDecimal);
    t.value = Convert.ToDecimal(t.value).ToString();
}

/**
* Validación de decimales.
* @param e.
* @param t.
* @return boolean de cumplir con las condiciones regresa valor false.
*/
function kpDec(e, t)
{
    var keyCode = kc(e);
    var keychar = String.fromCharCode(keyCode);
    var numcheck = /[1234567890]/;
    e.returnValue = numcheck.test(keychar) || keyCode == 9 || keyCode == 8 || keyCode == 13||keychar==Regional.SeparadorDecimal;
    if ((keychar == Regional.SeparadorDecimal)&& (t.value.indexOf(Regional.SeparadorDecimal) != -1))
    {
        e.returnValue = false;
    }    

    return e.returnValue;
}
function kdDec(e, s)
{
    return true;
}
function focusDec(t)
{
//    t.value = t.value.replace(RegExp("\\" + Regional.SeparadorMiles, "g"), "").replace(RegExp("\\" + Regional.SeparadorDecimal, "g"), ".");
    t.value = t.value.replace(RegExp("\\" + Regional.SeparadorMiles, "g"), "");
    return vacio(t);
}

/**
* Valida si el decimal es vacio en caso de ser true lo convirte en un dato float.
* @param t contiene valor a verificar.
* @return boolean de ser vacio el valor retorna valor true.
*/
function blurDec(t)
{
    if (t.value == '') return vacio(t);
    try
    {
        //t.value = t.value.replace(".", Regional.SeparadorDecimal);
        t.value = t.value.replace(RegExp("\\" + Regional.SeparadorMiles, "g"), "");
        t.value = Convert.ToDecimal(t.value).ToString();
    }
    catch (e)
    {
        return validez(t, false, "Debe escribir un valor Decimal válido");
    }
    return validez(t, true);
}


//------------------------------------------------------------------------------Matemáticas

//Cuenta
/**
* Verifica codigo mediante la función kc.
* @param e.
* @return boolean retorna valor true|false.
*/
function kpMath(e)
{
    var keyCode = kc(e);
    var keychar
    var numcheck
    keychar = String.fromCharCode(keyCode);
    numcheck = /[+-1234567890.]/; ///\d/
    e.returnValue = numcheck.test(keychar) || keyCode == 9 || keyCode == 8 || keyCode == 13 || keychar == '*' || keychar == '/';
    return e.returnValue;
}
function kdMath(e)
{
    return true;
}
function focusMath(t)
{
    return vacio(t);
}


//Cuenta
/**
* Valida cuenta no puede tener un valor negativo.
* @param t.
* @return boolean retorna valor true|false.
*/
function blurMath(t)
{
    if (t.value == '') return vacio(t);
    var result;
    try
    {
        result = eval(t.value);
    }
    catch (e)
    {
        return validez(t, false, "Hubo un error al computar la expresión");
    }
    if (result < 0)
    {
        return validez(t, false, "No debe ser negativo");
    }
    t.value = result;
    return validez(t, true);
}
//------------------------------------------------------------------------------Mail
function kpMail(e, t)
{
    var keyCode = kc(e);
    var keychar
    var numcheck
    keychar = String.fromCharCode(keyCode);
    numcheck = /[\w-+.'@]/; ///\d/
    e.returnValue = numcheck.test(keychar) || keyCode == 9 || keyCode == 8 || keyCode == 13;
    return e.returnValue;
}
function focusMail(t)
{
    return vacio(t);
}
function blurMail(t)
{
    if (t.value == '') return vacio(t);
    var check = /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;
    if (!check.test(t.value)) return validez(t, false, "Debe escribir un mail válido");
    return validez(t, true);
}
function kdMail(e, t)
{
    return true;
}
//------------------------------------------------------------------------------Text
function kpText(e)
{
    return true;
}
function focusText(t)
{
    return vacio(t);
}
function blurText(t)
{
    if (t.value == '') return vacio(t);
    return validez(t, true);
}
function kdText(e)
{
    return true;
}
//------------------------------------------------------------------------------Rif
function kpRif(e, t)
{
    var keyCode = kc(e);
    if (keyCode == 9 || keyCode == 8 || keyCode == 13) return true;
    var keychar
    var numcheck
    keychar = String.fromCharCode(keyCode).toUpperCase();
    if (t.value.length == 10) return false;
    if (t.value == "")
    {
        numcheck = /[JPEVGMD]/; ///\d/
    } else
    {
        numcheck = /[\d]/; ///\d/
    }
    e.returnValue = numcheck.test(keychar);
    return e.returnValue;
}
function focusRif(t)
{
    t.value = t.value.replace(/-/g, '');
    return vacio(t);
}
function blurRif(t)
{
    if (t.value == '') return vacio(t);
    t.value = t.value.toUpperCase();
    t.value = t.value.replace(/-/g, '');
    var check = /^[JPEVGMD]\d{9}/;
    if (!check.test(t.value)) return validez(t, false, "Debe escribir un rif en formato: J123456789");
    t.value = t.value.substr(0, 1) + "-" + t.value.substr(1, 8) + "-" + t.value.substr(9, 1);
    return validez(t, true);
}
function kdRif(e, t)
{
    return true;
}
//------------------------------------------------------------------------------CI
function kpCi(e, t)
{
    var keyCode = kc(e);
    if (keyCode == 9 || keyCode == 8 || keyCode == 13) return true;
    var keychar
    var numcheck
    keychar = String.fromCharCode(keyCode).toUpperCase();
    if (t.value.length == 9) return false;
    if (t.value == "")
    {
        numcheck = /[EVD]/; ///\d/
    } else
    {
        numcheck = /[\d]/; ///\d/
    }
    e.returnValue = numcheck.test(keychar);
    return e.returnValue;
}
function focusCi(t)
{
    t.value = t.value.replace(/-/g, '');
    return vacio(t);
}
function blurCi(t)
{
    if (t.value == '') return vacio(t);
    t.value = t.value.toUpperCase();
    t.value = t.value.replace(/-/g, '');
    var check = /^[EVD]\d{8}/;
    if (!check.test(t.value)) return validez(t, false, "Debe introducir una cédula en el formato: V12345678");
    t.value = t.value.substr(0, 1) + "-" + t.value.substr(1, 8);
    return validez(t, true);
}
function kdCi(e, t)
{
    return true;
}
//------------------------------------------------------------------------------Password
function kpPass(e)
{
    return true;
}
function focusPass(t)
{
    return vacio(t);
}
function blurPass(t)
{
    if (t.value == '') return vacio(t);
    return validez(t, t.value.length >= 8, "Debe tener al menos 8 caracteres");
}
function kdPass(e)
{
    return true;
}
//------------------------------------------------------------------------------

//------------------------------------------------------------------------------
function changeSelect(t)
{
    return validez(t, t.value != "", "Debe Seleccionar un valor");
}

function validar(t)
{
    elem = t.getElementsByTagName("INPUT");
    for (i = 0; i < elem.length; i++)
    {
        if (elem[i].type.toUpperCase() == "TEXT" || elem[i].type.toUpperCase() == "PASSWORD")
        {
            try
            {
                elem[i].fireEvent("onfocus");
                elem[i].fireEvent("onblur");
            } catch (e)
            {
                elem[i].focus(elem[i]);
                elem[i].blur(elem[i]);
            }
            if ((elem[i].getAttribute("valido") == undefined || elem[i].getAttribute("valido") == "undefined") && !(elem[i].getAttribute("requerido") == undefined || elem[i].getAttribute("requerido") == "undefined"))
            {
                msgbox("Debe llenar los campos obligatorios para continuar", "tiempo");
                elem[i].focus();
                return false;
            }
        }
    }
    elem = t.getElementsByTagName("SELECT");
    for (i = 0; i < elem.length; i++)
    {
        try
        {
            elem[i].fireEvent("onfocus");
            elem[i].fireEvent("onblur");
        } catch (e)
        {
            elem[i].focus(elem[i]);
            elem[i].blur(elem[i]);
        }
        if ((elem[i].getAttribute("valido") == undefined || elem[i].getAttribute("valido") == "undefined") && !(elem[i].getAttribute("requerido") == undefined || elem[i].getAttribute("requerido") == "undefined"))
        {
            msgbox("Debe llenar los campos obligatorios para continuar", "tiempo");
            elem[i].focus();
            return false;
        }
    }
    elem = t.getElementsByTagName("TEXTAREA");
    for (i = 0; i < elem.length; i++)
    {
        try
        {
            elem[i].fireEvent("onfocus");
            elem[i].fireEvent("onblur");
        } catch (e)
        {
            elem[i].focus(elem[i]);
            elem[i].blur(elem[i]);
        }
        if ((elem[i].getAttribute("valido") == undefined || elem[i].getAttribute("valido") == "undefined") && !(elem[i].getAttribute("requerido") == undefined || elem[i].getAttribute("requerido") == "undefined"))
        {
            msgbox("Debe llenar los campos obligatorios para continuar", "tiempo");
            elem[i].focus();
            return false;
        }
    }
    return true;
}

function getText(control)
{
    return control.value ? control.value : (control.innerText ? control.innerText : (control.textContent ? control.textContent : ""));
}
function setText(control, valor)
{
    if (control.value != undefined)
    {
        control.value = valor;
    } else
    {
        control.innerText = valor;
        control.textContent = valor;
    }
}

function mostrarOcultar(t)
{
    t.style.display = t.style.display == '' ? 'none' : '';
}
