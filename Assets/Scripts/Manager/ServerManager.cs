using System;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using SocketCommunication;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    private Thread _thread;
    private PublisherSocket _publisherSocket;
    private SubscriberSocket _subscriberSocket;
    private ClientCommunication _clientCommunication;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("here");
        _clientCommunication = new ClientCommunication(_publisherSocket, _subscriberSocket);
        _clientCommunication.Start();
    }

    void Update()
    {
        _clientCommunication.reciveData();
    }

    private void OnDestroy()
    {
        _clientCommunication.Stop();
    }

    private void OnApplicationQuit()
    {
        _clientCommunication.Stop();
        NetMQConfig.Cleanup();
    }
}