Number.prototype.ToString = function () {
    var valor = this.toFixed(2).replace(/\./g, Regional.SeparadorDecimal);
    var negativo = "";
    if (valor.substr(0, 1) == "-") {
        negativo = "-";
        valor = valor.substr(1);
    }
    pos = valor.indexOf(Regional.SeparadorDecimal);
    while ((pos -= 3) > 0) {
        valor = valor.substr(0, pos) + Regional.SeparadorMiles + valor.substr(pos);
    }
    return negativo + valor;
}
Date.prototype.ToString = function (a) {
    var fecha = this;
    if (a == "JSON") {
        return "\/Date(" + fecha.getTime() + "-0430)\/";
    }

    var Result = a || Regional.FormatoFechaHora;
    var dia = fecha.getDate();
    var mes = fecha.getMonth() + 1;
    var ano = fecha.getFullYear();
    var hora = fecha.getHours();
    var minuto = fecha.getMinutes();
    var segundo = fecha.getSeconds();
    var ap = hora < 12 ? "AM" : "PM";
    Result = Result.replace(/dd/g, Dos(dia));
    Result = Result.replace(/d/g, dia);
    Result = Result.replace(/MM/g, Dos(mes));
    Result = Result.replace(/M/g, mes);
    Result = Result.replace(/yyyy/g, ano);
    Result = Result.replace(/yy/g, ano.toString().substr(2));

    Result = Result.replace(/HH/g, Dos(hora));
    Result = Result.replace(/H/g, hora);
    var hora = hora > 12 ? hora - 12 : hora;

    Result = Result.replace(/hh/g, Dos(hora));
    Result = Result.replace(/h/g, hora);
    Result = Result.replace(/mm/g, Dos(minuto));
    Result = Result.replace(/m/g, minuto);
    Result = Result.replace(/ss/g, Dos(segundo));
    Result = Result.replace(/s/g, segundo);
    Result = Result.replace(/tt/g, ap);
    Result = Result.replace(/t/g, ap.substr(0, 1));
    return Result;
}

