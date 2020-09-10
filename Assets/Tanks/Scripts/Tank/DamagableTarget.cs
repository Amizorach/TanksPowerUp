using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableTarget : MonoBehaviour
{
    public virtual float TakeDamage(float damage) {
        return damage;
    }

}

