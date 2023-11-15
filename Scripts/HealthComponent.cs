using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class HealthComponent : MonoBehaviourPunCallbacks
{
    public float maxHealth = 100f;
    public float health;
    public Image healthBar;

    Rigidbody rb;
    public GameObject[] playerMesh;
    public bool isDead = false;

    
    public string playerName;
    private float lastDamageTime = 0f;
    private float damageCooldown = 0.3f;


    private void Start()
    {
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
        rb = GetComponent<Rigidbody>();
        playerName = PhotonNetwork.LocalPlayer.NickName; 
    }

    [PunRPC]
    public void TakeDamage(float damage, string playerWhoHit, PhotonMessageInfo info) //string playerWhoHit)
    {
        if (!isDead && Time.time > lastDamageTime + damageCooldown)
        {
            health -= damage;
            healthBar.fillAmount = health / maxHealth;

            if (health <= 0f)
            {
                PlayerDeath(playerWhoHit, photonView.Owner.NickName);
            }

            lastDamageTime = Time.time; 
        }
    }

    private void PlayerDeath(string bulletSource, string bulletCollided)
    {
        isDead = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        for (int i = 0; i < playerMesh.Length; i++)
        {
            Destroy(playerMesh[i]);
            this.GetComponent<BoxCollider>().enabled = false;
            this.GetComponent<Rigidbody>().useGravity = false;
            this.GetComponent<Shooting>().enabled = false;
        }

        if (photonView.IsMine)
        {
            photonView.RPC("UpdateEliminatedRacer", RpcTarget.AllBuffered, bulletSource, bulletCollided); //, PhotonNetwork.LocalPlayer.NickName);
        }
    }
}
