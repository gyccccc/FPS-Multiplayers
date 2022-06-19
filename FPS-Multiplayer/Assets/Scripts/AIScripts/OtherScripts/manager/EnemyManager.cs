using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyManager : MonoBehaviour
{
    // Start is called before the first frame update
    private int EnemyNum = 110;
    private float LevelNum = 1;
    private GameObject[] AiList;
    private IEnumerator FindAiIE;
    private int WaitTime = 5; // 每五秒更新一次列表

    public void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void initEnemy(float LevelNumb)
    {
        

        Debug.Log(LevelNumb+10000);
        LevelNum = LevelNumb;
        clearEnemy();
        GameObject[] SpawnPoseList = GameObject.FindGameObjectsWithTag("GuillySpawnPose");
        foreach (GameObject GuillySpawnPose in SpawnPoseList)
        {
            if (Random.value < (LevelNumb-1)/2.0f)
            {
                PhotonNetwork.Instantiate("GhillieSuit_ai", GuillySpawnPose.transform.position, Quaternion.identity);
                EnemyNum++;
            }
        }

        SpawnPoseList = GameObject.FindGameObjectsWithTag("NormalSpawnPose");

        foreach (GameObject GuillySpawnPose in SpawnPoseList)
        {
            if (Random.value < LevelNumb / 3.0f)
            {
                PhotonNetwork.Instantiate("ai", GuillySpawnPose.transform.position, Quaternion.identity);
                EnemyNum++;
            }
        }

        if (FindAiIE != null)
        {
            StopCoroutine(FindAiIE);
            FindAiIE = null;
        }

        FindAiIE = FindAi();
        StartCoroutine(FindAiIE);
    }

    private void clearEnemy()
    {
        GameObject[] BodyList = GameObject.FindGameObjectsWithTag("body");

        foreach (GameObject body in BodyList )
        {
            PhotonNetwork.Destroy(body);
        }

        BodyList = GameObject.FindGameObjectsWithTag("ai");

        foreach (GameObject body in BodyList)
        {
            PhotonNetwork.Destroy(body);
        }

        EnemyNum = 0;
    }

    private void Gameover()
    {

    }



    private IEnumerator FindAi()
    {

        while (true)
        {
            AiList = GameObject.FindGameObjectsWithTag("ai");
            if(AiList.Length == 0)
            {
                LevelNum++;
                if (LevelNum > 3)
                {
                    Gameover();
                }
                initEnemy(LevelNum);
            }
           


            yield return new WaitForSeconds(WaitTime);

        }

    }
}
