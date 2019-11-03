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
    NavMeshAgent agente;

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

        agente = FindObjectOfType<NavMeshAgent>();
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
        Debug.Log("Cambiando... ");
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
        }
        /*else
        {
            StartCoroutine(WaitASec(0.0f));
        }*/
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
        //Debug.Log((IAEndPos - TankTransf[tankIndex].position).sqrMagnitude);
        if ((IAEndPos-TankTransf[tankIndex].position).sqrMagnitude < 2.5f)
        {
            agente.isStopped = true;
            TankTransf[tankIndex].position = IAEndPos;
            TankTransf[tankIndex].rotation = IARot;
            StartCoroutine(waitToShoot());
            monitorearIA = false;
        }
    }
    public void SimulatePlayer( Vector3 _dest, Vector3 _cannonPos, Quaternion _rotDestino, Vector3 _bulletHit)
    {
        IAEndPos = _dest;
        IAshootPos = _cannonPos;
        IARot = _rotDestino;
        IAHitPos = _bulletHit;
        agente.SetDestination(_dest);
        monitorearIA = true;
    }

   // [ContextMenu("simulate")]
    public void SimulatePlayer(Vector3 _dest, Vector3 _cannonPos, Quaternion _rotDestino)
    {
         monitorearIA = true;
         IAEndPos = _dest;
         IAshootPos = _cannonPos;
         IARot = _rotDestino;
        agente.isStopped = false;
       /* monitorearIA = true;
        IAEndPos = new Vector3(-30.0f, 1.7881393432617188e-7f, -30.0f);
        IAshootPos = new Vector3(-29.696516036987306f, 1.700000286102295e-7f, -31.264080047607423f);
        IARot = new Quaternion(2.1704217090245949e-8f, 0.9930682182312012f, -2.5688182692107377e-9f, 0.11753948032855988f);*/

        agente.SetDestination(_dest);
    }

    IEnumerator waitToShoot()
    {
        yield return new WaitForSeconds(0.4f);
        misTanques[tankIndex].Shoot();
    }
}
