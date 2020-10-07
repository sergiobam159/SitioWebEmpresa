using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;


namespace noticias.Models
{
    public class conexion
    {
        #region singleton
        private static readonly conexion UnicaInstancia = new conexion();
        public static conexion Instancia
        {
            get
            {
                return conexion.UnicaInstancia;
            }

        }
        #endregion singleton

        #region metodos
        public SqlConnection Conectar()
        {
            SqlConnection cn = new SqlConnection();
          
            cn.ConnectionString = "data source = DESKTOP-I49TNNO\\SQLEXPRESS; database=noticias; integrated security = SSPI;";
            return cn;
        }
      
        #endregion metodos
    }
}