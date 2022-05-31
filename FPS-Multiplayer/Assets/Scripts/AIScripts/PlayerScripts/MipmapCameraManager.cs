using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MipmapCameraManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Mipmap_Camera;
    public GameObject player;
    public Vector3 offset = new Vector3(0,40,0);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player)
        {
            Mipmap_Camera.transform.position = player.transform.position + offset;
        }
    }
}
