/** ----------------------------------------------------------------------------
*      Descripcion     : Funciones varias para ordenar.
*      @author  Miguel Berroteran.
*      Date created    : 26/06/2011            Date updated: 27/06/2011
*      Docs By         : Miguel Berroteran.
*      @Copyright      : (c) 2011 by Password Techonology.
* -----------------------------------------------------------------------------
*/


/**
*  Funcion que remplaza caracacteres en datos string
*/
String.prototype.trim = function ()
{
    return this.replace(/^\s+|\s+$/g, "");
}

/**
*  Variable establecida para determinar el tipo de metodo a usar para ordenar.
*  @var integer umbral variable prefefinida.
*/
umbral = 100;

/**
*  Permite ordenar datos en una tabla.
*  @param encabezado contiene parametros del encabezado
*/
function Ordenar(encabezado)
{
    var tabla = encabezado.parentNode.parentNode;
    var columna = encabezado.cellIndex;
    if (tabla.getAttribute("columna") == undefined || parseInt(tabla.getAttribute("columna")) != columna)
    {
        if (tabla.getAttribute("columna") != undefined)
        {
            var temp = tabla.children[0].children[parseInt(tabla.getAttribute("columna"))];
            temp.className = temp.className.substr(0, temp.className.length - 11);
        }
        //quitar estilos de la columna actualmente ordenada
        tabla.setAttribute("columna", columna);
        //ordenarShell(tabla, columna, encabezado.className.split(' ')[1], true, 1, tabla.children.length - 1);
        ordenarSeleccion(tabla, columna, encabezado.className.split(' ')[1], true, 1, tabla.children.length - 1);
        encabezado.className += " grdOrdered";
    } else
    {
        invertir(tabla, 1, tabla.children.length - 1);
    }
    RedefinirAlternativo(tabla);
}

/**
*  Invierte tabla segun los parametro.
*  @param tabla.
*  @param indiceInf.
*  @param indiceSup.
*/
function invertir(tabla, indiceInf, indiceSup)
{
    var iI = indiceInf; //índice que aumenta
    var iS = indiceSup; //índice que disminuye
    while (iI < iS)
    {
        swap(tabla, iI, iS);
        iI++;
        iS--;
    }
    RedefinirAlternativo(tabla);
}

