using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    public Rigidbody bulletBody;
    public GameObject explosion;
    public LayerMask enemy;

    //Bullet stats
    [Range(0f, 1f)]
    public float bulletBounce;
    public float maxLife;
    public bool gravity;
    public int damage;
    public float explosionRadius;

    //Detect if bullet should destroy
    public int maxCollisions;
    public bool contactExplode = true;

    int collisions;
    PhysicMaterial physic_mat;

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        maxLife -= Time.deltaTime;
        if (collisions > maxCollisions || maxLife <= 0)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;

        if (collision.collider.CompareTag("Enemy") && contactExplode)
        {
            Debug.Log("Explosion!");
            Explode();
        }
    }

    private void Setup()
    {
        //Establishes values for new phsyics material
        physic_mat = new PhysicMaterial();
        physic_mat.bounciness = bulletBounce;
        physic_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physic_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assigns material to our bullet collider
        GetComponent<SphereCollider>().material = physic_mat;

        bulletBody.useGravity = gravity;
    }

    private void Explode()
    { 
        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
