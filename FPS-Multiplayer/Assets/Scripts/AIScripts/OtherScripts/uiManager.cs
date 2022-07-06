using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class uiManager : MonoBehaviour
{
    // Start is called before the first frame update\
    public GameObject crossHair;
    //public GameObject RedPoint;
    public GameObject m416;
    public GameObject glock;
    public GameObject bigmap;
    public GameObject RoundBar;
    private PhotonView photonView;

    public Text AmmoCountTextLabel;
    public Image healthBar;
    public Text healthText;
    public Text Round;


    private int rn = 1;

    private IEnumerator ShowRoundIE;
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        //RedPoint.SetActive(false);
        //bigmap.SetActive(map);

        //if (ShowRoundIE != null)
        //{
        //    StopCoroutine(ShowRoundIE);
        //    ShowRoundIE = null;
        //}

        //ShowRoundIE = ShouRound(1);
        //StartCoroutine(ShowRoundIE);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void aiming(bool isAiming)
    {
        crossHair.SetActive(!isAiming);
        //RedPoint.SetActive(isAiming);
    }

    public void showMap()
    {
        bigmap.SetActive(!bigmap.activeSelf);
        //RedPoint.SetActive(isAiming);
    }

    public void showGun(string name)
    {

    }

    public void UpdateAmmoInfo(int _ammo, int _remaningAmmo)
    {
        //Debug.Log(AmmoCountTextLabel);
        if (AmmoCountTextLabel)
            AmmoCountTextLabel.text = _ammo + "/" + _remaningAmmo;
    }

    public void unsetCarriedWeapon(string name)
    {
        if(name.Equals("arms_assault_rifle_02"))
        {
            m416.SetActive(false);
        }
        else if(name.Equals("arms_handgun_01"))
        {
            glock.SetActive(false);
        }
        Debug.Log(name);
    }

    public void setCarriedWeapon(string name)
    {
        if (name.Equals("arms_assault_rifle_02"))
        {
            m416.SetActive(true);
        }
        else if (name.Equals("arms_handgun_01"))
        {
            glock.SetActive(true);
        }
        Debug.Log(name);

    }

    public void setHealth(float healthNow, float healthAll)
    {
        if(healthBar)
        {
            healthBar.fillAmount = healthNow / healthAll;
            healthText.text = healthNow + "/" + healthAll;
        }

    }

    public void GameRound(int roundNumb)
    {
        //if (roundNumb < 1)
        //    roundNumb = rn;
        //if (ShowRoundIE != null)
        //{
        //    StopCoroutine(ShowRoundIE);
        //    ShowRoundIE = null;
        //}

        //ShowRoundIE = ShowRound(roundNumb);
        //StartCoroutine(ShowRoundIE);

        //rn = roundNumb;
        if(photonView)
        {
            photonView.RpcSecure("RPC_GameRound", RpcTarget.All, true, roundNumb);

        }
    }


    public void GameRoundThis(int _roundNumb)
    {
        if (_roundNumb < 1)
            _roundNumb = rn;
        if (ShowRoundIE != null)
        {
            StopCoroutine(ShowRoundIE);
            ShowRoundIE = null;
        }

        ShowRoundIE = ShowRound(_roundNumb);
        StartCoroutine(ShowRoundIE);

        rn = _roundNumb;
    }


    public void GameOver()
    {
        if (photonView)
        {
            photonView.RpcSecure("RPC_GameOver", RpcTarget.All, true);

        }
    }


    private IEnumerator ShowRound(int roundnum)
    {

        Hide hide = RoundBar.GetComponent<Hide>();
        Round.text = "ROUND  " + roundnum;
        hide.Show();
        yield return new WaitForSeconds(3);
        hide.Hiding();
    }



    [PunRPC]
    private void RPC_GameRound(int _roundNumb, PhotonMessageInfo _info)
    {
        if (_roundNumb < 1)
            _roundNumb = rn;
        if (ShowRoundIE != null)
        {
            StopCoroutine(ShowRoundIE);
            ShowRoundIE = null;
        }

        ShowRoundIE = ShowRound(_roundNumb);
        StartCoroutine(ShowRoundIE);

        rn = _roundNumb;

    }


    [PunRPC]
    private void RPC_GameOver(PhotonMessageInfo _info)
    {
        Hide hide = RoundBar.GetComponent<Hide>();
        Round.text = "GAME OVER!";
        hide.Show();
    }
}
