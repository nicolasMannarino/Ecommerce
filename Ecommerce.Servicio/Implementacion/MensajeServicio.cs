using Ecommerce.Servicio.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text.Json;
using RabbitMQ.Client.Exceptions;


namespace Ecommerce.Servicio.Implementacion
{
    public class MensajeServicio : IMensajeServicio
    {
        private readonly ConnectionFactory _factory;

        public MensajeServicio()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
        }
        public async Task EnviarMensajeAsync(string cola, string mensaje)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: cola, durable: false, exclusive: false, autoDelete: false, arguments: null);
                var body = Encoding.UTF8.GetBytes(mensaje);

                channel.BasicPublish(exchange: "", routingKey: cola, basicProperties: null, body: body);
            }
            await Task.CompletedTask;
        }
    }
}
