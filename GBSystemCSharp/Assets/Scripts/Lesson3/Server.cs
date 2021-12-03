using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class Server : MonoBehaviour
{
    private const int MAX_CONNECTION = 10;

    private int port = 5805;

    private int hostID;
    private int reliableChannel;

    private bool isStarted = false;
    private byte error;

    List<int> connectionIDs = new List<int>();
    private Dictionary<int, string> _userNames = new Dictionary<int, string>();

    public void StartServer()
    {        
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);

        HostTopology topology = new HostTopology(cc, MAX_CONNECTION);        
        hostID = NetworkTransport.AddHost(topology, port);

        isStarted = true;
    }

    void Update()
    {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelId;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

        while (recData != NetworkEventType.Nothing)
        {
            switch (recData)
            {
                case NetworkEventType.Nothing:
                    break;

                case NetworkEventType.ConnectEvent:
                    connectionIDs.Add(connectionId);
                    break;

                case NetworkEventType.DataEvent:
                    string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    Debug.Log($"{message}.");
                    switch (ParseMessage(message, out message))
                    {
                        case MessageTypes.UN:
                            _userNames[connectionId] = message;
                            SendMessageToAll($"Player {message} has connected.");
                            Debug.Log($"Player {message} has connected.");
                            break;
                        case MessageTypes.IM:
                            SendMessageToAll($"Player {_userNames[connectionId]}: {message}");
                            Debug.Log($"Player {_userNames[connectionId]}: {message}");
                            break;
                        default:
                            Debug.Log("Wrong message type");
                            break;
                    }
                    break;

                case NetworkEventType.DisconnectEvent:
                    connectionIDs.Remove(connectionId);

                    SendMessageToAll($"Player {_userNames[connectionId]} has disconnected.");
                    Debug.Log($"Player {_userNames[connectionId]} has disconnected.");
                    break;

                case NetworkEventType.BroadcastEvent:
                    break;

            }

            recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);
        }
    }

    public void ShutDownServer()
    {
        if (!isStarted)
            return;

        NetworkTransport.RemoveHost(hostID);
        NetworkTransport.Shutdown();
        isStarted = false;
    }

    public void SendMessage(string message, int connectionID)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(message);
        NetworkTransport.Send(hostID, connectionID, reliableChannel, buffer, message.Length * sizeof(char), out error);
        if ((NetworkError)error != NetworkError.Ok)
            Debug.Log((NetworkError)error);
    }

    public void SendMessageToAll(string message)
    {
        for (int i = 0; i < connectionIDs.Count; i++)        
            SendMessage(message, connectionIDs[i]);        
    }

    private MessageTypes ParseMessage(string message, out string user_message)
    {
        user_message = message.Substring(2);
        switch (message.Substring(0, 2))
        {
            case "UN":
                return MessageTypes.UN;
            case "IM":
                return MessageTypes.IM;
            default:
                return MessageTypes.IM;
        }
    }
}

public enum MessageTypes
{
    UN,
    IM
}
