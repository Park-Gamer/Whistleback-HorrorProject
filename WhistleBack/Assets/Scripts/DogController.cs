using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DogController : MonoBehaviour
{
    private bool canShoot = true;

    public float rayDistance = 100f;
    public LayerMask groundLayers;
    public bool debugRay = true;
    public NavMeshAgent agent;
    public Camera cam;
    public Transform player;
    

    private GameObject spawnedMarker;
    public GameObject marker;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            canShoot = false;
            StartCoroutine(WaitBeforeContinuing());
            ShootRay();
            Debug.Log("Shoot");
        }

        
    }

    void ShootRay()
    {
        Vector3 origin = player.transform.position;
        Vector3 direction = player.transform.forward;

        if (debugRay)
        {
            Debug.DrawRay(origin, direction * rayDistance, Color.red, 1f);
        }

        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, rayDistance, groundLayers))
        {
            Debug.Log("Hit");
            Vector3 pos = hitInfo.point;
            agent.SetDestination(pos);

            spawnedMarker = Instantiate(marker, pos, Quaternion.identity);

            Vector3 lookDirection = (pos - transform.position).normalized;
            if (lookDirection != Vector3.zero) // Ensure we have a valid direction
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 200f * Time.deltaTime);
            }
        }
    }

    IEnumerator WaitBeforeContinuing()
    {
        yield return new WaitForSeconds(5f);
        Destroy(spawnedMarker);
        canShoot = true;
    }
}
