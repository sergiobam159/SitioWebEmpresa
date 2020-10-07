using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noticias.Models
{
    public class Imagen
    {
        public int idImagen;
        public string descripcion;
        public HttpPostedFileBase imagen { get; set; }
        public byte[] img { get; set; }
        public int IdImagen { get => idImagen; set => idImagen = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
       
    }
}