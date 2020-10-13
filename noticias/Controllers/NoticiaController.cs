using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using noticias.Models;
using System.Data;
using System.IO;
using System.Web.Helpers;
using System.Drawing;
namespace noticias.Controllers
{
    
    public class NoticiaController : Controller
    {
        #region conexion

        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;

        // LISTADO DE NOTICIAS
        public ActionResult vistaCrud()
        {

            return View(ListadoNoticia());
        }
        #endregion
        #region lISTADO
        public List<Noticia> ListadoNoticia()
        {

            List<Noticia> lista = new List<Noticia>();
            try
            {
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("ListarNoticias", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {


                    Noticia n = new Noticia();
                    n.NIdPublicacion = Convert.ToInt16(dr["nIdPublicacion"]);
                    n.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
                    n.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
                    n.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
                    n.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
                    n.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
                    n.NidNoticia_seccion = Convert.ToInt16(dr["nidNoticia.Seccion"]);
                    n.CSubtitulo = Convert.ToString(dr["csubtitulo"]);
                    n.CTextoSubtitulo = Convert.ToString(dr["cTextoSubtitulo"]);
                    //VIDEO[idVideo]
                    n.IdAutor = Convert.ToInt16(dr["idAutor"]);
                    //IMAGEN[nIdNoticia.Pubimagen]
                    lista.Add(n);


                }
            } catch (Exception e)
            {
                throw e;
            }
            return lista;
        }
        #endregion
        #region eliminar
        public ActionResult EliminarNoticia(string id)
        {
            if (id != null)
            {

                try
                {
                    con = conexion.Instancia.Conectar();
                    con.Open();
                    cmd = new SqlCommand("EliminarNoticia", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@id", SqlDbType.VarChar).Value = id;
                    cmd.ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    throw e;
                }


            }
            return RedirectToAction("vistaCrud", "Noticia");
        }
        #endregion
        #region Crear
        [HttpGet]
        public ActionResult Crear()
        {


            return View(new Noticia());

        }

      

        [HttpPost]
        public ActionResult Crear(Noticia noticia)
        {

            //crear imagen
            try
            {


               /*
                Dentro de Imagen tengo un objeto httppostedfile, donde se guarda el archivo img, luego se convierte a WebImage, y luego el webimage se convierte en bytes
                y se almacena dentro de una variable byte[] en img, finalmente se convierte en un string base64 para ser enviado a la base de datos como un nVarchar
                */


                //SOPORTA IMAGENES MENORES A 4MB CUALQUIER RESOLUCION
                
                noticia.Img.imagen = Request.Files["Img.imagen"]; 
                WebImage imagen = new WebImage(noticia.Img.imagen.InputStream);
               noticia.Img.img = imagen.GetBytes();
               
                // conversor a base 64
                string br  = Convert.ToBase64String(noticia.Img.img);
                
                





                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("InsertarImagen", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = noticia.Img.descripcion;
                cmd.Parameters.Add("@imagen", SqlDbType.VarChar).Value = br;
                
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception e)
            {
                throw e;
            }



            //obtener id de ultima imagen insertada
            int idImagen = new int();
            try
            {
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("ObtenerUltimaImagen", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    Imagen img = new Imagen();
                    img.idImagen = Convert.ToInt16(dr["nIdNoticia.Pubimagen"]);
                    idImagen = img.idImagen;
                }

                con.Close();
               
            }
            catch (Exception e)
            {
                throw e;
            }


            try
            {
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("CREARPUBLICACION", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@lugar", SqlDbType.VarChar).Value = noticia.cLugarDePublicacion;
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = noticia.cUsuCodigo;
                cmd.Parameters.Add("@autorId", SqlDbType.Int).Value = noticia.idAutor;
                cmd.Parameters.Add("@seccionId", SqlDbType.Int).Value = noticia.NidNoticia_seccion;
                cmd.Parameters.Add("@titulo", SqlDbType.VarChar).Value = noticia.cTituloPublicacion;
                cmd.Parameters.Add("@contenido", SqlDbType.VarChar).Value = noticia.cContenidoPublicacion;
                cmd.Parameters.Add("@subtitulo", SqlDbType.VarChar).Value = noticia.cSubtitulo;
                cmd.Parameters.Add("@textoSubtitulo", SqlDbType.VarChar).Value = noticia.cTextoSubtitulo;
                // NO VIDEO >:c     cmd.Parameters.Add("@videoId", SqlDbType.Int).Value = noticia.idVideo;
                cmd.Parameters.Add("@imagenId", SqlDbType.Int).Value = idImagen;
               
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception e) {
                throw e;
            }

            return RedirectToAction("vistaCrud", "Noticia");

        }
        #endregion
        #region EDITAR

        [HttpGet]
        public ActionResult Editar(int id)
        {


            Noticia n = new Noticia();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            con = conexion.Instancia.Conectar();
            con.Open();
            cmd = new SqlCommand("Buscar", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            n.nIdPublicacion = id;
            n.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
            n.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
            n.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
            n.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
            n.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
            n.NidNoticia_seccion = Convert.ToInt16(dr["nidNoticia.Seccion"]);
            n.CSubtitulo = Convert.ToString(dr["csubtitulo"]);
            n.CTextoSubtitulo = Convert.ToString(dr["cTextoSubtitulo"]);
            //VIDEO[idVideo]
            n.IdAutor = Convert.ToInt16(dr["idAutor"]);
            //IMAGEN[nIdNoticia.Pubimagen]
            return View(n);
            
                


            
        }


        [HttpPost]
        public ActionResult Editar(int id, Noticia noticia)
        {
            //FECHA ESTA EN FORMATO YYYYMMDD '20170519' dentro del script

          
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("EditarPublicacion", con);
                cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@lugar", SqlDbType.VarChar).Value = noticia.cLugarDePublicacion;
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = noticia.cUsuCodigo;
                cmd.Parameters.Add("@autorId", SqlDbType.Int).Value = noticia.idAutor;
                cmd.Parameters.Add("@seccionId", SqlDbType.Int).Value = noticia.NidNoticia_seccion;
                cmd.Parameters.Add("@titulo", SqlDbType.VarChar).Value = noticia.cTituloPublicacion;
                cmd.Parameters.Add("@contenido", SqlDbType.VarChar).Value = noticia.cContenidoPublicacion;
                cmd.Parameters.Add("@subtitulo", SqlDbType.VarChar).Value = noticia.cSubtitulo;
                cmd.Parameters.Add("@textoSubtitulo", SqlDbType.VarChar).Value = noticia.cTextoSubtitulo;
                //      cmd.Parameters.Add("@videoId", SqlDbType.Int).Value = noticia.idVideo;
                //     cmd.Parameters.Add("@imagenId", SqlDbType.Int).Value = noticia.nIdNoticia_Imagen ;
                //SE TIENE QUE INVESTIGAR COMO INGRESAR IMAGENES Y VIDEO POR AHORA SON NULL en el stored procedure y aqui
                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return RedirectToAction("vistaCrud", "Noticia");

        }
        #endregion
        #region inicio

        public ActionResult InicioNoticias()
        {

            List<Noticia> lista = new List<Noticia>();
            try
            {
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("[UltimasNoticiasTodosLosDatos]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {


                    Noticia n = new Noticia();
                    n.Img = new Imagen();
                    n.NIdPublicacion = Convert.ToInt16(dr["nIdPublicacion"]);
                    n.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
                    n.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
                    n.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
                    n.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
                    n.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
                    n.NidNoticia_seccion = Convert.ToInt16(dr["nidNoticia.Seccion"]);
                    n.CSubtitulo = Convert.ToString(dr["csubtitulo"]);
                    n.CTextoSubtitulo = Convert.ToString(dr["cTextoSubtitulo"]);

                    n.NombreDeSeccion = Convert.ToString(dr["cNombreSeccion"]);
                    n.DescripcionDeImagen = Convert.ToString(dr["cDescripcion"]);
                    n.NombreAutor = Convert.ToString(dr["cNombre"]);
                    n.ApellidoAutor = Convert.ToString(dr["cApellido"]);
                    //VIDEO[idVideo]
                    n.IdAutor = Convert.ToInt16(dr["idAutor"]);
                    //imagen ↓
                    string base64= (Convert.ToString(dr["iImagen"]));
                    n.Img.Base64String = base64;
                    lista.Add(n);


                    /*
                 [nIdPublicacion] ok
      [dFechaPublicacion] ok
      ,[cContenidoPublicacion] ok
      ,[cTituloPublicacion] ok
      ,[cLugarDePublicacion] ok
      ,[PUBLICACION].[cUsuCodigo] ok
      [SECCION].[nidNoticia.Seccion] ok
	  [SECCION].cNombreSeccion no
      ,[csubtitulo] ok
      ,[cTextoSubtitulo] ok
      ,[idVideo], -
   
	  [PUBLICACION].idAutor ok
      ,[PUBLICACION].[nIdNoticia.Pubimagen], no
	  [IMAGEN].iImagen, ok
	  [IMAGEN].cDescripcion, no
	  [IMAGEN].cTipo, no
	  USUARIO.cNombreUsuario, no
	  persona.cNombre, no
	  persona.cApellido   no 
                 */

                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return View( lista);

        }
        #endregion
        #region VerNoticiaSeleccionada
        /*
         PRIMERO SE OBTIENE LA NOTICIA DESDE EL VIEW, SE RECIBE EL ID DE LA NOTICIA SELECCIONADA
         SE BUSCA ESA NOTICIA EN LA BASE DE DATOS Y SE GUARDA EN UN OBJETO NOTICIA
         SE LISTA LAS ULTIMAS NOTICIAS CREADAS EN LA BD
         SE AÑADE LA NOTICIA SELEECIONADA DENTRO DE LA LISTA COMO PRIMER ELEMENTO O ULTIMO
         SE MUESTRA LA VISTA CON EL ELEMENTO SELECCIONADO Y SE MUESTRAN LOS DEMAS ELEMENTOS COMO OPCIONES SELEECCIONABLES
             */
        public ActionResult NoticiaEspecifica()
        {
            List<Noticia> noticias = new List<Noticia>();


            return View(noticias);
        }
        
        #endregion
    }
}
