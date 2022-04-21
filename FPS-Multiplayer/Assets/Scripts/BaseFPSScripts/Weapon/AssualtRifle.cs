using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Scripts.Weapon
{
    public class AssualtRifle : Firearms
    {
        private IEnumerator reloadAmmoCheckerCoroutine;


        private FPMouseLook mouseLook;


        protected override void Awake()
        {
            base.Awake();
            FirearmsShootingAudioSource.clip = null;
            FirearmsShootingAudioSource.clip = null;

            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
            mouseLook = FindObjectOfType<FPMouseLook>();
        }


        protected override void Shooting()
        {
            if (CurrentAmmo <= 0) return;
            if (!IsAllowShooting()) return;
            GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(GunAnimator.GetLayerIndex("Base Layer"));
            if (GunStateInfo.IsTag("Takout_weapon") || GunStateInfo.IsTag("holster"))
            {
                return;
            }
            GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(GunAnimator.GetLayerIndex("Reload Layer"));
            if (GunStateInfo.IsTag("ReloadAmmo") || GunStateInfo.IsTag("ReloadOutOf"))
            {
                return;
            }
            MuzzleParticle.Play();
            CurrentAmmo -= 1;

            GunAnimator.Play("Fire", IsAiming ? 1 : 0, 0);

            FirearmsShootingAudioSource.clip = FirearmsAudioData.ShootingAudio;
            FirearmsShootingAudioSource.Play();

            CreateBullet();
            CasingParticle.Play();
            if (mouseLook)
                mouseLook.FiringForTest();
            LastFireTime = Time.time;
        }

        protected override void Reload()
        {
            GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(GunAnimator.GetLayerIndex("Reload Layer"));
            if (CurrentMaxAmmoCarried <= 0 || GunStateInfo.IsTag("ReloadAmmo")|| GunStateInfo.IsTag("ReloadOutOf")|| CurrentAmmo == AmmoInMag)
            {
                return;
            }
            GunAnimator.SetLayerWeight(2, 1);
            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            FirearmsReloadAudioSource.clip =
                CurrentAmmo > 0
                    ? FirearmsAudioData.ReloadLeft
                    : FirearmsAudioData.ReloadOutOf;

            FirearmsReloadAudioSource.Play();

            if (reloadAmmoCheckerCoroutine == null)
            {
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
            else
            {
                StopCoroutine(reloadAmmoCheckerCoroutine);
                reloadAmmoCheckerCoroutine = null;
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
        }

        private void CreateBullet()
        {
            GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);

            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();

            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
//            tmp_BulletScript.ImpactPrefab = BulletImpactPrefab;
//            tmp_BulletScript.ImpactAudioData = ImpactAudioData;
            tmp_BulletScript.BulletSpeed = BulletSpeed;
        }
    }
}