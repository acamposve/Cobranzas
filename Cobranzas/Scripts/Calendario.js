/** ----------------------------------------------------------------------------
 *      Descripcion     : Funciones varias para el calendario.
 *      @author  Miguel Berroteran.
 *      Date created    : 25/04/2011            Date updated: 28/06/2011
 *      Docs By         : Miguel Berroteran.
 *      @Copyright      : (c) 2011 by Password Techonology.
 * -----------------------------------------------------------------------------
 */

//Para poder hacer trim a cualquier string.
/**
 * Realiza trim a cualquier string.
 */
String.prototype.trim = function() {
	return this.replace(/^\s+|\s+$/g,"");
}
String.prototype.ltrim = function() {
	return this.replace(/^\s+/,"");
}
String.prototype.rtrim = function() {
	return this.replace(/\s+$/,"");
}


/**
 * Si el año viene en 2 digitos, determina hasta cual es 1900
 * @var integer Y2kPivotPoint
 */
//Si el a�o viene en 2 d�gitos, determina hasta cual es 1900
var Y2kPivotPoint = 69; // 2-digit years before this point will be created in the 21st century

//clase calendario
/**
 * clase calendario crea arreglo para los dia de la sena.
 * @var Array DiaSemana.
 */
var DiaSemana = new Array('Dom','Lun','Mar','Mie','Jue','Vie','Sab');

/**
 * Crea arreglo para los meses.
 * @var Array Mes.
 */
var Meses = new Array('Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre');


/**
 * Funcion que determina el numero de dias del mes mediante un switch.
 * @param ano contiene el año seleccionado.
 * @param mes contiene el mes seleccionado.
 */
function diasdelmes(ano,mes){
    bis=(ano%4==0)&&(!(ano%100==0)||((ano%100==0)&&(ano%400==0)));
    switch(mes){
    case 1:case 3:case 5:case 7:case 8:case 10:case 12:
        return 31;
        break;
    case 4:case 6:case 9: case 11:
        return 30;
        break;
    case 2:
        return bis?29:28;
        break;
    }
}
function min(a,b){return a<b?a:b;}
function max(a,b){return a>b?a:b;}
function asignarCalendario(txtbox,permiteNulo){
    while (!(txtbox.tagName&&txtbox.tagName.toUpperCase()=="INPUT")) {
        if (!txtbox.previousSibling) return;
        txtbox = txtbox.previousSibling;
    }
    try{
        eval(txtbox.id+"_cal.desaparecer();");
    }catch(e){}
    eval(txtbox.id+"_cal = new calendariodd(document.getElementById('"+txtbox.id+"'),"+(permiteNulo?"true":"false")+")");
}


/**
 * Permile agregar elementos del candelario.
 * @package calendariodd.
 * @param txtbox.
 * @param permiteNulo.
 */
function calendariodd(txtbox,permiteNulo)
{
    this.control=txtbox;//control al que se le va a asociar el calendario
    this.Nombre=txtbox.id+"_cal";//Nombre del control para anexarlo a los otros objetos.
    this.mostrarCalendario=mostrarCalendario;
    this.mostrarMeses=mostrarMeses;
    this.mostrarAnos=mostrarAnos;
    this.mostrarDecada=mostrarDecada;
    this.calendario=null;
    this.cargarDia=cargarDia;
    this.cambiarDia=cambiarDia;
    this.decadaAnterior=decadaAnterior;
    this.decadaSiguiente=decadaSiguiente;
    this.anoAnterior=anoAnterior;
    this.anoSiguiente=anoSiguiente;
    this.mesAnterior=mesAnterior;
    this.mesSiguiente=mesSiguiente;
    this.desaparecer=desaparecer;
    this.hacerNulo=hacerNulo;
    this.mesctrl=null;
    this.anoctrl=null;
    this.calendarioctrl=null;
    this.permiteNulo=permiteNulo;
    this.divMain=null;
    this.divMeses=null;
    this.selMes=selMes;
    this.selAno=selAno;
    if (txtbox.value.length<10){
        fecha=new Date();
        this.ano=fecha.getFullYear();
        this.mes=fecha.getMonth()+1;//no me pregunten, se que no tiene l�gica, pero es as�...
        this.dia=fecha.getDate();
    }
    else
    {
        this.dia=parseInt(txtbox.value.split('/')[0],10);
        this.mes=parseInt(txtbox.value.split('/')[1],10);
        this.ano=parseInt(txtbox.value.split('/')[2],10);
    }
    this.decada=this.ano-this.ano%10;
    this.mostrarCalendario();
}


/**
 * Descripcion: ..
 * @param control.
 * @return result+"px".
 */