/**
*  Metodo de ordenacion Shell de la tabla
*  @param tabla.
*  @param columna indica la columna por la que se va ordenar la tabla.
*  @param tipo.
*  @param asc.
*  @param indiceInf.
*  @param indiceSup.
*/
function ordenarShell(tabla, columna, tipo, asc, indiceInf, indiceSup)
{
    //algoritmo shell para ordenación, este algoritmo es recursivo, por lo cual, puede dar lugar a pilas de ejecución más o menos grandes.
    //si probando encontramos que hay peos con la pila, cambiamos la implementación sincrónica por una asincrónica y funcionaría igual.
    //el único peo con la implementación asincrónica es que si la gente manda a ordenar varias veces muy rápido la lista puede tener un comportamiento extraño...
    if (indiceInf >= indiceSup) return; //si los índices están invertidos, no hay nada que hacer...
    if (indiceInf == indiceSup - 1)
    {
        ordenarBurbuja1(tabla, columna, tipo, asc, indiceInf, indiceSup);
        return;
    } //si solo son 2 valores, con una burbuja, ya están ordenados. Para que matar una mosca con una bazooka?
    var val = tabla.children[Math.floor((indiceSup + indiceInf) / 2)].children[columna]; //obtiene el elemento central
    var iI = indiceInf; //índice que aumenta
    var iS = indiceSup; //índice que disminuye
    while (iI <= iS + 1)
    {//mientras los índices no se crucen...
        while (comparar(val, tabla.children[iI].children[columna], tipo, asc, true))
        { //corre el índice inferior a la derecha hasta que encuentres un valor mayor a "val"
            iI++;
            if (iI > indiceSup) break; //pero tampoco te vayas de largo...
        }
        while (comparar(tabla.children[iS].children[columna], val, tipo, asc, true))
        { //corre el índice superior a la izquierda hasta que encuentres un valor menor a "val"
            iS--;
            if (iS < indiceInf) break; //pero tampoco te vayas de largo...
        }
        if (iI > iS)
        {//si los índices se cruzaron...
            if (iI > indiceSup) iI = indiceSup; //el índice inferior no puede ser mayor al último
            if (iS < indiceInf) iS = indiceInf; //el índice superior no puede ser menor al primero
            break; //salte de esta vaina...
        }
        swap(tabla, iI, iS); //intercambia valores para seguir moviendo índices
    }
    if (iS == indiceSup)
    {//si el índice superior es igual al último, no tiene sentido seguir iterando con shell porque entraría en ciclo infinito, aplícale una burbuja para continuar...
        if (!ordenarBurbuja1(tabla, columna, tipo, asc, indiceInf, iS)) return; //y si la burbuja no hizo cambios, es porque la lista está ordenada, estamos listos.
        iS--; //si la burbuja hizo cambios, estamos seguros de que el último elemento ya está en su lugar, es la filosofía de la burbuja. Entonces, reducimos un puesto el índice superior.
    }
    if (iI == indiceInf)
    {//si el índice inferior es igual al primero, no tiene sentido seguir iterando con shell porque entraría en ciclo infinito, aplícale una burbuja para continuar...
        if (!ordenarBurbuja1(tabla, columna, tipo, asc, iI, indiceSup)) return; //y si la burbuja no hizo cambios, es porque la lista está ordenada, estamos listos.
        //si la burbuja hizo cambios, estamos seguros de que el último elemento ya está en su lugar, es la filosofía de la burbuja. Pero me da cague modificar indiceSup.
        //hay que evaluar si el cambio se propaga, si no lo hace no hay problemas en modificar indiceSup.
        //otra solución sería definir una burbuja inversa, para modificar iI mejor...
        //de todas maneras así funciona, que coño...
    }
    //en este momento, la lista inicial, se parte en 2 pedazos, donde los índices se cruzan, y se hace una llamada recursiva al método de ordenación con cada una de las listas.
    //el algoritmo shell es muy efectivo para listas asquerosamente grandes, hay que calcular el umbral de efectividad, por encima de el, es mejor shell, por debajo es mejor burbuja
    if ((iS - indiceInf) > umbral)
    {// si la longitud de la primera lista supera el umbral, hay que ordenar con shell
        ordenarShell(tabla, columna, tipo, asc, indiceInf, iS);
    } else
    {//si no, utilizamos un método efectivo, intuitivo y mejor que shell para listas cortas, la famosa burbuja!
        ordenarBurbuja(tabla, columna, tipo, asc, indiceInf, iS);
    }
    if ((indiceSup - iI) > umbral)
    {// si la longitud de la segunda lista supera el umbral, hay que ordenar con shell
        ordenarShell(tabla, columna, tipo, asc, iI, indiceSup);
    } else
    {//si no, utilizamos un método efectivo, intuitivo y mejor que shell para listas cortas, la famosa burbuja!
        ordenarBurbuja(tabla, columna, tipo, asc, iI, indiceSup);
    }
}


/**
*  Metodo de ordenacion Burbuja
*  @param tabla.
*  @param columna indica la columna por la que se va ordenar la tabla.
*  @param tipo.
*  @param asc.
*  @param indiceInf.
*  @param indiceSup.
*/
function ordenarBurbuja1(tabla, columna, tipo, asc, indiceInf, indiceSup)
{
    //1 paso del algoritmo de la burbuja para ordenación
    //útil para desatascar al algoritmo shell y base fundamental para el algoritmo completo de la burbuja,
    //lo interesante de implementar 1 solo paso es que con él podemos usarlo para los 2 métodos y además, sirve para identificar si una lista ya estaba ordenada antes de invocarlo
    var result = false; //indicamos que no se han hecho cambios...
    if (indiceInf >= indiceSup) return result; //si los índices están cruzados, no hay nada que hacer..
    for (i = indiceInf; i < indiceSup; i++)
    {//recorre la lista hasta el penúltimo índice...
        //compara valores por pares adyacentes..
        if (comparar(tabla.children[i].children[columna], tabla.children[i + 1].children[columna], tipo, asc, false))
        {//si el elemento actual es mayor que el siguiente...
            swap(tabla, i, i + 1); //cámbialos...
            result = true; //y marco que ya hice cambios...
        }
    }
    //si result quedó en falso, quiere decir que todos los elementos son menores o iguales que los siguientes
    //entonces estamos completamente seguros de que la lista ya está perfectamente ordenada
    //una mejora que pudiera hacerse es que sabiendo cual es el último cambio que se hizo, podemos reducir a índice sup en más de 1 en cada paso, lo cual aumentaría la velocidad del método.
    return result;
}


