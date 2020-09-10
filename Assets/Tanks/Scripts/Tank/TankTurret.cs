using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTurret : MonoBehaviour
{

    public float turnSpeed = 180f;            // How fast the tank turns in degrees per second.

    //Debug info
    public float turn;
    public float turnInput;
    public float inputForce;

    private bool stopped;

    public void Awake()
    {
    }

    public void Setup(TankSettings settings)
    {
        turnSpeed = settings.turretTurnSpeed;
    }

    public void Reset()
    {
        stopped = false;
        transform.Rotate(new Vector3(0f, 0f, 0f));
        turnInput = 0f;
    }

    internal void UpdateInput(float tInput)
    {
        turnInput = tInput;
    }

    public void FixedUpdate()
    {
        if (stopped)
            return;
        Turn();
    }

    public void Turn()
    {
        turn = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0f, turn, 0f));
    }

   

    internal void Stop()
    {
        stopped = true;
    }

   

}

