using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MailKit;
//using MailKit.Net.Imap;
//using MailKit.Search;
//using MimeKit;
using System.Threading;
//using MailKit.Security;
using ImapX;
using System.Net.Mail;
using BD;
using ImapX.Enums;

namespace IMAP
{
    public class IMAP
    {
        public string _PathDownload { get; set; }
        public string _PathDownloadOther { get; set; }
        public string _Host { get; set; }
        public int _Port { get; set; }
        public bool _UseSSL  { get; set; }
        public string _User { get; set; }
        public string _Password { get; set; }
        public  Int16 _TryLimit { get; set; }
        public bool _TryOnError { get; set; }
        public Int16 _SleepToTry { get; set; }
        public Int16 _NumberOfRetries { get; set; }
        public string _FolderNameWithoutAttachments { get; set; }
        public string _AllowedExtension { get; set; }
        public string _AllowedExtensionOther { get; set; }
        public Logger _Logger{ get; set; }
        public bool _UseDecrypt { get; set; }

        #region Constructor

        public IMAP(
            string PathDownload,
            string PathDownloadOther,
            string Host,
            int Port,
            bool UseSSL,
            string User,
            string Password,
            Int16 TryLimit,
            Int16 SleepToTry,
            bool TryOnError,
            string FolderNameWithoutAttachments,
            string AllowedExtension,
            string AllowedExtensionOther,
            Logger logger,
            bool UseDecrypt)
        {
            _PathDownloadOther = PathDownloadOther;
            _PathDownload = PathDownload;
            _Host = Host;
            _Port = Port;
            _UseSSL = UseSSL;
            _User = User;
            _Password = Password;
            _TryLimit = TryLimit;
            _SleepToTry = SleepToTry;
            _TryOnError = TryOnError;
            _FolderNameWithoutAttachments = FolderNameWithoutAttachments;
            _AllowedExtension = AllowedExtension;
            _AllowedExtensionOther = AllowedExtensionOther;
            _Logger = logger;
            _UseDecrypt = UseDecrypt;
        }
        #endregion