/**
*  Metodo de ordenacion burbuja
*  @param tabla.
*  @param columna indica la columna por la que se va ordenar la tabla.
*  @param tipo.
*  @param asc.
*  @param indiceInf.
*  @param indiceSup.
*/
function ordenarBurbuja(tabla, columna, tipo, asc, indiceInf, indiceSup)
{
    var iS = indiceSup;
    //repite un paso de burbuja hasta que devuelva falso, y ve rebajando el índice porque cada vez que se ejecuta una burbuja, el último elemento ya está ordenado.
    while (ordenarBurbuja1(tabla, columna, tipo, asc, indiceInf, iS))
    {
        iS--;
    }
    //¡asquerosamente simple!, ¿no?
}

function OrdenarSeleccion1(tabla, columna, tipo, asc, indiceInf, indiceSup)
{
    //1 paso del algoritmo de selección para ordenación
    //útil para desatascar al algoritmo shell y base fundamental para el algoritmo completo de seleccion,
    //lo interesante de implementar 1 solo paso es que con él podemos usarlo para los 2 métodos y además, sirve para identificar si una lista ya estaba ordenada antes de invocarlo
    var result = false; //indicamos que no se han hecho cambios...
    if (indiceInf >= indiceSup) return result; //si los índices están cruzados, no hay nada que hacer..
    var elegidoPrimero = indiceInf;
    var valorElegidoPrimero = $(tabla.children[elegidoPrimero].children[columna]).text();
    var elegidoUltimo = indiceSup;
    var valorElegidoUltimo = $(tabla.children[elegidoUltimo].children[columna]).text();
    for (i = indiceInf; i <= indiceSup; i++)
    {//recorre la lista hasta el penúltimo índice...
        //compara valores por pares adyacentes..
        var valorActual = $(tabla.children[i].children[columna]).text();
        if (comparar(valorElegidoPrimero, valorActual, tipo, asc, false))
        {//si el elemento elegido es mayor que el actual
            elegidoPrimero = i;
            valorElegidoPrimero = valorActual;
            result = true; //y marco que ya hice cambios...
        }
        if (comparar(valorActual, valorElegidoUltimo, tipo, asc, false))
        {//si el elemento elegido es mayor que el actual
            elegidoUltimo = i;
            valorElegidoUltimo = valorActual;
            result = true; //y marco que ya hice cambios...
        }
    }
    if (result)
    {
        //if (indiceInf == elegidoUltimo && indiceSup == elegidoPrimero)
        //{
        //    swap(tabla, indiceInf, elegidoPrimero); //cámbialos...
        //} else
        //{
        swap(tabla, indiceInf, elegidoPrimero); //cámbialos...
        if (elegidoUltimo == indiceInf) elegidoUltimo = elegidoPrimero;
        swap(tabla, indiceSup, elegidoUltimo); //cámbialos...
        //}
    }
    //si result quedó en falso, quiere decir que todos los elementos son menores o iguales que los siguientes
    //entonces estamos completamente seguros de que la lista ya está perfectamente ordenada
    //una mejora que pudiera hacerse es que sabiendo cual es el último cambio que se hizo, podemos reducir a índice sup en más de 1 en cada paso, lo cual aumentaría la velocidad del método.
    return result;
}
function ordenarSeleccion(tabla, columna, tipo, asc, indiceInf, indiceSup)
{
    var iI = indiceInf;
    var iS = indiceSup;
    while (iI < iS)
    {
        OrdenarSeleccion1(tabla, columna, tipo, asc, iI, iS);
        iI++;
        iS--;
    }
}

/**
*  esta función es para que el algoritmo de ordenación no dependa en lo absoluto del tipo de datos que está ordenando.
*  además, podemos definir nuevos tipos de datos y sus reglas de ordenación, y los métodos funcionarían igual.
*  @param v1
*  @param v2
*  @param tipo.
*  @param asc.
*  @param eq.
*  @return  compararTipo(valor1,valor2,asc,eq).
*/
function comparar(v1, v2, tipo, asc, eq)
{
    var valor1 = valor(v1, tipo);
    var valor2 = valor(v2, tipo);
    return compararTipo(valor1, valor2, asc, eq);
}


