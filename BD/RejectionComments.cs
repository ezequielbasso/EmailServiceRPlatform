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
    
    public partial class RejectionComments
    {
        public int RejectionCommentID { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Comment { get; set; }
        public int invoice_id { get; set; }
        public Nullable<int> department_id { get; set; }
    
        public virtual invoice invoice { get; set; }
    }
}
