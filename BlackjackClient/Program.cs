using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlackjackClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            client.Connect("127.0.0.1", 8888);
            client.Start();
        }
    }

    public class Client
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public void Connect(string ipAddress, int port)
        {
            _client = new TcpClient(ipAddress, port);
            _stream = _client.GetStream();
            Console.WriteLine("Подключено к серверу.");
        }

        public void Start()
        {
            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                _stream.Write(buffer, 0, buffer.Length);
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Сервер: " + message);
            }
        }
    }
}