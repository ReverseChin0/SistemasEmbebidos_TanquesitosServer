using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TcpUiManager : MonoBehaviour
{
    public TCPServer Server;
    public TCPClient Client;

    public void HacerServer()
    {
        Server.enabled = true;
        Client.enabled = false;
    }

    public void HacerCliente()
    {
        Server.enabled = false;
        Client.enabled = true;
    }
}
