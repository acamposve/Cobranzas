using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml;
using System.Xml.XPath;
//Objetos de Transporte
namespace Cobranzas.OT
{
    public class otCuenta
    {
        public Int32 idCuenta;
        public Int32 idPersona;
        public String Documento;
        public String Persona;
        public String CodigoCliente;
        public String CodigoPersona;
        public String Cliente;
        public String Producto;
        public DateTime? Fecha;
        public DateTime? FechaEntrega;
        public Decimal Total;
        public Decimal TotalDolar;
        public Decimal TotalLocal;
        public Decimal MontoBase;
        public Decimal MontoBaseDolar;
        public Decimal MontoBaseLocal;
        public Decimal MontoIva;
        public Decimal MontoIvaDolar;
        public Decimal MontoIvaLocal;
        public Decimal Deuda;
        public Decimal DeudaDolar;
        public Decimal DeudaLocal;
        public String Moneda;
        public String MonedaLocal;
        public Int32 Antiguedad;
        public Decimal CambioDolar;
        public Decimal CambioLocal;
        public Int32? idOperador;
        public Boolean EnReclamo;
        public String Status;
        public String EsMeta;
        public String CampoExtra;
        public static otCuenta FromCuenta(Entidades.Cuentas c)
        {
            otCuenta Nuevo = new otCuenta();
            Nuevo.idPersona = c.idPersona;
            Nuevo.idCuenta = c.idCuenta;
            Nuevo.Documento = c.Codigo;
            Nuevo.Persona = c.Personas.Nombre;
            Nuevo.CodigoPersona = c.Personas.Codigo;
            Nuevo.Fecha = c.FechaInicio;
            Nuevo.FechaEntrega = c.FechaEntrega;
            Nuevo.Total = c.Monto;
            Nuevo.TotalDolar = c.Monto / c.CambioDolar;
            Nuevo.TotalLocal = c.Monto * c.CambioLocal;
            Nuevo.MontoBase = c.MontoBase.Value;
            Nuevo.MontoBaseDolar = c.MontoBase.Value / c.CambioDolar;
            Nuevo.MontoBaseLocal = c.MontoBase.Value * c.CambioLocal;
            Nuevo.MontoIva = c.MontoIva.Value;
            Nuevo.MontoIvaDolar = c.MontoIva.Value / c.CambioDolar;
            Nuevo.MontoIvaLocal = c.MontoIva.Value * c.CambioLocal;
            Nuevo.Cliente = c.Clientes.Nombre;
            Nuevo.CodigoCliente = c.Clientes.Codigo;
            Nuevo.Producto = c.Productos.Nombre;
            Nuevo.Moneda = c.idMoneda;
            Nuevo.Antiguedad = (DateTime.Now - (c.FechaInicio ?? DateTime.Now)).Days;
            Nuevo.CambioDolar = c.CambioDolar;
            Nuevo.MonedaLocal = c.Personas.Paises.idMoneda;
            Nuevo.CambioLocal = c.CambioLocal;
            Nuevo.Deuda = c.MontoRestante;// Monto + ((Decimal?)c.Movimientos.Sum(x => x.Monto) ?? 0);
            Nuevo.DeudaDolar = Nuevo.Deuda / c.CambioDolar;
            Nuevo.DeudaLocal = Nuevo.Deuda * c.CambioLocal;
            Nuevo.EnReclamo = c.EnReclamo;//c.Cuentas_Reclamos.Any(x => x.StatusReclamo.Abierto);
            Nuevo.Status = c.EnReclamo ? "En Reclamo" : "En Gestión";// c.Flujos_Pasos == null ? "Ninguno" : c.Flujos_Pasos.Pasos.Nombre;
            Nuevo.EsMeta = "";// c.Metas_Operadores_Cuentas.Any(x => x.Finalizado == null && x.Activa).ToString();
            Nuevo.CampoExtra = (from d in c.Datos.Elements("Dato") where (string)d.Attribute("Clave") == "Bl" select d.Value).FirstOrDefault();
            //c.Datos.XPathEvaluate("/Datos/Dato[@Clave='Bl']").ToString();
            return Nuevo;
        }
        public static otCuenta FromvwCuenta(Entidades.vwCuentas c)
        {
            otCuenta Nuevo = new otCuenta();
            Nuevo.idPersona = c.idPersona;
            Nuevo.idCuenta = c.idCuenta;
            Nuevo.Documento = c.Documento;
            Nuevo.Persona = c.Persona;
            Nuevo.CodigoPersona = c.CodigoPersona;
            Nuevo.Fecha = c.Fecha;
            Nuevo.FechaEntrega = c.FechaEntrega;
            Nuevo.Total = c.Total;
            Nuevo.TotalDolar = c.TotalDolar;
            Nuevo.TotalLocal = c.TotalLocal;
            Nuevo.MontoBase = c.MontoBase;
            Nuevo.MontoBaseDolar = c.MontoBaseDolar;
            Nuevo.MontoBaseLocal = c.MontoBaseLocal;
            Nuevo.MontoIva = c.MontoIva;
            Nuevo.MontoIvaDolar = c.MontoIvaDolar;
            Nuevo.MontoIvaLocal = c.MontoIvaLocal;
            Nuevo.Cliente = c.Cliente;
            Nuevo.CodigoCliente = c.CodigoCliente;
            Nuevo.Producto = c.Producto;
            Nuevo.Moneda = c.Moneda;
            Nuevo.Antiguedad = c.Antiguedad;
            Nuevo.CambioDolar = c.CambioDolar;
            Nuevo.MonedaLocal = c.MonedaLocal;
            Nuevo.CambioLocal = c.CambioLocal;
            Nuevo.Deuda = c.Deuda;// Monto + ((Decimal?)c.Movimientos.Sum(x => x.Monto) ?? 0);
            Nuevo.DeudaDolar = c.DeudaDolar;
            Nuevo.DeudaLocal = c.DeudaLocal;
            Nuevo.EnReclamo = c.EnReclamo;//c.Cuentas_Reclamos.Any(x => x.StatusReclamo.Abierto);
            Nuevo.Status = c.Status;
            Nuevo.EsMeta = c.EsMeta.ToString();
            try
            {
                Nuevo.CampoExtra = (from d in c.Datos.Elements("Dato") where (string)d.Attribute("Clave") == "Bl" select d.Value).FirstOrDefault();
            }
            catch
            {
                Nuevo.CampoExtra = "";
            }
            //((IEnumerable<String>)(c.Datos.XPathEvaluate("/Datos/Dato[@Clave=\"Bl\"]"))).FirstOrDefault();
            return Nuevo;
        }

    }//Tiene creador
    public class otExclusiones
    {
        public List<otExclusionesDet> Personas;
        public List<otExclusionesDet> Cuentas;
    }
    public class otExclusionesDet
    {
        public Int32 idExclusion;
        public String Cuenta;
        public Int32 idPersona;
        public String Persona;
        public String Codigo;
        public String Cliente;
        public DateTime FechaInicio;
        public String Operador;
        public String Motivo;
        public String Aprobado;
    }
    public class otAviso
    {
        public Int32 idAviso;
        public String Aviso;
        public int? idOperadorCrea;
        public int? idOperador;
        public String OperadorCrea;
        public String Operador;
        public DateTime FechaAviso;
        public DateTime FechaCrea;
        public DateTime FechaOriginal;
        public DateTime? FechaCancelado;
        public Int32? idPersona;
        public String CodigoPersona;
        public String NombrePersona;
        public String Comentario;
        public Boolean Prioritario;
    }
    public class otPersonaContacto
    {
        public Int32 idPersonaContacto;
        public String Nombre;
        public String Cargo;
        public String Correo;
        public List<otTelefono> Telefonos;
    }
    public class otSoporte
    {
        public String Nombre;
        public String Ubicacion;
        public Int32 idSoporte;
    }
    public class otReporte
    {
        public Int32 idTipoReporte;
        public String Nombre;
    }
    public class otPersona_lst
    {
        public Int32 idPersona;
        public String Nombre;
        public String idPais;
    }
    public class otPersona
    {
        public Int32 idPersona;
        public Int32 idTipoPersona;
        public Int32 idTipoCliente;
        public String Codigo;
        public String Nombre;
        public String Rif;
        public String DireccionFiscal;
        public String URL;
        public String Email;
        public String ISO3;
        public String Pais;
        public String Contacto;
        public String DireccionEntrega;
        public String MostrarDatos;
        public String Comentarios;
        public List<Int32> AvisosPropios;
        public XElement Datos;
        public List<otTelefono> Telefonos;
        public List<otPersonaContacto> PersonasContacto;
        //public List<otCuenta> Deudas;
        public List<otSoporte> Soportes;
        public List<otPagoPersona> Pagos;
        //public List<otHistorialGestion> Gestiones;
        public List<otReclamosPersona> Reclamos;
        public List<otReporte> Reportes;
        public List<otPersonasObservaciones> Observaciones;
    }
    public class otPersonasObservaciones
    {
        public DateTime Fecha;
        public String Descripcion;
        public String Severidad;
    }
    /*public class otDeudaPersona
    {
        public Int32 idCuenta;
        public String Documento;
        public String Cliente;
        public String Producto;
        public DateTime? Fecha;
        public Decimal Total;
        public Decimal TotalDolar;
        public Decimal TotalLocal;
        public Decimal Deuda;
        public Decimal DeudaDolar;
        public Decimal DeudaLocal;
        public String Moneda;
        public String MonedaLocal;
        public int Antiguedad;
        public Decimal CambioDolar;
        public Decimal CambioLocal;
    }*/
    /*public class otPagoPersona
    {
        public Int32 idPago;
        public String Codigo;
        public String Documento;
        public DateTime Fecha;
        public Decimal Monto;
        public String Moneda;
        public String Resultado;
    }*/
    public class otPagoPersona
    {
        public Int32 idPago;
        public String Codigo;
        public DateTime Fecha;
        public Decimal Monto;
        public String Tipo;
        public String Resultado;
        public String Moneda;
        public String Referencia;
        public Boolean? Aprobado;
        public Boolean? Confirmado;
        public String Status;
    }
    public class otHistorialGestion
    {
        public Int32 id;
        public DateTime Fecha;
        public String idPais;
        public String Operador;
        public String Descripcion;
        public String Status;
        public String Tipo;
        public String Cuentas;
        public String Img;
        public String Codigo;
        public String Persona;
        public Decimal Restante;
    }
    public class otReclamosPersona
    {
        public Int32 idReclamo;
        public DateTime Fecha;
        public String Motivo;
        public Boolean Abierto;
        public Boolean Procede;
        public Decimal MontoLocal;
        public String MonedaLocal;
        public String Codigo;
        public String Resultado;
        public String Status;
        public String Departamento;
    }
    public class otCarteraGrupo
    {
        public String Nombre;
        public Int32 idCampana;
        public List<Entidades.Operador_CuentasResult> Personas;
    }
    public class otCartera
    {
        public Int32 idPersona;
        public String Codigo;
        public String Nombre;
        public String Rif;
        public Decimal Deuda;
        public Decimal DeudaDolar;
        public String Moneda;
        public String Iso3;
        public String TipoStatus;
        public Decimal Total;
        public Decimal TotalDolar;
        public DateTime? UltimaGestion;
        public Boolean TieneAviso;
        public String StatusUltimaGestion;
        public String UltimoOperador;
    }
    public class otOperadoresSimple
    {
        public Int32 idOperador;
        public String Nombre;
        public String Jerarquia;
        public String Status;
        //public Int32? idSupervisor;
    }
    public class otReclamosDepartamentos
    {
        public Int32 idDepartamento;
        public String Departamento;
    }
    public class otSuplente
    {
        public Int32 idSuplente;
        public Int32 idOperador;
        public String Operador;
        public DateTime FechaInicio;
        public DateTime? FechaFin;
        public Boolean Indicadores;
        public Boolean Correo;
        public Boolean Cartera;
        public Boolean Gestion;
        public Boolean Supervision;
        public Boolean Reportes;
        public Boolean Distribucion;
    }
    public class otCorreos
    {
        public Int32 idCorreo;
        public String Remitente;
        public String Asunto;
        public String Mensaje;
        public DateTime Fecha;
        public String Tipo;
        public Boolean Saliente;
        public Boolean Leido;
        public String Destinatarios;
        public String DestinatariosCopia;
        public String DestinatariosCopiaOculta;
        public Int32? idPersona;
        public Int32? Original;
        public Boolean Analisis;
        public Boolean Agrupado;
        public String Adjuntos;
        public List<Int32> Cuentas;
        public Int32 idOperador;
    }
    public class otCorreosLst
    {
        public Int32 idCorreo;
        public Boolean Leido;
        public String Remitente;
        public String Asunto;
        public DateTime Fecha;
    }
    public class otStatus
    {
        public Int32 idStatus;
        public String Tipo;
        public Int32 Nivel;
        public String idPais;
        public Boolean Activo;
        public String Nombre;
        public Int32 idTipoCliente;
    }
    public class otGestion
    {
        public Int32 idOperador;
        public Int32 idPersona;
        public Int32 idStatus;
        public string Descripcion;
        public List<Int32> Cuentas;
    }
    public class otRecursos
    {
        public String Clave;
        public String Valor;
    }
    public class otOperadores
    {
        public Int32 idOperador;
        public Int32? idSupervisor;
        public Int32? idGrupo;
        public String Nombre;
        public String Grupo;
        public String Correo;
        public DateTime FechaIngreso;
        public DateTime? FechaFin;
        public String Tipo;
        public String Cargo;
        public String Supervisor;

