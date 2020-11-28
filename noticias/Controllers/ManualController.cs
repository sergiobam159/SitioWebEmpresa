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
            con = conexion.Instancia.Conectar();
            con.Open();
            cmd = new SqlCommand("crearManual");
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            //INSERTAR CREAR
            return RedirectToAction("ListarManuales", "Manual");
        }
        #endregion
    }
}