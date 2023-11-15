using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class BulletMechanics : MonoBehaviourPunCallbacks
{
    private float bulletDamage;
    private string bulletOwner; 

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);

        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                string testOwner = bulletOwner;
                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, bulletDamage, testOwner); //, bulletOwner);
            }
            else if (!other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                string testOwner = bulletOwner;
                other.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, bulletDamage, testOwner); //, bulletOwner);
            }
        }
    }

    public void Initialize(Vector3 direction, float speed, float damage, string bulletOwnerPhoton)
    {
        bulletOwner = bulletOwnerPhoton; 
        bulletDamage = damage; 
        transform.forward = direction; 
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;

        Debug.Log("BULLET CHECK OWNER" + bulletOwnerPhoton); 
    } 
}
