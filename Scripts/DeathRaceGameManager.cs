using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DeathRaceGameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] vehiclePrefabs;
    public Transform[] startingPositions;
    public GameObject[] finisherTextUi;

    public static DeathRaceGameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, startingPositions[actorNumber - 1].rotation);
            }
        }

        foreach (GameObject go in finisherTextUi)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void QuitButton()
    {
        PhotonNetwork.LeaveRoom(); 
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene"); 
    }
}
