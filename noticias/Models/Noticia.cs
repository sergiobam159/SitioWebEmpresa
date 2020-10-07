using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace noticias.Models
{
    public class Noticia
    {
        public int nIdPublicacion;
        public DateTime dFechaPublicacion;
        public string cContenidoPublicacion;
        public string cTituloPublicacion;
        public string cLugarDePublicacion;
        public int cUsuCodigo;
        public int nidNoticia_seccion;
        public string cSubtitulo;
        public string cTextoSubtitulo;
        public int idVideo;
        public int idAutor;
        public int nIdNoticia_Imagen;
        private Imagen img;

        public int NIdPublicacion { get => nIdPublicacion; set => nIdPublicacion = value; }
        public DateTime DFechaPublicacion { get => dFechaPublicacion; set => dFechaPublicacion = value; }
        public string CContenidoPublicacion { get => cContenidoPublicacion; set => cContenidoPublicacion = value; }
        public string CLugarDePublicacion { get => cLugarDePublicacion; set => cLugarDePublicacion = value; }
        public int CUsuCodigo { get => cUsuCodigo; set => cUsuCodigo = value; }
        public int NidNoticia_seccion { get => nidNoticia_seccion; set => nidNoticia_seccion = value; }
        public string CSubtitulo { get => cSubtitulo; set => cSubtitulo = value; }
        public string CTextoSubtitulo { get => cTextoSubtitulo; set => cTextoSubtitulo = value; }
        public int IdVideo { get => idVideo; set => idVideo = value; }
        public int IdAutor { get => idAutor; set => idAutor = value; }
        public int NIdNoticia_Imagen { get => nIdNoticia_Imagen; set => nIdNoticia_Imagen = value; }
        public string CTituloPublicacion { get => cTituloPublicacion; set => cTituloPublicacion = value; }
        public Imagen Img { get => img; set => img = value; }
    }
}