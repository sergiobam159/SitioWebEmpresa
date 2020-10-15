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
          
            cn.ConnectionString = "Server=tcp:q4eqlp7t0w.database.windows.net,1433;Database=BDEPartners_DEV;User ID=AppWebEpartners@q4eqlp7t0w;Password=4ppW3b3p4r7n3r5;Trusted_Connection=False;Encrypt=True;Connection Timeout=600;";
            return cn;
        }
      
        #endregion metodos
    }
}


//---<add name = "IntegrationCN" connectionString="Server=tcp:q4eqlp7t0w.database.windows.net,1433;Database=BDEPartners_QA;User ID=AppWebEpartners@q4eqlp7t0w;Password=4ppW3b3p4r7n3r5;Trusted_Connection=False;Encrypt=True;Connection Timeout=600;"/>