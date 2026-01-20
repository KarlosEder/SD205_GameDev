using UnityEngine;

public class EnemyProjectiles : MonoBehaviour
{
    // Bullet
    public GameObject bullet;

    // Bullet force
    public float shootForce, upwardForce;

    // Projectile stats
    public float timeBetweenShooting, spread, reloadSpeed, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    float reloadTime;

    // Bools
    bool shooting, readyToShoot, reloading;

    // Ref
    public Camera fpsCam;
    public Transform attackPoint;

    public bool allowInvoke = true;

    private void Awake()
    {
        // Check if mag is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        // Check if allow to holf fire button
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Reload
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        // Auto reload
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        // Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            // Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Find hit position with raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        // Check if ray hits anything
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else 
            targetPoint = ray.GetPoint(75);

        // Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        // Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Recalculate direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        // Rotate projectile to shot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        // Add force
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse); // For bouncing projectiles

        bulletsLeft--;
        bulletsShot++;

        // Invoke resetShot function (if not already)
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        // If firing multiple projectiles per shot
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
