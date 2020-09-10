using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorInfo : MonoBehaviour
{
    // Start is called before the first frame update
    public bool moveable;
    public float health = 0f;
    public bool isFlag = false;
    public int teamId = -1;

    public  bool IsEnemy(int tId)
    {
        return (tId != -1 && teamId != -1 && teamId != tId);
    }


}
