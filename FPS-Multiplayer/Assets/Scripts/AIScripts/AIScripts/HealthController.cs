using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HealthController : MonoBehaviour
{
    // Start is called before the first frame update
    public Image healthBar;
    public Text healthText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setHealth(float healthNow, float healthAll)
    {
        if (healthBar)
        {
            healthBar.fillAmount = healthNow / healthAll;
            healthText.text = healthNow + "/" + healthAll;
        }

    }
}
