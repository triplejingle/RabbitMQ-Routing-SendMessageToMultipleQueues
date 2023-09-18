using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Client;
using Worker;

var factory = new ConnectionFactory { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
string exchangeName = "postExchange";

WorkerAMessage aMessage = new WorkerAMessage();
Content content = new Content();
content.message = "Data met alle info";
aMessage.message = content;
aMessage.eventType = "CreatePost";
string any = JsonSerializer.Serialize(aMessage);
var body = Encoding.UTF8.GetBytes(any);

channel.BasicPublish(
    exchange: exchangeName,
    routingKey: "createpost",
    basicProperties: null,
    body: body
    );

WorkerAMessage bMessage = new WorkerAMessage();
Content bcontent = new Content();
bcontent.message = "Data met alle info";
bMessage.message = content;
bMessage.eventType = "updatePost";
string bmessageserialized = JsonSerializer.Serialize(bMessage);
var newBody = Encoding.UTF8.GetBytes(bmessageserialized);

channel.BasicPublish(
    exchange: exchangeName,
    routingKey: "updatepost",
    basicProperties: null,
    body: newBody
);
Console.WriteLine($" [x] Sent {bMessage.eventType}");

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();