//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BD
{
    using System;
    using System.Collections.Generic;
    
    public partial class PropertysPaycheck
    {
        public int ID { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public int FormID { get; set; }
        public string DocumentName { get; set; }
        public int PaycheckID { get; set; }
    
        public virtual Paycheck Paycheck { get; set; }
        public virtual TypeOfForm TypeOfForm { get; set; }
    }
}
