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
    public TankTurret turret;

    private TankSettings settings;

  

    public Color baseColor;

    private Color color;
    public bool dead;

    MultiBrainAgent agent;

    private TanksAreaBase area;
    public float health = 1f;
    public float energy = 1f;
    private float shellCoolDown;

    public TankShell shellPrefab;                   // Prefab of the shell.
    public Transform fireTransform;

    private Vector3 lastPosition;
    //Debug info
    public float currentDistance;
    public float totalDistance;
    public float launchForce;

    private Slider slider;

    void Awake()
    {
        motor = GetComponent<TankMotor>();
        turret = GetComponentInChildren<TankTurret>();
        slider = GetComponentInChildren<Slider>();
        agent = GetComponent<MultiBrainAgent>();
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
        motor.Setup(ts);
        turret.Setup(ts);
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
        shellCoolDown -= Time.fixedDeltaTime;
    }

    private void CalcDistance()
    {
        if (lastPosition == Vector3.zero)
        {
            lastPosition = transform.position;
            return;
        }
        currentDistance += Vector3.Distance(lastPosition, transform.position);
        lastPosition = transform.position;
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
        health = settings.maxHealth;
        energy = settings.maxEnergy;
        slider.value = energy;
        totalDistance = 0f;
        lastPosition = Vector3.zero;
        motor.Reset();
        turret.Reset();
        ResetColor();
    }

    internal bool IsDead()
    {
        if (dead)
            return true;
        if (energy < settings.minEnergy || health < 0f)
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

    internal void OnActionReceived(float moveReq, float turnReq, float turretTurnReq, float fireReq)
    {
        motor.UpdateInput(moveReq, turnReq);
        turret.UpdateInput(turretTurnReq);
        if (fireReq > 0 && CanShoot())
        {
            Fire(fireReq);
        }
    }
    internal void UpdateMotorInput(float moveReq, float turnReq)
    {
        motor.UpdateInput(moveReq, turnReq);
      
    }
    internal void UpdateTurretInput(float turretTurnReq, bool fire)
    {
        turret.UpdateInput(turretTurnReq);
        if (fire && CanShoot())
        {
            Fire(0f);
        }
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

    internal void Hit(DamagableTarget hitObject, float damage)
    {
        if (agent != null)
            agent.OnHit(hitObject, damage);
    }

    internal float TakeDamage(float damage)
    {

        health -= damage;
        if (health < 0)
        {
            return -health;
        }
        return damage;
    }
    //Used for testing the motor with no agent
    //public void Update()
    //{
    //    OnActionReceived(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"));
    //}

    public bool CanShoot()
    {
        if (shellCoolDown > 0)
            return false;
        return true;
    }

    public bool Fire(float lForce)
    {
        if (dead)
            return false;
        shellCoolDown = settings.shellCoolDownTime;
        // Create an instance of the shell and store a reference to it's rigidbody.
        GameObject go = Instantiate(shellPrefab.gameObject, fireTransform.position, fireTransform.rotation, transform);
        TankShell shell = go.GetComponent<TankShell>();
       // launchForce = (settings.maxLaunchForce - settings.minLaunchForce) * lForce + settings.minLaunchForce;
        launchForce = settings.maxLaunchForce;
        shell.Shoot(this, launchForce * fireTransform.forward, settings.shellDamage);
        agent.OnFire();
        return true;
    }
}