        public Boolean Activo;
        public String Codigo;
        public String Login;
        public DateTime? FechaEgreso;
        public String FirmaCorreo;
        public String Telefonos;
        public String Extension;
        public String Pais;
        public String Zona;

        public static otOperadores FromOperador(Entidades.Operadores Op)
        {
            return new otOperadores
            {
                Activo = Op.Activo,
                Codigo = Op.Codigo,
                Login = Op.Login,
                FechaEgreso = Op.FechaFin,
                FirmaCorreo = Op.FirmaCorreo,
                Telefonos = Op.Telefonos,
                Extension = Op.Extension,
                Pais = Op.Pais,
                Zona = Op.Zona,

                idOperador = Op.idOperador,
                idSupervisor = Op.idSupervisor,
                idGrupo = Op.idGrupo,
                Nombre = Op.Nombre,
                Grupo = Op.Grupos.Nombre,
                Correo = Op.Correo,
                FechaIngreso = Op.FechaIngreso,
                FechaFin = Op.FechaFin,
                Tipo = Op.Tipo,
                Cargo = Op.Cargo,
                Supervisor = Op.Operadores1.Nombre
            };
        }
    }
    public class otDistribucionCampana
    {
        public Int32 idDistribucion;
        public String Nombre;
        public Int32 Orden;
        public String Regla;
        public String Campana;
        public String Flujo;
        public String Paso;
        public Int32 idRegla;
        public Int32 idCampana;
        public Int32 idFlujo;
        public Int32 idPaso;
        public Boolean Excluir;
    }
    public class otDistribucionOperador
    {
        public Int32 idDistribucion;
        public String Nombre;
        public Int32 Orden;
        public String Regla;
        public String Operador;
        public String Flujo;
        public String Paso;
        public Int32 idRegla;
        public Int32 idOperador;
        public Int32 idFlujo;
        public Int32 idPaso;
    }
    public class otCampanaslst
    {
        public Int32 idCampana;
        public String Nombre;
        public DateTime FechaInicio;
        public DateTime? FechaEstimadaFin;
        public DateTime? FechaFin;
        public Int32 Peso;
        public Boolean Activa;
    }
    public class otMetas
    {
        public Int32 idMeta;
        public String Nombre;
        public DateTime FechaInicio;
        public DateTime? FechaFin;
        public String Regla;
    }
    public class otMetasOperador
    {
        public Int32 idOperador;
        public String Operador;
        public Int32 idMeta;
        public Int32 Facturas;
        public Int32 FacturasCobradas;
        public DateTime? Fecha;
        public String Nombre;
        public Decimal Meta;
        public Decimal Real;
        public Decimal Porc;
        public Decimal? MetaFija;
        public Decimal? PorcMetaFija;
    }
    public class otFecha
    {
        public String Fecha;
    }
    public class otMeta
    {
        public Int32 idMeta;
        public String Nombre;
        public DateTime FechaInicio;
        public DateTime? FechaFin;
        public Int32? idRegla;
        public Int32 Frecuencia;
        public List<otCombo> Operadores;
        public List<otFecha> Metas;
        public Boolean AplicaExclusiones;
    }
    public class otMetaDetalle
    {
        public List<otMetaDetalleDet> Cuentas;
        public List<otMetaDetalleDet> Exclusiones;
        public List<otMetaDetalleDet> Inclusiones;
        public decimal Ajuste;
    }
    public class otMetaDetalleDet
    {
        public String Persona;
        public String Codigo;
        public Int32 idPersona;
        public String Documento;
        public DateTime Fecha;
        public Int32 Antiguedad;
        public String Cliente;
        public String Producto;
        public Decimal Meta;
    }
    public class otCuentasPago
    {
        public Int32 idCuenta;
        public String Documento;
        public String Cliente;
        public String Producto;
        public String Fecha;
        public Decimal Total;
        public Decimal TotalDolar;
        public Decimal TotalLocal;
        public Decimal Deuda;
        public Decimal DeudaDolar;
        public Decimal DeudaLocal;
        public String Moneda;
        public String MonedaLocal;
        public int Antiguedad;
        public Decimal CambioDolar;
        public Decimal CambioLocal;
    }
    public class otMoneda
    {
        public String idMoneda;
        public String Nombre;
    }
    public class otPago
    {
        public Int32 idPago;
        public String FechaPago;
        public String Descripcion;
        public Int32 TipoPago;
        public Int32 idBancoPropio;
        public String Referencia;
        public Decimal MontoCheque;
        public Decimal MontoEfectivo;
        public String idMoneda;
        public Int32 idPersona;
        public Int32? idBancoOrigen;
        public Int32 idOperador;
        public List<otPagos_Cuentas> Pagos_Cuentas;
        public List<otCheques> Cheques;
        public List<String> Adjuntos;
    }
    //public class otAdjuntos
    //{
    //    public String Ruta;
    //}
    public class otCheques
    {
        public String Nro;
        public Int32 Banco;
        public Decimal Monto;
    }
    public class otPagosOperador
    {
        public Int32 idPago;
        public String Pago;
        public Int32? idCuenta;
        public String Cuenta;
        public Int32 idPersona;
        public String Persona;
        public String Codigo;
        public Decimal Monto;
        public String Cliente;
        public Decimal MontoDolar;
        public String Moneda;
        public Boolean Esp;
    }
    public class otPagos_Cuentas
    {
        public Int32 idCuenta;
        public Decimal MontoCuenta;
        public Decimal Retencion1;
        public Decimal Retencion2;
    }
    public class otBancos
    {
        public Int32 idBanco;
        public String Nombre;
        public String Descripcion;
    }
    public class otBancosPropios
    {
        public Int32 idBancoPropio;
        public Int32 idBanco;
        public String Nombre;
        public String Descripcion;
        public String ReferenciasRegExp;
        public String ReferenciasInfo;
    }
    /*public class otCuentasReclamo
    {
        public Int32 idCuenta;
        public String Documento;
        public String Cliente;
        public String Producto;
        public String Fecha;
        public Decimal Total;
        public Decimal TotalDolar;
        public Decimal TotalLocal;
        public Decimal Deuda;
        public Decimal DeudaDolar;
        public Decimal DeudaLocal;
        public String Moneda;
        public String MonedaLocal;
        public int Antiguedad;
        public Decimal CambioDolar;
        public Decimal CambioLocal;
        public String Status;
    }*/
    public class otReclamosMotivos
    {
        public Int32 idReclamoMotivo;
        public String Descripcion;
    }
    public class otReclamoMostrar
    {
        public Int32 idReclamo;
        public DateTime Fecha;
        public String Descripcion;
        public String Codigo;
        public String Motivo;
        public String Status;
        public String Departamento;
        public List<otCuenta> CuentasReclamo;
        public List<otSoporte> Soportes;
        public Int32 idReclamoMotivo;
        public Int32? idReclamoStatus;
        public Int32? idDepartamento;
    }
    public class otReclamo
    {
        public String Descripcion;
        public Int32 idReclamoMotivo;
        public Int32 idDepartamento;
        public Int32 idPersona;
        public Int32 idOperador;
        public List<Int32> Cuentas;
    }
    public class otFlujos
    {
        public Int32 idFlujo;
        public String Nombre;
    }
    public class otReglas
    {
        public Int32 idRegla;
        public String Nombre;
    }
    public class otFlujoAvance
    {
        public String PasoInicio;
        public Int32? idPasoInicio;
        public String PasoFinal;
        public Int32 idPasoFinal;
        public String Regla;
        public Int32 idRegla;
    }
    public class otPasos
    {
        public Int32 idPaso;
        public String NombrePaso;
        public String Posicion;
    }
    public class otFlujo
    {
        public Int32 idFlujo;
        public String Nombre;
        public Int32 idTipoCliente;
        public Int32? idReglaSalida;
        public List<otFlujoAvance> FlujoAvance;
        public List<otPasos> Pasos;
    }
    public class otReglaDet
    {
        public Int32 idCampo;
        public Int32 Numero;
        public String Campo;
        public String Operador;
        public String Valor;
    }
    public class otRegla
    {
        public Int32 idRegla;
        public String Nombre;
        public Char TipoRegla;
        public String Criterios;
        public List<otReglaDet> ReglasDet;
    }
    public class otTipoCliente
    {
        public Int32 idTipoCliente;
        public String Nombre;
    }
    public class otPaso
    {
        public Int32 idPaso;
        public String Nombre;
    }
    public class otCampanasCuentas
    {
        public Int32 idCuenta;
        public DateTime FechaInicio;
        public DateTime? FechaFin;
        public Int32? idOperador;
        public otCuenta Cuenta;
    }
    public class otCampanasOperadores
    {
        public Int32 idOperador;
        public String Operador;
        public Int32? CuentasCampana;
        public Int32? CuentasTotales;
        public DateTime FechaInicio;
        public DateTime? FechaFin;
    }
    public class otCampanas
    {
        public Int32 idCampana;
        public Int32 idReglaCreacion;
        public Int32 idModoDistOperadores;
        public Int32 idFlujo;
        public Int32 idPaso;
        public String Nombre;
        public DateTime FechaInicio;
        public DateTime? FechaEstimadaFin;
        public DateTime? FechaFin;
        public List<otCampanasCuentas> Campanas_Cuentas;
        public List<otCampanasOperadores> Campanas_Operadores;
        public Int32 Peso;
        public Boolean Activa;
        public Int32 idOperadorDueno;
        public Int32 TipoCampana;
        public Decimal MontoMeta;
        public Int32 idReglaSalida;
    }
    public class otCombo
    {
        public Int32 id;
        public String Nombre;
    }
    public class otIndicadores
    {
        public Decimal MetaMonto;
        public Decimal RealMonto;
        public Int32 MetaGestiones;
        public Int32 RealGestiones;
        public Int32 MetaTiempo;
        public Int32 RealTiempo;
        public Int32 MetaTiempoInactivo;
        public Int32 RealTiempoInactivo;
        public Decimal MetaTMO;
        public Decimal RealTMO;
        public Int32 MetaGestionesPersonas;
        public Int32 RealGestionesPersonas;
        //Aisladas
        public Int32 TiempoEnLlamada;
        public Int32 LlamadasRealizadas;
        public Int32 LlamadasContestadas;
        public Int32 LlamadasNoContestadas;
        public Int32 CuentasAsignadas;
        public Int32 PersonasAsignadas;
    }
    public class otCorreosFiltros
    {
        public Int32 idCorreoFiltro;
        public Int32? idOperador;
        public String De;
        public Int32? idPersona;
        public String Persona;
        public static otCorreosFiltros FromCorreosFiltros(Entidades.CorreosFiltros Base)
        {
            otCorreosFiltros Result = new otCorreosFiltros();
            Result.idCorreoFiltro = Base.idCorreoFiltro;
            Result.idOperador = Base.idOperador;
            Result.De = Base.De;
            Result.idPersona = Base.idPersona;
            Result.Persona = Base.Personas.Nombre;
            return Result;
        }
    }
    public class otCompromisos_Cuentas
    {
        public Int32 idCuenta;
        public DateTime Fecha;
        public Decimal Monto;
    }
    public class otCompromisos
    {
        public String Descripcion;
        public Int32 idOperador;
        public List<otCompromisos_Cuentas> Compromisos_Cuentas;
    }
    public class otCompromisos_rpt
    {
        public Int32 idPersona;
        public String Codigo;
        public String Persona;
        public DateTime Fecha;
        public Decimal MontoLocal;
        public Decimal MontoDolar;
        public Decimal RestanteLocal;
        public Decimal RestanteDolar;
        public String Operador;
    }
    public class otCompromisos_lst
    {
        public String Documento;
        public DateTime Fecha;
        public Decimal Total;
        public String Creador;
    }
    public class otNovedades
    {
        public Boolean Avisos;
        public Boolean? Correos;
        public Boolean Cartera;
        public Boolean Pagos;
        public Boolean Reclamos;
        public Boolean Reiniciar;
        public Boolean Colas;
        public DateTime Hora;
    }
    public class otTelefono
    {
        public Int32 idTelefono;
        public String CodigoPais;
        public String CodigoArea;
        public String Telefono;
        public String Extension;
        public Int32? Reputacion;
        public static otTelefono FromTelefono(Entidades.Telefonos t)
        {
            return new otTelefono
            {
                CodigoArea = t.CodigoArea,
                CodigoPais = t.CodigoPais,
                Extension = t.Extension,
                idTelefono = t.idTelefono,
                Reputacion = t.Reputacion,
                Telefono = t.Telefono
            };
        }
    }
    public class otError
    {
        public String message;
        public String location;
        public Int32 line;
        public Int32 col;
        public String callstack;

    }
    public class otOperadores_Asignaciones
    {
        public Int32 idTipoCliente;
        public String idPais;
    }
    public class otPlantillas_lst
    {
        public Int32 idPlantilla;
        public String Nombre;
        public DateTime Fecha;
        public String Operador;
        public Boolean Privado;
    }
    public class otPlantillas
    {
        public Int32 idPlantilla;
        public Int32 idOperador;
        public String NombreOperador;
        public String NombrePlantilla;
        public DateTime FechaCreacion;
        public String Asunto;
        public String Mensaje;
        public String Adjunto;
        public String DestinatariosCopia;
        public String DestinatariosCopiaOculta;
        public String idPais;
        public Int32 idTipoCliente;
        public Boolean Privado;
    }
    public class otPlantillasCorreo
    {
        public String Asunto;
        public String Mensaje;
        public String Adjunto;
        public String DestinatariosCopia;
        public String DestinatariosCopiaOculta;
        public String idPais;
        public Int32 idTipoCliente;
    }
    public class otTablaContacto_lst
    {
        public String Nombre;
        public String Correo;
    }
    public class otTelefonos_lst
    {
        public Int32 idTelefono;
        public String NombrePersona;
        public String Telefono;
        public String NombreOperador;
    }
    public class otTelefono_sel
    {
        public Int32 idTelefono;
        public Int32 idOperador;
        public Int32 idOperadorConfirmado;
        public Int32 idOperadorEliminar;
        public Int32 idOperadorConfirmadoEliminar;
    }
    public class otClientes
    {
        public Int32 id;
        public String Nombre;
        public String idPais;
    }
}