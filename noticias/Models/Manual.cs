using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace noticias.Models
{
    public class Manual
    {
        public int nIdManual;
        public string cNombreManual;
        public string cDescripcion;
        public Boolean bEstado;
        public string cPadre;
        public string cJerarquia;
        public string cTipoDocumento;
        public int cUsuCodigo;
        public string version;
        public DateTime dFechaRegistro;
        public string ruta;
        public List<Manual> hijos;
        public HttpPostedFileBase archivo { get; set; }

        public int NIdManual { get => nIdManual; set => nIdManual = value; }
        public string CNombreManual { get => cNombreManual; set => cNombreManual = value; }
        public string CDescripcion { get => cDescripcion; set => cDescripcion = value; }
        public bool BEstado { get => bEstado; set => bEstado = value; }
        public string CPadre { get => cPadre; set => cPadre = value; }
        public string CJerarquia { get => cJerarquia; set => cJerarquia = value; }
        public string CTipoDocumento { get => cTipoDocumento; set => cTipoDocumento = value; }
        public int CUsuCodigo { get => cUsuCodigo; set => cUsuCodigo = value; }
        public string Version { get => version; set => version = value; }
        public DateTime DFechaRegistro { get => dFechaRegistro; set => dFechaRegistro = value; }
        public string Ruta { get => ruta; set => ruta = value; }
        public List<Manual> Hijos { get => hijos; set => hijos = value; }
    }
}