/**
*  funcion para comparar los tipos de dato.
*  @param valor1
*  @param valor2
*  @param asc.
*  @param eq.
*/
function compararTipo(valor1, valor2, asc, eq)
{
    if (eq) return asc ? (valor1 >= valor2) : (valor1 <= valor2);
    else return asc ? (valor1 > valor2) : (valor1 < valor2);
}


/**
*  funcion para de terminar el tipo de dato de una variable.
*  @param v contiene el valor para analizar el tipo de dato.
*  @param tipo determina el tipo dato.
*  @return integer|string|boolean|date puede retornar variable integer, string, boolean o date.
*/
function valor(v, tipo)
{
    switch (tipo)
    {
        case 'grdEntero':
            return val_int(valEx(v));
            break;
        case 'grdDecimal':
            return val_dec(valEx(v));
            break;
        case 'grdBoolean':
            return v.children[0].checked;
            break;
        case 'grdFecha':
            var valorEx = valEx(v);
            return valorEx.substr(6) + '-' + valorEx.substr(3, 2) + '-' + valorEx.substr(0, 2);
            break;
        default: //grdTexto
            return valEx(v);
            break;
    }
}


/**
*  funcion para de terminar el tipo de dato de una variable.
*  @param v contiene el valor para analizar el tipo de dato.
*  @param tipo determina el tipo de dato.
*  @return integer|string|boolean|date puede retornar variable integer, string, boolean o date.
*/
function valorNoCelda(v, tipo)
{
    if (v == undefined) return v;
    switch (tipo)
    {
        case 'grdEntero':
            return parseInt(v);
            break;
        case 'grdDecimal':
            return parseFloat(v);
            break;
        case 'grdBoolean':
            return v;
            break;
        case 'grdFecha':
            return v.substr(6) + '-' + v.substr(3, 2) + '-' + v.substr(0, 2);
            break;
        default: //grdTexto
            return v;
            break;
    }
}


/**
*  Descripción:
*  @param v 
*  @return.
*/
function valEx(v)
{
    //los exploradores son una ladilla con eso de saber los valores de las celdas...
    //alert ('|'+v.value+'|' +v.innerText+'|' +v.textContent+'|' +v.innerHTML+'|');
    return (v.value ? v.value : (v.innerText ? v.innerText : (v.textContent ? v.textContent : (v.innerHTML ? v.innerHTML : v))));
}


/**
*  Descripción:
*  @param tabla contenido de la tabla.
*  @param iI indice inferio de la tabla.
*  @param iS indice superior de la tabla.
*/
/* function swap(tabla, iI, iS)
{
if (iI == iS) return;
var r1 = tabla.children[iI].cloneNode(true);
var r2 = tabla.children[iS].cloneNode(true);
tabla.replaceChild(r2, tabla.children[iI]);
tabla.replaceChild(r1, tabla.children[iS]);
}
*/

