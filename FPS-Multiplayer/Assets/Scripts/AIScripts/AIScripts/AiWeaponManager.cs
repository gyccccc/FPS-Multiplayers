using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Scripts.Items;
using Scripts.Weapon;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AiWeaponInfo
{
    public int WeaponId;
    public string WeaponName;
    public Firearms FP_Weapon;
    public Firearms TP_Weapon;
}


public class AiWeaponManager : MonoBehaviour
{
    public Firearms MainWeapon;
    public Firearms SecondaryWeapon;
    public Text AmmoCountTextLabel;

    private Firearms carriedWeapon;

    [SerializeField] private List<WeaponInfo> WeaponInfos;

    [SerializeField] private FPCharacterControllerMovement CharacterControllerMovement;

    private AnimatorStateInfo animationStateInfo;
    private IEnumerator waitingForHolsterEndCoroutine;
    private PhotonView photonView;

    public List<Firearms> Arms = new List<Firearms>();
    public LayerMask CheckItemLayerMask;
    private uiManager ui;


    //private void UpdateAmmoInfo(int _ammo, int _remaningAmmo)
    //{
    //    Debug.Log(AmmoCountTextLabel);
    //    if (AmmoCountTextLabel)
    //        AmmoCountTextLabel.text = _ammo + "/" + _remaningAmmo;
    //}


    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        //Debug.Log($"Current weapon is null? {carriedWeapon == null}");

        MainWeapon = WeaponInfos[0].TP_Weapon;
        //attention

        //if (MainWeapon)
        //{
        //    if(photonView.IsMine)
        //        ui = GameObject.FindGameObjectWithTag("ui").GetComponent<uiManager>();

        //    carriedWeapon = MainWeapon;
        //    CharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);

