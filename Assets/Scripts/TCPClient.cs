﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour
{

   
    //Queue<MsgClass> mensajesEnCola = new Queue<MsgClass>();
    MsgClass mensajeMasNuevo = null;
    string IP;
    int Puerto;
    bool abierto = true;
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread, cambiadorDePosiciones;
    #endregion

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
        ConnectToTcpServer();
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
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();

            cambiadorDePosiciones = new Thread(ProcessData2ndPlayer);
            cambiadorDePosiciones.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            //socketConnection = new TcpClient("192.168.1.85", 8000);//jojo te voy a 
            socketConnection = new TcpClient(IP, Puerto);

            Byte[] bytes = new Byte[1024];
            while (abierto)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log("del server llego " + serverMessage);
                        try
                        {
                            MsgClass clasePrueba = JsonUtility.FromJson<MsgClass>(serverMessage);
                            mensajeMasNuevo = clasePrueba;
                        }catch(Exception ex)
                        {

                        }
                        //mensajesEnCola.Enqueue(clasePrueba);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void ProcessData2ndPlayer() 
    {
        while (abierto)
        {
            checkAndChange(ref IAPos, ref IAShootPos, ref IARot, ref IAdidshoot, ref IAlive);
        }
    }

    public void checkAndChange(ref Vector3 _IAPos,ref Vector3 _IAShPos,ref Quaternion _IARot,ref bool _IAdidshoot, ref bool _alive)
    {

        if (mensajeMasNuevo != null)
        {
            Debug.Log(mensajeMasNuevo.PosicionFinal + " " + mensajeMasNuevo.PosicionDisparo);
            _IAPos = mensajeMasNuevo.PosicionFinal;
            _IAShPos = mensajeMasNuevo.PosicionDisparo;
            _IARot = mensajeMasNuevo.rotacionFinal;
            _IAdidshoot = mensajeMasNuevo.didshoot;
            _alive = mensajeMasNuevo.alive;
        }
        /*while (mensajesEnCola.Count > 0)
        {
            //Debug.Log(mensajesEnCola.Peek().PosicionFinal +" "+ mensajesEnCola.Peek().PosicionDisparo +" "+ mensajesEnCola.Peek().rotacionFinal);
            manager.SimulatePlayer(mensajesEnCola.Peek().PosicionFinal, mensajesEnCola.Peek().PosicionDisparo, mensajesEnCola.Peek().rotacionFinal, mensajesEnCola.Peek().didshoot);
            mensajesEnCola.Dequeue();
        }*/
    }



    public void trySendingMsg(MsgClass msg)
    {
        SendMessage(msg);
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 
    /// 
    private void SendMessage(MsgClass claseprueba)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                /* Esta wea es la original
                string clientMessage = "This is a message from one of your clients.";
                */
                string json = JsonUtility.ToJson(claseprueba);

                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(json);
                // Write byte array to socketConnection stream.      
               // Debug.Log("Client tryed to send msg");
                //Debug.Log(json);
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void EndThreadsComunications() 
    {
        Debug.Log("CerrandoCliente");
        abierto = false;
        clientReceiveThread.Abort();
        cambiadorDePosiciones.Abort();
    }
}
