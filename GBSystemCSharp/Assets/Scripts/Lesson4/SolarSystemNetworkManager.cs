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

    //Остановка клиента
    public void StopClientButton()
    {
        singleton.StopClient();
    }

    //Старт сервера
    public void StartServerButton()
    {
        singleton.StartServer();
    }

    //Остановка сервера
    public void StopServerButton()
    {
        singleton.StopServer();
    }

    //Старт хоста
    public void StartHostButton()
    {
        singleton.StartHost();
    }

    //Остановка хоста
    public void StopHostButton()
    {
        singleton.StopHost();
    }
}