function calcularLeft(ctrl) {
    var result = 0;
    while (ctrl!=null && ctrl.tagName.toUpperCase() != "BODY") {
        result += ctrl.offsetLeft;
        ctrl = ctrl.offsetParent;
    }
    return result+"px";
}


/**
 * Descripcion: ..
 * @param control.
 * @return result+"px".
 */
function calcularTop(ctrl) {
    var result = 0;
    while (ctrl!=null&&ctrl.tagName.toUpperCase() != "BODY") {
        result += ctrl.offsetTop;
        ctrl = ctrl.offsetParent;
    }
    return result+"px";
}


/**
 * Muestra calendario.
 * @package mostrarCalendario.
 */
function mostrarCalendario()
{
    this.calendario=document.createElement("div");
    this.calendario.style.position="absolute";
    this.calendario.style.left = calcularLeft(this.control);
    this.calendario.style.top = calcularTop(this.control);
    this.calendario.className="cal_body";
    div=document.createElement("div");
    table=document.createElement("table");
    table.style.width="100%";
    table.style.borderCollapse="collapse";
    tr=document.createElement("tr");
    tr.className="cal_meses";
    td=document.createElement("td");//a�o Anterior
    td.style.width="10px";
    span=document.createElement("span");
    span.onclick=new Function(this.Nombre+".anoAnterior();");
    span.appendChild(document.createTextNode("<<"));
    span.style.cursor="Pointer";
    td.appendChild(span);
    tr.appendChild(td);

    td=document.createElement("td");//mes Anterior
    td.style.width="10px";
    span=document.createElement("span");
    span.onclick=new Function(this.Nombre+".mesAnterior();");
    span.appendChild(document.createTextNode("<"));
    span.style.cursor="Pointer";
    td.appendChild(span);
    tr.appendChild(td);

    td=document.createElement("td");// mes y a�o actual
    td.onclick=new Function(this.Nombre+".mostrarMeses();")
    td.style.textAlign="center";//textAlignment
    span=document.createElement("span");
    this.mesctrl=span;
    span.Name="Mes";
    td.appendChild(span);
    td.appendChild(document.createTextNode("/"))
    span=document.createElement("span");
    this.anoctrl=span;
    span.Name="Ano";
    td.appendChild(span);
    tr.appendChild(td);

    td=document.createElement("td");//mes siguiente
    td.style.width="10px";
    span=document.createElement("span");
    span.onclick=new Function(this.Nombre+".mesSiguiente();");
    span.appendChild(document.createTextNode(">"));
    span.style.cursor="Pointer";
    td.appendChild(span);
    tr.appendChild(td);

    td=document.createElement("td");//a�o siguiente
    td.style.width="10px";
    span=document.createElement("span");
    span.onclick=new Function(this.Nombre+".anoSiguiente();");
    span.appendChild(document.createTextNode(">>"));
    span.style.cursor="Pointer";
    td.appendChild(span);
    tr.appendChild(td);

    table.appendChild(tr);
    div.appendChild(table);
    table=document.createElement("table");
    table.style.borderCollapse="collapse";
    table.style.width="100%";
    table.Name="calendario";
    tr=document.createElement("tr");
    for (i=0;i<7;i++)
    {
        th=document.createElement("th");
        th.appendChild(document.createTextNode(DiaSemana[i]));
        th.className="cal_diasSemana"
        tr.appendChild(th);
    }
    table.appendChild(tr);
    for(i=0;i<6;i++)
    {
        tr=document.createElement("tr");
        for(j=0;j<7;j++)
        {
            td=document.createElement("td");
            if (j==0)
            {
                td.className="cal_diaDomingo";
            }
            else
            {
                td.className="cal_diaNormal";
            }
            span=document.createElement("span");
            span.onclick=new Function(this.Nombre+".cambiarDia(this);");
            span.style.cursor="Pointer";
            td.appendChild(span);
            tr.appendChild(td);
        }
        table.appendChild(tr);
    }
    this.calendarioctrl=table;
    div.appendChild(table);
    if (this.permiteNulo){

        span=document.createElement("span");
        span.innerText="[No seleccionar Fecha]";
        span.textContent="[No seleccionar Fecha]";
        span.onclick=new Function(this.Nombre+".hacerNulo();")
        span.style.cursor="Pointer";
        span.className="cal_fechaNula";
        div.appendChild(span);
    }
    this.divMain=div;
    this.calendario.appendChild(div);
    this.control.parentNode.appendChild(this.calendario);
    this.cargarDia(this.ano,this.mes,this.dia);
}

/**
 * Muestra los meses en el calendario.
 * @package mostrarMeses.
 */
