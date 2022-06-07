﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class launch : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public InputField RoomName;
    private bool connectToMaster;
    public bool joinedRoom;
    public bool isServer = false;
    public string PlayerPrefabName;
    //public string ShipPerfabName;
    public Vector3 SpawnPoint;
    public GameObject ship;
    private GameObject ui;
    public GameObject GameManager;
    public GameObject LevelManager;
    public Vector3 offset = new Vector3(3, 3, 3);


    public void Start()
    {
        ui = GameObject.FindGameObjectWithTag("ui");
        ui.SetActive(false);
    }

    public void ConnectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = "Alpha";

    }


    public void CreatRoom()
    {
        if (!connectToMaster || joinedRoom)
        {
            return;
        }
        PhotonNetwork.CreateRoom(RoomName.text,
            new Photon.Realtime.RoomOptions() { MaxPlayers = 16 }, Photon.Realtime.TypedLobby.Default);
        joinedRoom = true;
        isServer = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        connectToMaster = true;
    }

    public void joinRoom()
    {
        if (!connectToMaster || joinedRoom)
        {
            return;
        }
        PhotonNetwork.JoinRoom(RoomName.text);
        //GameObject.Find("LevelManager").SetActive(false);
        //GameObject.Find("GameManager").SetActive(false);
        
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        GameManager.SetActive(true);
        LevelManager.SetActive(true);

        ////GameObject player = PhotonNetwork.Instantiate(ShipPerfabName, new Vector3(0,0,0), Quaternion.identity);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        joinedRoom = true;
        Debug.Log("joined room");
        StartSpawn(0);
        Player.Respawn += StartSpawn;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Player.Respawn -= StartSpawn;
    }

    private void StartSpawn(float _timeToSpawn)
    {
        StartCoroutine(WaitToInstantiatePlayer(_timeToSpawn));
    }


    private IEnumerator WaitToInstantiatePlayer(float _timeToSpawn)
    {
        yield return new WaitForSeconds(_timeToSpawn);
        GameObject ship = GameObject.FindGameObjectWithTag("ship");
        Vector3 spawnPos = new Vector3(ship.transform.position.x + Random.Range(0, 20), ship.transform.position.y, ship.transform.position.z);
        //GameObject player = PhotonNetwork.Instantiate(PlayerPrefabName, ship.transform.position, Quaternion.identity);
        GameObject player = PhotonNetwork.Instantiate(PlayerPrefabName, spawnPos, Quaternion.identity);
        //ship.GetComponent<shipManager>().boarding(player);
        player.GetComponent<followShip>().temp = ship.GetComponent<shipManager>();
        ui.SetActive(true);
        spawnPos.y = -26.63112f;
        spawnPos.x = -25.34f;
        spawnPos.z = -34f;
        if (isServer)
        {
            GameObject ai = PhotonNetwork.Instantiate("ai", spawnPos, Quaternion.identity);
            GameObject ai2 = PhotonNetwork.Instantiate("GhillieSuit_ai", spawnPos+offset, Quaternion.identity);
            //ai.GetComponent<Nav>().goal = 
        }
    }


}
