using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using noticias.Models;


namespace noticias.Controllers
{
    public class ManualController : Controller
    {
        // GET: Manual
        public ActionResult manualCrud()
        {
            return View();
        }
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;


        #region listarManuales

        public ActionResult ListarManuales()
        {
            List<Manual> manualesPadres = new List<Manual>();
            Manual manual = new Manual();
            con = conexion.Instancia.Conectar();
            con.Open();
            cmd= new SqlCommand("ObtenerTodosLosPadres", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                manual.cNombreManual = Convert.ToString(dr["cNombreManual"]);
                manual.NIdManual = Convert.ToInt32(dr["nIDManual"]);
                manual.cDescripcion = Convert.ToString(dr["cDescripcion"]);
                manual.bEstado = Convert.ToBoolean(dr["bEstado"]);
                manual.cPadre = null;
                manual.cJerarquia = Convert.ToString(dr["cJerarquia"]);
                manual.CTipoDocumento = Convert.ToString(dr["ctipoDocumento"]);
                manual.cUsuCodigo = Convert.ToInt32(dr["CUsuCodigo"]);
                manual.version = Convert.ToString(dr["version"]);
                manual.dFechaRegistro = Convert.ToDateTime(dr["dFechaRegistro"]);
                manual.ruta = Convert.ToString(dr["ruta"]);
            }


            return View();
        }

        #endregion
    }
}