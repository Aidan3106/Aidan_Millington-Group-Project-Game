using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boss : MonoBehaviour
{
    public int bossHP = 10;
    int bulletSpeed = 5;
    float bossMoveSpeed = 5f;
    Vector2 currentVelo;
    Vector2 prevVelo;
    string sceneName;
    float horizontal;
    GameObject player;
    GameObject[] bossGoRndPoints;

    //sound part
    public AudioClip getHitSound;
    public AudioClip deathSound;
    public AudioClip rangedAttackSound;
    AudioSource audioSource;

    //timer to fire more projectiles
    float waitTime = 0.5f;
    private float timer = 0.0f;

    TextMesh bossHPText;

    Rigidbody2D bossBody;

    //projectile
    public GameObject bulletPreFab;

    private void Start()
    {
        bossHPText = this.transform.Find("bossHP").GetComponent<TextMesh>();
        bossHPText.text = bossHP.ToString();
        //StartCoroutine(bossShooting()); <- update used instead
        bossBody = GetComponent<Rigidbody2D>();
        sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Map1":
            case "Map4":
                bossBody.velocity = new Vector2(-1 * bossMoveSpeed, 0f);
                break;
            case "Map5":
                bossGoRndPoints = GameObject.FindGameObjectsWithTag("bossGoRandom");
                bossGoTowardsRandomPoint(bossGoRndPoints);
                break;
        }
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        currentVelo = bossBody.velocity;
        prevVelo = currentVelo;
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            timer -= waitTime;
            Shoot();
        }
        switch (sceneName)
        {
            case "Map2":
            case "Map3":
                horizontal = Input.GetAxisRaw("Horizontal");
                bossBody.velocity = new Vector2(horizontal * bossMoveSpeed, 0f);
                break;
            case "Map4":
            case "Map5":
                player = GameObject.Find("player");
                break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile")
        {
            bossHP--;
            Destroy(collision.gameObject);
            bossHPText.text = bossHP.ToString();
            switch (sceneName)
            {
                case "Map2":
                case "Map3":
                    waitTime -= 0.02f;
                    break;
                case "Map4":
                case "Map5":
                    waitTime -= 0.049f;
                    bossMoveSpeed += 1;
                    break;
            }
            audioSource.PlayOneShot(getHitSound, 1);
            if (bossHP < 1)
            {
                StartCoroutine(bossDeath());
            }
        }

        if (collision.gameObject.tag == "wall")
        {
            if (sceneName == "Map1")
            {
                bossBody.velocity = new Vector2(-1 * prevVelo.x, 0f);
            }
            if (sceneName == "Map5")
            {
                bossBody.velocity = -1 * prevVelo;
            }
        }

        if (sceneName == "Map4")
        { 
            switch (collision.gameObject.tag)
            {
                case "bossGoDown":
                    bossBody.velocity = new Vector2(0f, -1 * bossMoveSpeed);
                    break;
                case "bossGoUp":
                    bossBody.velocity = new Vector2(0f, bossMoveSpeed);
                    break;
                case "bossGoLeft":
                    bossBody.velocity = new Vector2(-1 * bossMoveSpeed, 0f);
                    break;
                case "bossGoRight":
                    bossBody.velocity = new Vector2(bossMoveSpeed, 0f);
                    break;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (sceneName == "Map5")
        {
            if (other.gameObject.tag == "bossGoRandom")
            {
                if (Mathf.Abs(other.gameObject.transform.position.x - bossBody.transform.position.x) < 0.1f & Mathf.Abs(other.gameObject.transform.position.y - bossBody.transform.position.y) < 0.1f)
                {
                    bossGoTowardsRandomPoint(bossGoRndPoints);
                }
            }
        }
    }
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPreFab) as GameObject;
        bullet.gameObject.layer = 11;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position;
        switch (sceneName)
        {
            case "Map1":
            case "Map2":
            case "Map3":
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -1 * bulletSpeed + bossBody.velocity.y);
                break;
            case "Map4":
            case "Map5":
                bullet.transform.up = new Vector3(
                    player.transform.position.x - transform.position.x,
                    player.transform.position.y - transform.position.y,
                    0f);
                bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * bulletSpeed;
                break;
        }
        audioSource.PlayOneShot(rangedAttackSound, 1);
    }
    IEnumerator bossShooting()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            //Shoot();
        }
    }
    IEnumerator bossDeath()
    {
        Time.timeScale = 0;
        audioSource.PlayOneShot(deathSound, 1);
        yield return new WaitWhile(() => audioSource.isPlaying);
        Time.timeScale = 1;
        switch (sceneName)
        {
            case "Map1":
                SceneManager.LoadScene("Map2", LoadSceneMode.Single);
                break;
            case "Map2":
                SceneManager.LoadScene("Map3", LoadSceneMode.Single);
                break;
            case "Map3":
                SceneManager.LoadScene("Map4", LoadSceneMode.Single);
                break;
            case "Map4":
                SceneManager.LoadScene("Map5", LoadSceneMode.Single);
                break;
            case "Map5":
                SceneManager.LoadScene("Credits", LoadSceneMode.Single);
                break;
        }
    }
    void bossGoTowardsRandomPoint(GameObject[] points)
    {
        int rnd = Random.Range(0, points.Length);
        GameObject destPoint = points[rnd];
        Vector2 direction = new Vector2(destPoint.transform.position.x - transform.position.x, destPoint.transform.position.y - transform.position.y);
        bossBody.velocity = new Vector2(direction.normalized.x * bossMoveSpeed, direction.normalized.y * bossMoveSpeed);
    }
}