function mostrarMeses(){
    div=document.createElement("div");
    div.style.height=this.calendario.offsetHeight+"px";
    div.style.width=this.calendario.offsetWidth+"px";
    this.divMain.style.display="none";
    table=document.createElement("table");
    table.style.height="100%";
    table.style.width="100%";
    tr=document.createElement("tr");
    td=document.createElement("td");
    td.colSpan="4";
    td.style.textAlign="center";
    span=document.createElement("span");
    span.innerText=this.ano;
    span.textContent=span.innerText;
    span.onclick=new Function(this.Nombre+".mostrarAnos();");
    span.style.cursor="Pointer";
    td.className="cal_diasSemana";
    td.appendChild(span);
    tr.appendChild(td);
    table.appendChild(tr);
    
    for(i=0;i<3;i++){
        tr=document.createElement("tr");
        for(j=0;j<4;j++){
            td=document.createElement("td");
            td.style.textAlign="center";
            span=document.createElement("span");
            span.innerText=Meses[i*4+j].substr(0,3);
            span.textContent=span.innerText;
            span.onclick=new Function(this.Nombre+".selMes("+(i*4+j).toString()+");");
            span.style.cursor="Pointer";
            td.className="cal_meses";
            td.appendChild(span);
            tr.appendChild(td);
        }
        table.appendChild(tr);
    }
    div.appendChild(table);
    this.divMeses=div;
    this.calendario.appendChild(div);
}

/**
 * Descripcion:
 * @package selMes.
 * @param mes.
 */
function selMes(mes){
    this.calendario.removeChild(this.divMeses);
    this.divMeses=null;
    this.divMain.style.display="";
    this.mes=mes+1;
    this.cargarDia(this.ano,this.mes,this.dia);
}


/**
 * Descripcion crea contenedor.
 * @package mostrarAnos contenedor para mostrar años.
 */
function mostrarAnos(){
    div=document.createElement("div");
    div.style.height=this.calendario.offsetHeight+"px";
    div.style.width=this.calendario.offsetWidth+"px";
    this.divMeses.style.display="none";
    table=document.createElement("table");
    table.style.height="100%";
    table.style.width="100%";
    tr=document.createElement("tr");
    td=document.createElement("td");
    td.colSpan="4";
    td.style.textAlign="center";
    span=document.createElement("span");
    span.innerText=this.decada.toString()+"-"+(this.decada+9).toString();
    span.textContent=span.innerText;
    //span.onclick=new Function(this.Nombre+".mostrarAnos();");
    //span.style.cursor="Pointer";
    td.className="cal_diasSemana";
    td.appendChild(span);
    tr.appendChild(td);
    table.appendChild(tr);
    
    for(i=0;i<3;i++){
        tr=document.createElement("tr");
        for(j=0;j<4;j++){
            td=document.createElement("td");
            td.style.textAlign="center";
            span=document.createElement("span");
            span.style.cursor="Pointer";
            if (i==0&&j==0){
                span.innerText="<<";
                span.onclick=new Function(this.Nombre+".decadaAnterior();");
            }else if (i==2&&j==3){
                span.innerText=">>";
                span.onclick=new Function(this.Nombre+".decadaSiguiente();");
            }else{
                span.innerText=(this.decada+i*4+j-1).toString();
                span.onclick=new Function(this.Nombre+".selAno(parseInt(this.innerText));");
            }
            span.textContent=span.innerText;
            td.className="cal_meses";
            td.appendChild(span);
            tr.appendChild(td);
        }
        table.appendChild(tr);
    }
    div.appendChild(table);
    this.divAnos=div;
    this.calendario.appendChild(div);
}


/**
 * Descripcion.
 * @package selAno se utiliza para borrar año.
 * @param ano.
 */
function selAno(ano){
    this.calendario.removeChild(this.divAnos);
    this.divAnos=null;
    this.divMeses.style.display="";
    this.ano=ano;
    this.divMeses.childNodes[0].childNodes[0].childNodes[0].childNodes[0].innerText=this.ano.toString();
    this.divMeses.childNodes[0].childNodes[0].childNodes[0].childNodes[0].textContent=this.ano.toString();
    this.cargarDia(this.ano,this.mes,this.dia);
}


/**
 * Descripcion
 * @package decadaAnterior.
 */
function decadaAnterior(){
    this.decada-=10;
    this.mostrarDecada();
}


/**
 * Descripcion
 * @package decadaSiguiente.
 */
function decadaSiguiente(){
    this.decada+=10;
    this.mostrarDecada();
}


/**
 * Descripcion
 * @package mostrarDecada.
 */
