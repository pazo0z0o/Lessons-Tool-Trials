using RabbitMQ.Client;
using System.Text;

ConnectionFactory factory = new ConnectionFactory();
factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
factory.ClientProvidedName = "Rabbit Sender App";


IConnection con = factory.CreateConnection();
//RabbitMQ model
IModel channel = con.CreateModel();

string exchangeName = "DemoExchange";
string routingKey = "demo-key";
string queName = "DemoQueue";

//Declare an exchange
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
//Declare a queue and bind an exchange to it
channel.QueueDeclare(queName,false,false,false,null);
channel.QueueBind(queName,exchangeName,routingKey,null);

//Publish a simple message
//byte[] messageBodyInBytes = Encoding.UTF8.GetBytes("Hello there!");
//channel.BasicPublish(exchangeName, routingKey, false, null, messageBodyInBytes);

//Publish multiple messages to test the queue
for (int i = 0; i < 60; i++)
{
    Console.WriteLine($"Sending message number {i} !!");
    byte[] messageBodyInBytes = Encoding.UTF8.GetBytes($"Message #{i}");
    channel.BasicPublish(exchangeName, routingKey, false, null, messageBodyInBytes);
    //The messages should not come at the same time
    Thread.Sleep(1000);
}

Console.ReadLine();
channel.Close();
con.Close();        






















