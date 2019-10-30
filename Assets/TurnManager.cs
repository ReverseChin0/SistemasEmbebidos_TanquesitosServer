using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instancia;
    public TankManager[] misTanques;
    List<Transform> TankTransf = new List<Transform>();
    public Vector3 offset;


    int nTanks, tankIndex=0,alivePlayers;

    public float TimeToMov=1.0f;
    float ValorT=0, factorMov;

    bool turnedOffLine = false, monitorearIA;

    LineRenderer ln;

    //============================DataIA====================================
    Vector3 IAshootPos, IAEndPos, IAHitPos;
    Quaternion IARot;
    //============================DataIA====================================

    void Start()
    {
        instancia = this;

        nTanks = misTanques.Length;
        alivePlayers = nTanks;
        misTanques[tankIndex].ActivateTank();

        foreach(TankManager t in misTanques)
        {
            TankTransf.Add(t.gameObject.transform);
        }

        ValorT = 0;
        factorMov = 1 / TimeToMov;

        ln = GetComponent<LineRenderer>();
        ln.positionCount = 3;
    }

    private void Update()
    {
        if (!turnedOffLine)
        {
            MakeLine();
        }

        if (monitorearIA)
        {
            IAPlayerBehaviour();
        }
        MoveCamToPos();
    }

    public void MakeLine()
    {
        ln.SetPosition(0, misTanques[tankIndex].EndCannon.position);
        RaycastHit hit;
        if (Physics.Raycast(misTanques[tankIndex].EndCannon.position, misTanques[tankIndex].EndCannon.forward, out hit, 10.0f))
        {
            ln.SetPosition(1, hit.point);
            
            Vector3 newDireccion = Vector3.Reflect(misTanques[tankIndex].EndCannon.forward, hit.normal);
            ln.SetPosition(2, newDireccion * 5.0f+hit.point);
        }
        else
        {
            ln.SetPosition(1, misTanques[tankIndex].EndCannon.position + (misTanques[tankIndex].EndCannon.forward * 5.0f));
            ln.SetPosition(2, misTanques[tankIndex].EndCannon.position + misTanques[tankIndex].EndCannon.forward);
        }
        
    }

    public void ChangeTurn()
    {
        StartCoroutine(WaitASec(1.0f));
    }

    public void turnOffLine(bool state)
    {
        turnedOffLine = state;
        ln.enabled = !state;
    }
    public IEnumerator WaitASec(float _t)
    {
        yield return new WaitForSeconds(_t);
        tankIndex++;

        if (tankIndex >= nTanks)
        {
            tankIndex = 0;
        }

        ValorT = 0;
        if (misTanques[tankIndex].Thealth > 0)
        {
            Debug.Log("CurrentTank " + tankIndex);
            misTanques[tankIndex].ActivateTank();
            turnedOffLine = false;
            if (!misTanques[tankIndex].IAPlayer)
            {
                ln.enabled = true;
            }
            else
            {
                monitorearIA = true;
            }
            
        }
        else
        {

            StartCoroutine(WaitASec(0.0f));
        }
        
    }

    void MoveCamToPos()
    {
        if (ValorT < 1.0f)
        {
            ValorT += factorMov * Time.deltaTime;
            if (ValorT >= 1.0f)
                ValorT = 1.0f;

            transform.position = Vector3.Lerp(transform.position, TankTransf[tankIndex].position + offset, ValorT);
        }
        else
        {
            transform.position = TankTransf[tankIndex].position + offset;
        }
    }

    void IAPlayerBehaviour()
    {
        if ((IAEndPos-TankTransf[tankIndex].position).sqrMagnitude < 1.5f)
        {
            misTanques[tankIndex].gameObject.GetComponent<NavMeshAgent>().isStopped=true;
            TankTransf[tankIndex].position = IAEndPos;
            TankTransf[tankIndex].rotation = IARot;
            misTanques[tankIndex].Shoot();
            monitorearIA = false;
        }
    }

    public void SimulatePlayer( Vector3 _dest, Vector3 _cannonPos, Quaternion _rotDestino, Vector3 _bulletHit)
    {
        IAEndPos = _dest;
        IAshootPos = _cannonPos;
        IARot = _rotDestino;
        IAHitPos = _bulletHit;
        misTanques[tankIndex].GetComponent<NavMeshAgent>().SetDestination(_dest);
    }

    public void SimulatePlayer( Vector3 _dest, Vector3 _cannonPos, Quaternion _rotDestino)
    {
        IAEndPos = _dest;
        IAshootPos = _cannonPos;
        IARot = _rotDestino;
        misTanques[tankIndex].GetComponent<NavMeshAgent>().SetDestination(_dest);
    }
}
