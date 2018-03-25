using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsterNET.Manager;
using AsterNET.Manager.Action;
using AsterNET.Manager.Response;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System.Data;
using System.Threading;
using System.Security.Cryptography;

namespace Cobranzas
{
    public class Result
    {
        public String Canal { get; set; }
        public String DestChannel { get; set; }
        public String UniqueId { get; set; }
        public String Error { get; set; }
        public String Status { get; set; }
    }
    public class StatusLlamada
    {
        public String Status { get; set; }
        public Int32 Duracion { get; set; }
        public Int32 DuracionEfectiva { get; set; }
        public String Grabacion { get; set; }
    }
    public class CentralIp
    {
        //Pruebas
        /*public static string Host = "192.168.6.110";
        public static Int32 Port = 5038;
        public static String User = "admin";
        public static String Password = "V3c0n1nt3r";*/

        //Producción
        public String Host;
        public Int32 Port;
        public String User;
        public String Password;
        public String Prefijo = "53981456935675425";
        public ManagerConnection Conn;

        public String UniqueId;
        public String Channel;
        public String DestChannel;
        public String Error;
        public String CodigoPais;
        public String CodigoArea;
        //public CentralIp()
        //{
        //    Host = "172.17.1.102";
        //    Port = 5038;
        //    User = "Admin";
        //    Password = "palosanto";
        //    Prefijo = "53981456935675425";
        //    Conn = new ManagerConnection(Host, Port, User, Password);
        //    Conn.Login(10000);
        //}
        public CentralIp(String Host, Int32 Port, String User, String Password, String Prefijo = "", String CodigoPais = "", String CodigoArea = "")
        {
            this.Host = Host;
            this.Port = Port;
            this.User = User;
            this.Password = Password;
            this.Prefijo = Prefijo;
            this.CodigoPais = CodigoPais;
            this.CodigoArea = CodigoArea;
            Conn = new ManagerConnection(Host, Port, User, Password);
            Conn.Login(10000);
        }
        ~CentralIp()
        {
            Conn.Logoff();
        }


        public static string Md5Hash(string texto)
        {
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(texto));
            return data.Aggregate(new StringBuilder(), (x, y) => x.Append(y.ToString("x2"))).ToString();
            //StringBuilder sBuilder = new StringBuilder();
            //for (int i = 0; i < data.Length; i++)
            //{
            //    sBuilder.Append(data[i].ToString("x2"));
            //}
            //return sBuilder.ToString();
        }

        public void Llamar(String Telefono, String Extension, Boolean Sincronico = false)
        {
            OriginateAction o = new OriginateAction();
            o.Channel = "SIP/" + Extension;
            o.Context = "from-internal";
            o.Exten = Prefijo + Limpiar(Telefono);
            o.CallerId = Limpiar(Telefono);
            o.Priority = "1";
            o.Timeout = 15000;
            o.Async = true;
            o.Account = "";
            Conn.Dial += new DialEventHandler(Conn_Dial);
            Conn.Status += new StatusEventHandler(Conn_Status);
            Conn.OriginateResponse += new OriginateResponseEventHandler(Conn_OriginateResponse);
            ResponseEvents response = Conn.SendEventGeneratingAction(o, 15000);
            Channel = response.Events[0].Channel;
            UniqueId = response.Events[0].UniqueId;
            Monitorear(Channel);
            DestChannel = "";

            //ManagerResponse SD = Conn.SendAction(new AgiAction(Canal, "exec(\"SENDDTMF\",\"3\")"));
            //ManagerResponse SD2 = Conn.SendAction(new AgiAction(Canal, "exec(\"SENDDTMF\",\"4\")"));
            //ManagerResponse SD3 = Conn.SendAction(new AgiAction(Canal, "exec(\"SENDDTMF\",\"2\")"));
            //ManagerResponse SD4 = Conn.SendAction(new AgiAction(Canal, "exec(\"SENDDTMF\",\"7\")"));
            //ManagerResponse SD5 = Conn.SendAction(new AgiAction(Canal, "exec(\"SENDDTMF\",\"#\")"));
            //Conn.SendAction(new StatusAction());
            //return new Result { Canal = Channel, UniqueId = UniqueId, Error = "", Status = "", DestChannel = DestChannel };
        }
        public String Monitorear(String Canal)
        {
            MonitorAction mo = new MonitorAction(Canal, UniqueId, "gsm", true);
            ManagerResponse response2 = Conn.SendAction(mo, 15000);
            return response2.Message;
        }
        private void Conn_Dial(object sender, AsterNET.Manager.Event.DialEvent e)
        {
            if ((e.SrcUniqueId == UniqueId || e.Channel == Channel) /*&& e.SubEvent == "Begin"*/)
            {
                DestChannel = e.Destination;
                //Monitorear(DestChannel);
            }
        }

        private void Conn_Status(object sender, AsterNET.Manager.Event.StatusEvent e)
        {
            Debug.Print("Hola!");
            //throw new NotImplementedException();
        }

        static void Conn_OriginateResponse(object sender, AsterNET.Manager.Event.OriginateResponseEvent e)
        {

            // throw new NotImplementedException();
        }


