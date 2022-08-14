using System;
using System.Threading;
using Json.JsonHelper;
using Json.JsonTemplates;
using NetMQ;
using NetMQ.Sockets;
using UnityEngine;

namespace SocketCommunication
{
    public class ClientCommunication
    {
        private PublisherSocket _publisher;
        private Thread _mainThread;
        private bool _isAllowedToSend;
        private string _message;
        private SubscriberSocket _subscriberSocket;
        private bool _isAllowedToRecive;

        public ClientCommunication(PublisherSocket publisher, SubscriberSocket subscriberSocket)
        {
            this._subscriberSocket = subscriberSocket;
            this._publisher = publisher;
            Debug.Log("ServerSocket created");
        }

        public void Start()
        {
            _isAllowedToRecive = true;
            _isAllowedToSend = false;
            _mainThread = new Thread(new ThreadStart(SocketImplement));
            _mainThread.Start();
        }

        private void SocketImplement()
        {
            AsyncIO.ForceDotNet.Force();
            Debug.Log("Server Starting !!!");
            _subscriberSocket = new SubscriberSocket(">tcp://127.0.0.1:5555");
            _subscriberSocket.Subscribe("Client");
            _publisher = new PublisherSocket(">tcp://127.0.0.1:6666");
            while (true)
            {
                if (_isAllowedToSend)
                {
                    _publisher.SendMoreFrame("Server").SendFrame(_message);
                    _isAllowedToSend = false;
                }

                if (_isAllowedToRecive)
                {
                    _subscriberSocket.TryReceiveFrameString(out string frameString);
                    if (!String.IsNullOrEmpty(frameString))
                    {
                        if (!frameString.Equals("Client"))
                        {
                            Player[] players = JsonHelper.FromJson<Player>(frameString);
                            Debug.Log(players[0].positionx);
                        }

                        _publisher.SendMoreFrame("Server").SendFrame("data");
                        _isAllowedToRecive = false;
                    }
                }
            }
        }

        public void SendData(String message)
        {
            _isAllowedToSend = true;
            this._message = message;
        }
        public void reciveData()
        {
            _isAllowedToRecive = true;
        }


        public void Stop()
        {
            _publisher.Dispose();
            _subscriberSocket.Close();
            _subscriberSocket.Dispose();
            _mainThread.Abort();
            NetMQConfig.Cleanup();
        }
    }
}

