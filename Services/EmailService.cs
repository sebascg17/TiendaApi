using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TiendaApi.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmailAsync(string toEmail, string nombre, string token);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string nombre, string token)
        {
            // En producción: usar _config["Email:SmtpServer"] etc.
            // Aquí usamos un mock o valores desde configuration. 
            // IMPORTANTE: El usuario debe configurar appsettings.json
            
            var fromEmail = _config["Email:From"] ?? "noreply@sbenix.com";
            var password = _config["Email:Password"];
            var host = _config["Email:Host"] ?? "smtp.gmail.com";
            var port = int.Parse(_config["Email:Port"] ?? "587");

            if(string.IsNullOrEmpty(password)) 
            {
                Console.WriteLine("⚠️ Email no enviado: Falta configuración SMTP en appsettings.json");
                return;
            }

            var verificationLink = $"http://localhost:4200/confirm-email?token={token}";

            // Plantilla HTML con Branding SFENIX (#08d15f)
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f3f4f6; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 40px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .header {{ background-color: #08d15f; padding: 20px; text-align: center; color: white; }}
        .header h1 {{ margin: 0; font-size: 24px; }}
        .content {{ padding: 30px; text-align: center; color: #333; }}
        .btn {{ display: inline-block; background-color: #08d15f; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold; margin-top: 20px; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #999; border-top: 1px solid #eee; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Verifica tu cuenta SBENIX</h1>
        </div>
        <div class='content'>
            <h2>¡Hola {nombre}!</h2>
            <p>Gracias por registrarte en SBENIX. Para comenzar a usar tu cuenta y activar todas las funciones, por favor confirma tu correo electrónico.</p>
            <a href='{verificationLink}' class='btn'>Confirmar Email</a>
            <p style='margin-top:20px; font-size:14px;'>O copia este enlace: <br/> <a href='{verificationLink}'>{verificationLink}</a></p>
        </div>
        <div class='footer'>
            &copy; {DateTime.Now.Year} SFENIX Platform. Todos los derechos reservados.
        </div>
    </div>
</body>
</html>";

            using var mail = new MailMessage();
            mail.From = new MailAddress(fromEmail, "SFENIX Support");
            mail.To.Add(toEmail);
            mail.Subject = "Bienvenido a SFENIX - Confirma tu correo";
            mail.Body = body;
            mail.IsBodyHtml = true;

            using var smtp = new SmtpClient(host, port);
            smtp.Credentials = new NetworkCredential(fromEmail, password);
            smtp.EnableSsl = true;

            try
            {
                await smtp.SendMailAsync(mail);
                Console.WriteLine($"✅ Correo de verificación enviado a {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando correo: {ex.Message}");
                // No lanzamos excepción para no romper el registro, pero logueamos
            }
        }
    }
}
