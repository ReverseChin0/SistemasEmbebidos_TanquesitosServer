using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MsgClass 
{
    public Vector3 PosicionFinal, PosicionDisparo, posFinalDisparo;
    public Quaternion rotacionFinal;

    public MsgClass(Vector3 _pos, Vector3 _posDisp, Vector3 _posDispFinal, Quaternion _rotfinal)
    {
        PosicionFinal = _pos;
        PosicionDisparo = _posDisp;
        posFinalDisparo = _posDispFinal;
        rotacionFinal = _rotfinal;
    }

    public MsgClass(Vector3 _pos, Vector3 _posDisp, Quaternion _rotfinal)
    {
        PosicionFinal = _pos;
        PosicionDisparo = _posDisp;
        rotacionFinal = _rotfinal;
    }
}
