using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class localManager : MonoBehaviour
{
    public List<MonoBehaviour> LocalScripts;
    public Camera FP_Camera;
    public Camera ENV_Camera;
    public Camera Mipmap_Camera;
    private PhotonView photonView;
    public List<Renderer> TPRenderers;
    public GameObject FPArms;
    public MipmapCameraManager MipmapManager;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        if(photonView.IsMine)
        {
            if(!gameObject.TryGetComponent<AudioListener>(out AudioListener tempAudioListener))
            {
                gameObject.AddComponent<AudioListener>();
            }
            GameObject mipmap = GameObject.Find("MipmapCamera");
            mipmap.GetComponent<MipmapCameraManager>().player = FPArms;
            return;
        }
        FPArms.SetActive(false);
        FP_Camera.enabled = false;
        ENV_Camera.enabled = false;
        


        foreach (MonoBehaviour behaviour in LocalScripts)
        {
            behaviour.enabled = false;
        }

        foreach(Renderer tpRenderer in TPRenderers)
        {
            tpRenderer.shadowCastingMode = ShadowCastingMode.On;
        }

    }
}
