using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Worker;


var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
string ExchangeName = "postExchange";
channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
string queuename = "queueB"; 
channel.QueueDeclare(queue: queuename,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);
channel.QueueBind(queue: queuename,
    exchange: ExchangeName,
    routingKey: "createpost");
channel.QueueBind(queue: queuename,
    exchange: ExchangeName,
    routingKey: "updatepost");
var consumer = new EventingBasicConsumer(channel);

consumer.Received += (IModel, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    WorkerAMessage workerAMessage = JsonSerializer.Deserialize<WorkerAMessage>(message)!;
    Console.WriteLine($" [x] Received {workerAMessage.eventType}");
};
channel.BasicConsume(queue: queuename,
    autoAck: true,
    consumer: consumer);
Console.WriteLine(consumer.ConsumerTags);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();