        public String Transferir(String Canal, String Otra)
        {
            String OtroCanal = Canal;
            RedirectAction o = new RedirectAction();
            o.Channel = OtroCanal;
            o.Context = "from-internal";
            o.Exten = Otra;
            o.Priority = 1;
            if (StatusExtension(Otra) == 0)
            {
                ManagerResponse response = Conn.SendAction(o, 30000);
                return response.Message;
            }
            else
            {
                throw new Exception("La Extensión no está disponible para transferir");
            }
        }
        public String Colgar(String Canal)
        {
            HangupAction o = new HangupAction();
            o.Channel = Canal;
            ManagerResponse response = Conn.SendAction(o, 30000);
            return response.Message;
        }
        public Int32 StatusExtension(String Extension)
        {
            ExtensionStateAction Ext = new ExtensionStateAction();
            Ext.Exten = Extension;
            Ext.Context = "from-internal";
            ExtensionStateResponse Resp = (ExtensionStateResponse)Conn.SendAction(Ext, 30000);
            return Resp.Status;

        }
        //public static Int32 Estado(String Extension)
        //{
        //    Int32 result = 0;
        //    CentralIp Central = new CentralIp();
        //    result = Central.Status(Extension);
        //    return result;
        //}
        public static StatusLlamada StatusLlamada(String ConnectionString, String Extension,String Telefono)
        {
            using (MySqlConnection Conn = new MySqlConnection(ConnectionString))//#lajeI1d89k
            {
                Conn.Open();
                MySqlCommand Comm = Conn.CreateCommand();
                Comm.CommandText = String.Format("SELECT disposition, duration, lastapp, billsec, uniqueid, userfield FROM cdr WHERE src='{0}' AND dst='{1}' ORDER BY calldate desc limit 1", Extension,Telefono);
                DataSet DSet = new DataSet();
                MySqlDataAdapter DAdap = new MySqlDataAdapter(Comm);
                DAdap.Fill(DSet);
                DataRow Fila = DSet.Tables[0].Rows[0];
                Conn.Close();
                String Resultado = Convert.ToString(Fila["disposition"]);
                String LastApp = Convert.ToString(Fila["lastapp"]);
                String UniqueId = Convert.ToString(Fila["uniqueid"]);
                String Grabacion = Convert.ToString(Fila["userfield"]);
                if (Grabacion.Length > 6) Grabacion = Grabacion.Substring(6); else Grabacion = UniqueId;
                if (LastApp == "Playback")
                {
                    Resultado = "Problema_Central_IP";
                }
                else
                {
                    switch (Resultado.ToUpper())
                    {
                        case "ANSWERED": Resultado = "Contestado"; break;
                        case "NO ANSWER": Resultado = "No Contestó"; break;
                        case "CONGESTION": Resultado = "Congestionado"; break;
                        case "FAILED": Resultado = "Falló"; break;
                        case "BUSY": Resultado = "Ocupado"; break;
                        default: Resultado = "Desconocido"; break;
                    }
                }

                return new StatusLlamada() { Duracion = Convert.ToInt32(Fila["duration"]), DuracionEfectiva = Convert.ToInt32(Fila["billsec"]), Grabacion = UniqueId/*Grabacion*/, Status = Resultado };
            }
        
        }
        public static StatusLlamada StatusLlamada(String ConnectionString, String UniqueId)
        {
            //"Server=172.17.1.102;Port=3306;Database=asteriskcdrdb;Uid=veconinterCC;Pwd=V3c0n1nt3r"
            using (MySqlConnection Conn = new MySqlConnection(ConnectionString))//#lajeI1d89k
            {
                Conn.Open();
                MySqlCommand Comm = Conn.CreateCommand();
                Comm.CommandText = String.Format("SELECT disposition, duration, lastapp, billsec, userfield FROM cdr WHERE uniqueid='{0}' ORDER BY calldate desc limit 1", UniqueId);
                DataSet DSet = new DataSet();
                MySqlDataAdapter DAdap = new MySqlDataAdapter(Comm);
                DAdap.Fill(DSet);
                DataRow Fila = DSet.Tables[0].Rows[0];
                Conn.Close();
                String Resultado = Convert.ToString(Fila["disposition"]);
                String LastApp = Convert.ToString(Fila["lastapp"]);
                String Grabacion = Convert.ToString(Fila["userfield"]);
                if (Grabacion.Length > 6) Grabacion = Grabacion.Substring(6); else Grabacion = UniqueId;
                if (LastApp == "Playback")
                {
                    Resultado = "Problema_Central_IP";
                }
                else
                {
                    switch (Resultado.ToUpper())
                    {
                        case "ANSWERED": Resultado = "Contestado"; break;
                        case "NO ANSWER": Resultado = "No Contestó"; break;
                        case "CONGESTION": Resultado = "Congestionado"; break;
                        case "FAILED": Resultado = "Falló"; break;
                        case "BUSY": Resultado = "Ocupado"; break;
                        default: Resultado = "Desconocido"; break;
                    }
                }

                return new StatusLlamada() { Duracion = Convert.ToInt32(Fila["duration"]), DuracionEfectiva = Convert.ToInt32(Fila["billsec"]), Grabacion = UniqueId/*Grabacion*/, Status = Resultado };
            }
        }
        public String Limpiar(String Telefono)
        {
            String Result = "";
            foreach (char c in Telefono)
            {
                if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9')
                {
                    Result += c;
                }
            }
            if (Result.StartsWith(CodigoPais))
            {
                Result = Result.Substring(CodigoPais.Length);
            }
            if (Result.StartsWith(CodigoArea))
            {
                Result = Result.Substring(CodigoArea.Length);
            }
            return Result;
        }

    }
}
