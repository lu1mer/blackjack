using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlackjackServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }
    }

    public class Server
    {
        private TcpListener _listener;
        private List<ClientHandler> _clients = new List<ClientHandler>();
        private Game _game;

        public Server()
        {
            _game = new Game();
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, 8888);
            _listener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine("Подключен новый клиент.");
                ClientHandler clientHandler = new ClientHandler(client, this, _game, _clients.Count == 0);
                _clients.Add(clientHandler);
                Thread clientThread = new Thread(clientHandler.HandleClient);
                clientThread.Start();
            }
        }

        public void Broadcast(string message)
        {
            foreach (var client in _clients)
            {
                client.SendMessage(message);
            }
        }
    }

public class ClientHandler
{
    private TcpClient _client;
    private NetworkStream _stream;
    private Server _server;
    private Game _game;
    private bool _isPlayer;

    public ClientHandler(TcpClient client, Server server, Game game, bool isPlayer)
    {
        _client = client;
        _stream = client.GetStream();
        _server = server;
        _game = game;
        _isPlayer = isPlayer;
    }

    public void HandleClient()
    {
        byte[] buffer = new byte[1024];
        int bytesRead;

        if (_isPlayer)
        {
            SendMessage("Вы игрок. Введите команду (hit/stand):");
            SendMessage(_game.GetPlayerState());
        }
        else
        {
            SendMessage("Вы дилер. Введите команду (hit/stand):");
            SendMessage(_game.GetDealerState());
        }

        while ((bytesRead = _stream.Read(buffer, 0, buffer.Length)) != 0)
        {
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Получено: " + message);

            if (_isPlayer)
            {
                if (message.ToLower() == "hit")
                {
                    _game.PlayerHit();
                    SendMessage(_game.GetPlayerState());
                    if (_game.IsGameOver)
                    {
                        SendMessage(_game.GetGameResult());
                    }
                }
                else if (message.ToLower() == "stand")
                {
                    _game.PlayerStand();
                    SendMessage(_game.GetPlayerState());
                    SendMessage(_game.GetGameResult());
                }
            }
            else
            {
                if (message.ToLower() == "hit")
                {
                    _game.DealerHit();
                    SendMessage(_game.GetDealerState());
                    if (_game.IsGameOver)
                    {
                        SendMessage(_game.GetGameResult());
                    }
                }
                else if (message.ToLower() == "stand")
                {
                    _game.PlayerStand();
                    SendMessage(_game.GetDealerState());
                    SendMessage(_game.GetGameResult());
                }
            }
        }

        _client.Close();
    }

    public void SendMessage(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        _stream.Write(buffer, 0, buffer.Length);
    }
}
}