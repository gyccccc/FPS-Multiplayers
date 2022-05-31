using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityTemplateProjects.MultiplayerScripts;

[RequireComponent(typeof(PhotonView))]
public class Player : MonoBehaviour, IDamager
{
    public int HeathAll;
    public int Heath;
    private PhotonView photonView;
    private GameObject globalCamera;
    private GameObject ui;
    private uiManager uimanager;
    public static event Action<float> Respawn;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        Heath = HeathAll;

        if (photonView.IsMine)
        {

            ui = GameObject.FindGameObjectWithTag("ui");
            uimanager = ui.GetComponent<uiManager>();
            uimanager.setHealth(Heath, Heath);
            globalCamera = GameObject.FindWithTag("GlobalCamera");
            if (globalCamera)
                globalCamera.SetActive(false);
        }
    }

    public void TakeDamage(int _damage)
    {
        photonView.RpcSecure("RPC_TakeDamage", RpcTarget.All, true, _damage);
        Debug.Log("ai take damage");
        //if(photonView.IsMine)
    }


    [PunRPC]
    private void RPC_TakeDamage(int _damage, PhotonMessageInfo _info)
    {
        Heath -= _damage;
        if (uimanager)
        {
            uimanager.setHealth(Heath, HeathAll);

        }

        if (IsDeath() && photonView.IsMine)
        {
            //gameObject.SetActive(false);
            PhotonNetwork.Destroy(this.gameObject);
            if(ui)
            {
                ui.SetActive(false);
            }
            if (globalCamera)
                globalCamera.SetActive(true);

            Respawn?.Invoke(3);

            return;
        }

    }


    private bool IsDeath()
    {
        return Heath <= 0;
    }


    public int getHealth => Heath;
}