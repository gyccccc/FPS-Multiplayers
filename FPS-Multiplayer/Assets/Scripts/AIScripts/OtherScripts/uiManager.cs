using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uiManager : MonoBehaviour
{
    // Start is called before the first frame update\
    public GameObject crossHair;
    //public GameObject RedPoint;
    public GameObject m416;
    public GameObject glock;
    public GameObject bigmap;

    public Text AmmoCountTextLabel;
    public Image healthBar;
    public Text healthText;
    void Start()
    {
        //RedPoint.SetActive(false);
        //bigmap.SetActive(map);

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
}
