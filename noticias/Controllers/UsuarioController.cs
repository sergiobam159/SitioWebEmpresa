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
       
        [HttpPost]
        public ActionResult Verificar(Usuario acc)
        {
            
            con = conexion.Instancia.Conectar();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from Usuario where cNombreUsuario='"+acc.cNombreUsuario+"' and cClaveUsuario='"+acc.cClaveUsuario+"'";
            dr = com.ExecuteReader();
            
            if (dr.Read())
            {

                return RedirectToAction("vistaCrud", "Noticia", new {inicial=0, elementos = 1 });
            }
            else
            {
                con.Close();
                return View("Error");
            }

            
            
        }

        
       
    }
}