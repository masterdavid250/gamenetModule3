using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public TextMeshProUGUI playerNameText; 

    private void Start()
    {
        this.camera = transform.Find("Camera").GetComponent<Camera>();

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine; 
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<VehicleMovement>().isControlEnabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
        }

        SetRacerUI(); 
    }

    private void SetRacerUI()
    {
        if (playerNameText!= null)
        {
            playerNameText.text = photonView.Owner.NickName;
            if (photonView.IsMine)
            {
                playerNameText.gameObject.SetActive(false);
            }
        }
    }
}
