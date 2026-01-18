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

    // Recoil
    public Transform gunPivot;
    public Vector3 recoilKickPosition = new Vector3(0f, 0.05f, -0.2f);  // Up/back
    public Vector3 recoilKickRotation = new Vector3(10f, 1f, 0f);        // Pitch/yaw
    public float recoilRecoverySpeed = 8f;

    private Vector3 initialLocalPos;
    private Quaternion initialLocalRotation;
    private Vector3 recoilPosition;
    private Vector3 targetRecoilPosition;
    private Vector3 recoilRotationEuler;
    private Vector3 targetRecoilRotationEuler;

    // Sway
    public float swayAmount = 0.02f;
    public float swaySmooth = 4f;

    // DOT
    public float burnDamage = 0f;
    public float burnDuration = 3f;
    public float burnTickRate = 0.5f;

    private float nextTimeToFire = 0f;
    public bool isFiring;
    public bool isReloading;
    public TextMeshProUGUI ammoCounter;

    // SFX
    public AudioSource audioSource;
    public AudioClip dryFireClip;
    public AudioClip[] gunshotClips;
    public AudioClip reloadMag;
    public AudioClip reloadCock;

    private int lastShotIndex = -1;
    public bool hasDryFired;

    // Particle system
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactParticles;

    public GameObject bulletTracer;
    public Transform firePoint;

    // Animations
    public Transform gunMesh;
    private Animator gunAnimator;

    // Store initial position
    private void Start()
    {
        initialLocalPos = gunPivot.localPosition;
        initialLocalRotation = gunPivot.localRotation;

        gunAnimator = gunMesh.GetComponent<Animator>();
    }

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
        ApplySway();
        ApplyRecoilRecovery();
    }

    // Fire weapon
    void Shoot()
    {
        // Animations
        if (gunAnimator != null)
            gunAnimator.SetTrigger("Fire");

        // Play VFX
        muzzleFlash.Play();

        // Play SFX
        if (audioSource != null && gunshotClips != null && gunshotClips.Length > 0)
        {
            // Select random audio clip
            int index;
            do
            {
                index = Random.Range(0, gunshotClips.Length);
            }
            while (index == lastShotIndex && gunshotClips.Length > 1);

            // Stop from repeating same audio clip
            lastShotIndex = index;
            AudioClip clip = gunshotClips[index];

            // Variation in volume / pitch
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            float volume = Random.Range(0.9f, 1.1f);

            audioSource.PlayOneShot(clip, volume);
            audioSource.pitch = 1f;
        }

        // Raycast
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

                if (burnDamage > 0)
                {
                    // TEST
                    Debug.Log($"Applying burn: {burnDamage} damage over {burnDuration}s to {target.name}");

                    target.ApplyBurn(burnDamage, burnDuration, burnTickRate);
                }
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

        ApplyRecoil();
    }
    private void ApplyRecoil()
    {
        targetRecoilPosition += recoilKickPosition;
        targetRecoilRotationEuler += new Vector3(-recoilKickRotation.x, Random.Range(-recoilKickRotation.y, recoilKickRotation.y), 0f);
    }

    private void ApplySway()
    {
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        Vector3 swayOffset = new Vector3(-mouseY, mouseX, 0f);
        gunPivot.localPosition = initialLocalPos + recoilPosition + swayOffset * 0.05f; // small sway
        gunPivot.localRotation = initialLocalRotation * Quaternion.Euler(recoilRotationEuler + swayOffset * 2f);
    }

    private void ApplyRecoilRecovery()
    {
        targetRecoilPosition = Vector3.Lerp(targetRecoilPosition, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
        recoilPosition = Vector3.Lerp(recoilPosition, targetRecoilPosition, recoilRecoverySpeed * Time.deltaTime);

        targetRecoilRotationEuler = Vector3.Lerp(targetRecoilRotationEuler, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
        recoilRotationEuler = Vector3.Lerp(recoilRotationEuler, targetRecoilRotationEuler, recoilRecoverySpeed * Time.deltaTime);
    }

    // Reload
    IEnumerator Reload()
    {
        isReloading = true;

        // Animations
        if (gunAnimator != null)
            gunAnimator.SetTrigger("Reload");

        // Reset recoil on reload
        targetRecoilPosition = Vector3.zero;
        recoilPosition = Vector3.zero;
        targetRecoilRotationEuler = Vector3.zero;
        recoilRotationEuler = Vector3.zero;
        gunPivot.localPosition = initialLocalPos;
        gunPivot.localRotation = initialLocalRotation;

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
}
   


