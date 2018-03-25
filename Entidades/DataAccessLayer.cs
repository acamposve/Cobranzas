using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Entidades
{
    public class DataAccessLayer
    {
        string StrConn = Properties.Settings.Default.CobranzasConnectionString;
        public DataTable EjecutarConsulta(string NombreSP, SqlParameter[] SqlParams)
        {
            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = default(SqlCommand);
            SqlDataAdapter da = default(SqlDataAdapter);
            DataTable dt = default(DataTable);
            try
            {
                cnn = new SqlConnection(StrConn);
                cmd = new SqlCommand(NombreSP, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                if ((SqlParams != null))
                {
                    for (int I = 0; I <= SqlParams.Length - 1; I++)
                    {
                        cmd.Parameters.Add(SqlParams[I]);
                    }
                }
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
                dt = new DataTable();
                return dt;
            }
            finally
            {
                if ((cnn != null))
                {
                    if (cnn.State == ConnectionState.Open)
                        cnn.Close();
                }
            }
        }

        public DataTable EjecutarConsulta(string NombreSP)
        {
            SqlConnection cnn = new SqlConnection(StrConn);
            SqlCommand cmd = default(SqlCommand);
            SqlDataAdapter da = default(SqlDataAdapter);
            DataTable dt = default(DataTable);
            try
            {
                cmd = new SqlCommand(NombreSP, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
                dt = new DataTable();
                return dt;
            }
            finally
            {
                if ((cnn != null))
                {
                    if (cnn.State == ConnectionState.Open)
                        cnn.Close();
                }
            }
        }

        public DataSet EjecutarConsultaDataSet(string NombreSP, SqlParameter[] SqlParams)
        {
            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = default(SqlCommand);
            SqlDataAdapter da = default(SqlDataAdapter);

            DataSet ds = new DataSet();
            try
            {
                cnn = new SqlConnection(StrConn);
                cmd = new SqlCommand(NombreSP, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                if ((SqlParams != null))
                {
                    for (int I = 0; I <= SqlParams.Length - 1; I++)
                    {
                        cmd.Parameters.Add(SqlParams[I]);
                    }
                }
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
                ds = new DataSet();
                return ds;
            }
            finally
            {
                if ((cnn != null))
                {
                    if (cnn.State == ConnectionState.Open)
                        cnn.Close();
                }
            }
        }

        public bool EjecutarAccion(string NombreSP, SqlParameter[] SqlParams)
        {
            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = default(SqlCommand);

            try
            {
                cnn = new SqlConnection(StrConn);
                cmd = new SqlCommand(NombreSP, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                if ((SqlParams != null))
                {
                    for (int I = 0; I <= SqlParams.Length - 1; I++)
                    {
                        cmd.Parameters.Add(SqlParams[I]);
                    }
                }
                cnn.Open();
                cmd.ExecuteNonQuery();
                cnn.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
            finally
            {
                if ((cnn != null))
                {
                    if (cnn.State == ConnectionState.Open)
                        cnn.Close();
                }
            }
        }

    }
}
