using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
public class GameTankManager : MonoBehaviour
{
    public static GameTankManager instancia;
    public TankManager miTanque;
    Transform TankTransf;
    public Vector3 offset;
    NavMeshAgent agente;
    public Image winner;
    public GameObject winnerScr;
    public Color[] colores;

    int alivePlayers;

    public float TimeToMov=1.0f;
    float ValorT=0, factorMov;


    LineRenderer ln;

    //============================DataIA====================================
    Vector3 IAshootPos, IAEndPos;
    Quaternion IARot;
    bool IAdidshoot;
    //============================DataIA====================================

    void Start()
    {
        instancia = this;

        TankTransf = miTanque.gameObject.transform;

        ValorT = 0;
        factorMov = 1 / TimeToMov;

        ln = GetComponent<LineRenderer>();
        ln.positionCount = 3;

        agente = FindObjectOfType<NavMeshAgent>();
    }

    private void Update()
    {
        MakeLine();

        IAPlayerBehaviour();
       
        MoveCamToPos();
    }

    public void MakeLine()
    {
        ln.SetPosition(0, miTanque.EndCannon.position);
        RaycastHit hit;
        if (Physics.Raycast(miTanque.EndCannon.position, miTanque.EndCannon.forward, out hit, 10.0f))
        {
            ln.SetPosition(1, hit.point);
            
            Vector3 newDireccion = Vector3.Reflect(miTanque.EndCannon.forward, hit.normal);
            ln.SetPosition(2, newDireccion * 5.0f+hit.point);
        }
        else
        {
            ln.SetPosition(1, miTanque.EndCannon.position + (miTanque.EndCannon.forward * 5.0f));
            ln.SetPosition(2, miTanque.EndCannon.position + miTanque.EndCannon.forward);
        }
        
    }

    public void ChangeTurn()
    {
        StartCoroutine(WaitASec(1.0f));
    }


    public IEnumerator WaitASec(float _t)
    {
        yield return new WaitForSeconds(_t);
        Debug.Log("Cambiando... ");

        

        ValorT = 0;
        if (miTanque.Thealth > 0)
        {
            miTanque.ActivateTank();

            if (!miTanque.IAPlayer)
            {
                ln.enabled = true;
            }
        }
       
    }

    void MoveCamToPos()
    {
        if (ValorT < 1.0f)
        {
            ValorT += factorMov * Time.deltaTime;
            if (ValorT >= 1.0f)
                ValorT = 1.0f;

            transform.position = Vector3.Lerp(transform.position, TankTransf.position + offset, ValorT);
        }
        else
        {
            transform.position = TankTransf.position + offset;
        }
    }

    void IAPlayerBehaviour()
    {
        /*
        if ((IAEndPos-TankTransf[tankIndex].position).sqrMagnitude < 2.5f)
        {
            agente.isStopped = true;
            TankTransf[tankIndex].position = IAEndPos;
            TankTransf[tankIndex].rotation = IARot;
            if (IAdidshoot)
            {
                StartCoroutine(waitToShoot());
            }
            else
            {
                ChangeTurn();
            }

            monitorearIA = false;
        }*/
    }
    public void SimulatePlayer( Vector3 _dest, Vector3 _cannonPos, Quaternion _rotDestino,bool didshoot)
    {
        IAEndPos = _dest;
        IAshootPos = _cannonPos;
        IARot = _rotDestino;
        IAdidshoot = didshoot;
        agente.isStopped = false;
        agente.SetDestination(_dest);
        agente.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

   // [ContextMenu("simulate")]
    public void SimulatePlayer(Vector3 _dest, Vector3 _cannonPos, Quaternion _rotDestino)
    {
        IAEndPos = _dest;
        IAshootPos = _cannonPos;
        IARot = _rotDestino;
        agente.isStopped = false;
        agente.SetDestination(_dest);
    }

    IEnumerator waitToShoot()
    {
        yield return new WaitForSeconds(0.4f);
        miTanque.Shoot();
    }

    public void CheckWinner()
    {
        Debug.Log("the winner is...");
        if (miTanque.Thealth > 0)
        {
            winnerScr.SetActive(true);
            Debug.Log("GREEEEEEEEEEN");
            winner.color = colores[0];
        }
        else
        {
            winnerScr.SetActive(true);
            Debug.Log("PURPLEEEEEEEE");
            winner.color = colores[1];
        }
    }
}