function mostrarDecada(){
    for(i=1;i<4;i++){
        for(j=0;j<4;j++){
            span=this.divAnos.childNodes[0].childNodes[i].childNodes[j].childNodes[0];
            if (i==1&&j==0){
            }else if (i==3&&j==3){
            }else{
                span.innerText=(this.decada+(i-1)*4+j-1).toString();
                span.textContent=span.innerText;
            }
        }
    }
    span=this.divAnos.childNodes[0].childNodes[0].childNodes[0].childNodes[0];
    span.innerText=this.decada.toString()+"-"+(this.decada+9).toString();
    span.textContent=span.innerText;
}


/**
 * Clase que permite colocar como nula la fecha.
 * @package hacerNulo.
 */
function hacerNulo(){
    this.control.value="";//Colocar aqu� cual fecha se quiere que sea nula...
    this.desaparecer();
}


/**
 * Clase que permite contolar el cambio de dia en la fecha.
 * @packege cambiarDia.
 * @param control.
 */
function cambiarDia(control){
    this.cargarDia(this.ano,this.mes,control.innerText);
    this.desaparecer();
}


/**
 * Permite eliminar calendario.
 * @package desaparecer elimina calendario.
 */
function desaparecer()
{
    this.calendario.parentNode.removeChild(this.calendario);
}


/**
 * Permite cargar en el calendario el año anterio.
 * @package anoAnterior carga año anterior.
 */
function anoAnterior ()
{
    ano2=this.ano-1;
    mes2=this.mes;
    dia2=min(diasdelmes(ano2,mes2),this.dia);
    this.cargarDia(ano2,mes2,dia2)
}


/**
 * Permite cargar en el calendario el año siguiente.
 * @package anoSiguiente carga año siguiente.
 */
function anoSiguiente ()
{
    ano2=this.ano+1;
    mes2=this.mes;
    dia2=min(diasdelmes(ano2,mes2),this.dia);
    this.cargarDia(ano2,mes2,dia2)
}


/**
 * Permite cargar en el calendario el mes anterior.
 * @package mesAnterior carga mes anterior.
 */
function mesAnterior ()
{
    if (this.mes==1) {
        mes2=12;
        ano2=this.ano-1;
    }else{
        mes2=this.mes-1;
        ano2=this.ano;
    }
    dia2=min(diasdelmes(ano2,mes2),this.dia);
    this.cargarDia(ano2,mes2,dia2)
}


/**
 * Permite cargar en el calendario el mes siguiente.
 * @package mesSiguiente carga mes siguiente.
 */
function mesSiguiente ()
{
    if (this.mes==12) {
        mes2=1;
        ano2=this.ano+1;
    }else{
        mes2=this.mes+1;
        ano2=this.ano;
    }
    dia2=min(diasdelmes(ano2,mes2),this.dia);
    this.cargarDia(ano2,mes2,dia2)
}

/**
 * Realiza la carga de la fecha en el calendario.
 * @param ano.
 * @param mes.
 * @param dia.
 */
function cargarDia (ano,mes,dia)
{
    var i;
    tabla=this.calendarioctrl;
    for(f=1;f<7;f++)
    {
        for(c=0;c<7;c++)
        {
            celda=tabla.childNodes[f].childNodes[c];
            if (c==0){
                celda.className="cal_diaDomingo";
            }
            else
            {
                celda.className="cal_diaNormal";
            }
            celda.childNodes[0].innerText="";
            celda.childNodes[0].textContent="";//para firefox
        }
    }
    var diasemana=(new Date(ano.toString()+"/"+mes.toString()+"/01")).getDay();
    f=0;
    c=diasemana;
    dias=diasdelmes(ano,mes);
    for (i=1;i<=dias;i++)
    {
        celda=tabla.childNodes[f+1].childNodes[c];
        celda.childNodes[0].textContent=i;
        celda.childNodes[0].innerText=i;
        if (i==dia){ celda.className="cal_diaSel"}
        c++;
        if (c==7){f++;c=0;}
    }
    this.dia=dia;
    this.mes=mes;
    this.ano=ano;
    this.control.value=doscifras(this.dia)+"/"+doscifras(this.mes)+"/"+this.ano.toString();
    this.mesctrl.innerText=Meses[mes-1];
    this.anoctrl.innerText=ano.toString();
    this.mesctrl.textContent=Meses[mes-1];
    this.anoctrl.textContent=ano.toString();
}


/**
 * Descripcion.
 * @param k.
 */
function doscifras(k){
    a="00"+k.toString();
    a=a.substring(a.length-2);
    return a;
}


