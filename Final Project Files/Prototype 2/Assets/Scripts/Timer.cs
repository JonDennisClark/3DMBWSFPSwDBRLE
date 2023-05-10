using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeVal;
    public TextMeshProUGUI timeDisplay;
    // Start is called before the first frame update
    void Start()
    {
        timeVal = 3;
        timeDisplay.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeVal > 0)
        {
            timeVal -= Time.deltaTime;
           
        } else
        {
            timeVal = 0;
        }

        DisplayTime();
        
        if (timeVal == 0)
        {
            timeDisplay.gameObject.SetActive(false);
        }
    }

    public void DisplayTime()
    {
        timeDisplay.text = string.Format("Next round begins in: {0:0}", timeVal);
    }
}
