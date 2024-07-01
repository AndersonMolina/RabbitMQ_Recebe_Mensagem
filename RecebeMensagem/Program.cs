using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

//Utilizando padrão factory
var factory = new ConnectionFactory { HostName = "localhost" };

//Cria a conexão
using var connection = factory.CreateConnection();

//criamos um canal para a transmissão das mensagens.
//Podemos criar diversos canais para serem utilizados na mesma conexão, com finalidades diferentes
using var canal = connection.CreateModel();


//Criamos uma fila para receber a mensagem que enviamos
canal.QueueDeclare(queue: "Olá",
                   durable: false,
                   exclusive: false,
                   autoDelete: false,
                   arguments: null);

//Mensagem para mostrar que o serviço está aguardando novas mensagem
Console.WriteLine(value: "[*] Aguardando novas mensagens");

//Criamos um "consumidor" de mensagem e passamos o canal de recebimento,
//no caso "canal" criado lá em cima
var consumidor = new EventingBasicConsumer(model: canal);

//Delegate onde passamos uma função para ser executada quando o evento Received ocorrer
consumidor.Received += (model, ea) =>
{
    //Transforma o que receber em um array de bytes
    var corpo = ea.Body.ToArray();
    
    //Transforma os bytes em string
    var mensagem = Encoding.UTF8.GetString(bytes: corpo); 

    //Exibe o que recebeu
    Console.WriteLine(value: $" [OK] Recebido - Mensagem: {mensagem}");
};

//Avisa o RabbitMQ que a mensagem foi recebida e que pode remover da fila
canal.BasicConsume(queue: "Olá",
                   autoAck: true,
                   consumer: consumidor);

Console.WriteLine(value: "Aperte <ENTER> para sair!");
Console.ReadLine(); 


