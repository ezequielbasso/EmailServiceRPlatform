using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.IO;

namespace BD
{
    public class Insert
    {
        public bool Received(received_emails mail)
        {
            bool ret = false;
            try
            {
                using (var db = new RPEntities())
                {
                    if (mail != null)
                    {
                        db.received_emails.Add(mail);
                    }

                    ret = db.SaveChanges() > 0;
                }
                return ret;
            }
            catch (Exception)
            {
                return ret;
            }


        }

        public bool Attachments(attachments Attachment, Guid email_guid)
        {
            bool ret = false;
            try
            {
                using (var db = new RPEntities())
                {
                    if (Attachment != null)
                    {
                        var mail_ID = db.received_emails.Where(x => x.received_email_GUID == email_guid).SingleOrDefault();
                        Attachment.received_emails_id = mail_ID.received_emails_id;
                        Attachment.status_attachment_id = 1;
                        db.attachments.Add(Attachment);
                    }

                    ret = db.SaveChanges() > 0;
                }
                return ret;
            }
            catch (Exception)
            {
                return ret;
            }
        }


        public bool Attachments_page(Attachments_pages attachment_page, string attachment_guid)
        {
            bool ret = false;
            try
            {
                using (var db = new RPEntities())
                {
                    if (attachment_page != null)
                    {
                        var attachment_ID = db.attachments.Where(x => x.attachment_GUID == attachment_guid).SingleOrDefault();
                        attachment_page.Attachment_id = attachment_ID.attachment_id;
                        db.Attachments_pages.Add(attachment_page);
                    }

                    ret = db.SaveChanges() > 0;
                }
                return ret;
            }
            catch (Exception)
            {
                return ret;
            }
        }

    }
}
