using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Same proccess as the sender
ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Receiver2 App";


IConnection con = factory.CreateConnection();
//RabbitMQ model
IModel channel = con.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-key";
string queName = "DemoQueue";

//Declare an exchange
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
//Declare a queue and bind an exchange to it
channel.QueueDeclare(queName, false, false, false, null);
channel.QueueBind(queName, exchangeName, routingKey, null);

//Set the Quality of Service
channel.BasicQos(prefetchSize: 0, 1, false);

//Setup a consumer of the message. Has to also be decoded from its byte[] form
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(3)).Wait();
    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message Received: {message}");
    channel.BasicAck(args.DeliveryTag, false);

};

string consumerTag = channel.BasicConsume(queName, false, consumer);

Console.ReadLine();

channel.BasicCancel(consumerTag);

channel.Close();
con.Close();






