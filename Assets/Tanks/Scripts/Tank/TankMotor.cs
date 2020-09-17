using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMotor : MonoBehaviour
{
    public float maxSpeed = 12f;                 // How fast the tank moves forward and back.
    public float turnSpeed = 180f;            // How fast the tank turns in degrees per second.

    private Rigidbody rigid;              // Reference used to move the tank.
    private bool stopped;

    public float speed;
    //Debug info
    public float turnInput;
    public float moveInput;

    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void Setup(TankSettings settings)
    {
        maxSpeed = settings.maxSpeed;
        turnSpeed = settings.turnSpeed;
    }

    internal void UpdateInput(float moveInput, float turnInput)
    {
        this.moveInput = moveInput;
        this.turnInput = turnInput;
    }

    public void FixedUpdate()
    {
        if (stopped)
            return;
        speed = rigid.velocity.magnitude;
        if (moveInput < 0)
            speed *= -1;
        float turn = turnInput * turnSpeed * Time.deltaTime;


        //rigid.AddForce(turnInput * turnSpeed, 0f, moveInput * maxSpeed);
        // Apply this rotation to the rigidbody's rotation.
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(0f, turn, 0f));

        ////aplly forward velocity
        rigid.velocity = transform.forward * moveInput * maxSpeed;
    }

    internal Vector3 GetVelocity()
    {
        return rigid.velocity;
    }

    internal void Reset()
    {
        rigid.isKinematic = false;
        stopped = false;

    }

    internal void Stop()
    {
        rigid.isKinematic = true;
        rigid.velocity = Vector3.zero;
        stopped = true;
    }
}
