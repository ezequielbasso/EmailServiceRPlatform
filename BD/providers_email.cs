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
    
    public partial class providers_email
    {
        public int providers_email_id { get; set; }
        public string providers_email_address { get; set; }
        public int provider_id { get; set; }
    
        public virtual provider provider { get; set; }
    }
}
