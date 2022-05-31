using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followShip : MonoBehaviour
{
    public shipManager temp;
    public GameObject ship;
    public float groundHeight;
    // Start is called before the first frame update
    void Start()
    {

        temp = GameObject.FindGameObjectWithTag("ship").GetComponent<shipManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (gameObject.transform.position.y < groundHeight)
        //{
        //    this.transform.parent = null;
        //}

        gameObject.transform.position += temp.offset;
        if (gameObject.transform.position.y < groundHeight)
        {
            gameObject.GetComponent<followShip>().enabled = false;
        }

    }
}
