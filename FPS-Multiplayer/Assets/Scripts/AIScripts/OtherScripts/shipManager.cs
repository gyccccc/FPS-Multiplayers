using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipManager : MonoBehaviour
{
    public List<GameObject> PlayersOnShip = new List<GameObject>();
    private Vector3 target;
    private Vector3 oldpos;
    public Vector3 offset;
    public bool isServer = false;
    private float lastMoveTime;
    launch temp;

    //private Vector3 target1 = new Vector3(10, 0, 10);
    //private Vector3 target2 = new Vector3(10, 0, -90);
    //private Vector3 target3 = new Vector3(-90, 0, 10);
    //private Vector3 target4 = new Vector3(-90, 0, -90);
    //private Vector3 target5 = new Vector3(-40, 0, 10);
    //private Vector3 target6 = new Vector3(10, 0, -40);
    //private Vector3 target7 = new Vector3(-40, 0, -90);
    //private Vector3 target8 = new Vector3(-90, 0, -40);
    private List<Vector3> targetList = new List<Vector3>()
    {
        new Vector3(10, 0, 10),
        new Vector3(10, 0, -90),
        new Vector3(-90, 0, 10),
        new Vector3(-90, 0, -90),
        new Vector3(-40, 0, 10),
        new Vector3(10, 0, -40),
        new Vector3(-40, 0, -90),
        new Vector3(-90, 0, -40)
    };
    private float endtime;
    // Start is called before the first frame update
    void Start()
    {
        endtime = Time.time + Random.Range(6, 10);
        target = targetList[Random.Range(0, 7)];
        GameObject canvas = GameObject.FindGameObjectWithTag("canvas");
        temp = canvas.GetComponent<launch>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isServer)
        {
            //控制飞船位移
            //传出信息同步飞船位置
            if (Time.time > endtime)
            {
                endtime = Time.time + Random.Range(6, 10);
                target = targetList[Random.Range(0, 7)];
            }
            oldpos = this.transform.position;
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(target.x, this.transform.position.y, target.z), 0.002f);
            offset = this.transform.position - oldpos;

        }
        else
        {
            offset = this.transform.position - oldpos;
            oldpos = this.transform.position;
            if(!offset.Equals(Vector3.zero))
            {
                lastMoveTime = Time.time;
            }
            if (Time.time - lastMoveTime > 4)
            {
                offset = this.transform.position - oldpos;
                oldpos = this.transform.position;
                if (offset.Equals(Vector3.zero) && temp.joinedRoom)
                {
                    isServer = Random.Range(-10, 0.1f) > 0;
                }
            }
        }
        
        //foreach (GameObject player in PlayersOnShip)
        //{
        //    player.transform.position = Vector3.Lerp(player.transform.position, new Vector3(target.x, player.transform.position.y, target.z), 0.002f);
        //    //player.transform.position.Set(0, 0, 1);
        //}
    }

    //public void boarding(GameObject player)
    //{
    //    PlayersOnShip.Add(player);
    //}

    //public void deplane(GameObject player)
    //{
    //    PlayersOnShip.Remove(player);
    //}
}
