jQuery.fn.rotar = function (angulo, und)
{
    $(this).css({ '-webkit-transform': 'rotate(' + angulo + und + ')',
        '-moz-transform': 'rotate(' + angulo + und + ')',
        '-ms-transform': 'rotate(' + angulo + und + ')',
        '-o-transform': 'rotate(' + angulo + und + ')',
        'transform': 'rotate(' + angulo + und + ')'
    });
};
try
{
    CanvasRenderingContext2D.prototype.Flecha = function (x1, y1, x2, y2, texto, color)
    {
        this.strokeStyle = color;
        this.fillStyle = color;
        this.beginPath();
        this.moveTo(x1, y1);
        this.lineTo(x2, y2);
        var a = Math.atan2(y2 - y1, x2 - x1);
        var angulo = Math.PI * (6 / 7);
        var tamano = 10;
        this.lineTo(x2 + tamano * Math.cos(a + angulo), y2 + tamano * Math.sin(a + angulo));
        this.lineTo(x2 + tamano * Math.cos(a - angulo), y2 + tamano * Math.sin(a - angulo));
        this.lineTo(x2, y2);
        this.fill();
        this.stroke();
        this.beginPath();
        this.arc(x1, y1, 4, 0, Math.PI * 2);
        this.fill();
    }

    CanvasRenderingContext2D.prototype.FlechaCuadrosXY = function (x1, y1, x2, y2, texto, color)
    {
        var AltoCuadro = 30;
        var AnchoCuadro = 120;
        x1 += AnchoCuadro / 2;
        x2 += AnchoCuadro / 2;
        y1 += AltoCuadro / 2;
        y2 += AltoCuadro / 2;
        //this.fillRect(x1 - AnchoCuadro / 2, y1 - AltoCuadro / 2, AnchoCuadro, AltoCuadro);
        //this.fillRect(x2 - AnchoCuadro / 2, y2 - AltoCuadro / 2, AnchoCuadro, AltoCuadro);
        var AnguloLimite = Math.atan2(AltoCuadro, AnchoCuadro);
        var a = Math.atan2(y2 - y1, x2 - x1);
        var b = Math.abs(a);
        if (b > Math.PI / 2) b = Math.PI - b;
        var Vert = (Math.abs(b) > AnguloLimite);
        if (Vert)
        {
            Modulo = Math.abs(AltoCuadro / Math.sin(a)) / 2;
        }
        else
        {
            Modulo = Math.abs(AnchoCuadro / Math.cos(a)) / 2;
        }
        this.Flecha(x1 + Modulo * Math.cos(a), y1 + Modulo * Math.sin(a), x2 - Modulo * Math.cos(a), y2 - Modulo * Math.sin(a), texto, color);
    }
    CanvasRenderingContext2D.prototype.FlechaCuadros = function (c1, c2, texto, color)
    {
        this.FlechaCuadrosXY(c1.x, c1.y, c2.x, c2.y, texto, color);
    }
    CanvasRenderingContext2D.prototype.FlechaDivs = function (c1, c2, texto, color)
    {
        this.FlechaCuadrosXY($(c1).position().left, $(c1).position().top, $(c2).position().left, $(c2).position().top, texto, color);
    }
    CanvasRenderingContext2D.prototype.FlechaFlujoAvance = function (FlujoAvance, color)
    {
        this.FlechaDivs(DevolverPaso(FlujoAvance.idPasoInicio), DevolverPaso(FlujoAvance.idPasoFinal), FlujoAvance.Regla, color);
    }

}
catch (e)
{
    //Mensaje({mensaje:"Navegador muy Antiguo, necesita un navegador que soporte HTML5 para usar esta opción"});
}
