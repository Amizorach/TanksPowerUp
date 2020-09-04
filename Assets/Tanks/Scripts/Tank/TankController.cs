using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(TankMotor))]

public class TankController : MonoBehaviour
{
    private TankMotor motor;

    private TankSettings settings;

    public Color baseColor;

    private Color color;
    public bool dead;


    private TanksAreaBase area;
    public float health = 1f;
    public float energy = 1f;

  

    private Vector3 lastPosition;
    //Debug info
    public float currentDistance;
    public float totalDistance;

    private Slider slider;

    void Awake()
    {
        motor = GetComponent<TankMotor>();
        slider = GetComponentInChildren<Slider>();
        ConfigSetup(new TankSettings());
    }

    public void Start()
    {
        //area = GetComponentInParent<TanksAreaBase>();
    }

 
    public void ConfigSetup(TankSettings ts)
    {
        settings = ts;
        GetComponent<SphereCollider>().radius = settings.triggerRadius;
    }

    public void SetBaseColor(Color c)
    {
        baseColor = c;
    }

    internal void ResetColor()
    {
        SetColor(UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 1f, 1f));

    }

    public void SetColor(Color c)
    {
        color = c;
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        // Go through all the renderers...
        for (int i = 0; i < renderers.Length; i++)
        {
            // ... set their material color to the color specific to this tank.
            renderers[i].material.color = color;
        }
    }

    public void FixedUpdate()
    {
        if (IsDead())
            return;
        UpdateEnergy();
        CalcDistance();
    }

    private void CalcDistance()
    {
        if (lastPosition == Vector3.zero)
        {
            lastPosition = transform.position;
            return;
        }
        currentDistance += Vector3.Distance(lastPosition, transform.position);
    }

    public float GetCurrentDistance()
    {
        totalDistance += currentDistance;
        float d = currentDistance;
        currentDistance = 0f;
        return d;
    }

    public float GetTotalDistance()
    {
        GetCurrentDistance();
        return totalDistance;
    }

    private void UpdateEnergy() {
        energy -= settings.energyUsage * Time.fixedDeltaTime;
        // Set the slider's value appropriately.
        slider.value = energy;//
    }


    public void OnEpisodeBegin(TankSettings tankSettings)
    {
        if (tankSettings != null)
            ConfigSetup(tankSettings);
        gameObject.SetActive(true);
        dead = false;
        energy = settings.maxEnergy;
        slider.value = energy;
        totalDistance = 0f;
        lastPosition = Vector3.zero;
        motor.Reset();
        ResetColor();
    }

    internal bool IsDead()
    {
        if (dead)
            return true;
        if (energy < settings.minEnergy)
        {
            Die();
            return true;
        }
        return false;
    }

    public void OnEpisodeEnd()
    {
        Die();
    }
  
    public void Die()
    {
        dead = true;
        motor.Stop();
        SetColor(Color.black);
    }

    internal void RechargeEnergy()
    {
        energy = Mathf.Min(energy + settings.energyRecharge, settings.maxEnergy);
    }

    internal void OnActionReceived(float moveReq, float turnReq)
    {
        motor.UpdateInput(moveReq, turnReq);
    }

    internal Vector3 GetVelocity()
    {
        return motor.GetVelocity();
    }

    internal float GetTurnInput()
    {
        return motor.turnInput;
    }

    internal float GetMoveInput()
    {
        return motor.moveInput;
    }

    internal float GetSpeed()
    {
        return motor.speed;
    }
    //Used for testing the motor with no agent
    //public void Update()
    //{
    //    OnActionReceived(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    //}
}
