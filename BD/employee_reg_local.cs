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
    
    public partial class employee_reg_local
    {
        public int user_id { get; set; }
        public string Reg_employee { get; set; }
        public string local_employee_id { get; set; }
    
        public virtual users users { get; set; }
    }
}
