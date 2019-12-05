using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class TankManager : MonoBehaviour
{
    Collider miColi;
    Rigidbody RigBo;
    Vector3 Direccion = Vector3.zero;
    public Transform EndCannon;
    Quaternion rotation = Quaternion.identity;
    public float velocidad = 3.0f;

    float anguloGiro = 0.0f;

    public GameObject Bullet;

    public TCPClient myClient;
    public TCPServer myServer;

    public Image fuelImage;
    float Combustible = 10.0f;
    bool hasFuel = true;

    [Range(0.1f, 2f)]
    public float gastoFuel = 0.1f;

    public bool IAPlayer = false, isServer = false;
    public int playerID = 0, Thealth = 100;

    float Timer = 0.0f, interval=0.06f, newtime=1.0f;

    void Start()
    {
        miColi = GetComponent<Collider>();
        RigBo = GetComponent<Rigidbody>();
        Stop();

        if (isServer && !IAPlayer)
        {
            myServer = FindObjectOfType<TCPServer>();
        }
        else if(!isServer && !IAPlayer)
        {
            myClient = FindObjectOfType<TCPClient>();
        }
        //StartCoroutine(SendMssgs());
    }

    void Update()
    {
        if (!IAPlayer && hasFuel)
        {
            TankMovimiento();
            //SendMssgToBoss(new MsgClass(transform.position, EndCannon.position, transform.rotation, didShoot));
            if (Combustible <= 0)
            {
                hasFuel = false;
                fuelImage.color = new Color(1.0f, 0.3f, 0.5f);
                Stop();
            }
            else
            {
                RegenerateFuel();
            }
            Timer += Time.deltaTime;
            if (Timer >= newtime)
            {
                newtime = Timer + interval;
                SendMssgToBoss(new MsgClass(transform.position, EndCannon.position, transform.rotation, false , true));
            }
        }
        else if(!hasFuel)
        {
            RegenerateFuel();
            CheckIfFull();
        }
    }

    private void FixedUpdate()
    {
        RigBo.MovePosition(transform.localPosition + Direccion * Time.deltaTime * velocidad);
    }

    void Forward()
    {
        Direccion = transform.forward;
    }

    void Back()
    {
        Direccion = -transform.forward;
    }

    public void Shoot()
    {
        //TurnManager.instancia.turnOffLine(true);
        GameObject Go = Instantiate(Bullet, EndCannon.position, transform.rotation);
        Go.GetComponent<Bullet>().Shoot(EndCannon.forward,this);
       //Stop();
        
        if (!IAPlayer)
        {
            newtime = Timer + interval;
            SendMssgToBoss(new MsgClass(transform.position, EndCannon.position, transform.rotation, true, true));
        }
        
    }

    void Stop()
    {
        Direccion = Vector3.zero;
    }

    void Rotate()
    {
        transform.Rotate(0, anguloGiro * Time.deltaTime, 0);
    }

    public void TankMovimiento()
    {
        Stop();
        anguloGiro = 0;

        if (Input.GetKey(KeyCode.W))
        {
            Forward();
            Combustible -= gastoFuel * 1.5f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Back();
            Combustible -= gastoFuel * 1.5f * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            anguloGiro = 50.0f;
            Combustible -= gastoFuel * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            anguloGiro = -50.0f;
            Combustible -= gastoFuel * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Combustible >= 3.5f)
            {
                Shoot();
                Combustible -= 5.5f;
                if (Combustible < 0)
                {
                    Combustible = -0.1f;
                }
            }
        }

        Rotate();
        fuelImage.fillAmount = Combustible * 0.1f;
    }

    public void RegenerateFuel() 
    {
        if (Combustible < 10.0f)
        {
            Combustible += gastoFuel * 0.5f * Time.deltaTime;
        }

        if (!hasFuel)
        {
            Combustible += gastoFuel * Time.deltaTime;
        }
    }

    public void CheckIfFull()
    {
        fuelImage.fillAmount = Combustible * 0.1f;
        if (Combustible >= 10.0f)
        {
            hasFuel = true;
            fuelImage.color = new Color(0.4154f, 1.0f, 0.3537736f);
        }
    }

    public void ActivateTank()
    {
        hasFuel = true;
        Combustible = 10.0f;
        fuelImage.fillAmount = 1;
        //didShoot = false;
    }

    public void TakeDMG(int dmg)
    {
        if (!IAPlayer)
        {
            Thealth = Thealth - dmg;
            RigBo.velocity = Vector3.zero;
            Debug.Log("Soy " + playerID + " y mi salud es " + Thealth);
            if (Thealth <= 0)
            {
                Die();
                newtime = Timer + interval;
                SendMssgToBoss(new MsgClass(transform.position, EndCannon.position, transform.rotation, true, false));
            }
        }
    }

    void Die()
    {
        GameTankManager.instancia.CheckWinner(IAPlayer);
        gameObject.SetActive(false);
    }
    
    /*IEnumerator SendMssgs()
    {
        yield return new WaitForSeconds(0.05f);
        SendMssgToBoss(new MsgClass(transform.position, EndCannon.position, transform.rotation, false));
        StartCoroutine(SendMssgs());
    }*/

    void SendMssgToBoss(MsgClass _msg)
    {
        if (isServer)
        {
            myServer.trySendingMsg(_msg);
        }
        else
        {
            myClient.trySendingMsg(_msg);
        }
    }
}
