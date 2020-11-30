using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noticias.Models
{
    public class Usuario
    {
        public int cUsuCodigo { get; set; }
        public string   cNombreUsuario { get; set; }
        public string cClaveUsuario { get; set; }
        public Boolean Activo { get; set; }
    }
}