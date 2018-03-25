using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Data;
using System.Reflection;
namespace Cobranzas
{
    public static class General
    {
        public static string Md5Hash(string texto)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(texto));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        public static string Encriptar(string texto)
        {
            return cifrarTextoAES(texto, "&&admin%%ABC", "&&admin%%ABC", "MD5", 22, "1234567891234567", 128);
        }
        public static string Desencriptar(string texto)
        {
            return descifrarTextoAES(texto, "&&admin%%ABC", "&&admin%%ABC", "MD5", 22, "1234567891234567", 128);
        }
        public static string cifrarTextoAES(string textoCifrar, string palabraPaso,
                   string valorRGBSalt, string algoritmoEncriptacionHASH,
                   int iteraciones, string vectorInicial, int tamanoClave)
        {
            try
            {
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(vectorInicial);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(valorRGBSalt);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(textoCifrar);

                PasswordDeriveBytes password =
                    new PasswordDeriveBytes(palabraPaso, saltValueBytes,
                        algoritmoEncriptacionHASH, iteraciones);

                byte[] keyBytes = password.GetBytes(tamanoClave / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform encryptor =
                    symmetricKey.CreateEncryptor(keyBytes, InitialVectorBytes);

                MemoryStream memoryStream = new MemoryStream();

                CryptoStream cryptoStream =
                    new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                cryptoStream.FlushFinalBlock();

                byte[] cipherTextBytes = memoryStream.ToArray();

                memoryStream.Close();
                cryptoStream.Close();

                string textoCifradoFinal = Convert.ToBase64String(cipherTextBytes);

                return textoCifradoFinal;
            }
            catch
            {
                return null;
            }
        }


        public static string descifrarTextoAES(string textoCifrado, string palabraPaso,
            string valorRGBSalt, string algoritmoEncriptacionHASH,
            int iteraciones, string vectorInicial, int tamanoClave)
        {
            try
            {
                byte[] InitialVectorBytes = Encoding.ASCII.GetBytes(vectorInicial);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(valorRGBSalt);

                byte[] cipherTextBytes = Convert.FromBase64String(textoCifrado);

                PasswordDeriveBytes password =
                    new PasswordDeriveBytes(palabraPaso, saltValueBytes,
                        algoritmoEncriptacionHASH, iteraciones);

                byte[] keyBytes = password.GetBytes(tamanoClave / 8);

                RijndaelManaged symmetricKey = new RijndaelManaged();

                symmetricKey.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, InitialVectorBytes);

                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                memoryStream.Close();
                cryptoStream.Close();

                string textoDescifradoFinal = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

                return textoDescifradoFinal;
            }
            catch
            {
                return null;
            }
        }
        //public static Cobranzas.CobranzasDataContext db;
        public static string FormatoFecha;
        public static string FormatoFechaHora;
        public static string FormatoFechaW;
        public static String AFechaCorta(this DateTime Fecha)
        {
            return Fecha.ToString(FormatoFechaW);
        }
        public static String AFechaCorta(this DateTime? Fecha)
        {
            return Fecha == null ? "" : Fecha.Value.AFechaCorta();
        }
        public static String AFechaMuyCorta(this DateTime Fecha)
        {
            return Fecha.ToString(FormatoFecha);
        }
        public static String AFechaMuyCorta(this DateTime? Fecha)
        {
            return Fecha == null ? "" : Fecha.Value.AFechaMuyCorta();
        }
        public static String AFechaHora(this DateTime Fecha)
        {
            return Fecha.ToString(FormatoFechaHora);
        }
        public static String AFechaHora(this DateTime? Fecha)
        {
            return Fecha == null ? "" : Fecha.Value.AFechaHora();
        }

        public static string ArreglarNombre(string p)
        {
            String Result = "";
            for (int i = 0; i < p.Length; i++)
            {
                if (char.IsLetterOrDigit(p, i) || "_,.@ ".Contains(p[i]))
                {
                    Result += p[i];
                }
            }
            return Result;
        }

        public static String DataTableToCSV(DataTable Tabla)
        {
            String result = "";
            String Fila = "";
            foreach (DataColumn Col in Tabla.Columns)
            {
                Fila += ";" + Col.ColumnName;
            }
            Fila = Fila.Substring(1) + "\r\n";
            result += Fila;
            foreach (DataRow Row in Tabla.Rows)
            {
                Fila = "";
                foreach (DataColumn Col in Tabla.Columns)
                {
                    Fila += ";" + Row[Col].ToString();
                }
                Fila = Fila.Substring(1) + "\r\n";
                result += Fila;
            }
            return result;
        }
        private static PropertyInfo ObtenerPropiedad(Type tipoObjeto, String nombrePropiedad)
        {

            if ((tipoObjeto == null) || (String.IsNullOrEmpty(nombrePropiedad))) throw new ArgumentNullException();

            try
            {
                return tipoObjeto.GetProperty(nombrePropiedad, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public static object LeerValorPropiedad(Object objetoClase, String nombrePropiedad)
        {

            if ((objetoClase == null) || (String.IsNullOrEmpty(nombrePropiedad))) throw new ArgumentNullException();
            try
            {
                PropertyInfo pi = ObtenerPropiedad(objetoClase.GetType(), nombrePropiedad);

                if (pi == null) return null;

                return pi.GetValue(objetoClase, null);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public static string ToCSV(this string Texto)
        {
            if (Texto == null) return Texto;
            return "\"" + Texto.Replace("\n", "").Replace("\r", "").Replace("\t", " ").Replace("\"", "\"\"") + "\"";
        }

        public static string ObtenerNumeros(string TelefonoCrudo)
        {
            String Result = "";
            foreach (char c in TelefonoCrudo)
            {
                if ("0123456789".IndexOf(c) != -1)
                {
                    Result += c;
                }
            }
            return Result;
        }
        private static FieldInfo ObtenerCampo(Type tipoObjeto, String NombreCampo)
        {

            if ((tipoObjeto == null) || (String.IsNullOrEmpty(NombreCampo))) throw new ArgumentNullException();

            try
            {
                return tipoObjeto.GetField(NombreCampo, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public static object LeerValorCampo(Object objetoClase, String NombreCampo)
        {

            if ((objetoClase == null) || (String.IsNullOrEmpty(NombreCampo))) throw new ArgumentNullException();
            try
            {
                FieldInfo pi = ObtenerCampo(objetoClase.GetType(), NombreCampo);

                if (pi == null) return null;

                return pi.GetValue(objetoClase);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }


}