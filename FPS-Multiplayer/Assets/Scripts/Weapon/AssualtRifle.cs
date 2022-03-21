using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Scripts.Weapon2
{
    public class AssualtRifle:Firearms
    {
        protected override void Shooting()
        {
            CurrentAmmo -= 1;
            GunAnimator.Play("Fire", 0, 0);
            MuzzleParticle.Play();
            Casingparticle.Play();
            CreatBullet();
            lastFireTime = Time.time;
        }


        protected override void Reload()
        {
            if (CurrentAmmo == AmmoInMag)
                return;
            GunAnimator.SetLayerWeight(2, 1);
            GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
            if (GunStateInfo.IsTag("ReloadAmmo"))
            {
                return;
            }
            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft":"ReloadOutOf");
            StartCoroutine(CheckReloadAmmoAnimationEnd());
        }

        private void Update()
        {
            if(Input.GetMouseButton(0))
            {
                DoAttack();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }


        private IEnumerator CheckReloadAmmoAnimationEnd()
        {
            while(true)
            {
                yield return null;
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);
                if(GunStateInfo.IsTag("ReloadAmmo"))
                {
                    if(GunStateInfo.normalizedTime >= 0.89f)
                    {
                        CurrentMaxAmmoCarried += CurrentAmmo;
                        CurrentAmmo = AmmoInMag;
                        CurrentMaxAmmoCarried -= AmmoInMag;
                        if (CurrentMaxAmmoCarried <= 0)
                        {
                            CurrentAmmo += CurrentMaxAmmoCarried;
                            CurrentMaxAmmoCarried = 0;
                        }
                        yield break;
                    }
                }
            }
        }

    }


    
}