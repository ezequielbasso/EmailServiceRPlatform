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
    
    public partial class users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public users()
        {
            this.departaments_users = new HashSet<departaments_users>();
            this.Paycheck = new HashSet<Paycheck>();
        }
    
        public int user_id { get; set; }
        public string user_login { get; set; }
        public string user_name { get; set; }
        public Nullable<int> rule_id { get; set; }
        public string Pwd { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<departaments_users> departaments_users { get; set; }
        public virtual employee_reg_local employee_reg_local { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Paycheck> Paycheck { get; set; }
        public virtual roles roles { get; set; }
    }
}
