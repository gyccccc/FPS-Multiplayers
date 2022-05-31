using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using Photon.Pun;

public class aim : MonoBehaviour,IPunObservable
{
    public Transform Arms;
    public Transform AimTarget;

    public float AimTargetDistance = 5f;


    private Vector3 localPosition;
    private Quaternion localRotation;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        localPosition = AimTarget.position;
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            //若是本地则进行计算
            localRotation = Arms.localRotation;
            localPosition = localRotation * Vector3.forward * AimTargetDistance;
            AimTarget.localPosition = localPosition;
        }

        AimTarget.localPosition = Vector3.Lerp(AimTarget.localPosition, localPosition, Time.deltaTime * 20);

    }


    public void OnPhotonSerializeView(PhotonStream stream,PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(localPosition);

        }
        else
        {
            localPosition = (Vector3)stream.ReceiveNext();
        }
    }

}
