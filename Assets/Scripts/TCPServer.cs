using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPServer : MonoBehaviour 
{


    //Queue<MsgClass> mensajesEnCola = new Queue<MsgClass>();
    MsgClass mensajeMasNuevo = null;
    string IP;
    int Puerto;
    bool abierto = true;
    #region private members 	
    /// <summary> 	
    /// TCPListener to listen for incomming TCP connection 	
    /// requests. 	
    /// </summary> 	
    private TcpListener tcpListener;
    /// <summary> 
    /// Background thread for TcpServer workload. 	
    /// </summary> 	
    private Thread tcpListenerThread, cambiadorDePosiciones;

   

    /// <summary> 	
    /// Create handle to connected tcp client. 	
    /// </summary> 	
    private TcpClient connectedTcpClient;
    #endregion
    // Use this for initialization

    //============================DataIA====================================
    public TankManager tank;
    Transform MyTank, EndCannon;
    Vector3 IAPos, IAShootPos;
    Quaternion IARot;
    bool IAdidshoot, IAlive;
    //============================DataIA====================================
    void Start()
    {
        MyTank = tank.transform;
        EndCannon = tank.EndCannon.transform;
        IAPos = MyTank.position;
        IAShootPos = tank.EndCannon.position;
        IARot = MyTank.rotation;
        IAdidshoot = false;
        IAlive = true;
    }

    public void InitializeConnection(string _IP, int _port)
    {
        IP = _IP;
        Puerto = _port;
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();

        cambiadorDePosiciones = new Thread(ProcessData2ndPlayer);
        cambiadorDePosiciones.Start();
    }

    // Update is called once per frame
    void Update()
    {
        MyTank.position = IAPos;
        EndCannon.position = IAShootPos;
        MyTank.rotation = IARot;
        if (IAdidshoot)
        {
            tank.Shoot();
            IAdidshoot = false;
            //mensajeMasNuevo = null;
            mensajeMasNuevo = new MsgClass(IAPos, IAShootPos, IARot, IAdidshoot, IAlive);
        }
        if (!IAlive)
        {
            tank.gameObject.SetActive(false);
            //mensajeMasNuevo = null;
            mensajeMasNuevo = new MsgClass(IAPos, IAShootPos, IARot, IAdidshoot, IAlive);
        }

    }

    /// <summary> 	
    /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
    /// </summary> 	
    private void ListenForIncommingRequests()
    {
        try
        {
            // Create listener on localhost port 8052. 			
            //tcpListener = new TcpListener(IPAddress.Parse("192.168.1.82"), 82); // este es el bueno de las pruebas con el router del profe
            tcpListener = new TcpListener(IPAddress.Parse(IP), Puerto);

            tcpListener.Start();
            Debug.Log("Server is listening");
            Byte[] bytes = new Byte[1024];
            //bool received = false; 
            while (abierto)
            {
                using (connectedTcpClient = tcpListener.AcceptTcpClient())
                {
                    // Get a stream object for reading 					
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 						
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 							
                            string clientMessage = Encoding.ASCII.GetString(incommingData);
                            Debug.Log("client message received as: " + clientMessage);
                            try
                            {
                                MsgClass clasePrueba = JsonUtility.FromJson<MsgClass>(clientMessage);
                                mensajeMasNuevo = clasePrueba;
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }
            }
            
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
    }

    private void ProcessData2ndPlayer()
    {
        while (abierto)
        {
            checkAndChange(ref IAPos, ref IAShootPos, ref IARot, ref IAdidshoot, ref IAlive);
        }
    }

    public void checkAndChange(ref Vector3 _IAPos, ref Vector3 _IAShPos, ref Quaternion _IARot, ref bool _IAdidshoot, ref bool _alive)
    {
        if (mensajeMasNuevo != null)
        {
            _IAPos = mensajeMasNuevo.PosicionFinal;
            _IAShPos = mensajeMasNuevo.PosicionDisparo;
            _IARot = mensajeMasNuevo.rotacionFinal; 
            _IAdidshoot = mensajeMasNuevo.didshoot;
            _alive = mensajeMasNuevo.alive;
        }
        /*while(mensajesEnCola.Count > 0)
        {
            manager.SimulatePlayer(mensajesEnCola.Peek().PosicionFinal, mensajesEnCola.Peek().PosicionDisparo, mensajesEnCola.Peek().rotacionFinal, mensajesEnCola.Peek().didshoot);
            mensajesEnCola.Dequeue();
        }*/
    }

    public void trySendingMsg(MsgClass msg)
    {
        SendMessage(msg);
    }

   
    /// <summary> 	
    /// Send message to client using socket connection. 	
    /// </summary> 	
    private void SendMessage(MsgClass claseprueba)
    {
        if (connectedTcpClient == null)
        {
            return;
        }

        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = connectedTcpClient.GetStream();
            if (stream.CanWrite)
            {
                string json = JsonUtility.ToJson(claseprueba);
                // Convert string message to byte array.                 
                byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(json);
                // Write byte array to socketConnection stream.               
                stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                //Debug.Log("Server sent his message - should be received by client");
            }
        }

        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void EndThreadsComunications()
    {
        Debug.Log("CerrandoServer");
        abierto = false;
        tcpListenerThread.Abort();
        cambiadorDePosiciones.Abort();
    }

}
