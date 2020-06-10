using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace RicohMailService
{
    public partial class ServiceMail : ServiceBase
    {

        public int ApplicationID { get; set; }
        public string ApplicationName { get; set; }
        public bool IsMonitoring { get; set; }
        public string ToMonitor { get; set; }


        public int Inverval { get; set; }
        public Int16 SleepToTry { get; set; }
        public string FolderWithoutAttachments { get; set; }
        public string DownloadDirectory { get; set; }
        public string DownloadDirectoryOther { get; set; }
        public string AllowedExtensions { get; set; }
        public string AllowedExtensionsOther { get; set; }
        public string Host { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string LogPath { get; set; }
        public bool UseSSL { get; set; }
        public bool UseDecrypt { get; set; }
        public Int16 Port { get; set; }

        private Timer Timer_1 = null;
        IMAP.Logger logger = null;
        IMAP.Monitoring monitor = null;

        public string SmtpServer { get; set; }
        public string EmailFromMonitor { get; set; }
        public string UserMonitor { get; set; }
        public string PasswordMonitor { get; set; }
        //private Timer Timer_2 = null;

        public ServiceMail()
        {
            InitializeComponent();
            try
            {
                this.Inverval = int.Parse(ConfigurationManager.AppSettings["Interval"]);
                this.SleepToTry = Int16.Parse(ConfigurationManager.AppSettings["SleepToTry"]);
                this.FolderWithoutAttachments = ConfigurationManager.AppSettings["FolderNameWithoutAttachments"];
                this.DownloadDirectory = ConfigurationManager.AppSettings["DownloadDirectory"];
                this.DownloadDirectoryOther = ConfigurationManager.AppSettings["DownloadDirectoryOther"];
                this.AllowedExtensions = ConfigurationManager.AppSettings["AllowedExtensions"];
                this.AllowedExtensionsOther = ConfigurationManager.AppSettings["AllowedExtensionsOther"];
                this.Host = ConfigurationManager.AppSettings["Host"];
                this.User = ConfigurationManager.AppSettings["User"];
                this.Password = ConfigurationManager.AppSettings["Password"];
                this.LogPath = ConfigurationManager.AppSettings["LogPath"];
                this.Port = Int16.Parse(ConfigurationManager.AppSettings["Port"]);
                this.UseSSL = int.Parse(ConfigurationManager.AppSettings["UseSSL"]) == 1;
                this.UseDecrypt = int.Parse(ConfigurationManager.AppSettings["UseDecrypt"]) == 1;
                this.ApplicationID = Int16.Parse(ConfigurationManager.AppSettings["ApplicationID"]);
                this.ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
                this.IsMonitoring = int.Parse(ConfigurationManager.AppSettings["EnableMonitoring"]) == 1;

                this.ToMonitor = ConfigurationManager.AppSettings["ToMonitor"];
                this.SmtpServer = IMAP.Security.Decode(ConfigurationManager.AppSettings["SmtpServer"]);
                this.EmailFromMonitor = IMAP.Security.Decode(ConfigurationManager.AppSettings["EmailFromMonitor"]);
                this.UserMonitor = IMAP.Security.Decode(ConfigurationManager.AppSettings["UserMonitor"]);
                this.PasswordMonitor = IMAP.Security.Decode(ConfigurationManager.AppSettings["PasswordMonitor"]);



                logger = new IMAP.Logger(LogPath);
                Timer_1 = new Timer();
                Timer_1.Interval = this.Inverval;

                Timer_1.Elapsed += StartProcess;
                monitor = new IMAP.Monitoring(SmtpServer, UserMonitor, PasswordMonitor, EmailFromMonitor, ToMonitor);
            }
            catch (Exception ex)
            {

                logger.ToFile("error", ex);
            }

        }


        protected override void OnStart(string[] args)
        {
            Timer_1.Start();
            logger.ToFile("Servicio Iniciado.");

        //    if(this.IsMonitoring)
        //        monitor.SendMail(string.Format("[{0}]-[:{1}] [{2}] [{3}])",ApplicationName,ApplicationID,DateTime.Now,"INICIO OK"),"No se registraron errores al iniciar el servicio");
        //
        }

        protected override void OnStop()
        {
            logger.ToFile("Servicio detenido.");
            //if(IsMonitoring)
            //{
            //    monitor.SendMail(string.Format("[{0}]-[:{1}] [{2}] [{3}])", ApplicationName, ApplicationID, DateTime.Now, "DETENIDO OK"), "No se registraron errores al detener el servicio");
            //}

        }

        protected void StartProcess(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                new IMAP.IMAP(
                    this.DownloadDirectory,
                    this.DownloadDirectoryOther,
                    this.Host,
                    this.Port,
                    UseSSL,
                    this.User,
                    this.Password,
                    3,
                    this.SleepToTry,
                    true,
                    this.FolderWithoutAttachments,
                    this.AllowedExtensions,
                    this.AllowedExtensionsOther,
                    logger,
                    UseDecrypt).DownloadMails();
            }
            catch (Exception ex)
            {

                logger.ToFile("[ERROR EN StartProcess() primer método]" + ex.StackTrace);


            }


        }
            
    }
        
}
