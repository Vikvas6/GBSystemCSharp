using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SolarSystemNetworkManager : NetworkManager
{
    public void StartClientButton()
    {
        singleton.StartClient();
    }

    //��������� �������
    public void StopClientButton()
    {
        singleton.StopClient();
    }

    //����� �������
    public void StartServerButton()
    {
        singleton.StartServer();
    }

    //��������� �������
    public void StopServerButton()
    {
        singleton.StopServer();
    }

    //����� �����
    public void StartHostButton()
    {
        singleton.StartHost();
    }

    //��������� �����
    public void StopHostButton()
    {
        singleton.StopHost();
    }
}
