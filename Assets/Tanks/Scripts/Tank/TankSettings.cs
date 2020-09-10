using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TankSettings
{
    [HeaderAttribute("Speed")]
    public float maxSpeed = 12f;                 // How fast the tank moves forward and back.
    public float turnSpeed = 180f;            // How fast the tank turns in degrees per second.
    public float turretTurnSpeed = 60f;

    [HeaderAttribute("Energy")]
    public float energyUsage = 1f / 20f; //40 seconds of life
    public float energyRecharge = 1f;

    public float minEnergy = -1f;
    public float maxEnergy = 1f;

    public float maxHealth = 1f;

    [HeaderAttribute("Trigger")]
    public float triggerRadius = 1f;

    [HeaderAttribute("Shell")]
    public float maxLaunchForce = 30f;
    public float minLaunchForce = 15f;
    public float maxChargeTime = 0.75f;
    public float shellDamage = 0.5f;

    public float shellCoolDownTime = 2f;
}
