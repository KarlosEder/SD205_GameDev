using UnityEngine;

public class CustomProjectile : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask isPlayer;

    // Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    // Damage
    public int explosionDamage;
    public float explosionRange;

    // Lifetime
    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnHit = true;

    int collisionsl;
    PhysicsMaterial physics_mat;

    private void Setup()
    {
        // New physics material
        physics_mat = new PhysicsMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicsMaterialCombine.Minimum;
    }
}
