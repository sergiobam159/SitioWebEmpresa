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
        #region Inicial

        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;

        // LISTADO DE NOTICIAS
        public ActionResult vistaCrud(int inicial, int elementos)
        {
           
            return View(ListadoNoticia(inicial,elementos));
        }
        #endregion
        #region lISTADO
        public int obtenerIdUltimaPublicacion()
        {
            con = conexion.Instancia.Conectar();
            con.Open();
            int idUltimaPublicacion = 0;
            cmd = new SqlCommand("obtenerIdultimaPublicacion", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                idUltimaPublicacion = Convert.ToInt16(reader["nIdPublicacion"]);
            }
            con.Close();
            return idUltimaPublicacion;
        }


        public int obteneridPrimeraPublicacion()
        {
            int idprimera = 0; 
            con = conexion.Instancia.Conectar();
            con.Open();
            
            cmd = new SqlCommand("obtenerIdPrimeraPublicacion", con);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                idprimera = Convert.ToInt16(reader["nIdPublicacion"]);
            }
            con.Close();
           

            return idprimera;
        }
        public List<Noticia> ListadoNoticia(int inicial, int elementos)
        {

            List<Noticia> lista = new List<Noticia>();
            try
            {
                
                ViewBag.IdUltimaPublicacion = obtenerIdUltimaPublicacion();
                ViewBag.IdPrimeraPublicacion = obteneridPrimeraPublicacion();
                int paginas = ViewBag.IdUltimaPublicacion / 5;
                if (paginas < 1) paginas = 1;
                if ((ViewBag.IdUltimaPublicacion - (ViewBag.IdPrimeraPublicacion-1)) % 5 > 0) paginas++;
                

                ViewBag.cantidadDePaginas = paginas;



                

                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("ListarNoticias", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@inicial", SqlDbType.Int).Value = inicial;
                cmd.Parameters.Add("@elementos", SqlDbType.Int).Value = elementos;


                SqlDataReader dr = cmd.ExecuteReader();
                //FALTA AÑADIR VALORES A ESTE PROCEDIMIENTO Y TAMBIEN JALAR LOS VALORES DESDE LA VISTA LUEGO IMPLEMENTAR BOTONES PARA LA VISTA 
                while (dr.Read())
                {


                    Noticia n = new Noticia();
                    n.NIdPublicacion = Convert.ToInt16(dr["nIdPublicacion"]);
                    n.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
                    n.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
                    n.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
                    n.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
                    n.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
                    n.NidNoticia_seccion = Convert.ToString(dr["nidNoticia.Seccion"]);
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
            return RedirectToAction("vistaCrud", "Noticia", new { inicial = 0, elementos = 5 });
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
                List<string> imagenes = new List<string>();
                noticia.Img.imagen = Request.Files["Img.imagen"]; 
                WebImage imagen = new WebImage(noticia.Img.imagen.InputStream);
               noticia.Img.img = imagen.GetBytes();
                // conversor a base 64
                string br  = Convert.ToBase64String(noticia.Img.img);
                imagenes.Add(br);

                noticia.ImgSecundaria.imagen = Request.Files["ImgSecundaria.imagen"];
                WebImage imagenSecundaria = new WebImage(noticia.imgSecundaria.imagen.InputStream);
                noticia.imgSecundaria.img = imagenSecundaria.GetBytes();
                br = Convert.ToBase64String(noticia.imgSecundaria.img);
                imagenes.Add(br);

                int idImagen = new int();
                con = conexion.Instancia.Conectar();
                con.Open();


                cmd = new SqlCommand("InsertarYObtenerImagen", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = noticia.Img.descripcion;
                cmd.Parameters.Add("@imagen", SqlDbType.VarChar).Value = imagenes[0];
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar).Value = 1;
                cmd.Parameters.Add("@descripcion2", SqlDbType.VarChar).Value = noticia.Img.descripcion;
                cmd.Parameters.Add("@imagen2", SqlDbType.VarChar).Value = imagenes[1];
                cmd.Parameters.Add("@tipo2", SqlDbType.VarChar).Value = 2;


                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    Imagen img = new Imagen();
                    img.idImagen = Convert.ToInt16(dr["nIdNoticia.Pubimagen"]);
                    idImagen = img.idImagen;
                }
                /*
                for (int i = 0; i < imagenes.Count(); i++)

                {
                    cmd = new SqlCommand("InsertarImagen", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@descripcion", SqlDbType.VarChar).Value = noticia.Img.descripcion;
                    cmd.Parameters.Add("@imagen", SqlDbType.VarChar).Value = imagenes[i];
                    cmd.Parameters.Add("@tipo", SqlDbType.VarChar).Value = i+1;
                    cmd.ExecuteNonQuery();
                }
                */
                con.Close();
           



            //obtener id de ultima imagen insertada ULTIMA IMAGEN INGRESADA ES LA SEGUNDA DE CADA PUBLICACION
           /* int idImagen = new int();
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

            */
            
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("CREARPUBLICACION", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@lugar", SqlDbType.VarChar).Value = noticia.cLugarDePublicacion;
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = Int32.Parse(Session["CodigoUsuario"].ToString());
                cmd.Parameters.Add("@autorId", SqlDbType.Int).Value = noticia.idAutor;
                cmd.Parameters.Add("@seccionId", SqlDbType.Int).Value = noticia.NidNoticia_seccion;
                cmd.Parameters.Add("@titulo", SqlDbType.VarChar).Value = noticia.cTituloPublicacion;
                cmd.Parameters.Add("@contenido", SqlDbType.VarChar).Value = noticia.cContenidoPublicacion;
                cmd.Parameters.Add("@subtitulo", SqlDbType.VarChar).Value = noticia.cSubtitulo;
                cmd.Parameters.Add("@textoSubtitulo", SqlDbType.VarChar).Value = noticia.cTextoSubtitulo;
                // NO VIDEO >:c     cmd.Parameters.Add("@videoId", SqlDbType.Int).Value = noticia.idVideo;
                cmd.Parameters.Add("@imagenIdSecundaria", SqlDbType.Int).Value = idImagen;
                cmd.Parameters.Add("@imagenId", SqlDbType.Int).Value = idImagen-1;
                cmd.ExecuteNonQuery();
                //REEMPLAZAR EL JOIN DEIMAGEN Y HACER UN SELEECT A IMAGEN CON EL ID DE LA PUBLICACION
                con.Close();
            }
            catch (Exception e) {
                throw e;
            }

            return RedirectToAction("vistaCrud", "Noticia", new { inicial = 0, elementos = 5 });

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
            n.NidNoticia_seccion = Convert.ToString(dr["nidNoticia.Seccion"]);
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
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = Int32.Parse( Session["CodigoUsuario"].ToString());
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

            return RedirectToAction("vistaCrud", "Noticia", new { inicial = 0, elementos = 5 });

        }
        #endregion
        #region inicio

        public ActionResult MostrarSeccion(int seccion)
        {
            List<Noticia> lista = new List<Noticia>();
            try
            {

                // cambiar 
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("[UtimasNoticiasPorSeccion]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@seccion", seccion);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    while (dr.Read())
                    {


                        Noticia n = new Noticia();
                        n.Img = new Imagen();
                        n.imgSecundaria = new Imagen();
                        n.NIdPublicacion = Convert.ToInt16(dr["nIdPublicacion"]);
                        n.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
                        n.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
                        n.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
                        n.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
                        n.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
                        n.NidNoticia_seccion = Convert.ToString(dr["nidNoticia.Seccion"]);
                        n.CSubtitulo = Convert.ToString(dr["csubtitulo"]);
                        n.CTextoSubtitulo = Convert.ToString(dr["cTextoSubtitulo"]);

                        n.NombreDeSeccion = Convert.ToString(dr["cNombreSeccion"]);
                        n.DescripcionDeImagen = Convert.ToString(dr["cDescripcion"]);
                        n.NombreAutor = Convert.ToString(dr["cPerNombre"]);
                        n.ApellidoAutor = Convert.ToString(dr["cPerApellido"]);
                        //VIDEO[idVideo]
                        n.IdAutor = Convert.ToInt16(dr["idAutor"]);
                        //imagen ↓


                        if (!dr.IsDBNull(13))
                        {
                            int IdImagenSec = Convert.ToInt16(dr["nIdNoticia.PubimagenSEC"]);

                            SqlConnection conect = new SqlConnection();
                            conect = conexion.Instancia.Conectar();
                            conect.Open();
                            SqlCommand cmdd = new SqlCommand("[ObtenerImagenSecundaria]", conect);
                            cmdd.CommandType = CommandType.StoredProcedure;
                            cmdd.Parameters.AddWithValue("@id", IdImagenSec);
                            SqlDataReader drd = cmdd.ExecuteReader();
                            if (drd.Read())
                            {
                                string base64ImgSec = (Convert.ToString(drd["iImagen"]));
                                n.imgSecundaria.descripcion = Convert.ToString(drd["cDescripcion"]);
                                n.imgSecundaria.base64String = base64ImgSec;
                            }
                            conect.Close();

                        }

                        string base64ImgPrin = (Convert.ToString(dr["iImagen"]));
                        n.Img.Base64String = base64ImgPrin;

                        lista.Add(n);




                    }
                }
                else
                {
                    return View("No_Existen_noticias");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return View("InicioNoticias",lista);


        }




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
                    n.imgSecundaria = new Imagen();
                    n.NIdPublicacion = Convert.ToInt16(dr["nIdPublicacion"]);
                    n.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
                    n.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
                    n.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
                    n.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
                    n.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
                    n.NidNoticia_seccion = Convert.ToString(dr["nidNoticia.Seccion"]);
                    n.CSubtitulo = Convert.ToString(dr["csubtitulo"]);
                    n.CTextoSubtitulo = Convert.ToString(dr["cTextoSubtitulo"]);

                    n.NombreDeSeccion = Convert.ToString(dr["cNombreSeccion"]);
                    n.DescripcionDeImagen = Convert.ToString(dr["cDescripcion"]);
                    n.NombreAutor = Convert.ToString(dr["cPerNombre"]);
                    n.ApellidoAutor = Convert.ToString(dr["cPerApellido"]);
                    //VIDEO[idVideo]
                    n.IdAutor = Convert.ToInt16(dr["idAutor"]);
                    //imagen ↓

                    
                    if (!dr.IsDBNull(13))
                    {
                        int IdImagenSec = Convert.ToInt16(dr["nIdNoticia.PubimagenSEC"]);

                        SqlConnection conect = new SqlConnection();
                        conect = conexion.Instancia.Conectar();
                        conect.Open();
                        SqlCommand cmdd = new SqlCommand("[ObtenerImagenSecundaria]", conect);
                        cmdd.CommandType = CommandType.StoredProcedure;
                        cmdd.Parameters.AddWithValue("@id", IdImagenSec);
                        SqlDataReader drd = cmdd.ExecuteReader();
                        if (drd.Read())
                        {
                            string base64ImgSec = (Convert.ToString(drd["iImagen"]));
                            n.imgSecundaria.descripcion = Convert.ToString(drd["cDescripcion"]);
                            n.imgSecundaria.base64String = base64ImgSec;
                        }
                        conect.Close();

                    }

                    string base64ImgPrin= (Convert.ToString(dr["iImagen"]));
                    n.Img.Base64String = base64ImgPrin;

                    lista.Add(n);


                   

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

            
            public ActionResult VerNoticiaSeleccionada(int id)
        {

            Noticia noticiaSel = new Noticia();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();

            con = conexion.Instancia.Conectar();
            con.Open();
            cmd = new SqlCommand("[BuscarNoticiaTodosLosDatos]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", id);
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            noticiaSel.Img = new Imagen();
            noticiaSel.imgSecundaria = new Imagen();
            noticiaSel.NIdPublicacion = Convert.ToInt16(dr["nIdPublicacion"]);
            noticiaSel.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
            noticiaSel.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
            noticiaSel.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
            noticiaSel.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
            noticiaSel.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
            noticiaSel.NidNoticia_seccion = Convert.ToString(dr["nidNoticia.Seccion"]);
            noticiaSel.CSubtitulo = Convert.ToString(dr["csubtitulo"]);
            noticiaSel.CTextoSubtitulo = Convert.ToString(dr["cTextoSubtitulo"]);

            noticiaSel.NombreDeSeccion = Convert.ToString(dr["cNombreSeccion"]);
            noticiaSel.DescripcionDeImagen = Convert.ToString(dr["cDescripcion"]);
            noticiaSel.NombreAutor = Convert.ToString(dr["cPerNombre"]);
            noticiaSel.ApellidoAutor = Convert.ToString(dr["cPerApellido"]);
            //VIDEO[idVideo]
            noticiaSel.IdAutor = Convert.ToInt16(dr["idAutor"]);
            //imagen ↓


            if (!dr.IsDBNull(13))
            {
                int IdImagenSec = Convert.ToInt16(dr["nIdNoticia.PubimagenSEC"]);

                SqlConnection conect = new SqlConnection();
                conect = conexion.Instancia.Conectar();
                conect.Open();
                SqlCommand cmdd = new SqlCommand("[ObtenerImagenSecundaria]", conect);
                cmdd.CommandType = CommandType.StoredProcedure;
                cmdd.Parameters.AddWithValue("@id", IdImagenSec);
                SqlDataReader drd = cmdd.ExecuteReader();
                if (drd.Read())
                {
                    string base64ImgSec = (Convert.ToString(drd["iImagen"]));

                    noticiaSel.imgSecundaria.base64String = base64ImgSec;
                }
                conect.Close();

            }






            string base64 = (Convert.ToString(dr["iImagen"]));
            noticiaSel.Img.Base64String = base64;
            con.Close();

            List<Noticia> lista = new List<Noticia>();
            lista.Add(noticiaSel);
            
                con = conexion.Instancia.Conectar();
                con.Open();
                cmd = new SqlCommand("[UltimasNoticiasTodosLosDatos]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                 dr = cmd.ExecuteReader();

                while (dr.Read())
                {


                    Noticia n = new Noticia();
                    n.Img = new Imagen();
                       n.imgSecundaria = new Imagen();
                    n.NIdPublicacion = Convert.ToInt16(dr["nIdPublicacion"]);
                    n.DFechaPublicacion = Convert.ToDateTime(dr["dFechaPublicacion"]);
                    n.CContenidoPublicacion = Convert.ToString(dr["cContenidoPublicacion"]);
                    n.CTituloPublicacion = Convert.ToString(dr["cTituloPublicacion"]);
                    n.CLugarDePublicacion = Convert.ToString(dr["cLugarDePublicacion"]);
                    n.CUsuCodigo = Convert.ToInt32(dr["cUsuCodigo"]);
                    n.NidNoticia_seccion = Convert.ToString(dr["nidNoticia.Seccion"]);
                    n.CSubtitulo = Convert.ToString(dr["csubtitulo"]);
                    n.CTextoSubtitulo = Convert.ToString(dr["cTextoSubtitulo"]);

                    n.NombreDeSeccion = Convert.ToString(dr["cNombreSeccion"]);
                    n.DescripcionDeImagen = Convert.ToString(dr["cDescripcion"]);
                    n.NombreAutor = Convert.ToString(dr["cPerNombre"]);
                    n.ApellidoAutor = Convert.ToString(dr["cPerApellido"]);
                    //VIDEO[idVideo]
                    n.IdAutor = Convert.ToInt16(dr["idAutor"]);
                //imagen ↓


                if (!dr.IsDBNull(13))
                {
                    int IdImagenSec = Convert.ToInt16(dr["nIdNoticia.PubimagenSEC"]);

                    SqlConnection conect = new SqlConnection();
                    conect = conexion.Instancia.Conectar();
                    conect.Open();
                    SqlCommand cmdd = new SqlCommand("[ObtenerImagenSecundaria]", conect);
                    cmdd.CommandType = CommandType.StoredProcedure;
                    cmdd.Parameters.AddWithValue("@id", IdImagenSec);
                    SqlDataReader drd = cmdd.ExecuteReader();
                    if (drd.Read())
                    {
                        string base64ImgSec = (Convert.ToString(drd["iImagen"]));

                        n.imgSecundaria.base64String = base64ImgSec;
                    }
                    conect.Close();

                }



                base64 = (Convert.ToString(dr["iImagen"]));
                    n.Img.Base64String = base64;

                    if (n.NIdPublicacion != noticiaSel.nIdPublicacion)
                    {
                        lista.Add(n);
                    }



                }
          
            return View(lista);

        }




        #endregion

        #region BuscadorDeNoticias
        /*public List<Noticia> BuscardorDeNoticias()
        {

        }
        */

        #endregion

    }
}
