using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MsgClass 
{
    
    public Vector3 PosicionFinal, PosicionDisparo;
    public Quaternion rotacionFinal;
    public bool didshoot, alive;

    public MsgClass(Vector3 _pos, Vector3 _posDisp, Quaternion _rotfinal, bool _didshoot, bool _alive)
    {
        PosicionFinal = _pos;
        PosicionDisparo = _posDisp;
        rotacionFinal = _rotfinal;
        didshoot = _didshoot;
        alive = _alive;
    }

    public MsgClass(Vector3 _pos, Vector3 _posDisp, Quaternion _rotfinal)
    {
        PosicionFinal = _pos;
        PosicionDisparo = _posDisp;
        rotacionFinal = _rotfinal;
        didshoot = false;
    }

    public MsgClass()
    {

        PosicionFinal = Vector3.zero;
        PosicionDisparo = Vector3.zero;
        rotacionFinal = Quaternion.identity;
        didshoot = false;
    }
}
