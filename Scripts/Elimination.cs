using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class Elimination : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI eliminatedText;
    public TextMeshProUGUI whoKilledWhoText;
    public TextMeshProUGUI winnerText;

    private int playersAlive;
    private HealthComponent[] players;
    private string killerName; 

    private void Start()
    {
        eliminatedText.text = "";
        whoKilledWhoText.text = "";
        winnerText.text = "";

        players = FindObjectsOfType<HealthComponent>();
        playersAlive = players.Length;
    }

    [PunRPC]
    public void UpdateEliminatedRacer(string bulletSource, string bulletCollision) //(string playerWhoHit, string theEliminatedPlayer) 
    {
        //TO CHANGE
        eliminatedText.text = bulletCollision + " Killed";
        whoKilledWhoText.text = bulletSource + " Eliminated " + bulletCollision;
        killerName = bulletSource; 
        Invoke(nameof(HideEliminatedRacer), 2f);
        
        playersAlive--;

        
        if (playersAlive == players.Length - 2)
        {
            //CheckWinner();
        }
    }

    private void HideEliminatedRacer()
    {
        if (eliminatedText.text != "")
        {
            eliminatedText.text = "";
        }
        if (whoKilledWhoText.text != "")
        {
            whoKilledWhoText.text = "";
        }
    }

    private void CheckWinner()
    {
        string winnerName = GetWinnerName();
        if (!string.IsNullOrEmpty(winnerName))
        {
            photonView.RPC("BroadcastWinnerInfo", RpcTarget.AllBuffered, winnerName);
        }
    }

    [PunRPC]
    private void BroadcastWinnerInfo(string winnerName)
    {
        winnerText.text = "Winner: " + winnerName;
    }

    private string GetWinnerName()
    {
        string winnerName = "";

        int aliveCount = 0;
        foreach (HealthComponent player in players)
        {
            if (player.health > 0)
            {
                aliveCount++;
                winnerName = killerName; 
            }
        }

        return (aliveCount == 1) ? winnerName : ""; 
    }
}