        #region Métodos publicos
        public void DownloadMails()
        {
            try
            {
                //incremento intento
                _NumberOfRetries++;
                string[] arrAllowedExtension = _AllowedExtension.Split(',');
                string[] arrAllowedExtensionOther = _AllowedExtensionOther.Split(',');
                int MessagesMarkedDeleted = 0;
                string user = string.Empty;
                string password = string.Empty;

                if (_UseDecrypt)
                {
                    user = Security.Decrypt(_User);
                    password = Security.Decrypt(_Password);
                }
                else 
                {
                    user = _User;
                    password = _Password;
                }

                //Carpeta que contien mail sin adjuntos
                Folder FolderWithoutAttachments = null;

                bool HasError = false;

                using (ImapClient client = new ImapClient(_Host, _UseSSL))
                {

                    client.UseSsl = _UseSSL;
                    DateTime _InicioProceso = new DateTime(1900, 1, 1);
                    DateTime _FinProceso = new DateTime(1900, 1, 1);

                    if (client.Connect())
                    {

                        if (client.Login(user, password))
                        {
                            //client.Behavior.MessageFetchMode = MessageFetchMode.Full;
                            client.Behavior.AutoPopulateFolderMessages = true;

                            //client.Folders.Inbox.Messages.Download();

                            client.Folders.Inbox.Select();


                                long unseen = client.Folders.Inbox.Unseen;


                                var Messages = client.Folders.Inbox.Messages.ToList();

                                _InicioProceso = DateTime.Now;
                                _Logger.ToFile(string.Format("=== PROCESO INICIADO: {0}", _InicioProceso));

                                //obtengo carpeta contenedora de archivos sin adjuntos.

                                FolderWithoutAttachments = client.Folders.FirstOrDefault(f => f.Name == _FolderNameWithoutAttachments);

                                #region Crear Carpetas inexistentes

                                //si no existen las carpetas las creo

                                if (FolderWithoutAttachments == null)
                                {
                                    //carpeta no existe, crear.
                                    client.Folders.Add(_FolderNameWithoutAttachments);
                                    FolderWithoutAttachments = client.Folders.FirstOrDefault(f => f.Name == _FolderNameWithoutAttachments);
                                }

                                #endregion

                                foreach (var message in Messages)
                                {

                                    if (message.Attachments.Length == 0 && message.EmbeddedResources.Length == 0)
                                    {
                                        message.MoveTo(FolderWithoutAttachments);
                                        _Logger.ToFile(string.Format("=== MOVIENDO ARCHIVO SIN ADJUNTOS A: {0}", _FolderNameWithoutAttachments));
                                    }
                                    else
                                    {
                                        message.Download(ImapX.Enums.MessageFetchMode.Attachments);

                                        //se insertan los datos del mail en la base, tabla received email. 
                                        Guid email_guid = Guid.NewGuid();
                                        Insert insert = new Insert();

                                        received_emails mail = new received_emails
                                        {
                                            email_address = message.From.Address,
                                            SenderName = message.From.DisplayName,
                                            email_subject = message.Subject.Replace("[EXTERNAL]", ""),
                                            email_date = DateTime.Parse(message.Date.ToString()),
                                            email_attachment_quantity = message.Attachments.Length + message.EmbeddedResources.Length,
                                            ReadedDate = null,
                                            received_email_GUID = email_guid
                                        };

                                        if (!insert.Received(mail))
                                        {
                                            message.MoveTo(FolderWithoutAttachments);
                                            _Logger.ToFile(string.Format("=== MOVIENDO ARCHIVO SIN ADJUNTOS ERROR EN DB: {0}", _FolderNameWithoutAttachments));
                                        }
                                        else
                                        {
                                            //adjuntos modalidad embebida
                                            foreach (var embebedAttachment in message.EmbeddedResources)
                                            {

                                                string exte = Path.GetExtension(embebedAttachment.FileName).ToLower();

                                                if (_AllowedExtension.Contains(exte))
                                                {
                                                        embebedAttachment.Download();
                                                        _Logger.ToFile(string.Format("<<< RECIBIENDO ARCHIVO:[{0} Bytes] {1} ", embebedAttachment.FileSize, embebedAttachment.FileName));

                                                        string attachment_guid = Guid.NewGuid().ToString("n");

                                                     

                                                        attachments attach = new attachments
                                                        {
                                                            attachment_GUID = attachment_guid,
                                                            pathfile = _PathDownload + attachment_guid + exte,
                                                            filename = attachment_guid + exte

                                                        };

                                                        if (insert.Attachments(attach, email_guid))
                                                        {
                                                            _Logger.ToFile(string.Format("<=> DATOS INSERTADOS {0} ", "OK"));
                                                            embebedAttachment.Save(_PathDownload, attachment_guid + exte);
                                                            _Logger.ToFile(string.Format("<=> GUARDADO {0} ", "OK"));
                                                        }
                                                        else
                                                        {
                                                            _Logger.ToFile(string.Format("<=> ERROR GUARDADO {0} ", "NO SE INSERTO EN DB"));
                                                        }
                                                }

                                                else  if (_AllowedExtensionOther.Contains(exte))
                                                {
                                                        embebedAttachment.Download();
                                                        _Logger.ToFile(string.Format("<<< RECIBIENDO ARCHIVO:[{0} Bytes] {1} ", embebedAttachment.FileSize, embebedAttachment.FileName));

                                                        string ext = Path.GetExtension(embebedAttachment.FileName).ToLower();
                                                        string attachment_guid = Guid.NewGuid().ToString("n");
                                                        string attachment_page_guid = Guid.NewGuid().ToString("n");

                                                        attachments attach = new attachments
                                                        {
                                                            attachment_GUID = attachment_guid,
                                                            pathfile = _PathDownloadOther + attachment_guid + ext,
                                                            filename = attachment_guid + ext

                                                        };

                                                        Attachments_pages attach_page = new Attachments_pages
                                                        {
                                                            Attachment_page_id = attachment_page_guid,
                                                            Reg_date = DateTime.Now,
                                                            IsProcessing = true
                                                        };

                                                        if (insert.Attachments(attach, email_guid))
                                                        {
                                                            if (insert.Attachments_page(attach_page, attachment_guid))
                                                            {
                                                                _Logger.ToFile(string.Format("<=> DATOS INSERTADOS {0} ", "OK"));
                                                                embebedAttachment.Save(_PathDownloadOther, attachment_page_guid + ext);
                                                                _Logger.ToFile(string.Format("<=> GUARDADO {0} ", "OK"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _Logger.ToFile(string.Format("<=> ERROR GUARDADO {0} ", "NO SE INSERTO EN DB"));
                                                        }
                                                    }
                                            }
                         

                                            foreach (var attachment in message.Attachments)
                                            {

                                                string exte = Path.GetExtension(attachment.FileName).ToLower();

                                                if (_AllowedExtension.Contains(exte))
                                                {
                                                        attachment.Download();
                                                        _Logger.ToFile(string.Format("<<< RECIBIENDO ARCHIVO:[{0} Bytes] {1} ", attachment.FileSize, attachment.FileName));

                                                        string attachment_guid = Guid.NewGuid().ToString("n");

                                                        attachments attach = new attachments
                                                        {
                                                            attachment_GUID = attachment_guid,
                                                            pathfile = _PathDownload + attachment_guid + exte,
                                                            filename = attachment_guid + exte

                                                        };

                                                        if (insert.Attachments(attach, email_guid))
                                                        {
                                                            _Logger.ToFile(string.Format("<=> DATOS INSERTADOS {0} ", "OK"));
                                                            attachment.Save(_PathDownload, attachment_guid + exte);
                                                            _Logger.ToFile(string.Format("<=> GUARDADO {0} ", "OK"));
                                                        }
                                                        else
                                                        {
                                                            _Logger.ToFile(string.Format("<=> ERROR GUARDADO {0} ", "NO SE INSERTO EN DB"));
                                                        }
                                                }
                                                else if (_AllowedExtensionOther.Contains(exte))
                                                {
                                                        attachment.Download();
                                                        _Logger.ToFile(string.Format("<<< RECIBIENDO ARCHIVO:[{0} Bytes] {1} ", attachment.FileSize, attachment.FileName));

                                                        string ext = Path.GetExtension(attachment.FileName).ToLower();
                                                        string attachment_guid = Guid.NewGuid().ToString("n");
                                                        string attachment_page_guid = Guid.NewGuid().ToString("n");

                                                        attachments attach = new attachments
                                                        {
                                                            attachment_GUID = attachment_guid,
                                                            pathfile = _PathDownloadOther + attachment_guid + ext,
                                                            filename = attachment_guid + ext

                                                        };

                                                        Attachments_pages attach_page = new Attachments_pages
                                                        {
                                                            Attachment_page_id = attachment_page_guid,
                                                            Reg_date = DateTime.Now,
                                                            IsProcessing = true
                                                        };

                                                        if (insert.Attachments(attach, email_guid))
                                                        {
                                                            if (insert.Attachments_page(attach_page, attachment_guid))
                                                            {
                                                                _Logger.ToFile(string.Format("<=> DATOS INSERTADOS {0} ", "OK"));
                                                                attachment.Save(_PathDownloadOther, attachment_page_guid + ext);
                                                                _Logger.ToFile(string.Format("<=> GUARDADO {0} ", "OK"));
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _Logger.ToFile(string.Format("<=> ERROR GUARDADO {0} ", "NO SE INSERTO EN DB"));
                                                        }
                                                    }
                                                
                                            }

                                            //message.Flags.Add(ImapX.Flags.MessageFlags.Deleted);
                                            if (message.Remove())
                                            {
                                                MessagesMarkedDeleted++;
                                            }

                                        }
                                    }
                                    _Logger.ToFile(string.Format(">>> MENSAJES ELIMINADOS {0} ", MessagesMarkedDeleted));
                                }

                                _FinProceso = DateTime.Now;
                                _Logger.ToFile(string.Format("=== PROCESO FINALIZADO: {0}", _FinProceso));

                          
                        }
                        else
                        {
                            _Logger.ToFile(string.Format("[!] LAS CREDENCIALES SON ERRÓNEAS, VERIFIQUE SI NO ESTAN VENCIDAS: {0}", _Host));

                        }
                    }
                    else
                    {
                        _Logger.ToFile(string.Format("[!] NO SE PUEDE CONECTAR AL SERVIDOR: {0}", _Host));
                    }

                    if (client.IsAuthenticated)
                    {
                        bool ret = client.Logout();
                        _Logger.ToFile(string.Format(">>> DESCONEXIÓN: {0}", ret ? "OK!" : "FALLO"));
                    }

                    if (HasError)
                    {
                        _Logger.ToFile(">>> [!] FIN PROCESOS CON ERRORES");
                    }
                    else
                    {
                        _Logger.ToFile(">>> FIN PROCESOS SIN ERRORES");
                    }
                    #region Código no utilizado
                    //using (var client = new ImapClient())
                    //{
                    //    client.Connect("imap.gmail.com", 993, SslOptions);
                    //    client.Authenticate("ricohlademo", "Ricoh2019");

                    //    client.Inbox.Open(FolderAccess.ReadWrite);
                    //    IList<UniqueId> uids = client.Inbox.Search(SearchQuery.All);

                    //    foreach (UniqueId uid in uids)
                    //    {
                    //        MimeMessage message = client.Inbox.GetMessage(uid);

                    //        foreach (MimeEntity attachment in message.Attachments)
                    //        {
                    //            var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

                    //            using (var stream = File.Create(fileName))
                    //            {
                    //                if (attachment is MessagePart)
                    //                {
                    //                    var rfc822 = (MessagePart)attachment;

                    //                    rfc822.Message.WriteTo(stream);
                    //                }
                    //                else
                    //                {
                    //                    var part = (MimePart)attachment;

                    //                    part.Content.DecodeTo(stream);
                    //                }
                    //            }
                    //        }
                    //        // Marco como leído 
                    //        //client.Inbox.AddFlags(uid, MessageFlags.Seen, true);
                    //        // Marco el mensaje como eliminado
                    //        client.Inbox.AddFlags(uid, MessageFlags.Deleted, false);
                    //    }
                    //Elimino el email


                    //    client.Disconnect(true);
                    //}
                    #endregion
                }
            }
            catch(ImapX.Exceptions.ServerAlertException exServer)
            {
                _Logger.ToFile("[ERROR]", exServer);
            }
            catch(ImapX.Exceptions.OperationFailedException OpFail)
            {
                _Logger.ToFile("[ERROR]", OpFail);
            }
            catch(ImapX.Exceptions.InvalidStateException InvalidState)
            {
                _Logger.ToFile("[ERROR]", InvalidState);
            }
            catch (Exception ex)
            {
                
                var a = ex;
                _Logger.ToFile(a.ToString());
            }   
        }
        #endregion
    }
}
