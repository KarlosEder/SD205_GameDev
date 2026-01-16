using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gun : MonoBehaviour
{
    // Weapon properties
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 5f;
    public float knockback = 100;
    public float maxMagazineSize = 21;
    public float magazineSize = 21;
    public float reloadSpeed = 1.5f;

    private float nextTimeToFire = 0f;

    public bool isFiring;
    public bool isReloading;
    public TextMeshProUGUI ammoCounter;

    // SFX
    public AudioSource audioSource;
    public AudioClip dryFireClip;
    public AudioClip gunshotClip;
    public AudioClip reloadMag;
    public AudioClip reloadCock;

    public bool hasDryFired;

    // Particle system
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactParticles;

    public GameObject bulletTracer;
    public Transform firePoint;

    void Update()
    {
        // Update ammo counter
        ammoCounter.text = magazineSize.ToString();

        // Manual reload
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && magazineSize < maxMagazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        // Block firing while reloading
        if (isReloading)
            return;

        // Full auto
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            if (magazineSize > 0)
            {
                // Fire normally
                magazineSize--;
                hasDryFired = false;

                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
            else
            {
                // Ammo empty: play dry-fire once and stop firing
                if (!hasDryFired)
                {
                    if (audioSource != null && dryFireClip != null)
                    {
                        audioSource.pitch = Random.Range(0.95f, 1.05f);
                        audioSource.PlayOneShot(dryFireClip, 1f);
                        audioSource.pitch = 1f;
                    }

                    hasDryFired = true;
                    nextTimeToFire = Time.time + 0.2f;
                }
                else
                {
                    // Second click auto reloads
                    if (!isReloading)
                    {
                        StartCoroutine(Reload());
                    }
                }
            }
        }
    }

    // Reload
    IEnumerator Reload()
    {
        isReloading = true;
        ammoCounter.text = "RELOADING";

        // Mag removal/insert sound 
        if (audioSource != null && reloadMag != null)
        {
            audioSource.PlayOneShot(reloadMag, 1f);
        }

        // Wait
        yield return new WaitForSeconds(reloadSpeed * 0.7f); // e.g., magazine takes 70% of the reload

        // Play cocking sound after inserting mag
        if (audioSource != null && reloadCock != null)
        {
            audioSource.PlayOneShot(reloadCock, 1f);
        }

        // Finish reload
        yield return new WaitForSeconds(reloadSpeed * 0.3f); // remaining time

        magazineSize = maxMagazineSize;
        hasDryFired = false;
        isReloading = false;

        // Add a slight delay before shooting again
        nextTimeToFire = Time.time + 0.2f; 
    }

    // Bullet tracer
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

    // Fire weapon
    void Shoot()
    {
        // Play VFX
        muzzleFlash.Play();

        // Play SFX
        if (audioSource != null && gunshotClip != null)
        {
            // Randomize pitch
            audioSource.pitch = Random.Range(0.4f, 1f);

            // Randomize volume 
            float volume = Random.Range(0.8f, 1.5f);

            audioSource.PlayOneShot(gunshotClip, volume);

            // Reset pitch to avoid affecting other sounds
            audioSource.pitch = 1f;
        }

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