        //    SecondaryWeapon.gameObject.SetActive(false);
        //}
        carriedWeapon = MainWeapon;
        //CharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);
    }

    private void Update()
    {


        if (!photonView.IsMine) return;
        //attention


        if (!carriedWeapon) return;




        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    photonView.RPC("RPC_SwapWeapon1", RpcTarget.All);

        //}

        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    photonView.RPC("RPC_SwapWeapon2", RpcTarget.All);

        //}

        //photonView.RPC("RPC_HoldTrigger", RpcTarget.All);


        //if (Input.GetMouseButtonUp(0))
        //{
        //    //TODO: release the Trigger

        //    //attention
        //    //carriedWeapon.ReleaseTrigger();

        //    photonView.RPC("RPC_ReleaseTrigger", RpcTarget.All);
        //}


        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    //TODO:Reloading the ammo

        //    //attention
        //    //carriedWeapon.ReloadAmmo();

        //    photonView.RPC("RPC_ReloadAmmo", RpcTarget.All);
        //}

        //if (Input.GetMouseButtonDown(1))
        //{
        //    //TODO:瞄准
        //    carriedWeapon.Aiming(true);
        //    ui.aiming(true);
        //}

        //if (Input.GetMouseButtonUp(1))
        //{
        //    //TODO:退出瞄准
        //    carriedWeapon.Aiming(false);
        //    ui.aiming(false);
        //}

        //ui.UpdateAmmoInfo(carriedWeapon.GetCurrentAmmo, carriedWeapon.GetCurrentMaxAmmoCarried);
    }


    private void PickupWeapon(BaseItem _baseItem)
    {
        if (!(_baseItem is FirearmsItem tmp_FirearmsItem)) return;
        foreach (Firearms tmp_Arm in Arms)
        {
            if (tmp_FirearmsItem.ArmsName.CompareTo(tmp_Arm.name) != 0) continue;
            switch (tmp_FirearmsItem.CurrentFirearmsType)
            {
                case FirearmsItem.FirearmsType.AssultRefile:
                    MainWeapon = tmp_Arm;
                    break;
                case FirearmsItem.FirearmsType.HandGun:
                    SecondaryWeapon = tmp_Arm;

                    break;
            }

            SetupCarriedWeapon(tmp_Arm);
        }
    }


    private void PickupAttachment(BaseItem _baseItem)
    {
        if (!(_baseItem is AttachmentItem tmp_AttachmentItem)) return;

        switch (tmp_AttachmentItem.CurrentAttachmentType)
        {
            case AttachmentItem.AttachmentType.Scope:
                foreach (ScopeInfo tmp_ScopeInfo in carriedWeapon.ScopeInfos)
                {
                    if (tmp_ScopeInfo.ScopeName.CompareTo(tmp_AttachmentItem.ItemName) != 0)
                    {
                        tmp_ScopeInfo.ScopeGameObject.SetActive(false);
                        continue;
                    }

                    tmp_ScopeInfo.ScopeGameObject.SetActive(true);
                    carriedWeapon.BaseIronSight.ScopeGameObject.SetActive(false);
                    carriedWeapon.SetupCarriedScope(tmp_ScopeInfo);
                }

                break;
            case AttachmentItem.AttachmentType.Other:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    


    private void StartWaitingForHolsterEndCoroutine()
    {
        if (waitingForHolsterEndCoroutine == null)
            waitingForHolsterEndCoroutine = WaitingForHolsterEnd();
        StartCoroutine(waitingForHolsterEndCoroutine);
    }


    private IEnumerator WaitingForHolsterEnd()
    {
        while (true)
        {
            AnimatorStateInfo tmp_AnimatorStateInfo = carriedWeapon.GunAnimator.GetCurrentAnimatorStateInfo(0);
            if (tmp_AnimatorStateInfo.IsTag("holster"))
            {
                if (tmp_AnimatorStateInfo.normalizedTime >= 0.9f)
                {
                    var tmp_TargetWeapon = carriedWeapon == MainWeapon ? SecondaryWeapon : MainWeapon;
                    SetupCarriedWeapon(tmp_TargetWeapon);
                    waitingForHolsterEndCoroutine = null;
                    yield break;
                }
            }

            yield return null;
        }
    }


    private void SetupCarriedWeapon(Firearms _targetWeapon)
    {
        if (carriedWeapon)
            carriedWeapon.gameObject.SetActive(false);
        ui.unsetCarriedWeapon(carriedWeapon.name);

        carriedWeapon = _targetWeapon;
        carriedWeapon.gameObject.SetActive(true);
        ui.setCarriedWeapon(carriedWeapon.name);

        CharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);
    }

    [PunRPC]
    private void RPC_AiHoldTrigger()
    {
        carriedWeapon.HoldTrigger();
        if(carriedWeapon.GetCurrentAmmo == 0)
        {
            carriedWeapon.ReloadAmmo();
        }
    }

    [PunRPC]
    private void RPC_ReleaseTrigger()
    {
        carriedWeapon.ReleaseTrigger();
    }

    [PunRPC]
    private void RPC_ReloadAmmo()
    {
        carriedWeapon.ReloadAmmo();
    }


    [PunRPC]
    private void RPC_SwapWeapon1()
    {
        if (MainWeapon == null) return;
        //更换为主武器
        if (carriedWeapon == MainWeapon) return;
        carriedWeapon.FirearmsReloadAudioSource.clip = null;
        carriedWeapon.FirearmsShootingAudioSource.clip = null;
        if (carriedWeapon.gameObject.activeInHierarchy)
        {
            StartWaitingForHolsterEndCoroutine();
            carriedWeapon.GunAnimator.SetTrigger("holster");
        }
        else
        {
            SetupCarriedWeapon(MainWeapon);
        }


    }

    [PunRPC]
    private void RPC_SwapWeapon2()
    {
        if (SecondaryWeapon == null) return;
        //更换为副武器
        if (carriedWeapon == SecondaryWeapon) return;
        carriedWeapon.FirearmsReloadAudioSource.clip = null;
        carriedWeapon.FirearmsShootingAudioSource.clip = null;
        if (carriedWeapon.gameObject.activeInHierarchy)
        {
            StartWaitingForHolsterEndCoroutine();
            carriedWeapon.GunAnimator.SetTrigger("holster");
        }
        else
        {
            SetupCarriedWeapon(SecondaryWeapon);
        }


    }


    public void Shooting()
    {
        photonView.RPC("RPC_AiHoldTrigger", RpcTarget.All);

    }
}