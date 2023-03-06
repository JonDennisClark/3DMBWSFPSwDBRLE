using UnityEngine;
using System.Collections;
using TMPro;

public class ProjectileShooting : MonoBehaviour
{
    public GameObject bullet;
    public float shootForce, upwardForce;
    public float damage;
    public float reloadTime, timeBetweenShots, spread, timeBetweenShooting;
    public int magazineSize, bulletsPerTap;
    public bool fullAuto;
    int bulletsLeft, bulletsShot;
    bool readyToShoot, shooting, reloading;
    bool allowInvoke = true;

    public Camera fpsCam;
    public Transform attackPoint;
    // Start is called before the first frame update

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    void Start()
    {
        
    }

    private void OnEnable()
    {
        reloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (fullAuto)
        {
            shooting = Input.GetMouseButton(0);
        } else
        {
            shooting = Input.GetMouseButtonDown(0);
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;
            Shoot();
        }

        if ((Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) || bulletsLeft <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Ray that points to the middle of the screen
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Enemy successHit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
            successHit = hit.transform.GetComponent<Enemy>();
            if (successHit != null)
            {
                successHit.TakeDamage(damage);
            }
            if (hit.rigidbody != null)
            {
                //Debug.Log("tes");
                hit.rigidbody.AddForce(-hit.normal * shootForce);
            }
        } else
        {
            targetPoint = ray.GetPoint(75);
        }
        Vector3 directionNoSpread = targetPoint - attackPoint.position;

        float xSpread = Random.Range(-spread, spread);
        float ySpread = Random.Range(-spread, spread);
        float zSpread = Random.Range(-spread, spread);

        Vector3 directionPlusSpread = directionNoSpread + new Vector3(xSpread, ySpread, zSpread);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //Rotates bullet to face direction of shooting.
        currentBullet.transform.forward = directionPlusSpread.normalized;
        //Puts force behind the bullet.
        currentBullet.GetComponent<Rigidbody>().AddForce(directionPlusSpread.normalized * shootForce, ForceMode.Impulse);
        //Used for things such as bouncing grenades.
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //Allows for shooting multiple bullets per click, i.e. a shotgun
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

    IEnumerator Reload()
    {
        Debug.Log("Reload time");
        reloading = true;

        yield return new WaitForSeconds(reloadTime - 0.25f);
        yield return new WaitForSeconds(0.25f);
        bulletsLeft = magazineSize;

        reloading = false;
    }
}