/**
 * valida que en el control text se haya escrito una fecha correcta, debe corregir
 * por lo menos los caracteres de separación, colocar en onblur="validaFecha(this,true);"
 * @param text.
 * @param PermitirNulo.
 * @return bool|string
 */
function validaFecha(text,PermitirNulo){
    var str = text.value;
    str=str.trim(); //ver trim(), rtrim() y ltrim() al principio de este archivo
	
    //si el texto est� vac�o
    if (str=="" && PermitirNulo==true){
        //devuelve true porque es v�lido dejar la fecha vac�a
        text.value="";
        return true;
    }
	if (str=="" && PermitirNulo==false){
        //devuelve false porque no es v�lido dejar la fecha vac�a
		alert("La fecha no puede estar vacia");
		str=strFechaHoy();
        text.value=str;
        return false;
    }
    //Determinar el formato en que se ingres� la fecha
    var sep = "/";
    var n = str.indexOf(sep);
    if (n<0){
        sep="-";
        n = str.indexOf(sep);
    }
    if (n<0){
        sep=".";
        n = str.indexOf(sep);
    }
    if (n<0){
        sep=" ";
        n = str.indexOf(sep);
    }
    if (n<0){
        sep="";
    }

    //Determinar el d�a, mes y a�o
    var dd="";
    var mm="";
    var yy="";
    if (n<0){ //no se coloc� ning�n separador, se asume ddmmyyyy � ddmmyy
        dd=str.substr(0,2);
        mm=str.substr(2,2);
        yy=str.substr(4); //si viene una hora esto puede ser inconveniente
    }else if (sep!=""){
		if (n==2) {// separador despu�s de los 2 primeros d�gitos se asume dd-mm-yyyy
			dd=str.split(sep)[0];
			mm=str.split(sep)[1];
			yy=str.split(sep)[2];	
		}else if (n==4){ // separador despu�s de los 4 primeros d�gitos, se asume yyyy-mm-dd
			yy=str.split(sep)[0];
			mm=str.split(sep)[1];
			dd=str.split(sep)[2];	
		}
	}else{
        alert("No se reconoce el formato de la fecha ("+str+"), por favor verifique");
		str='';
		if (PermitirNulo==false) str=strFechaHoy();
        text.value=str;
        return false;
    }
    //Validaciones adicionales del a�o, puede tener una hora empatada
    if(yy.length>4){
        yy=yy.substr(0,4)
        if(str.indexOf(" ")>=0){
            yy=yy.substr(0,2)
        }
    }
    //Tratar de formar de nuevo la fecha:
    var d=0;
    var m=0;
    var y=0;

    //convertir a numero:
    try{
        d=parseInt(dd,10);
        m=parseInt(mm,10);
        y=parseInt(yy,10);
    }catch(exception){
        alert ("Error al interpretar la fecha ("+str+"), "+exception);
		str='';
		if (PermitirNulo==false) str=strFechaHoy();
        text.value=str;
        return false;
    }

    //Validaciones adicionales:
    if(y<100){ //El a�o siempre debe ser full
        if (y<Y2kPivotPoint){
            y=y+2000;
        }else{
            y=y+1900;
        }
    }
    if (m>12 && d<=12){
        var aux=m;
        m=d;
        d=aux;
    }
    //Ultima validaci�n, verificar si la fecha es correcta para javascript
    var f = new Date();
	try{
		f = new Date(y, m-1, d, 0, 0, 0, 0);
    }catch(exception){
        alert ("Error al interpretar la fecha ("+str+"), "+exception);
		str='';
		if (PermitirNulo==false) str=strFechaHoy();
        text.value=str;
        return false;
    }	
    dd=d;
    if (d<10) dd="0"+d;
    mm=m;
    if (m<10) mm="0"+m;
	
    str=dd+"/"+mm+"/"+f.getFullYear();
	
	if(str.indexOf("NaN")>=0){
        alert ("Error al interpretar la fecha ("+str+")");
		str='';
		if (PermitirNulo==false) str=strFechaHoy();
        text.value=str;
        return false;
	}
	
	//Por fin, la fecha es v�lida
	text.value=str;
    return true;
}


/**
 * funcion que genera fecha actual.
 * @return hoy contiene la fecha actual.
 */
function strFechaHoy(){
	var hoy='';
		var fh = new Date();
		var dh=fh.getDate();
		var mh=fh.getMonth()+1;
		var yh=fh.getFullYear();
		if(dh<10) hoy="0"+dh+"/";
		else hoy=dh+"/";
		if(mh<10) hoy=hoy+"0"+mh+"/";
		else hoy=hoy+dh+"/";
		hoy=hoy+yh;
		return hoy;
}