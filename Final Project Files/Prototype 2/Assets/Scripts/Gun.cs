using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class Gun : MonoBehaviour
{
    [System.Serializable]
    public class Game
    {
        public bool hitScan;
        public bool spreadShot;
        public bool piercing;
        public bool autoFire;
        public float fireRate;
        public int damage;
        public int maxAmmo;
        public int range;
        public float shootForce;
        public string gun;
        public int index;
        public Game(bool hitScan, bool spreadShot, bool piercing, bool autoFire, float fireRate, int damage, int maxAmmo, int range, float shootForce, int index, string gun) {
            this.hitScan = hitScan;
            this.spreadShot = spreadShot;
            this.piercing = piercing;
            this.autoFire = autoFire;
            this.fireRate = fireRate;
            this.damage = damage;
            this.maxAmmo = maxAmmo;
            this.range = range;
            this.shootForce = shootForce;
            this.index = index;
            this.gun = gun;
        }
    }

    public GameObject bullet;
    public ParticleSystem flash;
    private int rand1, rand2;
    public Image[] card;
    public Camera fpsCam;
    public Transform attackPoint;
    public float spread;
    private float nextFire = 0;
    public Transform muzzle;
    public bool shooting, reloading;
    public int randomAccess;
    public Game[] pool;
    public List<Game> deck = new();
    public List<Game> tempDeck;
    public Game[] hand;
    public Game temp = new(false, false, false, false, 1, 1, 1, 1, 1, 1, "temp");

    // Start is called before the first frame update
    void Start()
    {
        reloading = false;
        hand = new Game[2];
        hand[0] = new(false, false, false, false, 1, 1, 1, 1, 1, 1, "0");
        hand[1] = new(false, false, false, false, 1, 1, 1, 1, 1, 1, "1");
        pool = new Game[4];

        /* Order of parameters: hitScan, spreadShot, piercing, autoFire, firerate, damage, maxAmmo, range, shootForce, gun)*/
        //Shotgun stats
        pool[0] = new(false, true, false, false, 2, 20, 8, 100, 200, 0, "shotgun");

        //Rifle stats
        pool[1] = new(true, true, false, true, 5, 15, 60, 100, 150, 1, "rifle");

        //Pistol stats
        pool[2] = new(false, false, false, false, 3, 12, 10, 100, 100, 2, "pistol");

        //Sniper stats
        pool[3] = new(true, false, true, false, 1, 30, 5, 100, 400, 3, "sniper");

        deck.Add(pool[0]);
        deck.Add(pool[1]);
        deck.Add(pool[2]);
        deck.Add(pool[3]);

        Shuffle();
        StartCoroutine(Draw());
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.R) || temp.maxAmmo <= 0) && !reloading) {
            StartCoroutine(Draw());
        }

        if (temp.autoFire)
        {
            shooting = Input.GetMouseButton(0);
        } else
        {
            shooting = Input.GetMouseButtonDown(0);
        }

        if (shooting && !reloading && temp.maxAmmo > 0 && temp.hitScan && Time.time > nextFire)
        {
            Debug.Log("Shooting");
            nextFire = Time.time + 1 / temp.fireRate;
            Shoot();
        }

        if (shooting && !reloading && temp.maxAmmo > 0 && !temp.hitScan)
        {
            Debug.Log("Projectile");
            ProjectileShoot();
        }
    }

    IEnumerator Draw()
    {
        ResetCard();
        Debug.Log($"{tempDeck[0].index}, {tempDeck[1].index}");
        Debug.Log("Reloading");
        reloading = true;
        yield return new WaitForSeconds(1f);
        randomAccess = Random.Range(0, tempDeck.Count);
        hand[0] = tempDeck[randomAccess];
        tempDeck.Remove(hand[0]);
        randomAccess = Random.Range(0, tempDeck.Count);
        hand[1] = tempDeck[randomAccess];
        Debug.Log($"{hand[0].index}, {hand[1].index}");
        tempDeck.Remove(hand[1]);

        if (hand[0].spreadShot || hand[1].spreadShot)
        {
            temp.spreadShot = true;
        }
        else
        {
            temp.spreadShot = false;
        }

        if (hand[0].piercing || hand[1].piercing)
        {
            temp.piercing = true;
        }
        else
        {
            temp.piercing = false;
        }

        if (hand[0].autoFire || hand[1].autoFire)
        {
            temp.autoFire = true;
        }
        else
        {
            temp.autoFire = false;
        }

        if (hand[0].hitScan || hand[1].hitScan)
        {
            temp.hitScan = true;
        }
        else
        {
            temp.hitScan = false;
        }
        if (tempDeck.Count <= 1)
        {
            Reshuffle();
        }
        SpawnCard();
        temp.damage = (hand[0].damage + hand[1].damage);
        temp.damage = hand[0].damage + hand[1].damage;
        temp.fireRate = (hand[0].fireRate + hand[1].fireRate);
        temp.maxAmmo = hand[0].maxAmmo + hand[1].maxAmmo;
        temp.range = (hand[0].range + hand[1].range);
        temp.shootForce = (hand[0].shootForce + hand[1].shootForce);
        Debug.Log($"{hand[0].gun}, {hand[1].gun}");

        
        reloading = false;
    }

    void Shoot()
    {
        temp.maxAmmo--;
        flash.Play();

        if (Physics.Raycast(muzzle.transform.position, muzzle.transform.forward, out RaycastHit hit, temp.range))
        {
            //Stores the data of the object that was hit into an Enemy type variable
            EnemyAI successHit = hit.transform.GetComponent<EnemyAI>();
            //Ensures damage won't be applied to objects we don't wish it to (environment, background, etc.)
            if (successHit != null)
            {
                successHit.TakeDamage(temp.damage);
            }
        }
    }

    void ProjectileShoot()
    {
        temp.maxAmmo--;
        flash.Play();
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        EnemyAI successHit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            targetPoint = hit.point;
            successHit = hit.transform.GetComponent<EnemyAI>();
            if (successHit != null)
            {
                successHit.TakeDamage(temp.damage);
            }
        }
        else
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
        currentBullet.GetComponent<Rigidbody>().AddForce(directionPlusSpread.normalized * temp.shootForce, ForceMode.Impulse);
    }

    void Shuffle()
    {
        Game temporary;
        for (int i = 0; i < deck.Count; i++)
        {
            temporary = deck[i];
            int random = Random.Range(i, deck.Count);
            deck[i] = deck[random];
            deck[random] = temporary;
        }
        tempDeck = DeepCopy(deck);
    }

    //Allows seeting one list to another with the use of an instance, rather than a reference.
    //Source: https://stackoverflow.com/questions/13447248/how-to-assign-listt-without-it-being-a-reference-to-the-original-listt
    public static Game DeepCopy<Game>(Game item)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, item);
        stream.Seek(0, SeekOrigin.Begin);
        Game result = (Game)formatter.Deserialize(stream);
        stream.Close();
        return result;
    }

    void Reshuffle()
    {
        tempDeck.Clear();
        Shuffle();
    }

    void ResetCard()
    {
        for (int i = 0; i < card.Length; i++)
        {
            card[i].color = Color.white;
        }
    }

    void SpawnCard()
    {
        for (int i = 0; i < card.Length; i++)
        {
            if (i != hand[0].index && i != hand[1].index)
            {
                card[i].color = Color.clear;
            }
        }
        /*card[rand1].color = Color.blue;
        card[rand2].color = Color.blue;*/
    }
}
