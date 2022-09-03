using ApiProcess.Objetos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ApiProcess.Funciones
{
    public class EmailHelper
    {
        private readonly string _host;
        private readonly string _cuenta;
        private readonly string _alias;
        private readonly string _usuario;
        private readonly string _password;
        private readonly int _puerto;


        public EmailHelper(IConfiguration iConfiguration)
        {
            _host = iConfiguration.GetSection("SMTP").GetSection("Host").Value;
            _cuenta = iConfiguration.GetSection("SMTP").GetSection("Cuenta").Value;
            _alias = iConfiguration.GetSection("SMTP").GetSection("Alias").Value;
            _usuario = iConfiguration.GetSection("SMTP").GetSection("Usuario").Value;
            _password = iConfiguration.GetSection("SMTP").GetSection("Password").Value;
            _puerto = Int16.Parse(iConfiguration.GetSection("SMTP").GetSection("Puerto").Value);
        }

        public void SendEmail(EmailModel emailModel)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_host))
                {
                    client.Credentials = new NetworkCredential(_usuario, _password);
                    client.EnableSsl = true;
                    client.Port = _puerto;
                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => { return true; };

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(_cuenta, _alias);

                    mailMessage.BodyEncoding = Encoding.UTF8;

                    mailMessage.To.Add(emailModel.To);
                    mailMessage.Body = emailModel.Message;
                    mailMessage.Subject = emailModel.Subject;
                    mailMessage.IsBodyHtml = emailModel.IsBodyHtml;

                    client.Send(mailMessage);
                }
            }
            catch
            {
                throw;
            }

        }

    }
}
