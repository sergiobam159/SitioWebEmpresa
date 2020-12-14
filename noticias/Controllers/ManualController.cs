using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using noticias.Models;
using System.Data;
using System.Web.Hosting;
using System.IO;

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

        #region vistaUsuario

        public ActionResult vistaManuales()
        {
            List<Manual> manuales = new List<Manual>();
            List<Manual> manualesHijos = new List<Manual>();

            con = conexion.Instancia.Conectar();
            con.Open();
            cmd = new SqlCommand("ObtenerTodosLosManuales", con);
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
                manual.nombreArchivo = Convert.ToString(dr["cNombreArchivo"]);
                manuales.Add(manual);

            }
            con.Close();

            asignarHijos(manuales);


            List<Manual> Padres = new List<Manual>();

            foreach (var manual in manuales)
            {
                if (manual.CPadre == "") { Padres.Add(manual); }
            }

            return View(Padres);

           
        }
        #endregion

        #region listarManuales
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
                manual.nombreArchivo = Convert.ToString(dr["cNombreArchivo"]);

                if(manual.nombreArchivo.Length > 0)
                {
                    
                    manual.archivoLectura = Descargar(manual.nombreArchivo);
                }
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

        #region #crearManuales--SUBIDA Y DECARGA
        public void subirArchivo(HttpPostedFileBase archivoManual)
        {

        }
        public FileResult Descargar(string NombreArchivo)
        {

            var FileVirtualPath = "~/FILE_SYSTEM/WEB/FILE/" + NombreArchivo;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }
        [HttpGet]
        public ActionResult CrearPadre()
        {

            return View(new Manual());

        }

        [HttpPost]
        public ActionResult CrearPadre(Manual manual)
        {
            var bandera = true;

            if (manual.cTipoDocumento == "PDF")
            {
                if (manual.archivo != null)
                {
                    string fileExtension = System.IO.Path.GetExtension(manual.archivo.FileName).ToUpper();
                    if (fileExtension == ".PDF")
                    {
                        DataTable dtResponse = fn_Get_SysFileConfiguracion_cPerJurCodigo_nSysModulo(99);
                        var rutafisica = (from c in dtResponse.AsEnumerable()
                                          where c.Field<string>("cSysFileKey") == "LogFileManual"
                                          select new
                                          {
                                              cSysFileValue = c.Field<String>("cSysFileValue")
                                          }).Select(c => c.cSysFileValue).SingleOrDefault();
                        //var rutavirtual = (from c in dtResponse.AsEnumerable()
                        //                  where c.Field<string>("cSysFileKey") == "EvPathFileContrato"
                        //                   select new
                        //                  {
                        //                      cSysFileValue = c.Field<String>("cSysFileValue")
                        //                  }).Select(c => c.cSysFileValue).SingleOrDefault();
                        string filename = manual.archivo.FileName.ToString();
                        string directory = rutafisica.Replace("\\", "/");
                        //string rutaguardado = string.Concat(rutafisica.Replace("\\", "/"), filename);
                        //manual.ruta = string.Concat(rutavirtual.Replace("\\", "/"), filename);
                        manual.ruta = string.Concat(rutafisica.Replace("\\", "/"), filename);
                        if (!System.IO.Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }
                        if (!System.IO.File.Exists(manual.ruta))
                        {
                            manual.archivo.SaveAs(manual.ruta);
                            bandera = true;
                        }
                    }
                    else
                    {

                        //RETORNAR ERROR -- IGRESE UN PDF
                        bandera = false;
                    }
                }
                else
                {
                    //RETORNAR ERROR -- INGRESE UN ARCHIVO
                    bandera = false;
                }
            }
            else
            {
                // asignar ruta si es video de youtube VIDEO DE YOUTUBE 
                bandera = true;
            }

            if (bandera)
            {
                SqlCommand com = new SqlCommand();
                con = conexion.Instancia.Conectar();
                con.Open();
                com.Connection = con;
                com.CommandText = "select top 1 cJerarquia from MANUAL where cPadre is null order by cJerarquia desc";
                dr = com.ExecuteReader();
                
                if (dr.Read())
                {
                    string ultimoPadre= dr["cJerarquia"].ToString();
                    if(int.Parse(ultimoPadre) > 9) {
                        int nuevopadre = int.Parse(ultimoPadre) + 1;
                        manual.cJerarquia = nuevopadre.ToString();
                    }
                    else
                    {
                        int nuevopadre = int.Parse(ultimoPadre) + 1;
                        manual.cJerarquia = string.Concat(0,nuevopadre.ToString());

                    }



                }
                else
                {
                    manual.cJerarquia = "01";
                }
                con.Close();
                if (manual.cTipoDocumento == "PDF")
                {
                    // si el manual es un pdf se pasa la ruta donde se guardo el archivo
                    manual.cUsuCodigo = (int)Session["CodigoUsuario"];
                    manual.version = "1.0";
                    manual.nombreArchivo = manual.archivo.FileName.ToString();
                   

                    try
                    {
                        con = conexion.Instancia.Conectar();
                        con.Open();
                        cmd = new SqlCommand("CrearNuevoManualPadre", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@cNombreManual", SqlDbType.VarChar).Value = manual.CNombreManual;
                        cmd.Parameters.Add("@cDescripcion", SqlDbType.VarChar).Value = manual.cDescripcion;
                        cmd.Parameters.Add("@bEstado", SqlDbType.Bit).Value = true;
                        //NO TIENE PADRE 
                        cmd.Parameters.AddWithValue("@cPadre", DBNull.Value);
                        cmd.Parameters.Add("@cJerarquia", SqlDbType.VarChar).Value = manual.cJerarquia;
                        cmd.Parameters.Add("@cTipoDocumento", SqlDbType.VarChar).Value = manual.CTipoDocumento;
                        cmd.Parameters.Add("@cUsuCodigo", SqlDbType.Int).Value = manual.cUsuCodigo;
                        cmd.Parameters.Add("@version", SqlDbType.VarChar).Value = manual.version;
                        cmd.Parameters.Add("@dFechaRegistro", SqlDbType.DateTime).Value = DateTime.Today;
                        cmd.Parameters.Add("@ruta", SqlDbType.VarChar).Value = manual.ruta;
                        cmd.Parameters.Add("@nombreArchivo", SqlDbType.VarChar).Value = manual.nombreArchivo;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    // si el manual es un video se pasa el link del video solamente
                    manual.cUsuCodigo = (int)Session["CodigoUsuario"];
                    manual.version = "1.0";

                   

                    try
                    {
                        con = conexion.Instancia.Conectar();
                        con.Open();
                        cmd = new SqlCommand("CrearNuevoManualPadre", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@cNombreManual", SqlDbType.VarChar).Value = manual.CNombreManual;
                        cmd.Parameters.Add("@cDescripcion", SqlDbType.VarChar).Value = manual.cDescripcion;
                        cmd.Parameters.Add("@bEstado", SqlDbType.Bit).Value = true;
                        //NO TIENE PADRE 
                        cmd.Parameters.AddWithValue("@cPadre", DBNull.Value);
                        cmd.Parameters.AddWithValue("@cJerarquia",SqlDbType.VarChar).Value= manual.cJerarquia;
                        cmd.Parameters.AddWithValue("@nombreArchivo", DBNull.Value);
                        cmd.Parameters.Add("@cTipoDocumento", SqlDbType.VarChar).Value = manual.CTipoDocumento;
                        cmd.Parameters.Add("@cUsuCodigo", SqlDbType.Int).Value = manual.cUsuCodigo;
                        cmd.Parameters.Add("@version", SqlDbType.VarChar).Value = manual.version;
                        cmd.Parameters.Add("@dFechaRegistro", SqlDbType.DateTime).Value = DateTime.Today;
                        cmd.Parameters.Add("@ruta", SqlDbType.VarChar).Value = manual.ruta;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }


            }

            return RedirectToAction("ListarManuales", "Manual");
            /*  try
              {
                  manual.CTipoDocumento = "PDF";
                  manual.cUsuCodigo = (int)Session["CodigoUsuario"];
                  manual.version = "1.0";
                  manual.ruta = "RutaPorDefault-NoDisponible";
                  con = conexion.Instancia.Conectar();
                  con.Open();
                  cmd = new SqlCommand("CrearNuevoManualPadre", con);
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
                  cmd.Parameters.Add("nombreArchivo", SqlDbType.VarChar).Value = manual.nombreArchivo;
                  cmd.ExecuteNonQuery();
                  con.Close();

              }
              catch (Exception e)
              {
                  throw e;
              }



            return RedirectToAction("ListarManuales", "Manual");*/
        }



        
        [HttpGet]
        public ActionResult CrearHijo(string  jerarquiaPadre)
        {
            TempData["jerarquiaPadre"] = jerarquiaPadre;

            return View(new Manual());
            
        }

        [HttpPost]
        public ActionResult CrearHijo(string jerarquiaPadre, Manual manual)
        {
            jerarquiaPadre = TempData["jerarquiaPadre"].ToString();
            
               var bandera = true;

            if (manual.cTipoDocumento == "PDF")
            {
                if (manual.archivo != null)
                {
                    string fileExtension = System.IO.Path.GetExtension(manual.archivo.FileName).ToUpper();
                    if (fileExtension == ".PDF")
                    {
                        DataTable dtResponse = fn_Get_SysFileConfiguracion_cPerJurCodigo_nSysModulo(99);
                        var rutafisica = (from c in dtResponse.AsEnumerable()
                                          where c.Field<string>("cSysFileKey") == "LogFileManual"
                                          select new
                                          {
                                              cSysFileValue = c.Field<String>("cSysFileValue")
                                          }).Select(c => c.cSysFileValue).SingleOrDefault();
                        //var rutavirtual = (from c in dtResponse.AsEnumerable()
                        //                  where c.Field<string>("cSysFileKey") == "EvPathFileContrato"
                        //                   select new
                        //                  {
                        //                      cSysFileValue = c.Field<String>("cSysFileValue")
                        //                  }).Select(c => c.cSysFileValue).SingleOrDefault();
                        string filename = manual.archivo.FileName.ToString();
                        string directory = rutafisica.Replace("\\", "/");
                        //string rutaguardado = string.Concat(rutafisica.Replace("\\", "/"), filename);
                        //manual.ruta = string.Concat(rutavirtual.Replace("\\", "/"), filename);
                        manual.ruta = string.Concat(rutafisica.Replace("\\", "/"), filename);
                        if (!System.IO.Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);                            
                        }
                        if (!System.IO.File.Exists(manual.ruta))
                        {
                            manual.archivo.SaveAs(manual.ruta);
                            bandera = true;
                        }
                    }
                    else
                    {

                        //RETORNAR ERROR -- IGRESE UN PDF
                        bandera = false;
                    }
                }
                else
                {
                    //RETORNAR ERROR -- INGRESE UN ARCHIVO
                    bandera = false;
                }
            }
            else {
                // asignar ruta si es video de youtube VIDEO DE YOUTUBE 
                bandera = true;
            }

            if (bandera)
            {
                if (manual.cTipoDocumento == "PDF")
                {
                    // si el manual es un pdf se pasa la ruta donde se guardo el archivo
                    manual.cUsuCodigo = (int)Session["CodigoUsuario"];
                    manual.version = "1.0";
                    manual.nombreArchivo = manual.archivo.FileName.ToString();
                    manual.CPadre = jerarquiaPadre;

                    try
                    {
                        con = conexion.Instancia.Conectar();
                        con.Open();
                        cmd = new SqlCommand("CrearManualHijo", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@cNombreManual", SqlDbType.VarChar).Value = manual.CNombreManual;
                        cmd.Parameters.Add("@cDescripcion", SqlDbType.VarChar).Value = manual.cDescripcion;
                        cmd.Parameters.Add("@bEstado", SqlDbType.Bit).Value = true;
                        //NO TIENE PADRE 
                        cmd.Parameters.AddWithValue("@cPadre", SqlDbType.VarChar).Value = manual.CPadre;
                        // SE AUTOASIGNA JERARQUIA EN SQL
                        cmd.Parameters.Add("@cTipoDocumento", SqlDbType.VarChar).Value = manual.CTipoDocumento;
                        cmd.Parameters.Add("@cUsuCodigo", SqlDbType.Int).Value = manual.cUsuCodigo;
                        cmd.Parameters.Add("@version", SqlDbType.VarChar).Value = manual.version;
                        cmd.Parameters.Add("@dFechaRegistro", SqlDbType.DateTime).Value = DateTime.Today;
                        cmd.Parameters.Add("@ruta", SqlDbType.VarChar).Value = manual.ruta;
                        cmd.Parameters.Add("@nombreArchivo", SqlDbType.VarChar).Value = manual.nombreArchivo;
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    // si el manual es un video se pasa el link del video solamente
                    manual.cUsuCodigo = (int)Session["CodigoUsuario"];
                    manual.version = "1.0";
                    
                    manual.CPadre = jerarquiaPadre;

                    try
                    {
                        con = conexion.Instancia.Conectar();
                        con.Open();
                        cmd = new SqlCommand("CrearManualHijo", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@cNombreManual", SqlDbType.VarChar).Value = manual.CNombreManual;
                        cmd.Parameters.Add("@cDescripcion", SqlDbType.VarChar).Value = manual.cDescripcion;
                        cmd.Parameters.Add("@bEstado", SqlDbType.Bit).Value = true;
                        //NO TIENE PADRE 
                        cmd.Parameters.AddWithValue("@cPadre", SqlDbType.VarChar).Value = manual.CPadre;
                        // SE AUTOASIGNA JERARQUIA EN SQL
                        cmd.Parameters.Add("@cTipoDocumento", SqlDbType.VarChar).Value = manual.CTipoDocumento;
                        cmd.Parameters.Add("@cUsuCodigo", SqlDbType.Int).Value = manual.cUsuCodigo;
                        cmd.Parameters.Add("@version", SqlDbType.VarChar).Value = manual.version;
                        cmd.Parameters.Add("@dFechaRegistro", SqlDbType.DateTime).Value = DateTime.Today;
                        cmd.Parameters.Add("@ruta", SqlDbType.VarChar).Value = manual.ruta;

                        cmd.Parameters.AddWithValue("@nombreArchivo", DBNull.Value);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                

            }

            return RedirectToAction("ListarManuales", "Manual");
        }




        #endregion

        #region BorrarManuales
        
        public ActionResult BorrarManual(int id)
        {
           

                try
                {
                    con = conexion.Instancia.Conectar();
                    con.Open();
                    cmd = new SqlCommand("BorrarManual", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@nIdManual", SqlDbType.Int).Value = id;
                    cmd.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    throw e;
                }


            
            return RedirectToAction("ListarManuales", "Manual");
        }




        public static DataTable fn_Get_SysFileConfiguracion_cPerJurCodigo_nSysModulo(int nSysModulo)
        {
            DataTable dtResponse = new DataTable();
            SqlConnection con = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;
            try
            {

                using (con = conexion.Instancia.Conectar())
                {
                    con.Open();
                    using (cmd = new SqlCommand())
                    {
                        cmd.CommandText = "Usp_Get_SysFileConfiguracion_cPerJurCodigo_nSysModulo";
                        cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.AddWithValue("@cPerJurCodigo ", cPerJurCodigo);
                        cmd.Parameters.AddWithValue("@nSysModulo", nSysModulo);
                        cmd.Connection = con;
                        using (dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dtResponse.Load(dr);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dtResponse;
        }

        #endregion
    }
}