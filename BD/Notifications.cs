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
    
    public partial class Notifications
    {
        public int NotificationID { get; set; }
        public int PrinterID { get; set; }
        public int ContractID { get; set; }
        public System.DateTime ExpirationDate { get; set; }
        public int RemainingDays { get; set; }
        public Nullable<System.DateTime> LastNotification { get; set; }
        public Nullable<System.DateTime> NextNotification { get; set; }
        public long CanNotify { get; set; }
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int NotificationNumber { get; set; }
        public Nullable<int> EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeEmail { get; set; }
        public string BusinessName { get; set; }
        public Nullable<int> ContractNumber { get; set; }
        public string SerialNumber { get; set; }
    }
}