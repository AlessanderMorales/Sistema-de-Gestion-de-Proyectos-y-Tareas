using System.Net;
using System.Net.Mail;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> EnviarEmailContrase�a(string destinatario, string nombreCompleto, string contrase�a)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"]);
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var fromEmail = smtpSettings["FromEmail"];
                var fromName = smtpSettings["FromName"];

                using var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromName);
                message.To.Add(new MailAddress(destinatario));
                message.Subject = "Bienvenido - Credenciales de Acceso";
                message.IsBodyHtml = true;
                message.Body = GenerarCuerpoEmail(nombreCompleto, destinatario, contrase�a);

                using var smtpClient = new SmtpClient(smtpHost, smtpPort);
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                // Log el error para debugging
                Console.WriteLine($"Error al enviar email: {ex.Message}");
                return false;
            }
        }

        private string GenerarCuerpoEmail(string nombreCompleto, string email, string contrase�a)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 30px auto;
                            background-color: #ffffff;
                            border-radius: 10px;
                            box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                            overflow: hidden;
                        }}
                        .header {{
                            background: linear-gradient(45deg, #D32F2F, #FF5722);
                            color: white;
                            padding: 30px;
                            text-align: center;
                        }}
                        .content {{
                            padding: 30px;
                            color: #333;
                        }}
                        .credentials {{
                            background-color: #f8f9fa;
                            border-left: 4px solid #FF5722;
                            padding: 20px;
                            margin: 20px 0;
                            border-radius: 5px;
                        }}
                        .credential-item {{
                            margin: 10px 0;
                            font-size: 14px;
                        }}
                        .credential-label {{
                            font-weight: bold;
                            color: #555;
                        }}
                        .credential-value {{
                            color: #D32F2F;
                            font-family: 'Courier New', monospace;
                            font-size: 16px;
                        }}
                        .footer {{
                            background-color: #f8f9fa;
                            padding: 20px;
                            text-align: center;
                            color: #666;
                            font-size: 12px;
                        }}
                        .warning {{
                            background-color: #fff3cd;
                            border: 1px solid #ffc107;
                            padding: 15px;
                            margin: 20px 0;
                            border-radius: 5px;
                            color: #856404;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>�Bienvenido al Sistema!</h1>
                        </div>
                        <div class='content'>
                            <p>Hola <strong>{nombreCompleto}</strong>,</p>
                            <p>Tu cuenta ha sido creada exitosamente en nuestro Sistema de Gesti�n de Proyectos y Tareas.</p>
                            
                            <div class='credentials'>
                                <div class='credential-item'>
                                    <span class='credential-label'>Usuario (Email):</span><br/>
                                    <span class='credential-value'>{email}</span>
                                </div>
                                <div class='credential-item'>
                                    <span class='credential-label'>Contrase�a temporal:</span><br/>
                                    <span class='credential-value'>{contrase�a}</span>
                                </div>
                            </div>

                            <div class='warning'>
                                <strong>?? Importante:</strong><br/>
                                Por seguridad, te recomendamos cambiar tu contrase�a despu�s del primer inicio de sesi�n.
                                Nunca compartas tus credenciales con nadie.
                            </div>

                            <p>Puedes iniciar sesi�n en el sistema con estas credenciales.</p>
                            <p>Si tienes alguna pregunta o necesitas ayuda, no dudes en contactar con el administrador del sistema.</p>
                        </div>
                        <div class='footer'>
                            <p>Este es un correo autom�tico, por favor no respondas a este mensaje.</p>
                            <p>&copy; 2024 Sistema de Gesti�n de Proyectos y Tareas</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}
