using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    // Weapon properties
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 5f;
    public float knockback = 100;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactParticles;

    public GameObject bulletTracer;
    public Transform firePoint;

    private float nextTimeToFire = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    void SpawnTracer(Vector3 hitpoint)
    {
        GameObject tracerGO = Instantiate(bulletTracer, firePoint.position, Quaternion.identity);
        TrailRenderer tracer = tracerGO.GetComponent<TrailRenderer>();

        StartCoroutine(AnimateTracer(tracer, hitpoint));
    }

    IEnumerator AnimateTracer(TrailRenderer tracer, Vector3 hitpoint)
    {
        float time = 0;
        Vector3 startPos = tracer.transform.position;

        while (time < 1)
        {
            tracer.transform.position = Vector3.Lerp(startPos, hitpoint, time);
            time += Time.deltaTime * 100;
            yield return null;
        }

        tracer.transform.position = hitpoint;
        Destroy(tracer.gameObject, tracer.time);
    }

    void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        Vector3 hitPoint;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            hitPoint = hit.point;

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * knockback);
            }

            GameObject impactGO = Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
            Debug.Log("PE");
            Destroy(impactGO, 1f);
        }
        else
        {
            hitPoint = fpsCam.transform.position + fpsCam.transform.forward * range;
        }

        SpawnTracer(hitPoint);
    }
}
