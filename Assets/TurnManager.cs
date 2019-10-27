using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instancia;
    public TankManager[] misTanques;
    List<Transform> TankTransf = new List<Transform>();
    public Vector3 offset;

    int nTanks, tankIndex=0,alivePlayers;

    public float TimeToMov=1.0f;
    float ValorT=0, factorMov;

    bool turnedOffLine = false;

    LineRenderer ln;
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
            //Debug.Log(misTanques[tankIndex].EndCannon.forward);
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
        StartCoroutine(WaitASec(2.0f));
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
            ln.enabled = true;
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
}
