using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TankSettings
{
    [HeaderAttribute("Speed")]
    public float maxSpeed = 12f;      // How fast the tank moves forward and back.
    public float turnSpeed = 180f;    // How fast the tank turns in degrees per second.

    [HeaderAttribute("Energy")]
    public float energyUsage = 0.05f; // Amount of energy used every FixedUpdate
    public float energyRecharge = 1f; // Amount of energy recharged when hitting a powerUp

    public float minEnergy = -1f; // Minimal energy amount - beyond this the tank is considered dead
    public float maxEnergy = 1f;  // Maximum energy a tank can hold 

    [HeaderAttribute("Trigger")]
    public float triggerRadius = 1f; //The size of the trigger around the tank

    [HeaderAttribute("Color")]
    public Color baseColor = Color.black; //Tank color 
    public bool randomColor = true; //use a random color each episode

}