function swap(oTable, i, j)
{
    var trs = oTable.getElementsByTagName("tr");

    if (i == j + 1)
    {
        oTable.insertBefore(trs[i], trs[j]);
    }
    else if (j == i + 1)
    {
        oTable.insertBefore(trs[j], trs[i]);
    }
    else
    {
        var tmpNode = oTable.replaceChild(trs[i], trs[j]);
        if (typeof (trs[i]) != "undefined")
        {
            oTable.insertBefore(tmpNode, trs[i]);
        }
        else
        {
            oTable.appendChild(tmpNode);
        }
    }
}
/**
*  Realiza filtros del encabezado.
*  @param encabezadoF.
*/
function Filtrar(encabezadoF, Contenedor, Campo)
{

    var encabezado = encabezadoF.parentNode;
    //    var filtros = document.querySelectorAll(".filtro_body");
    //    for (var i = 0; i < filtros.length; i++)
    //    {
    //        try
    //        {
    //            filtros[i].style.display = "none";
    //            filtros[i].parentNode.removeChild(filtros[i]);
    //        }
    //        catch (e) { }
    //    }
    var tabla = encabezado.parentNode.parentNode;
    var columna = encabezado.cellIndex;
    var tipo = tabla.children[0].children[columna].className; //.split(' ')[1]
    formFiltro = document.createElement("div");
    encabezado.appendChild(formFiltro);
    //formFiltro.nodeName = "Filtro";
    //formFiltro.setAttribute("name", "Filtro");
    //formFiltro.setAttribute("Name", "Filtro");
    //formFiltro.setAttribute("NAME", "Filtro");

    //formFiltro.style.position = "fixed"; //fixed
    //formFiltro.style.left = calcularLeft(encabezadoF);
    //formFiltro.style.top = calcularTop(encabezadoF);

    //$(formFiltro).position({ left: calcularLeft(encabezadoF), top: calcularTop(encabezadoF) });
    /*formFiltro.style.left = 0;
    formFiltro.style.top = 0;*/
    /*    formFiltro.style.position = "fixed";*/
    var Titulo = "";
    formFiltro.className = "filtro_body";
    if (tipo.indexOf("grdEntero") != -1)
    {
        tipo = "grdEntero";
        Titulo = "Filtro para Valores Enteros"
        //span = document.createElement("H3");
        //span.innerText = "Filtro para Valores Enteros";
        //span.textContent = span.innerText;
        //formFiltro.appendChild(span);
    } else if (tipo.indexOf("grdTimeStamp") != -1)
    {
        tipo = "grdTimeStamp";
        Titulo = "Filtro para Intervalos de Tiempo"
        //span = document.createElement("H3");
        //span.innerText = "Filtro para Valores Enteros";
        //span.textContent = span.innerText;
        //formFiltro.appendChild(span);
    } else if (tipo.indexOf("grdFecha") != -1)
    {
        tipo = "grdFecha";
        //span = document.createElement("H3");
        Titulo = "Filtro para Fechas";
        //span.innerText = "Filtro para Fechas";
        //span.textContent = span.innerText;
        //formFiltro.appendChild(span);
    } else if (tipo.indexOf("grdDecimal") != -1)
    {
        tipo = "grdDecimal";
        Titulo = "Filtro para Valores Decimales";
        //span = document.createElement("H3");
        //span.innerText = "Filtro para Valores Decimales";
        //span.textContent = span.innerText;
        //formFiltro.appendChild(span);
    } else if (tipo.indexOf("grdBooleano") != -1)
    {
        tipo = "grdBooleano";
        Titulo = "Filtro para Valores Booleanos";
        //span = document.createElement("H3");
        //span.innerText = "Filtro para Valores Booleanos";
        //span.textContent = span.innerText;
        //formFiltro.appendChild(span);
    } else
    {
        tipo = "grdTexto";
        Titulo = "Filtro para Texto";
        //span = document.createElement("H3");
        //span.innerText = "Filtro para Texto";
        //span.textContent = span.innerText;
        //formFiltro.appendChild(span);
    }

    var Aplicar = function ()
    {
        try
        {
            if (this.type.toLowerCase() != "button")
            {
                var evento;
                try { evento = event; }
                catch (e) { evento = arguments[0]; }
                //var evento = event ? event : arguments[0];
                var keyCode = kc(evento);
                if (keyCode != 13) return;
            }
        }
        catch (e)
        {
        }
        var val1 = undefined;
        var val2 = undefined;
        if ("grdEntero,grdDecimal,grdFecha,grdFechaHora,grdTimeStamp".indexOf(tipo) != -1)
        {
            val1 = valor1.value == "" ? undefined : valor1.value;
            val2 = valor2.value == "" ? undefined : valor2.value;
            encabezado.setAttribute("val1", val1);
            encabezado.setAttribute("val2", val2);
        }
        else if ("grdBooleano".indexOf(tipo) != -1)
        {
            val1 = valor1.checked ? "true" : "false";
            encabezado.setAttribute("val1", val1);
        }
        else
        {
            val1=valor1.value == "" ? undefined : valor1.value;
            encabezado.setAttribute("val1", val1);
        }
        if (Contenedor)
        {
                Tabla(Contenedor, Campo, val1, val2 );
            //    try {
            //        Tabla(Contenedor, Campo, valor1.value == undefined ? undefined : valor1.value, valor2 == undefined ? undefined : valor2.value);
            //    }
            //    catch (e) {
            //        Tabla(Contenedor, Campo, valor1.value == undefined ? undefined : valor1.value, undefined);
            //    }
            }
        else
        {
            filtrar(tabla, columna, tipo);
            RedefinirAlternativo(tabla);
        }
        try
        {
            var formFiltro2 = this.parentNode;
            $(formFiltro2).dialog("destroy");
            formFiltro2.parentNode.removeChild(formFiltro2);
        } catch (e) { };
        encabezadoF.className = "filtroActivo";
    }

    var valor1;
    var valor2;
    if ("grdEntero,grdDecimal,grdFecha,grdFechaHora,grdTimeStamp".indexOf(tipo) != -1)
    {
        span = document.createElement("SPAN");
        span.innerText = tipo == "grdFecha" ? "Desde" : ">=";
        span.textContent = span.innerText;
        formFiltro.appendChild(span);
        valor1 = document.createElement("INPUT");
        valor1.type = "TEXT";
        $(valor1).val(encabezado.getAttribute("val1") || "");
        valor1.style.width = "80px";
        valor1.onkeypress = Aplicar;
        //if (tipo == "grdFecha") $(valor1).datepicker($.datepicker.regional["es"]);
        formFiltro.appendChild(valor1);
        span = document.createElement("SPAN");
        formFiltro.appendChild(document.createElement("BR"));
        span.innerText = tipo == "grdFecha" ? "Hasta" : "<=";
        span.textContent = span.innerText;
        formFiltro.appendChild(span);
        valor2 = document.createElement("INPUT");
        valor2.type = "TEXT";
        $(valor2).val(encabezado.getAttribute("val2") || "");
        valor2.style.width = "80px";
        valor2.onkeypress = Aplicar;
        //if (tipo == "grdFecha") $(valor2).datepicker($.datepicker.regional["es"]);
        formFiltro.appendChild(valor2);
    } else if ("grdBooleano".indexOf(tipo) != -1)
    {
        span = document.createElement("SPAN");
        span.innerText = "¿Activo?";
        span.textContent = span.innerText;
        formFiltro.appendChild(span);
        valor1 = document.createElement("INPUT");
        valor1.type = "CHECKBOX";
        valor1.checked = encabezado.getAttribute("val1") || false;
        valor1.onkeypress = Aplicar;
        valor1.style.width = "80px";
        formFiltro.appendChild(valor1);
    } else
    {
        span = document.createElement("SPAN");
        span.innerText = "Contiene:";
        span.textContent = span.innerText;
        formFiltro.appendChild(span);
        valor1 = document.createElement("INPUT");
        valor1.type = "TEXT";
        $(valor1).val(encabezado.getAttribute("val1"));
        valor1.onkeypress = Aplicar;
        valor1.style.width = "80px";
        formFiltro.appendChild(valor1);
    }
    formFiltro.appendChild(document.createElement("BR"));
    boton = document.createElement("INPUT");
    boton.type = "BUTTON";
    boton.value = "Aplicar";
    boton.onclick = Aplicar;
    formFiltro.appendChild(boton);
    boton = document.createElement("INPUT");
    boton.type = "BUTTON";
    boton.value = "Eliminar";
    boton.onclick = function ()
    {
        encabezado.setAttribute("val1", "");
        encabezado.setAttribute("val2", "");

        if (Contenedor)
        {
            Tabla(Contenedor, Campo, undefined, undefined);
        }
        else
        {
            desfiltrar(tabla, columna, tipo);
            RedefinirAlternativo(tabla);
        }
        try
        {
            var formFiltro = this.parentNode;
            $(formFiltro).dialog("destroy");
            formFiltro.style.display = "none";
            formFiltro.parentNode.removeChild(formFiltro);
            encabezadoF.className = "filtroInactivo";
        } catch (e) { }
    }
    formFiltro.appendChild(boton);
    $(formFiltro).dialog({ title: Titulo, modal: true, width: 200, height: 120 });
    //    encabezado.appendChild(formFiltro);
    valor1.focus();

}

