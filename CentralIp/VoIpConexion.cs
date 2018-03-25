using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
namespace CentralIp
{
    //public class VoIpConexion
    //{
    //    public String Host;
    //    public Int32 Puerto;
    //    private TcpClient Cliente;
    //    private NetworkStream Stream;
    //    private String Cookie;
    //    public VoIpConexion(String Host, Int32 Puerto = 20005)
    //    {
    //        this.Host = Host;
    //        this.Puerto = Puerto;
    //    }
    //    public Boolean Conectar()
    //    {
    //        try
    //        {
    //            if (Cliente != null)
    //            {
    //                Desconectar();
    //            }
    //            Cliente = new TcpClient(Host, Puerto);
    //            Stream = Cliente.GetStream();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //    public void Desconectar()
    //    {
    //        Cliente.Close();
    //        Stream.Close();
    //    }
    //    public Respuesta Enviar(Peticion Peticion)
    //    {
    //        byte[] Buffer = Encoding.ASCII.GetBytes(Peticion.ObtenerXML());
    //        Stream.Write(Buffer, 0, Buffer.Length);
    //        Buffer = new byte[4096];

    //        Int32 Numero = Stream.Read(Buffer, 0, 4096);
    //        String Resultado = Encoding.ASCII.GetString(Buffer, 0, Numero);

    //        return new Respuesta { Cuerpo = Resultado };
    //    }
    //    public void LLamar(String Telefono)
    //    {
    //        MySqlConnection Conn = new MySqlConnection("Server=172.17.1.102;Port=3306;Database=call_center;Uid=veconinterCC;Pwd=#lajeI1d89k;");
    //        Conn.Open();
    //        MySqlCommand Comm = Conn.CreateCommand();
    //        Comm.CommandText = String.Format("INSERT INTO calls (id_campaign,phone) VALUES (4,'{0}')", Telefono);
    //        Comm.ExecuteNonQuery();
    //        Conn.Close();
    //    }

    //    public Respuesta Login(String UserName, String Password)
    //    {
    //        Respuesta Result = Enviar(new PetLogin { UserName = UserName, Password = General.Md5Hash(Password) });
    //        Cookie = Result.XML.Descendants("app_cookie").First().Value;
    //        return Result;
    //    }
    //    public Respuesta Logout()
    //    {
    //        Respuesta Result = Enviar(new PetLogout { });
    //        Cookie = "";
    //        return Result;
    //    }
    //    public Respuesta LoginAgent(String AgentNumber, String Password, String Extension)
    //    {
    //        Respuesta Result = Enviar(new PetLoginAgent { AgentNumber = AgentNumber, AgentHash = CentralIp.Md5Hash(Cookie + AgentNumber + Password), Extension = Extension, Password = Password });
    //        return Result;
    //    }
    //    public Respuesta LogoutAgent(String AgentNumber, String Password)
    //    {
    //        Respuesta Result = Enviar(new PetLogoutAgent { AgentNumber = AgentNumber, AgentHash = CentralIp.Md5Hash(Cookie + AgentNumber + Password)});
    //        return Result;
    //    }
    //    public Respuesta GetAgentStatus(String AgentNumber) {
    //        Respuesta Result = Enviar(new PetGetAgentStatus { AgentNumber = AgentNumber});
    //        return Result;
    //    }
    //    public String GetAgentStatusS(String AgentNumber) {
    //        Respuesta Result = Enviar(new PetGetAgentStatus { AgentNumber = AgentNumber });

    //        return Result.XML.Descendants("app_cookie").First().Value; ;
    //    }
    //}
}
