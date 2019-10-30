using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public Image fuelImage;
    float Combustible = 10.0f;
    bool hasFuel = true, activeTank = false;

    [Range(0.1f, 2f)]
    public float gastoFuel = 0.1f;

    public bool IAPlayer = false, didShoot=false;
    public int playerID = 0, Thealth = 100;

    void Start()
    {
        miColi = GetComponent<Collider>();
        RigBo = GetComponent<Rigidbody>();
        Stop();
    }

    void Update()
    {
        if (!IAPlayer && hasFuel && activeTank)
        {
            TankMovimiento();
            if (Combustible <= 0)
            {
                hasFuel = false;
                activeTank = false;
                Stop();
                if (!didShoot)
                {
                    TurnManager.instancia.ChangeTurn();
                    SendTroughClient();
                }
            }
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

    void Shoot()
    {
        GameObject Go = Instantiate(Bullet, EndCannon.position, transform.rotation);
        Go.GetComponent<Bullet>().Shoot(EndCannon.forward,this);
    }

    void Stop()
    {
        Direccion = Vector3.zero;
    }

    void Rotate()
    {
        transform.Rotate(0, anguloGiro, 0);
    }

    public void TankMovimiento()
    {
        Stop();
        anguloGiro = 0;

        if (Input.GetKey(KeyCode.W))
        {
            Forward();
            Combustible -= gastoFuel * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            Back();
            Combustible -= gastoFuel * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            anguloGiro = 1.5f;
            Combustible -= gastoFuel * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            anguloGiro = -1.5f;
            Combustible -= gastoFuel * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
            Combustible = 0;
            TurnManager.instancia.turnOffLine(true);
        }

        Rotate();
        fuelImage.fillAmount = Combustible * 0.1f;
    }

    public void ActivateTank()
    {
        activeTank = true;
        hasFuel = true;
        Combustible = 10.0f;
        fuelImage.fillAmount = 1;
        didShoot = true;
    }

    public void TakeDMG(int dmg)
    {
        Thealth = Thealth - dmg;
        Debug.Log("Im " + playerID + "and my health is" + Thealth);
        if (Thealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        TurnManager.instancia.ChangeTurn();
        //TurnManager.instancia.alivePlayers--;
        gameObject.SetActive(false);
    }

    public void SendTroughClient()
    {
        TCPClient tcp = FindObjectOfType<TCPClient>();

        tcp.clasePrueba.Health = Thealth;
        tcp.clasePrueba.PosicionFinal = transform.position;
        tcp.clasePrueba.rotacionFinal = transform.rotation;

        tcp.clasePrueba.PosicionDisparo = EndCannon.position;

        tcp.trySendingMsg();
    }

    public void SendTroughClient(Vector3 DisparoFinal)
    {
        TCPClient tcp = FindObjectOfType<TCPClient>();

        tcp.clasePrueba.Health = Thealth;
        tcp.clasePrueba.PosicionFinal = transform.position;
        tcp.clasePrueba.rotacionFinal = transform.rotation;

        tcp.clasePrueba.PosicionDisparo = EndCannon.position;
        tcp.clasePrueba.posFinalDisparo = DisparoFinal;

        tcp.trySendingMsg();
    }
}
