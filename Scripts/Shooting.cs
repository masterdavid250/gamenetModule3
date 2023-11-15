using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Shooting : MonoBehaviourPunCallbacks
{
    public GameObject bulletPrefab;
    public Transform muzzlePosition;
    public Camera playerCamera;
    public DeathRaceLineup racerProperties;
    public LineRenderer lineRenderer; 
    private float fireRate;
    private float fireDelay = 0f;
    private bool isLaserEquipped; 

    // Start is called before the first frame update
    void Start()
    {
        fireRate = racerProperties.weaponFireRate; 

        if (racerProperties.weaponName == "Laser")
        {
            isLaserEquipped = true; 
        }
        else
        {
            isLaserEquipped = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            return; 
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (fireDelay > fireRate)
            {
                Debug.Log("CHECKPOINT 1"); 
                //Shoot();
                photonView.RPC("Shoot", RpcTarget.AllBuffered, muzzlePosition.position, muzzlePosition.forward); 
                fireDelay = 0f;
            }
        }

        if (fireDelay < fireRate)
        {
            fireDelay += Time.deltaTime; 
        }
    }

    [PunRPC]
    public void Shoot(Vector3 shootingPosition, Vector3 shootingForward)
    {
        if (isLaserEquipped)
        {
            Debug.Log("CHECKPOINT 2");
            RaycastHit hit;
            Ray ray = new Ray(shootingPosition, shootingForward);

            //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Debug.Log("Hit object tag: " + hit.collider.gameObject.tag);

                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                }

                lineRenderer.startWidth = 0.5f;
                lineRenderer.endWidth = 0.3f;

                lineRenderer.SetPosition(0, shootingPosition);
                lineRenderer.SetPosition(1, hit.point);

                if (hit.collider.gameObject.CompareTag("Player"))
                {
                    if (hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                    {
                        string playerWhoHit = PhotonNetwork.LocalPlayer.NickName;
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, racerProperties.weaponDamage, playerWhoHit); //, bulletOwner);
                    }
                    else if (!hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                    {
                        string playerWhoHit = PhotonNetwork.LocalPlayer.NickName;
                        hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, racerProperties.weaponDamage, playerWhoHit); //, bulletOwner);
                    }

                    /*Debug.Log("HIT");
                    PhotonView photonView = hit.collider.gameObject.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        Debug.Log("PhotonView.IsMine: " + photonView.IsMine);
                        string playerWhoHit = PhotonNetwork.LocalPlayer.NickName;
                        photonView.RPC("TakeDamage", RpcTarget.AllBuffered, racerProperties.weaponDamage, playerWhoHit); //, playerWhoHit);
                    }*/
                }
                StopAllCoroutines();
                StartCoroutine(CO_FadeLaser(0.2f));
            }
            else
            {
                Debug.Log("Raycast didn't hit anything.");
            }
        }
        else
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            GameObject bulletGameObject = Instantiate(bulletPrefab, shootingPosition, Quaternion.identity);
            string playerWhoHit = PhotonNetwork.LocalPlayer.NickName;
            bulletGameObject.GetComponent<BulletMechanics>().Initialize(ray.direction, racerProperties.weaponBulletSpeed, racerProperties.weaponDamage, playerWhoHit);
        }
    }

    public IEnumerator CO_FadeLaser(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        lineRenderer.enabled = false; 
    }
}
