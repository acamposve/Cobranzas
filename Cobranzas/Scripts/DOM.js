/// <reference path="jquery-1.10.2-vsdoc.js"/>
/// <reference path="general.js"/>
/// <reference path="validaciones.js"/>
//Manipulación de tablas html
function CrearTabla(clase, limite) {
    var Result = document.createElement("table");
    if (clase != undefined) Result.className = clase;
    var encabezado = document.createElement("thead");
    Result.appendChild(encabezado);
    var cuerpo = document.createElement("tbody");
    Result.appendChild(cuerpo);
    if (limite) {
        //cuerpo.style.maxHeight = "" + limite + "px";
        //encabezado.style.display = "block";
        //cuerpo.style.display = "block";
        //cuerpo.style.overflow = "auto";
        //cuerpo.style.width = "100%";
    }
    return Result;
}
function AgregarFila(tabla, clase, head) {
    var Result = document.createElement("tr");
    if (head != undefined) {
        Result.style.position = "relative";
        Result.style.top = '0px';
    }
    if (clase != undefined) Result.className = clase;

    var Cuerpo = tabla.getElementsByTagName(head == undefined ? "tbody" : "thead")[0];
    Cuerpo.appendChild(Result);
    return Result;
}
function AgregarHeader(fila, texto, clase, ordenacion, filtro, nombreTabla) {
    var Result = document.createElement("th");
    if (clase != undefined) Result.className = clase;
    if (filtro) {
        var filtro = document.createElement("div");
        filtro.className = "filtroInactivo";
        filtro.onclick = new Function("return Filtrar(this,'" + nombreTabla + "','" + ordenacion + "');");
        Result.appendChild(filtro);
    }
    if (ordenacion) {
        var span = document.createElement("span");
        if (nombreTabla) {
            span.onclick = new Function("Tabla('" + nombreTabla + "','" + ordenacion + "');");
        }
        else {
            span.onclick = new Function("return Ordenar(this.parentNode);");
        }
        span.style.cursor = "Pointer";
        $(span).text(texto);
        Result.appendChild(span);
    }
    else {
        $(Result).text(texto);
    }
    fila.appendChild(Result);
    return Result;
}
function AgregarCelda(fila, texto, clase) {
    var Result = document.createElement("td");
    if (clase != undefined) Result.className = clase;
    Result.innerText = texto;
    Result.textContent = texto;
    //$(Result).text(texto);
    fila.appendChild(Result);
    return Result;
}
function CrearCheckBox(valor) {
    return CrearInputBox(valor, "checkbox");
}
function CrearInputBox(valor, tipo, clase) {
    var Result = document.createElement("input");
    if (clase != undefined) Result.className = clase;

    Result.setAttribute("type", tipo);
    Result.setAttribute("value", valor);
    return Result;
}
function CreateInput(Clase) {
    var input = document.createElement("input");
    input.type = "text";
    switch (Clase) {
        case "grdDecimal":
            input.onkeypress = new Function("return kpDec(event,this);");
            input.onfocus = new Function("return focusDec(this);");
            input.onblur = new Function("return blurDec(this);");
            break;
    }
    return input;
}
function LlenarCombo(Parametros) {
    /// <summary>
    ///     Permite Llenar un combo con data
    /// </summary>
    /// <param name="Parametros" type="Object">
    ///     Un objeto con las propiedades para el dibujado de la tabla:
    ///     &#10;    * Combo: Combo a llenar (Tipo Combo o String)
    ///     &#10;    * Resultado: Objeto que contiene los resultados a visualizar en el Combo
    ///     &#10;    * CampoId: Campo a utilizar como id del combo
    ///     &#10;    * CampoValor: Campo a utilizar como Display del Combo
    ///     &#10;    * TextoNull: texto a utilizar para la no-selección de elementos
    ///     &#10;    * ValorNull: Valor a utilizar para la no-selección de elementos
    ///     &#10;    * DivResultado: Div donde se reflerajan los elementos seleccionados
    /// </param>
    /// <returns type="void" />
    if (typeof (Parametros.Combo) == "string") {
        Parametros.Combo = $("#" + Parametros.Combo)[0];
    }
    $(Parametros.Combo).empty();
    if (Parametros.TextoNull != undefined) {
        var itemn = document.createElement("OPTION");
        $(itemn).val(Parametros.ValorNull);
        $(itemn).text(Parametros.TextoNull);
        Parametros.Combo.appendChild(itemn);
    }
    for (var i = 0; i < Parametros.Resultado.length; i++) {
        var op = Parametros.Resultado[i];
        var item = document.createElement("OPTION");
        $(item).text(Parametros.CampoValor ? op[Parametros.CampoValor] : (Parametros.CampoId ? op[Parametros.CampoId] : op));
        $(item).val(Parametros.CampoId ? op[Parametros.CampoId] : op);
        Parametros.Combo.appendChild(item);
    }
    if (Parametros.DivResultado) {
        var oc = Parametros.Combo.onchange;
        Parametros.Combo.onchange = function (a) {
            var lista = $("#" + Parametros.DivResultado)[0];
            var idElemento = $(this).val();
            if (idElemento == "") return;

            var Elementos = lista.getElementsByTagName("SPAN");

            for (var i = Elementos.length - 1 ; i >= 0; i--) {
                if (Elementos[i].title == idElemento) return;
            }

            if (idElemento == "*") {
                lista = [];
                for (var i = Elementos.length - 1; i >= 0; i--) {
                    Elementos[i].parentNode.removeChild(Elementos[i]);
                }
                return;
            }
            var text = $("#" + this.id + " option:selected").html();
            $(this).val("");
            var span = document.createElement("SPAN");
            $(span).text(text);
            $(span)[0].title = idElemento;
            $(span)[0].className = "Telefono";
            $(span).click(function Eliminar(t) {
                this.parentNode.removeChild(this);
                oc();
            });
            lista.appendChild(span);
            oc();
        }
    }
}
function ObtenerLista(contenedor) {
    var padre = $("#" + contenedor)[0];
    var hijos = padre.getElementsByTagName("span");
    var result = [];
    for (var i = 0; i < hijos.length; i++) {
        result.push(hijos[i].title);
    }
    return result;
}
function Comparar(a, b) {
    if (a == null) return -1;
    if (b == null) return 1;
    if (typeof (a) == "string") {
        a = a.toLowerCase();
        b = b.toLowerCase();
    }

    return (a == 0) ? 0 : (a > b ? 1 : -1);
}
function Tabla(Parametros) {
    /// <summary>
    ///     Permite Crear una tabla con los Resultados.
    ///     &#10;    Permite un solo parámetro de tipo Objeto que contiene las propiedades para generar la tabla
    /// </summary>
    /// <param name="Parametros" type="Object">
    ///     Un objeto con las propiedades para el dibujado de la tabla:
    ///     &#10;.    * Contenedor: Div que va a contener a la tabla
    ///     &#10;.    * Resultado: Objeto que contiene los resultados a visualizar en la tabla
    ///     &#10;.    * TodosSeleccionados: Indica si los elementos deberían aparecer seleccionados
    ///     &#10;.    * idSeleccion: Campo que se utiliza para identificar la tabla al momento de seleccionar los elementos
    ///     &#10;.    * CheckBox: Campo que se utiliza para indicar que no se va a utilizar un campo para checkbox
    ///     &#10;.    * Paginacion: Entero que indica cuantas cuentas se muestran por página
    ///     &#10;.    * Pagina: Entero que indica cual página se va a mostrar
    ///     &#10;.    * Campos: Arreglo que indica las columnas a visualizar
    ///     &#10;.           * Titulo: Indica lo que se va a mostrar en el encabezado
    ///     &#10;.           * Campo: Campo del resultado que se va a mostrar
    ///     &#10;.           * Campo2: Campo scundario del resultado que se va a mostrar
    ///     &#10;.           * Clase: Tipo de datos de la columna [grdTexto,grdBooleano,grdFecha,grdFechaHora,grdEntero,grdDecimal]
    ///     &#10;.           * Ordenacion: Indica si se va a permitir ordenar esa columna
    ///     &#10;.           * Filtro: Indica si se va a permitir filtrar esa columna
    ///     &#10;.           * Pre: Prefijo de los datos en la columna
    ///     &#10;.           * Post: Sufijo de los datos en la columna
    ///     &#10;.           * Combo: Objeto de tipo Select para utilizar en la columna
    ///     &#10;.           * Imagen: String o funcion que devuelve string para el src de una imagen en el campo
    ///     &#10;.           * ToolTip: Campo que se va a utilizar para el ToolTip del campo.
    ///     &#10;.           * Resumen: Que tipo de Resumen se va a utilizar (SUM,COUNT,PROM)
    ///     &#10;.           * OnClick: Función a ejecutar Cuando se da click en la celda.
    ///     &#10;.    * FuncSeleccionar: Funcion a ejecutar al hacer click en la fila
    ///     &#10;.    * FuncEliminar: Function a ejecutar al hacer click en el botón eliminar
    ///     &#10;.    * DivExportar: Div donde se colocara el boton de Exportar
    ///     &#10;.    * NombreExportar: Nombre que tendra el archivo a Exportar
    /// </param>
    /// <returns type="void" />
    if (typeof (Parametros) == "string") //solo página...
    {
        Parametros = Tablas[Parametros];
        switch (arguments.length) {
            case 2:
                if (typeof (arguments[1]) == "number")//Consultar Página
                {
                    //NombreTabla,Página
                    Parametros.Pagina = arguments[1];
                }
                else //Nombre de Campo para ordenar;
                {
                    //NombreTabla,Campo
                    var Campo = arguments[1];
                    if (Parametros.OrdenadoPor == Campo) {
                        Parametros.OrdenadoPor = "-" + Parametros.OrdenadoPor;
                        //Parametros.Resultado.reverse();
                    } else {
                        Parametros.OrdenadoPor = Campo;
                        //Parametros.Resultado.sort(function (a, b) { return Comparar(a[Campo], b[Campo]); });
                    }
                }
                break;
            case 3:
            case 4:
                //NombreTabla,Campo,Valor1[,Valor2]
                var Campo = arguments[1];
                if (Parametros.Filtros == undefined) { Parametros.Filtros = []; }
                if (arguments[2] == undefined && arguments[3] == undefined) {
                    try {
                        //                        Parametros.Filtros[Campo] = {};
                        Parametros.Filtros[Campo] = undefined;
                    } catch (e) { }
                }
                else {
                    Parametros.Filtros[Campo] = { Campo: Campo, Valor1: arguments[2], Valor2: arguments[3] };
                }
                break;
        }
    }
    else {
        //var Filtros = undefined;
        //var OrdenadoPor = undefined;
        try {
            Tablas.length;

            var CNL = Parametros.Contenedor;
            try {
                Parametros.Filtros = Parametros.Filtros || Tablas[CNL].Filtros;
                Parametros.OrdenadoPor = Parametros.OrdenadoPor || Tablas[CNL].OrdenadoPor;
            } catch (e) { }

        } catch (e) {
            Tablas = Object();
        }
        Tablas[Parametros.Contenedor] = Parametros;

    }
    var Resultados = Parametros.Resultado;

    //Ordenación:
    if (Parametros.OrdenadoPor != undefined) {
        var Inverso = Parametros.OrdenadoPor.substring(0, 1) == "-";
        var Campo = Parametros.OrdenadoPor;
        if (Inverso) Campo = Campo.substring(1);
        Resultados.sort(function (a, b) { return Comparar(a[Campo], b[Campo]); });
        if (Inverso) Resultados.reverse();
    }
    //Filtros:
    if (Parametros.Filtros != undefined) {
        var len = Parametros.Filtros.length;
        for (var i in Parametros.Filtros) {
            Filtro = Parametros.Filtros[i];
            if (typeof (Filtro) != "object") continue;
            var Campo = Filtro.Campo;
            var Valor1 = Filtro.Valor1;
            var Valor2 = Filtro.Valor2;
            var Clase = Parametros.Campos.where(function (a) { return a.Campo == Campo; })[0].Clase;
            switch (Clase) {
                case "grdBooleano":
                    Filtro.Valor1 = Valor1 == "true";
                    break;
                case "grdFecha":
                    Filtro.Valor1 = Convert.ToDate(Valor1);
                    Filtro.Valor2 = Convert.ToDate(Valor2);
                    break;
                case "grdFechaHora":
                    Filtro.Valor1 = Convert.ToDateTime(Valor1);
                    Filtro.Valor2 = Convert.ToDateTime(Valor2);
                    break;
                case "grdHora":
                    Filtro.Valor1 = Convert.ToDateTime(Valor1);
                    Filtro.Valor2 = Convert.ToDateTime(Valor2);
                    break;
                case "grdDecimal":
                    Filtro.Valor1 = Convert.ToDecimal(Valor1);
                    Filtro.Valor2 = Convert.ToDecimal(Valor2);
                    break;
                case "grdTimeStamp":
                case "grdEntero":
                    Filtro.Valor1 = Convert.ToInt32(Valor1);
                    Filtro.Valor2 = Convert.ToInt32(Valor2);
                    break;
                case "grdTexto":
                    Filtro.Valor1 = Filtro.Valor1.toLowerCase();
                    break;
            }
        }

        Resultados = Parametros.Resultado.where(function (a) {
            var len = Parametros.Filtros.length;
            for (var i in Parametros.Filtros) {
                var Filtro = Parametros.Filtros[i];
                if (typeof (Filtro) != "object") continue;
                var Campo = Filtro.Campo;
                var Valor1 = Filtro.Valor1;
                var Valor2 = Filtro.Valor2;
                var Clase = Parametros.Campos.where(function (b) { return b.Campo == Campo; })[0].Clase;
                var Valor = "";
                if (Campo.indexOf(".") == -1) {
                    Valor = a[Campo];
                } else {
                    with (a) {
                        Valor = eval(Campo);
                    }
                }
                if (Clase == "grdTexto") {
                    if (Valor.toLowerCase().indexOf(Valor1) == -1) { return false; }
                } else if (Clase == "grdBooleano") {
                    return Valor1 == Valor;
                } else {
                    if (Valor1 != undefined && Valor < Valor1) return false;
                    if (Valor2 != undefined && Valor > Valor2) return false;
                }
            }
            return true;
        });
    }
    //Preparando al Contenedor
    var Contenedor = $("#" + Parametros.Contenedor);
    Contenedor[0].style.overflowX = "scroll";
    Contenedor[0].style.overflowY = "auto";
    var Scroll = Contenedor.scrollTop();
    Contenedor.html("");

    //Paginación:
    if (Resultados.length > 1000 && Parametros.Paginacion == undefined) {
        Parametros.Paginacion = 1000;
    }
    var Paginador;
    if (Parametros.Paginacion && Resultados.length > Parametros.Paginacion) {
        if (Parametros.Pagina == undefined) Parametros.Pagina = 1;
        Paginador = document.createElement("div");
        Contenedor[0].appendChild(Paginador);
        var Paginas = Math.ceil(Resultados.length / Parametros.Paginacion);
        for (var i = 0; i < Paginas; i++) {
            var Pagina = document.createElement("span");
            Paginador.appendChild(Pagina);
            $(Pagina).text(i + 1);
            if (i + 1 == Parametros.Pagina) {
                Pagina.className = "PaginaActual";
            }
            else {
                Pagina.className = "Pagina";
                Pagina.onclick = new Function("Tabla('" + Parametros.Contenedor + "'," + (i + 1) + ");");
            }
            var min = i * Parametros.Paginacion;
            var max = (i + 1) * Parametros.Paginacion;
            max = max < Resultados.length ? max : Resultados.length;
            max = max - 1;
            var ToolTip = (i * Parametros.Paginacion + 1) + "-" + (max + 1);
            if (Parametros.OrdenadoPor != undefined) {
                var ini = Resultados[min][Parametros.OrdenadoPor];
                var fin = Resultados[max][Parametros.OrdenadoPor];
                ToolTip += "\n" + Parametros.OrdenadoPor + " Desde: " + ini + " Hasta: " + fin;
            }
            Pagina.title = ToolTip;
        }
    }

    //Dibujado de la Tabla como tal:
    var tabla = CrearTabla("Tabla", Parametros.LimitarAltura);
    if (Parametros.LimitarAltura) {
        Contenedor[0].style.maxHeight = "" + Parametros.LimitarAltura + "px";
    }
    Contenedor[0].appendChild(tabla);
    if (Paginador != undefined) {
        Contenedor[0].appendChild(SuperClonar(Paginador));
    }
    //Dibujado de la Fila de encabezados:
    var fila = AgregarFila(tabla, undefined, true);
    if (Parametros.idSeleccion) {
        fila.setAttribute("idSeleccion", Parametros.idSeleccion);
    }
    if (Parametros.idSeleccion != undefined && Parametros.CheckBox !== false) {
        var checkbox = CrearCheckBox("0");
        checkbox.onclick = function () { Seleccionar_TodosCheckBox(this); }
        checkbox.checked = Parametros.TodosSeleccionados;
        var celda = AgregarHeader(fila, "", "grdCheckBox", false, false);
        celda.appendChild(checkbox);
        celda.setAttribute("campo", Parametros.idSeleccion);
        tabla.setAttribute("idSeleccion", Parametros.idSeleccion);
    }
    var i;
    var len = Parametros.Campos.length;
    for (i = 0; i < len; i++) {
        var Campo = Parametros.Campos[i];
        if (Campo == undefined) continue;
        var CampoH;
        try { CampoH = Recursos[Campo.Titulo] }
        catch (e) { CampoH = undefined }
        if (CampoH == undefined) {
            var Ast = Parametros.Traduccion === false ? "" : "*";
            if (Campo.Titulo != undefined) {
                CampoH = Ast + Campo.Titulo;
            }
            else {
                CampoH = Ast + Campo.Campo;
            }
        }
        var Alias = Campo.Alias || Campo.Campo;
        if (typeof (alias) == "function") Alias = Titulo.replace(/\s/g, "");
        var Header = AgregarHeader(fila, CampoH, Campo.Clase, (Campo.Ordenacion || false) ? (Campo.Ordenacion === true ? Campo.Campo : Campo.Ordenacion) : false, (Campo.Filtro || false) ? Campo.Campo : false, Parametros.Contenedor);
        Header.setAttribute("campo", Alias);
        if (Campo.Campo == Parametros.OrdenadoPor || ("-" + Campo.Campo) == Parametros.OrdenadoPor) Header.style.fontStyle = "italic";
        try {
            if (typeof (Campo.Campo) == "string" && Parametros.Filtros[Campo.Campo] != undefined) {
                Header.getElementsByTagName("div")[0].className = "filtroActivo";
            };
        } catch (e) { }
    }
    if (Parametros.FuncEliminar != undefined) {
        AgregarHeader(fila, "", "", false, false);
    }

    //Recorrido de resultados
    Principio = (Parametros.Pagina == undefined) ? 0 : (Parametros.Pagina - 1) * Parametros.Paginacion;
    //Parametros.Pagina = Principio;
    var len = Resultados.length;
    if (Parametros.Paginacion) {
        len = len < Principio + Parametros.Paginacion ? len : Principio + Parametros.Paginacion;
    }
    for (i = Principio; i < len; i++) {
        var Registro = Resultados[i];
        if (Registro == undefined) continue;
        fila = AgregarFila(tabla, ((Parametros.FuncSeleccionar != undefined) ? "itmSeleccionable" : ""));
        var funcSel;
        if (Parametros.FuncSeleccionar != undefined) {
            funcSel = new Function(Arreglar(Parametros.FuncSeleccionar, Registro));
        }
        if (Parametros.idSeleccion != undefined) {
            fila.setAttribute("idSeleccion", (Registro[Parametros.idSeleccion]));
        }
        if (Parametros.idSeleccion != undefined && Parametros.CheckBox !== false) {
            var checkbox = CrearCheckBox(Registro[Parametros.idSeleccion]);
            checkbox.checked = Parametros.TodosSeleccionados;
            var celda = AgregarCelda(fila, "", "grdCheckBox");
            celda.appendChild(checkbox);
            celda.style.width = "24px";
        }
        var len2 = Parametros.Campos.length;
        for (j = 0; j < len2; j++) {
            var Cont;
            var celda;
            var Campo = Parametros.Campos[j];
            if (Campo == undefined) continue;
            if (typeof (Campo.Campo) == "string") {
                if (Campo.Campo.indexOf(".") == -1) {
                    Cont = Registro[Campo.Campo];
                }
                else {
                    try {
                        with (Registro) {
                            Cont = eval(Campo.Campo);
                        }
                    }
                    catch (e) { Cont = ""; }
                }
            } else {
                Cont = Campo.Campo(Registro);
            }
            try {
                switch (Campo.Clase) {
                    case "grdTimeStamp": Cont = Convert.ToTimeStamp(Cont); break;
                    case "grdDecimal": Cont = Cont.ToString(); break;
                    case "grdFecha": Cont = AFechaMuyCorta(Cont); break;
                    case "grdFechaHora": Cont = AFechaHora(Cont); break;
                    case "grdHora": Cont = Convert.ToDateTime(Cont).ToString("HH:mm"); break;
                    case "grdBooleano": Cont = Cont ? "√" : ""; break;
                }
            }
            catch (ex) { }
            if (Cont == undefined || Cont == "undefined") Cont = "";

            if (Campo.Combo != undefined) {
                var combo2 = Campo.Combo.cloneNode(true);
                $(combo2).val(Cont);
                celda = AgregarCelda(fila, "", ""); //Peligro
                celda.appendChild(combo2);
            }
            else if (Campo.Imagen != undefined) {
                var img = document.createElement("IMG");
                img.src = Cont;
                celda = AgregarCelda(fila, "", "");
                celda.appendChild(img);
                if (typeof (Campo.Imagen) == "string") {
                    img.onclick = new Function(Arreglar(Campo.Imagen, Registro));
                } else {
                    if (Parametros.FuncSeleccionar != undefined) celda.onclick = funcSel;
                }
            }
            else if (Campo.Input === true) {
                var input = CreateInput(Campo.Clase);
                input.style.width = "80px";
                $(input).val(Cont);
                celda = AgregarCelda(fila, "", "");
                celda.appendChild(input);
            }
            else {
                celda = AgregarCelda(fila, (Campo.Pre || "") + Cont + (Registro[Campo.Campo2] || "") + (Campo.Post || ""), Campo.Clase);
                if (Campo.OnClick != undefined) {
                    celda.onclick = new Function(Arreglar(Campo.OnClick, Registro));
                    celda.style.color = "#0000FF";
                    celda.style.textDecoration = "underline";
                } else {
                    if (Parametros.FuncSeleccionar != undefined) celda.onclick = funcSel;
                }
            }
            if (Campo.Tooltip) {
                if (typeof (Campo.Tooltip) == "function") {
                    celda.title = Campo.Tooltip(Registro);
                }
                else {
                    celda.title = Registro[Campo.Tooltip];
                }
            }
            if (Campo.Color) {
                celda.style.backgroundColor = eval("Colores." + Registro[Campo.Color]);
            }
        }
        if (Parametros.FuncEliminar != undefined) {
            var btn = document.createElement("input");
            btn.type = "button";
            btn.className = "btnEliminar16"
            btn.onclick = new Function(Arreglar(Parametros.FuncEliminar, Registro) + ";event.cancelBubble=true;return false;");
            AgregarCelda(fila, "", "").appendChild(btn);
        }
    }
    //Totalizar...
    fila = AgregarFila(tabla);
    if (Parametros.idSeleccion != undefined) {
        var celda = AgregarHeader(fila, "", "grdCheckBox", false, false);
    }
    len = Parametros.Campos.length;
    for (i = 0; i < len; i++) {
        var Campo = Parametros.Campos[i];
        if (Campo == undefined) continue;
        if (!((Campo.Clase == "grdEntero" || Campo.Clase == "grdDecimal" || Campo.Clase == "grdTimeStamp") && Campo.Ordenacion) && Campo.Resumen == undefined) {
            AgregarHeader(fila, "", Campo.Clase, false, false);
            continue;
        }
        if (Campo.Resumen == "NINGUNO") {
            AgregarHeader(fila, "", Campo.Clase, false, false);
            continue;
        }
        if (Campo.Resumen == undefined || Campo.Resumen == "TOTAL") {
            Campo.Resumen = "TOTAL";
            var Total = Resultados.sum(function (n) { return n[Campo.Campo]; });
        } else if (Campo.Resumen == "CUENTA") {
            var Total = Resultados.length;
        }
        var CampoH = "";
        if (Campo.Clase == "grdDecimal") {
            if (Campo.Resumen == "CUENTA") {
                CampoH = Total.toString();
            } else {
                CampoH = Total.ToString();
            }
        } else if (Campo.Clase == "grdTimeStamp") {
            CampoH = Convert.ToTimeStamp(Total);
        } else {
            CampoH = Total.toString();
        }
        AgregarHeader(fila, (Campo.Pre || "") + CampoH + (Campo.Post || ""), Campo.Clase, false, false);
        //        AgregarCelda(fila, (Campo.Pre || "") + CampoH + (Campo.Post || ""), Campo.Clase);
    }
    if (Parametros.FuncEliminar != undefined) {
        AgregarHeader(fila, "", "", false, false);
        //        AgregarCelda(fila, "", "");
    }

    //end Totalización
    RedefinirAlternativo(tabla);
    Contenedor.scrollTop(Scroll);
    //Contenedor[0].scrollTop = 0;

    if (Parametros.DivExportar != undefined) {
        if ($('#' + Parametros.DivExportar).html() == "") {
            if (Parametros.NombreExportar != undefined) {
                nombre = Parametros.NombreExportar+".xls";
            } else {
                nombre = "defaul.xls";
            }
            var boton = $("#" + Parametros.DivExportar)[0];
            $(this).val("");
            var btn = document.createElement("input");
            $(btn).text("Exportar");
            $(btn).attr({
                type: 'button',
                id: 'idExpor1',
                name: 'Exportar',
                value: 'Exportar'
            })
            var tabla = Parametros.Contenedor;
            $(btn).click(function Exportar(t) {
                ExportarXLS(tabla, nombre);
            });
            boton.appendChild(btn);
        }
    }
}
function Arreglar(Texto, Registro) {
    var re = /«([\w\d]+)»/g;
    var result = Texto.replace(re, function (a1, a2, a3, a4) { return (Registro[a2] == null ? "null" : Registro[a2]).toString().replace(/["]/g, '\\"'); });
    return result;
}
function ObtenerSeleccionados(Panel) {
    var result = Array();
    var tabla = $("#" + Panel)[0].getElementsByTagName("table")[0];
    var id = tabla.getAttribute("idSeleccion");
    var campos = obtenerCampos(tabla);
    var colId = $.inArray(id, campos);
    elem = tabla.getElementsByTagName("tr");
    for (var i = 1; i < elem.length; i++) {
        if (elem[i] == undefined) continue;
        try {
            var checkBox = elem[i].getElementsByTagName("td")[colId].getElementsByTagName("input")[0];
            if (checkBox.checked) {
                var obj = {};
                obj[id] = $(checkBox).val();
                if (arguments.length == 1) arguments = campos;
                for (var j = 1; j < arguments.length; j++) {
                    var colAr = $.inArray(arguments[j], campos);
                    var celda = elem[i].getElementsByTagName("td")[colAr];
                    var input;
                    input = celda.getElementsByTagName("input")[0];
                    if (input == undefined) input = celda.getElementsByTagName("select")[0];
                    if (input == undefined) {
                        obj[arguments[j]] = $(celda).text();
                    }
                    else {
                        obj[arguments[j]] = $(input).val();
                    }
                }
                result.push(obj);
            }
        }
        catch (ex) { }
    }
    return result;
}
function ObtenerTodos(Panel) {
    var result = Array();
    var tabla = $("#" + Panel)[0].getElementsByTagName("table")[0];
    var id = tabla.getAttribute("idSeleccion");
    var campos = obtenerCampos(tabla);
    var colId = $.inArray(id, campos);
    elem = tabla.getElementsByTagName("tr");
    for (var i = 1; i < elem.length; i++) {
        if (elem[i] == undefined) continue;
        try {
            var checkBox = elem[i].getElementsByTagName("td")[colId].getElementsByTagName("input")[0];
            var obj = {};
            obj[id] = $(checkBox).val();
            if (arguments.length == 1) arguments = campos;
            for (var j = 1; j < arguments.length; j++) {
                var colAr = $.inArray(arguments[j], campos);
                var celda = elem[i].getElementsByTagName("td")[colAr];
                var input;
                input = celda.getElementsByTagName("input")[0];
                if (input == undefined) input = celda.getElementsByTagName("select")[0];
                if (input == undefined) {
                    obj[arguments[j]] = $(celda).text();
                }
                else {
                    obj[arguments[j]] = $(input).val();
                }
            }
            result.push(obj);
        } catch (ex) { }
    }
    return result;
}
function XMLParse(Texto) {
    if (window.DOMParser) {
        parser = new DOMParser();
        xmlDoc = parser.parseFromString(Texto, "text/xml");
    }
    else // Internet Explorer
    {
        xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = false;
        xmlDoc.loadXML(Texto);
    }
    return xmlDoc;
    //}     var xmlDoc = undefined;
    //    try
    //    {
    //        if (document.all) //IE
    //        {
    //            xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
    //        }
    //        else //firefox
    //        {
    //            xmlDoc = document.implementation.createDocument("", "", null);
    //        }
    //        xmlDoc.async = false;
    //        xmlDoc.load(Texto);
    //    }
    //    catch (e)
    //    {
    //        try
    //        { //otros safari, chrome
    //            var xmlhttp = new window.XMLHttpRequest();
    //            xmlhttp.open("GET", Texto, false);
    //            xmlhttp.send(null);
    //            xmlDoc = xmlhttp.responseXML.documentElement;
    //            return xmlDoc;
    //        }
    //        catch (e)
    //        {
    //            return undefined;
    //        }

    //    }
    //    return xmlDoc;
}
function TablaDataSet(Resultado, Panel) {
    var xmlDoc;
    if (window.DOMParser) {
        parser = new DOMParser();
        xmlDoc = parser.parseFromString(Resultado, "text/xml");
    }
    else // Internet Explorer
    {
        xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = false;
        xmlDoc.loadXML(Resultado);
    }
    var Campos = []
    var Secuencia = xmlDoc.getElementsByTagName("xs:sequence");
    if (Secuencia.length == 0) Secuencia = xmlDoc.getElementsByTagName("sequence");
    if (Secuencia.length == 0) {
        Mensaje({ mensaje: "No se pudieron obtener datos..." });
        return;
    }
    Secuencia = Secuencia[0];
    var CamposCrudos = Secuencia.getElementsByTagName("xs:element");
    if (CamposCrudos.length == 0) CamposCrudos = Secuencia.getElementsByTagName("element");
    if (CamposCrudos.length == 0) {
        Mensaje({ mensaje: "No se pudieron obtener datos..." });
        return;
    }
    var j = 0;
    for (var i = 0; i < CamposCrudos.length; i++) {
        var Nombre = CamposCrudos[i].getAttribute("name");
        if (Nombre.substr(0, 1) == '_') {
            Campos[j - 1].OnClick = 'Emergente({url:"«' + Nombre + '»"})';
            continue;
        }
        Campo = {};
        Campo.Campo = Nombre;
        Campo.Titulo = Campo.Campo.replace(/_x\w{4}_/g, function (a) { return String.fromCharCode(HexToDec(a.substr(2, 4))); });
        Campo.Titulo = Campo.Titulo.replace(/_/g, " ");
        switch (CamposCrudos[i].getAttribute("type")) {
            case "xs:string":
                Campo.Clase = "grdTexto";
                break;
            case "xs:int":
                Campo.Clase = "grdEntero";
                break;
            case "xs:decimal":
                Campo.Clase = "grdDecimal";
                break;
            case "xs:dateTime":
                Campo.Clase = "grdFechaHora";
                break;
            default:
                Campo.Clase = "grdTexto";
        }
        Campo.Ordenacion = true;
        Campo.Filtro = true;
        Campos[j] = Campo;
        j++;
    }

    var Filas = xmlDoc.getElementsByTagName("Table");
    var Result = [];

    for (var i = 0; i < Filas.length; i++) {
        var Elemento = Filas[i];
        var Objeto = {};
        for (var j = 0; j < Elemento.childNodes.length; j++) {
            var Campo = Elemento.childNodes[j];
            var Contenido = Campo.textContent || Campo.text;
            var Clase = Campos.where(function (x) { return x.Campo == Campo.tagName; }).select("Clase")[0];
            switch (Clase) {
                case "grdDecimal": Contenido = Convert.ToDecimal(Contenido.replace(/[.]/g, Regional.SeparadorDecimal)); break;
                case "grdFechaHora": Contenido = Convert.ToDateTime(Contenido); break;
                case "grdEntero": Contenido = Convert.ToInt32(Contenido); break;
                default:
            }
            Objeto[Campo.tagName] = Contenido;
        }
        Result[i] = Objeto;
    }
    Tabla({
        Traduccion: false,
        Contenedor: Panel,
        Resultado: Result,
        Campos: Campos
    });
}
function ExportarXLS(Contenedor, nombre) {
    csvFile = "<!DOCTYPE html>" +
    "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />" + //la codificacion para que se vean los acentos
    "<html><head>" +
    "<style>" +
    ".enc {background-color: #000040;color:White;}" +
    ".norm{ background-color: #FFFFFF;}" +
    ".alt{background-color: #CCCCFF;}" +
    ".celda{border: 1px solid #000040;}" +
    ".monto{text-align:rigth;}" +
    "body{font-family: Verdana,Arial; font-size:small !important}" +
    "</style>" +
    "</head><body>";

    //clona y formatear el contenedor para descargarlo
    tmp = $("#" + Contenedor).clone();
    tmp.find('img').parents('td').remove();
    tmp.find('.grdCheckBox').remove();
    tmp.find('input').remove();
    tmp.find('.grdImagen').remove();

    csvFile += tmp.html();
    csvFile += "</body></html>";

    var blob = new Blob([csvFile], { type: 'application/xls;charset=utf-8;' });
    //var blob = new Blob([csvFile], { type: 'application/xls;charset=utf-8;' });
    if (navigator.msSaveOrOpenBlob) { // IE 10+
        //navigator.msSaveBlob(blob, nombre);
        navigator.msSaveOrOpenBlob(blob, nombre)
    } else {
        var link = document.createElement("a");
        if (link.download !== undefined) { // feature detection
            // Browsers that support HTML5 download attribute
            var url = URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", nombre);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }
}