Convert = {};
Convert.ToDecimal = function (valor) {
    if (valor == undefined) return undefined;
    if (typeof (valor) == "number") return valor;
    if (valor == "") return 0;
    return parseFloat(valor.replace(RegExp("[" + Regional.SeparadorMiles + "]", "g"), "").replace(RegExp("[" + Regional.SeparadorDecimal + "]", "g"), "."));
}
Convert.ToInt32 = function (valor) {
    if (valor == undefined) return undefined;
    if (typeof (valor) == "number") return Math.round(valor);
    return parseInt(valor.replace(RegExp("[" + Regional.SeparadorMiles + "]"), "g", "").replace(RegExp("[" + Regional.SeparadorDecimal + "]", "g"), "."));
}
Convert.ToDateTime = function (Texto) {
    if (Texto == undefined) return undefined;
    if (typeof (Texto) == "Date") return Texto;
    if (Texto.substr(0, 1) == "/")//JSON
    {
        return new Date(eval(Texto.substr(6, 13)));
    }
    Texto = Texto.replace(/[-]/g, "/");
    Texto = Texto.replace(/[T]/g, " ");
    var Fecha = Texto.substr(0, 10).toUpperCase();
    var Hora = Texto.substr(11).toUpperCase();

    var ano;
    var mes;
    var dia;
    var hora = 0;
    var minuto = 0;
    var segundo = 0;
    var AP = "AM";
    if (Fecha.substr(2, 1) == "/")// dd/mm/yyyy o mm/dd/yyyy
    {
        //012345678901234567890
        //12/11/1983 02:50PM
        ano = Fecha.substr(6, 4);
        mes = Fecha.substr(3, 2);
        dia = Fecha.substr(0, 2);
        if (mes > 12) {
            var temp = dia;
            dia = mes;
            mes = temp;
        }
    } else if (Fecha.substr(4, 1) == "/")// yyyy/mm/dd
    {
        ano = Fecha.substr(0, 4);
        mes = Fecha.substr(5, 2);
        dia = Fecha.substr(8, 2);
    }
    else //yyyymmdd
    {
        ano = Fecha.substr(0, 4);
        mes = Fecha.substr(4, 2);
        dia = Fecha.substr(6, 2);
        Hora = Fecha.substr(8) + Hora;
    }

    if (Hora != "") {
        if (Hora.indexOf("/") != -1) {
            Hora = Hora.substr(0, Hora.indexOf("/"));
        }
        Hora = Hora.replace(/[:]/g, ""); //quitar dos puntos
        Hora = Hora.replace(/[\s]/g, ""); //quitar espacios
        AP = (Hora.substr(Hora.length - 2));
        var AP1 = AP.substr(1, 1);
        if (AP == "AM" || AP == "PM") {
            Hora = Hora.substr(0, Hora.length - 2);
        } else if (AP1 == "A" || AP1 == "P") {
            AP = AP1 + "M";
            Hora = Hora.substr(0, Hora.length - 1);
        } else {
            AP = "AM";
        }
        hora = Hora.substr(0, 2) * 1;
        minuto = Hora.substr(2, 2) * 1;
        segundo = Hora.substr(4, 2) * 1;
        if (AP == "PM" && hora < 12) hora += 12;
    }
    if (ano < 50) ano += 2000;
    if (ano < 100) ano += 1900;
    if (ano < 1900 || ano > 2100) {
        throw ("El Año está fuera de Rango");
    }
    var bis = (ano % 4 == 0 && ano % 100 != 0) || ano % 400 == 0;
    if (mes < 1 || mes > 12) {
        throw ("El Mes está fuera de Rango");
    }
    if (dia < 1 || dia > 31) {
        throw ("El Día está fuera de Rango");
    }
    switch (mes) {
        case 4:
        case 6:
        case 9:
        case 11:
            if (dia > 30) {
                throw ("El Día está fuera de Rango");
            }
            break;
        case 2:
            if (dia > (bis ? 29 : 28)) {
                throw ("El Día está fuera de Rango");
            }
            break;
    }

    return new Date(ano, mes - 1, dia, hora, minuto, segundo);
}
Convert.ToDate = function (Texto) {
    return Convert.ToDateTime(Texto);
}
function ObtenerNumeros(Texto) {
    var Result = "";
    for (var i = 0, l = Texto.length; i < l; i++) {
        if ("0123456789".indexOf(Texto.substr(i, 1)) != -1) {
            Result += Texto.substr(i, 1);
        }
    }
    return Result;
}

Convert.ToTimeStamp = function (Tiempo) {
    Segundos = (Tiempo % 60);
    Minutos = ((Tiempo - Segundos) / 60) % 60;
    Horas = ((Tiempo - Segundos - Minutos * 60) / 3600);

    Segundos = ("0" + Segundos).substr(("" + Segundos).length - 1, 2);
    Minutos = ("0" + Minutos).substr(("" + Minutos).length - 1, 2);
    Horas = ("0" + Horas).substr(("" + Horas).length - 1, 2);
    Resultado = Segundos ;
    if (Minutos != "00" || Horas != "00") Resultado = Minutos + ":" + Resultado;
    if (Horas != "00") Resultado = Horas + ":" + Resultado;
    return Resultado;
}
Convert.ToTimeSpan = function (Tiempo) {
    Segundos = (Tiempo % 60);
    Minutos = ((Tiempo - Segundos) / 60) % 60;
    Horas = ((Tiempo - Segundos - Minutos * 60) / 3600);
    Resultado = (Horas == 0 ? "" : (Horas + "h")) + (Minutos == 0 && (Horas==0||Segundos==0) ? "" : (Minutos + "m")) + (Segundos == 0 ? "" : (Segundos + "s"));
    return Resultado;
}