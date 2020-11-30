using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using noticias.Models;
using System.Data;



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

        public void asignarHijos(List<Manual> manuales)
        {
            

            for (int j = 0; j < manuales.Count; j++)
            {
                List<Manual> hijos = new List<Manual>();
                manuales[j].hijos = hijos;
                for (int i = 0; i < manuales.Count; i++)
                {
                    if (manuales[i].cPadre == manuales[j].cJerarquia)
                    {
                        manuales[j].hijos.Add(manuales[i]);

                    }
                }
            }
        }

        #region listarManuales

        public ActionResult ListarManuales()
        {
            List<Manual> manuales = new List<Manual>();
            List<Manual> manualesHijos = new List<Manual>();
            
            con = conexion.Instancia.Conectar();
            con.Open();
            cmd= new SqlCommand("ObtenerTodosLosManuales", con);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {

                Manual manual = new Manual();
                manual.cNombreManual = Convert.ToString(dr["cNombreManual"]);
                manual.NIdManual = Convert.ToInt32(dr["nIDManual"]);
                manual.cDescripcion = Convert.ToString(dr["cDescripcion"]);
                manual.bEstado = Convert.ToBoolean(dr["bEstado"]);
                manual.cPadre = Convert.ToString(dr["cPadre"]);
                manual.cJerarquia = Convert.ToString(dr["cJerarquia"]);
                manual.CTipoDocumento = Convert.ToString(dr["ctipoDocumento"]);
                manual.cUsuCodigo = Convert.ToInt32(dr["CUsuCodigo"]);
                manual.version = Convert.ToString(dr["version"]);
                manual.dFechaRegistro = Convert.ToDateTime(dr["dFechaRegistro"]);
                manual.ruta = Convert.ToString(dr["ruta"]);
                
                manuales.Add(manual);

            }
            con.Close();

            asignarHijos(manuales);


            List<Manual> Padres = new List<Manual>();

            foreach (var manual in manuales)
            {
                if(manual.CPadre == "") { Padres.Add(manual); }
            }

            return View(Padres);
        }

        #endregion

        #region #crearManuales
        public ActionResult CrearManualHijo (Manual manual)
        {
            con = conexion.Instancia.Conectar();
            con.Open();
            cmd = new SqlCommand("crearManual");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            //INSERTAR CREAR
            return RedirectToAction("ListarManuales", "Manual");
        }

        [HttpGet]
        public ActionResult CrearPadre()
        {

            return View(new Manual());

        }
        [HttpPost]
        public ActionResult CrearPadre(Manual manual)
        {
            try
            {
                manual.CTipoDocumento = "PDF";
                manual.cUsuCodigo = (int)Session["CodigoUsuario"];
                manual.version = "1.0";
                manual.ruta = "RutaPorDefault-NoDisponible";
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("CrearNuevoManualPadre",con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cNombreManual", SqlDbType.VarChar).Value = manual.CNombreManual;
                cmd.Parameters.Add("@cDescripcion", SqlDbType.VarChar).Value = manual.cDescripcion;
                cmd.Parameters.Add("@bEstado", SqlDbType.Bit).Value = true;
                //NO TIENE PADRE 
                cmd.Parameters.AddWithValue("@cPadre", DBNull.Value);
                // SE AUTOASIGNA JERARQUIA EN SQL
                cmd.Parameters.Add("@cTipoDocumento", SqlDbType.VarChar).Value = manual.CTipoDocumento;
                cmd.Parameters.Add("@cUsuCodigo", SqlDbType.Int).Value = manual.cUsuCodigo;
                cmd.Parameters.Add("@version", SqlDbType.VarChar).Value = manual.version;
                cmd.Parameters.Add("@dFechaRegistro", SqlDbType.DateTime).Value = DateTime.Today;
                cmd.Parameters.Add("@ruta", SqlDbType.VarChar).Value = manual.ruta;
                cmd.ExecuteNonQuery();
                con.Close();
                //INSERTAR CREAR
            }catch(Exception e)
            {
                throw e;
            }
                return RedirectToAction("ListarManuales", "Manual");
        }
        #endregion
    }
}