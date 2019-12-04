using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TcpUiManager : MonoBehaviour
{

    public string IPelegida = "127.0.0.1";
    public string Puertos = "82";

    public Text Inputtexto, Puerto, IPpropia;

    public GameObject screen1, screen2;

    bool server = false;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        string ipv4 = IPManager.GetIP(ADDRESSFAM.IPv4);
        Debug.Log(ipv4);
        IPpropia.text = "Tu IP: "+ipv4;
    }

    
    public void getValuesAndGo()
    {
        if(Inputtexto.text != "" && Puerto.text!= "") 
        {
            IPelegida = Inputtexto.text;
            Puertos = Puerto.text;
        }
        
        StartCoroutine(iniciarComunicacion());
        if (server)
        {
            SceneManager.LoadScene("ServerGame");
        }
        else
        {
            SceneManager.LoadScene("ClientGame");
        }
    }

    public void menuToggler()
    {
        screen1.SetActive(!screen1.activeSelf);
        screen2.SetActive(!screen2.activeSelf);
    }

    public void setServer(bool _s)
    {
        server = _s;
    }

    public IEnumerator iniciarComunicacion()
    {
        yield return new WaitForSeconds(1.5f);
        if (server)
        {
            FindObjectOfType<TCPServer>().InitializeConnection(IPelegida, int.Parse(Puertos));
        }
        else
        {
            FindObjectOfType<TCPClient>().InitializeConnection(IPelegida, int.Parse(Puertos));
        }
        Destroy(gameObject);
    }
}
