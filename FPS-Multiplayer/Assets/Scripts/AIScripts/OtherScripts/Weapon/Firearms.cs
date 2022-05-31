using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Weapon2
{
    public abstract class Firearms:MonoBehaviour,IWeapon
    {
        public Transform MuzzlePoint;
        public Transform CasingPoint;

        public ParticleSystem MuzzleParticle;
        public ParticleSystem Casingparticle;

        public int AmmoInMag = 30;
        public int MaxAmmoCarried = 120;
        public GameObject BulletPrefab;

        protected Animator GunAnimator;
        public float FireRate;
        protected float lastFireTime;

        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;

        protected AnimatorStateInfo GunStateInfo;


        public AudioSource FirearmsShootingAudioSource;
        public AudioSource FirearmsReloadAudioSource;


        protected virtual void Start()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            GunAnimator = GetComponent<Animator>();

        }




        public void DoAttack()
        {
            if(CurrentAmmo <= 0)
            {
                return;
            }
            if(IsAllowShooting())
            {
                Shooting();
            }
            
        }


        protected abstract void Shooting();
        protected abstract void Reload();

        protected bool IsAllowShooting()
        {
            return Time.time - lastFireTime > 1 / FireRate;
        }

        protected void CreatBullet()
        {
            GameObject temp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            var temp_BulletRigidbody = temp_Bullet.AddComponent<Rigidbody>();
            temp_BulletRigidbody.velocity = temp_Bullet.transform.forward * 100f;
            
        }

    }
}