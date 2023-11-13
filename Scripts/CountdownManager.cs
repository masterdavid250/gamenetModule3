using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;
    public float timeToStartRace = 2.0f;

    private void Start()
    {
        timerText = RacingGameManager.instance.timeText;
    }

    private void Update()
    {
        if (timeToStartRace > 0)
        {
            timeToStartRace -= Time.deltaTime;
            photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStartRace); 
        }
        else if (Time.deltaTime > 0)
        {
            photonView.RPC("StartRace", RpcTarget.AllBuffered); 
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (timerText)
        {
            if (time > 0)
            {
                timerText.text = time.ToString("F1");
            }
            else
            {
                timerText.text = "";
            }
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<VehicleMovement>().isControlEnabled = true;
        this.enabled = false; 
    }


}
