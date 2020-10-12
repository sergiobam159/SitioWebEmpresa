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
        public string base64String;
        public HttpPostedFileBase imagen { get; set; }
        public byte[] img { get; set; }
        public int IdImagen { get => idImagen; set => idImagen = value; }
        public string Descripcion { get => descripcion; set => descripcion = value; }
        public string Base64String { get => base64String; set => base64String = value; }
    }
}