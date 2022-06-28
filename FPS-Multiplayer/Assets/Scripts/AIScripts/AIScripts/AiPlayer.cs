using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityTemplateProjects.MultiplayerScripts;

[RequireComponent(typeof(PhotonView))]
public class AiPlayer : MonoBehaviour, IDamager
{
    public int HeathAll;
    public int Heath;
    public string BodyName;
    private PhotonView photonView;
    private HealthController healthController;
    private GameObject globalCamera;
    //public GameObject ui;
    //public uiManager uimanager;
    //public static event Action<float> Respawn;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        healthController = GetComponent<HealthController>();
        //if (photonView.IsMine)
        //{
        //    ui = GameObject.FindGameObjectWithTag("ui");
        //    uimanager = ui.GetComponent<uiManager>();
        //    globalCamera = GameObject.FindWithTag("GlobalCamera");
        //    if (globalCamera)
        //        globalCamera.SetActive(false);
        //}
        Heath = HeathAll;
    }

    public void TakeDamage(int _damage)
    {
        photonView.RpcSecure("RPC_TakeDamage", RpcTarget.All, true, _damage);
        //if(photonView.IsMine)
    }


    [PunRPC]
    private void RPC_TakeDamage(int _damage, PhotonMessageInfo _info)
    {
        Heath -= _damage;
        if (healthController)
        {
            //需要改变显示的ui
            //uimanager.setHealth(Heath, HeathAll);
            healthController.setHealth(Heath, HeathAll);

        }

        if (IsDeath() && photonView.IsMine)
        {
            gameObject.SetActive(false);
            Vector3 tempPose = gameObject.transform.position;
            PhotonNetwork.Destroy(gameObject);
            PhotonNetwork.Instantiate(BodyName, tempPose, Quaternion.identity);

            //
            //死亡就销毁，全部死亡游戏结束 

            //if(ui)
            //{
            //    ui.SetActive(false);
            //}
            //if (globalCamera)
            //    globalCamera.SetActive(true);

            //Respawn?.Invoke(3);

            return;
        }

    }


    private bool IsDeath()
    {
        return Heath <= 0;
    }


    public int getHealth => Heath;
}