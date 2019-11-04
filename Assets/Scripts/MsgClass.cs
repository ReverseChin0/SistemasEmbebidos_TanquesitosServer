﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MsgClass 
{
    public Vector3 PosicionFinal, PosicionDisparo;
    public Quaternion rotacionFinal;
    public bool didshoot;

    public MsgClass(Vector3 _pos, Vector3 _posDisp, Quaternion _rotfinal, bool _didshoot)
    {
        PosicionFinal = _pos;
        PosicionDisparo = _posDisp;
        rotacionFinal = _rotfinal;
        didshoot = _didshoot;

    }

    public MsgClass(Vector3 _pos, Vector3 _posDisp, Quaternion _rotfinal)
    {
        PosicionFinal = _pos;
        PosicionDisparo = _posDisp;
        rotacionFinal = _rotfinal;
        didshoot = true;
    }
}