using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();

    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
    }

    private int finishOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent; 
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string)data[0]; 
            finishOrder = (int)data[1]; 
            int viewId = (int)data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUiText = RacingGameManager.instance.finisherTextUi[finishOrder - 1];
            orderUiText.SetActive(true);

            if (viewId == photonView.ViewID) //this is you
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nickNameOfFinishedPlayer + " (YOU)";
                orderUiText.GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                orderUiText.GetComponent<TextMeshProUGUI>().text = finishOrder + " " + nickNameOfFinishedPlayer;
            }


        }
    }
    
    private void Start()
    {
        foreach (GameObject go in RacingGameManager.instance.lapTriggers)
        {
            lapTriggers.Add(go);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (lapTriggers.Contains(other.gameObject))
        {
            int indexOfTrigger = lapTriggers.IndexOf(other.gameObject);
            lapTriggers[indexOfTrigger].SetActive(false);
        }

        if (other.gameObject.tag == "FinishTrigger")
        {
            GameFinish(); 
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

        finishOrder++; 
        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID; 

        //event data
        object[] data = new object[] { nickName, finishOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOption); 
    }
}
