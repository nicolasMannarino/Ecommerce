using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Ecommerce.Servicio.Contrato;
using Ecommerce.DTO;
//using System.Net.Mail;
//using System.Net;
//using SendGrid;
//using SendGrid.Helpers.Mail;
using MailKit.Net.Smtp;
using MimeKit;

namespace Ecommerce.Servicio.Implementacion
{
    public class CorreoServicio : ICorreoServicio
    {
        private readonly IModel _channel;
        /*private readonly string _fromAddress = "mannarinonico@gmail.com";
        private readonly string _fromPassword = "Mannarino98";
        private const string _smtpHost = "smtp.gmail.com";
        private const int _smtpPort = 587;*/

        public CorreoServicio()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: "cola_registro_usuario", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        /*public async Task EnviarCorreoBienvenida(string destinatario, string nombreUsuario, string mensaje)
        {
            var fromMailAddress = new MailAddress(_fromAddress, "MANIES SRL.");
            var toMailAddress = new MailAddress(destinatario);

            using (var smtp = new SmtpClient
            {
                Host = _smtpHost,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMailAddress.Address, _fromPassword)
            })
            using (var mailMessage = new MailMessage(fromMailAddress, toMailAddress)
            {
                Subject = "¡Bienvenido a MANIES SRL!",
                Body = $"<h1>Bienvenido, {nombreUsuario}!</h1><p>{mensaje}</p>",
                IsBodyHtml = true
            })
            {
                await smtp.SendMailAsync(mailMessage);
            }
        }*/
        /*public async Task EnviarCorreoBienvenida(string destinatario, string asunto, string mensaje)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("mannarinonico@gmail.com", "MANIES SRL.");
            var to = new EmailAddress(destinatario);
            var plainTextContent = mensaje;
            var htmlContent = $"<p>{mensaje}</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, asunto, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error enviando el correo: {response.StatusCode}");
            }
        }*/

        public async Task EnviarCorreoBienvenida(string destinatario, string asunto, string mensaje)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("MANIES SRL.", "mannarinonico@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", destinatario));
            emailMessage.Subject = asunto;
            emailMessage.Body = new TextPart("html") { Text = mensaje };

            
            using (var client = new SmtpClient())
            {
                // Conectar al servidor SMTP de Gmail
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                client.Authenticate("mannarinonico@gmail.com", "nbjh ipza xytc uyer"); 

                await client.SendAsync(emailMessage);
                client.Disconnect(true);
            }
        }

        
        /*public void Iniciar()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var usuario = JsonSerializer.Deserialize<UsuarioDTO>(message);

                if (usuario != null)
                {
                    string mensajeBienvenida = "Gracias por unirte a nuestra plataforma. Estamos felices de tenerte con nosotros.";
                    await EnviarCorreoBienvenida(usuario.Correo, usuario.Nombre, mensajeBienvenida);
                }
            };
            _channel.BasicConsume(queue: "cola_registro_usuario", autoAck: true, consumer: consumer);
        }*/
    }
}
