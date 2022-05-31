using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class AilocalManager : MonoBehaviour
{
    public List<MonoBehaviour> LocalScripts;
    public Camera FP_Camera;
    public Camera ENV_Camera;
    private PhotonView photonView;
    public List<Renderer> TPRenderers;
    public GameObject FPArms;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        FPArms.SetActive(false);
        FP_Camera.enabled = false;
        ENV_Camera.enabled = false;

        foreach(MonoBehaviour behaviour in LocalScripts)
        {
            behaviour.enabled = false;
        }

        foreach(Renderer tpRenderer in TPRenderers)
        {
            tpRenderer.shadowCastingMode = ShadowCastingMode.On;
        }

    }
}
