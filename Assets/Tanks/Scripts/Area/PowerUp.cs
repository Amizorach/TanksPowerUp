
using System;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private TanksAreaBase area;
    internal void Awake()
    {
        area = GetComponentInParent<TanksAreaBase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TankDriverAgent ta = other.GetComponent<TankDriverAgent>();
        if (ta != null)
        {
            ta.OnEnergyRecharge();
            area.ResetPowerUp(this.gameObject);
            return;
        }
        TankAgent ta1 = other.GetComponent<TankAgent>();
        if (ta1 != null)
        {
            ta1.OnEnergyRecharge();
            area.ResetPowerUp(this.gameObject);
            return;
        }
        ITankAgent mb = other.GetComponent<ITankAgent>();
        if (mb != null)
        {
            mb.OnEnergyRecharge();
            area.ResetPowerUp(this.gameObject);
            return;
        }
    }

    public void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, 1f) * Time.deltaTime * 90f);
    }

    internal void SetTriggerRadius(float powerUpTriggerRadius)
    {
        GetComponent<SphereCollider>().radius = powerUpTriggerRadius;
    }
}
