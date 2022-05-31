using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class launch1 : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public InputField RoomName;
    private bool connectToMaster;
    private bool joinedRoom;
    public string PlayerPrefabName;
    //public string ShipPerfabName;
    public Vector3 SpawnPoint;
    public GameObject ship;

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
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        joinedRoom = true;
        ////GameObject player = PhotonNetwork.Instantiate(ShipPerfabName, new Vector3(0,0,0), Quaternion.identity);

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
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
        GameObject player = PhotonNetwork.Instantiate(PlayerPrefabName, ship.transform.position, Quaternion.identity);
        //ship.GetComponent<shipManager>().boarding(player);
        player.GetComponent<followShip>().temp = ship.GetComponent<shipManager>();
    }


}
