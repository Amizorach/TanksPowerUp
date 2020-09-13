using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITankAgent {
    void OnEnergyRecharge();
    void OnHit(DamagableTarget hitObject, float damage);
    void OnFire();
}