/**
*  Realiza filtros en columna de la tabla.
*  @param tabla.
*  @param columna.
*  @param tipo.
*/
function filtrar(tabla, columna, tipo)
{
    var val1 = valorNoCelda(tabla.children[0].children[columna].getAttribute("val1"), tipo);
    var val2 = valorNoCelda(tabla.children[0].children[columna].getAttribute("val2"), tipo);
    for (i = 1; i < tabla.children.length; i++)
    {
        try
        {
            var val = valor(tabla.children[i].children[columna], tipo);
            var visibilidad = true;
            if ("grdEntero,grdDecimal,grdFecha".indexOf(tipo) != -1)
            {
                visibilidad = (val2 == undefined || compararTipo(val2, val, true, true)) && (val1 == undefined || compararTipo(val, val1, true, true));
            } else if ("grdBooleano".indexOf(tipo) != -1)
            {
                visibilidad = (val1 == undefined) || (val == val1);
            } else if ("grdTexto".indexOf(tipo) != -1)
            {
                visibilidad = (val1 == undefined) || (val.trim().toUpperCase().indexOf(val1.trim().toUpperCase()) != -1);
            } else
            {
                visibilidad = true;
            }
            tabla.children[i].children[columna].setAttribute("ocultar", !visibilidad);
            esVisible(tabla.children[i]);
            //tabla.children[i].style.display=visibilidad?"":"none";
        } catch (ex) { }
    }
}


