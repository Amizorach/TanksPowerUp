//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Flag : SensorInfo
//{
//    public Renderer rend;
//    public Color color;
//    public Color baseColor;

//    internal void Awake()
//    {
//        rend = GetComponentInChildren<Renderer>();

//        isFlag = true;
//        teamId = -1;
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        MultiBrainAgent ta = other.GetComponent<MultiBrainAgent>();
//        if (ta == null)
//            return;
//        color = ta.tank.baseColor;
//        color.a = 0.2f;
//        rend.material.color = color;
//        if (teamId != ta.teamId)
//        {
//            teamId = ta.teamId;
//            ta.OnCapturedFlag();
//        }
//    }

//    internal void Reset()
//    {
//        rend = GetComponentInChildren<Renderer>();
//        teamId = -1;
//        color = baseColor;
//        rend.material.color = color;
//    }
//}
