﻿using System;
using DragonDataSniffer.Manager;
using DragonDataSniffer.ServerHandler;
using System.Reflection;
using System.Net.Sockets;

namespace DragonDataSniffer.Network
{
    public class ServerClient : ClientBase
    {
        public GameClient CClient { get; set; }
        public ServerClient() : base()
        {

        }

        public ServerClient(string IP, int Port, GameClient pClient) : base()
        {
            Disconnected += new EventHandler<EventArgs>(Client_OnDisconnect);
            PacketReceived += new EventHandler<PacketReceivedEventArgs>(Client_OnPacket);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            pType = pClient.pType;
            CClient = pClient;
            Socket.Connect(IP, Port);
            Log.WriteLine(LogLevel.Info, "New ServerClient Starts To {0} port {1}", IP, Port);
            Start();
        }

        void Client_OnDisconnect(object sender, EventArgs e)
        {
            Log.WriteLine(LogLevel.Info, " ServerClient {0} Disconnected.", IP);
            ServerClientManager.Instance.RemovByType(pType);
        }

        void Client_OnPacket(object sender, PacketReceivedEventArgs e)
        {
            MethodInfo method = ServerHandlerStore.GetHandler(e.Packet.Header, e.Packet.Type);
            if (method != null)
            {
                Action action = ServerHandlerStore.GetCallback(method, this, e.Packet);
                Worker.Instance.AddCallback(action);
            }
            else
            {
                CClient.SendPacket(e.Packet);
            }
        }
    }
}
