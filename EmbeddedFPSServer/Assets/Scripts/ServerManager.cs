using DarkRift;
using DarkRift.Server;
using DarkRift.Server.Unity;
using System.Collections.Generic;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance;

    private XmlUnityServer xmlServer;
    private DarkRiftServer server;

    public Dictionary<ushort, ClientConnection> Players = new Dictionary<ushort, ClientConnection>();
    public Dictionary<string, ClientConnection> PlayersByName = new Dictionary<string, ClientConnection>();

    private void Awake()
    {
        Singleton();
    }

    private void Start()
    {
        xmlServer = GetComponent<XmlUnityServer>();
        server = xmlServer.Server;
        server.ClientManager.ClientConnected += OnClientConnected;
        server.ClientManager.ClientDisconnected += OnClientDisconnectd;
    }

    private void Singleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnClientConnected(object sender, ClientConnectedEventArgs e)
    {
        e.Client.MessageReceived += OnMessage;
    }

    private void OnClientDisconnectd(object sender, ClientDisconnectedEventArgs e)
    {
        e.Client.MessageReceived -= OnMessage;
    }

    private void OnMessage(object sender, MessageReceivedEventArgs e)
    {
        IClient client = (IClient)sender;

        using(Message message = e.GetMessage())
        {
            switch((Tags)message.Tag)
            {
                case Tags.LoginRequest:
                    OnClientLogin(client, message.Deserialize<LoginRequestData>());
                    break;
            }
        }
    }

    private void OnClientLogin(IClient client, LoginRequestData data)
    {
        if(PlayersByName.ContainsKey(data.Name))
        {
            using (Message message = Message.CreateEmpty((ushort)Tags.LoginResquestDenied))
            {
                client.SendMessage(message, SendMode.Reliable);
            }
            return;
        }

        client.MessageReceived -= OnMessage;

        new ClientConnection(client, data);
    }

    private void OnDestroy()
    {
        server.ClientManager.ClientConnected -= OnClientConnected;
        server.ClientManager.ClientDisconnected -= OnClientDisconnectd;
    }
}
