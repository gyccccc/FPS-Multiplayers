using System.Collections;
using System.Collections.Generic;
using Scripts.Weapon;
using UnityEngine;

public class AiMultiplayerWeapon : Firearms
{
    private IEnumerator reloadAmmoCheckerCoroutine;
    public GameObject AI;
    public AiController ac;

    private void Start()
    {
        ac = AI.GetComponent<AiController>();

    }

    protected override void Shooting()
    {
        if (CurrentAmmo <= 0) return;
        if (!IsAllowShooting()) return;
        MuzzleParticle.Play();
        CurrentAmmo -= 1;
        GunAnimator.SetLayerWeight(2, 1);
        GunAnimator.SetTrigger("Fire");
        CasingParticle.Play();
        FirearmsShootingAudioSource.clip = FirearmsAudioData.ShootingAudio;
        FirearmsShootingAudioSource.Play();
        LastFireTime = Time.time;
        ac.Damage();
    }

    protected override void Reload()
    {
        GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(GunAnimator.GetLayerIndex("Reload Layer"));
        if (CurrentMaxAmmoCarried <= 0 || GunStateInfo.IsTag("ReloadAmmo") || GunStateInfo.IsTag("ReloadOutOf") || CurrentAmmo == AmmoInMag)
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

}
