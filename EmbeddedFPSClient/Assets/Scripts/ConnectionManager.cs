using System;
using System.Net;
using DarkRift;
using DarkRift.Client.Unity;
using UnityEngine;

[RequireComponent(typeof(UnityClient))]
public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    public delegate void OnConnectedDelegate();
    public event OnConnectedDelegate OnConnected;

    [Header("Settings")]
    [SerializeField] private string ipAdress;
    [SerializeField] private int port;

    public UnityClient Client { get; private set; }

    private void Awake()
    {
        Singleton();

        Client = GetComponent<UnityClient>();
    }

    private void Start()
    {
        Client.ConnectInBackground(IPAddress.Parse(ipAdress), port, false, ConnectCallback);
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

    private void ConnectCallback(Exception exception)
    {
        if(Client.ConnectionState == ConnectionState.Connected)
        {
            OnConnected?.Invoke();
        }
        else
        {
            Debug.LogError("Unable to connect to server.");
        }
    }
}
