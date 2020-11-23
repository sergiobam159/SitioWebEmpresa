using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using noticias.Models;
using System.Data.SqlClient;

namespace noticias.Controllers
{
    public class UsuarioController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        
        // GET: Usuario
        [HttpGet]
        public ActionResult Login()
        {
            return View();

        }
       
        public bool verificarTexto(string texto)
        {
            var validarExpresionRegular = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9]*$");

            if (!validarExpresionRegular.IsMatch(texto))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [HttpPost]
        public ActionResult Verificar(Usuario acc, FormCollection frm)
        {
            Usuario user = new Usuario();

            string seleccionRadioButton = frm["seleccion"].ToString();


            bool validarNombre = verificarTexto(acc.cNombreUsuario);
            bool validarContraseña = verificarTexto(acc.cClaveUsuario);
            string manual = Request.Form["manuales"];

            if (validarNombre && validarContraseña)
            {
                con = conexion.Instancia.Conectar();
                con.Open();
                com.Connection = con;
                com.CommandText = "select * from Usuario where cNombreUsuario='" + acc.cNombreUsuario + "' and cClaveUsuario='" + acc.cClaveUsuario + "'";
                dr = com.ExecuteReader();

                if (dr.Read())
                {
                    user.cNombreUsuario = Convert.ToString(dr["cNombreUsuario"]);
                    user.cClaveUsuario = Convert.ToString(dr["cClaveUsuario"]);
                    user.Activo = Convert.ToBoolean(dr["bActivo"]);

                    Session["usuario"] = user.cNombreUsuario;
                    Session["contraseña"] = user.cClaveUsuario;
                    Session["activo"] = user.Activo;

                    if (user.Activo == true)
                    {
                        if (seleccionRadioButton == "Noticias")
                        {
                            return RedirectToAction("vistaCrud", "Noticia", new { inicial = 0, elementos = 5 });
                        }
                        else
                        {
                            return RedirectToAction("ListarManuales", "Manual");
                        }
                    }
                    else {
                        con.Close();
                        return View("Error");

                    }
                }
                else
                {
                    con.Close();
                    return View("Error");
                }
            }
            
            else
            {
                con.Close();
                return View("Error");
            }

            
            
        }

        
       
    }
}