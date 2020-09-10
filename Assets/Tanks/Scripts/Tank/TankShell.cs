using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TankShell : MonoBehaviour
{
    public ParticleSystem explosionParticles;         // Reference to the particles that will play on explosion.
    public ParticleSystem hitParticles;         // Reference to the particles that will play on explosion.

    public AudioSource explosionAudio;                // Reference to the audio that will play on explosion.
    public float maxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    public float damage = 0.5f;                // The maximum distance away from the explosion tanks can be and are still affected.

    public TankController tank;

    private Rigidbody rigid;
    private bool fired = false;
    private DamagableTarget hitObject;
    public void Awake()
    {
        rigid = GetComponent<Rigidbody>();
       // explosionParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {

        // If it isn't destroyed by then, destroy the shell after it's lifetime.
      //  Destroy(gameObject, maxLifeTime);
        Invoke("Explode", 2);
    }


    public bool Shoot(TankController t, Vector3 launchForce, float damage)
    {
        if (fired)
        {
            return false;
        }
        this.damage = damage;
        tank = t;
        fired = true;
        Vector3 r = launchForce;
        r.y = 0f;
        rigid.velocity = r;
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!fired || tank == null)
            return;
        if (other.gameObject == tank.gameObject)
            return;
        if (other.gameObject.layer == 9)
            return;
        hitObject = other.GetComponent<DamagableTarget>();
        Explode(true);
    }
    private void Explode()
    {
        Explode(false);
    }

    private void Explode(bool hit)
    {
        float aDamage = 0f;
        if (hitObject != null)
            aDamage = hitObject.TakeDamage(damage);
        if (hit)
        {
            ParticleSystem p = explosionParticles;
            if (hitObject != null)
                p = hitParticles;
            if (p != null)
            {
                p.transform.parent = null;
                // Play the particle system.
                p.Play();

                // Once the particles have finished, destroy the gameobject they are on.
                ParticleSystem.MainModule mainModule = p.main;
                Destroy(p.gameObject, mainModule.duration);
            }
        }
        

        if (hit && explosionAudio != null)
            // Play the explosion sound effect.
            explosionAudio.Play();

       
        tank.Hit(hitObject, aDamage);
        // Destroy the shell.
        Destroy(gameObject);

    }
}