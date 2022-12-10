using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto,Burst,Single};
    public FireMode fireMode;
    public Transform[] projectileSpawn;
    public Projectile projectile;
    //toc' do. ban' cua? vu~ khi'
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;//so' luong. vu. no? trong tung` phat; ban'

    public Transform shell;
    public Transform shellEjection;
    public Vector2 kickMinMax;
    MuzzleFlash muzzleFlash;

    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBust;
    Vector3 recoilSmoothDampVelocity;
    float recoilAngle;
    float recoilRotSmoothDampVelocity;

    private void Start() 
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBust = burstCount;
    }
    private void LateUpdate()
   {
        //tao. hoat. anh? cho do. giat. cua? sung'
       transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, 0.1f);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, 0.1f);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;
    }
    void Shoot()
    {
        if(Time.time > nextShotTime)
        {
            if(fireMode == FireMode.Burst)
            {
                if(shotsRemainingInBust == 0)
                {
                    return;
                }
                shotsRemainingInBust--;
            }
            else
            {
                if (fireMode == FireMode.Single)
                {
                    if (!triggerReleasedSinceLastShot)
                    {
                        return;
                    }
                }
            }

            for(int i = 0; i< projectileSpawn.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
               
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x,kickMinMax.y);
            recoilAngle += 20;
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

        }
       
    }

    public void Aim(Vector3 aimPoint)
    {
        transform.LookAt(aimPoint);
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBust = burstCount;
    }
   
}
