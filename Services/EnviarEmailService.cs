using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace PDF_EMAIL.Services
{
    public class EnviarEmailService: IEnviarEmail
    {
        private IConfiguration _configuration;
        public EnviarEmailService(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
         public Task<int> SendEmailAsync(string filename)
        {
            string emailTo = "leosaglia@hotmail.com";
            string subject = "Novo colaborador";
            string message = "Novo colaborador cadastrado";
            
            Execute(emailTo, subject, message, filename).Wait();
            return Task.FromResult(0);
        }

        public async Task Execute(string emailTo, string subject, string message, string filename)
        {
            string toEmail = emailTo;

            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(_configuration["EmailConfiguracoes:EmailFrom"], "Nome Remetente")
            };
            try
            {

                mail.To.Add(new MailAddress(toEmail));
                mail.CC.Add(new MailAddress(_configuration["EmailConfiguracoes:CcEmail"]));

                mail.Subject = "Envio de email teste" + subject;
                mail.Body = message;
                mail.IsBodyHtml = true;

                //outras opções
                mail.Attachments.Add(new Attachment("tmp/" + filename + ".pdf"));
                //

                using (SmtpClient smtp = new SmtpClient(_configuration["EmailConfiguracoes:Dominio"], 
                    Convert.ToInt32(_configuration["EmailConfiguracoes:Porta"])))
                {
                    smtp.Credentials = new NetworkCredential(_configuration["EmailConfiguracoes:Usuario"], 
                        _configuration["EmailConfiguracoes:Senha"]);
                    smtp.EnableSsl = true;

                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                mail.Dispose();
            }
        }
    }
}