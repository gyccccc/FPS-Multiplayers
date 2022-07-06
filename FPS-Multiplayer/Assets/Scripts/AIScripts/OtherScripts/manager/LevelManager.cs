using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    private int EnemyNum = 110;
    private int LevelNum = 3;
    private GameObject[] AiList;
    private GameObject ui;
    private IEnumerator FindAiIE;
    private IEnumerator GenerateAiIE;
    private int WaitTime = 5; // 每五秒更新一次列表
    private GameObject Boss;

    public void Start()
    {
        
        ui = GameObject.Find("ui");
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void initEnemy(int LevelNumb)
    {
        LevelNum = LevelNumb;
        float LevelNumbf = LevelNumb;
        clearEnemy();
        GameObject[] SpawnPoseList = GameObject.FindGameObjectsWithTag("GuillySpawnPose");
        foreach (GameObject GuillySpawnPose in SpawnPoseList)
        {
            if (Random.value < (LevelNumbf - 1)/2.0f)
            {
                //PhotonNetwork.Instantiate("Boss_ai", GuillySpawnPose.transform.position, Quaternion.identity);
                PhotonNetwork.Instantiate("GhillieSuit_ai", GuillySpawnPose.transform.position, Quaternion.identity);
            }
        }

        SpawnPoseList = GameObject.FindGameObjectsWithTag("NormalSpawnPose");

        foreach (GameObject GuillySpawnPose in SpawnPoseList)
        {
            if (Random.value < LevelNumbf / 3.0f)
            {
                PhotonNetwork.Instantiate("ai", GuillySpawnPose.transform.position, Quaternion.identity);
            }
        }
        if(LevelNum == 3)
        {
            Boss = PhotonNetwork.Instantiate("Boss_ai", GameObject.Find("BossSpawnPose").transform.position, Quaternion.identity);

            if (GenerateAiIE != null)
            {
                StopCoroutine(GenerateAiIE);
                GenerateAiIE = null;
            }

            GenerateAiIE = GenerateAi();
            StartCoroutine(GenerateAiIE);
        }

        if (FindAiIE != null)
        {
            StopCoroutine(FindAiIE);
            FindAiIE = null;
        }

        FindAiIE = FindAi();
        StartCoroutine(FindAiIE);
    }

    public void clearEnemy()
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
        ui.GetComponent<uiManager>().GameOver();
    }



    private IEnumerator FindAi()
    {

        while (true)
        {
            AiList = GameObject.FindGameObjectsWithTag("ai");
            EnemyNum = AiList.Length;
            if (EnemyNum == 0)
            {
                LevelNum++;
                if (LevelNum > 3)
                {
                    Gameover();
                    break;
                }
                initEnemy(LevelNum);
                ui = GameObject.Find("ui");

                ui.GetComponent<uiManager>().GameRound(LevelNum);

            }
            yield return new WaitForSeconds(WaitTime);
        }

    }


    private IEnumerator GenerateAi()
    {
        Vector3 offset;
        while (true)
        {
            yield return new WaitForSeconds(WaitTime*3);
            //生成一个小怪 一个狙击手
            if (Boss == null)
                break;
            if(EnemyNum < 10)
            {
                offset = new Vector3(Random.value * 10, 0, Random.value * 10);
                PhotonNetwork.Instantiate("ai", Boss.transform.position + offset, Quaternion.identity);


                offset = new Vector3(Random.value * 10, 0, Random.value * 10);
                PhotonNetwork.Instantiate("GhillieSuit_ai", Boss.transform.position + offset, Quaternion.identity);

            }
            


        }

    }
}
