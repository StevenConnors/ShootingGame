using UnityEngine;
using System.Collections;
using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;
using Thalmic;


public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;

	// Myo game object to connect with.
	// This object must have a ThalmicMyo script attached.
	public GameObject myo = null;

    float timer;
    float myoTimer;
    Ray shootRay;
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;


    int bulletCount = 0;
    int count = 0;
    float myoCountConstant = 5.0f;

	private Pose _lastPose = Pose.Unknown;

    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");
        gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
        gunLight = GetComponent<Light> ();
    }
	
    void Update ()
    {
       	timer += Time.deltaTime;
        myoTimer += Time.deltaTime;
		ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
    	if (thalmicMyo.pose == Pose.Fist && timer >= timeBetweenBullets && Time.timeScale != 0 && myoTimer < myoCountConstant) {
            if (bulletCount < 10)
            {
                Shoot ();                    
            }
            bulletCount = bulletCount + 1;
            //}
            ExtendUnlockAndNotifyUserAction (thalmicMyo);
		} 
        _lastPose = thalmicMyo.pose;

        if (myoTimer > myoCountConstant && thalmicMyo.pose != Pose.Fist){
            myoTimer = 0f;
        }

        if (timer >= timeBetweenBullets * effectsDisplayTime){
            DisableEffects();
            bulletCount = bulletCount - 1;
            if (bulletCount < 0) bulletCount = 0;
        }


    }

    // Extend the unlock if ThalmcHub's locking policy is standard, and notifies the given myo that a user action was
    // recognized.
    void ExtendUnlockAndNotifyUserAction (ThalmicMyo myo)
    {
        ThalmicHub hub = ThalmicHub.instance;

        if (hub.lockingPolicy == LockingPolicy.Standard) {
            myo.Unlock (UnlockType.Timed);
        }

        myo.NotifyUserAction ();
    }

    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }


    void Shoot ()
    {
        timer = 0f;

        gunAudio.Play ();

        gunLight.enabled = true;

        gunParticles.Stop ();
        gunParticles.Play ();

        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
            if(enemyHealth != null)
            {
                enemyHealth.TakeDamage (damagePerShot, shootHit.point);
            }
            gunLine.SetPosition (1, shootHit.point);
        }
        else
        {
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
        }
    }
}
