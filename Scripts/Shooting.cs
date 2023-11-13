using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 

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
                //Shoot();
                photonView.RPC("Shoot", RpcTarget.AllBuffered, muzzlePosition.position); 
                fireDelay = 0f;
            }
        }

        if (fireDelay < fireRate)
        {
            fireDelay += Time.deltaTime; 
        }
    }

    [PunRPC]
    public void Shoot(Vector3 shootingPosition)
    {
        if (isLaserEquipped)
        {
            RaycastHit hit;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            Debug.DrawRay(ray.origin, ray.direction * 200, Color.red); 

            if (Physics.Raycast(ray, out hit, 200))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name); 

                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                }

                lineRenderer.startWidth = 0.5f;
                lineRenderer.endWidth = 0.3f;

                lineRenderer.SetPosition(0, shootingPosition);
                lineRenderer.SetPosition(1, hit.point);

                if (hit.collider.gameObject.GetComponent<PlayerSetup>() != null)
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, racerProperties.weaponDamage);
                }

                StopAllCoroutines();
                StartCoroutine(CO_FadeLaser(0.2f));
            }

            /* RaycastHit hit;
             Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

             if (Physics.Raycast(ray, out hit, 200))
             {
                 Debug.Log("Hit object tag: " + hit.collider.gameObject.tag);

                 if (!lineRenderer.enabled)
                 {
                     lineRenderer.enabled = true;
                 }

                 lineRenderer.startWidth = 0.3f;
                 lineRenderer.endWidth = 0.1f;

                 lineRenderer.SetPosition(0, shootingPosition);
                 lineRenderer.SetPosition(1, hit.point);

                 if (hit.collider.gameObject.CompareTag("Player"))
                 {
                     Debug.Log("HIT");

                     PhotonView photonView = hit.collider.gameObject.GetComponent<PhotonView>();
                     if (photonView != null)
                     {
                         Debug.Log("PhotonView.IsMine: " + photonView.IsMine);
                         photonView.RPC("TakeDamage", RpcTarget.AllBuffered, racerProperties.weaponDamage);
                     }
                 }
                 StopAllCoroutines();
                 StartCoroutine(CO_FadeLaser(0.2f));
             }*/
            /* RaycastHit hit; 
             Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

             if (Physics.Raycast(ray, out hit, 200))
             {
                 if (!lineRenderer.enabled)
                 {
                     lineRenderer.enabled = true;
                 }

                 lineRenderer.startWidth = 0.8f;
                 lineRenderer.endWidth = 0.4f; 

                 lineRenderer.SetPosition(0, shootingPosition);
                 lineRenderer.SetPosition(1, hit.point);

                 *//*if (hit.collider.CompareTag("Player"))
                 {
                     if (hit.collider.GetComponent<PhotonView>().IsMine)
                     {
                         hit.collider.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, racerProperties.weaponDamage);
                     }
                 }*/

            /*if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("HIT");
                if (hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, racerProperties.weaponDamage);
                }
            }*//*

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("HIT");
                PhotonView photonView = hit.collider.gameObject.GetComponent<PhotonView>();
                if (photonView != null && photonView.IsMine)
                {
                    photonView.RPC("TakeDamage", RpcTarget.All, racerProperties.weaponDamage);
                }
            }

            StopAllCoroutines();
            StartCoroutine(CO_FadeLaser(0.2f));
        }*/
        }
        else
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            GameObject bulletGameObject = Instantiate(bulletPrefab, shootingPosition, Quaternion.identity);
            bulletGameObject.GetComponent<BulletMechanics>().Initialize(ray.direction, racerProperties.weaponBulletSpeed, racerProperties.weaponDamage);
        }
    }

    public IEnumerator CO_FadeLaser(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        lineRenderer.enabled = false; 
    }
}
