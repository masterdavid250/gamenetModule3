using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun; 

public class HealthComponent : MonoBehaviourPunCallbacks
{
    public float maxHealth = 100f;
    private float health;
    public Image healthBar;

    Rigidbody rb;
    public GameObject[] playerMesh;


    private void Start()
    {
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.fillAmount = health / maxHealth; 

        if (health <= 0f)
        {
            PlayerDeath();
        }
    }

    private void PlayerDeath()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        for (int i = 0; i < playerMesh.Length; i++)
        {
            Destroy(playerMesh[i]);
            this.GetComponent<BoxCollider>().enabled = false;
            this.GetComponent<Rigidbody>().useGravity = false;
        }

        if (photonView.IsMine)
        {

        }
    }
}