/**
*  Elimina filtros en columna de la tabla.
*  @param tabla.
*  @param columna.
*  @param tipo.
*/
function desfiltrar(tabla, columna, tipo)
{
    for (i = 1; i < tabla.children.length; i++)
    {
        try
        {
            tabla.children[i].children[columna].setAttribute("ocultar", false);
            esVisible(tabla.children[i]);
        } catch (ex) { }
    }
    RedefinirAlternativo(tabla);
}


/**
*  Hace visible las filas.
*  @param Row contiene la columna para determinar si es visible.
*/
function esVisible(Row)
{
    var visibilidad = true;
    for (j = 0; j < Row.children.length; j++)
    {
        if (Row.children[j].getAttribute("ocultar") != undefined && Row.children[j].getAttribute("ocultar") == "true")
        {
            visibilidad = false;
            break;
        }
    }
    Row.style.display = visibilidad ? "" : "none";
}

/**
*  Permite redefinir de la tabla.
*  @param tabla.
*/
function RedefinirAlternativo(tabla)
{
    if (tabla.tagName.toUpperCase() == "TABLE")
    {
        tabla = tabla.getElementsByTagName("TBODY")[0];
    }
    var alt = false;
    var filas=tabla.getElementsByTagName("TR");
    for (i = 0; i < filas.length; i++)
    {
        try
        {
            if (filas[i].style.display != "none")
            {
                $(filas[i]).removeClass(alt ? 'grdRow' : 'grdAlternatingRow');
                $(filas[i]).addClass(alt ? 'grdAlternatingRow' : 'grdRow');
                alt = !alt;
            }
        } catch (ex) { }
    }
}

/**
* Descripcion: ..
* @param control.
* @return result+"px".
*/
function calcularLeft(control)
{
    /*    var result = 0;
    while (control!=null&&control.tagName.toUpperCase() != "BODY") {
    result += control.offsetLeft;
    control = control.offsetParent;
    }
    return result+"px";*/
    return $(control).position.left;
}


/**
* Descripcion: ..
* @param control.
* @return result+"px".
*/
function calcularTop(control)
{
    /*    var result = 0;
    while (control!=null&&control.tagName.toUpperCase() != "BODY") {
    result += control.offsetTop;
    control = control.offsetParent;
    }
    return result+"px";*/
    return $(control).position.top;

}


/**
* Descripcion: Selecciona todos los checkbox de la columna perteneciente al checkbox pasado por parámetro
* @param checkbox al que se hizo click
* @return 
*/
function Seleccionar_TodosCheckBox(checkbox)
{
    var columna = checkbox.parentNode.cellIndex;
    var tabla = checkbox.parentNode.parentNode.parentNode.parentNode;
    elem = tabla.getElementsByTagName("input");
    for (i = 0; i < elem.length; i++)
    {
        var columna2 = elem[i].parentNode.cellIndex;
        if (columna2 == columna)
        {
            elem[i].checked = checkbox.checked;
        }
    }
}
/** 
* Descripcion: Busca el atributo campo en los encabezados de una tabla y obtiene el número de columna que representa el campo en la tabla
* @param Tabla a buscar campos en las columnas
* @return Retorna un array con la relación de campos y el número de columna en que está
*/
function obtenerCampos(tabla)
{
    var respuesta = Array();


    var columns = tabla.getElementsByTagName("tr")[0].getElementsByTagName("th"); ;

    for (j = 0; j < columns.length; j++)
    {
        var campo = columns[j].getAttribute("campo");
        if (campo != undefined)
        {
            respuesta.push(campo);
        }
        else
        {
            respuesta.push("");
        }
    }

    return respuesta;
}