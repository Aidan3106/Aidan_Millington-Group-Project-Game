using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boss : MonoBehaviour
{
    public int bossHP = 10;
    int bulletSpeed = 5;
    float bossMoveSpeed = 5;
    float currentVelo;
    float prevVelo;

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
        StartCoroutine(bossShooting());
        bossBody = GetComponent<Rigidbody2D>();
        bossBody.velocity = new Vector2(-1 * bossMoveSpeed, 0f);
        audioSource = GetComponent<AudioSource>();
        //audioSource.volume = 0.2f;
    }
    private void Update()
    {
        currentVelo = bossBody.velocity.x;
        prevVelo = currentVelo;
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            timer -= waitTime;
            Shoot();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile")
        {
            bossHP--;
            Destroy(collision.gameObject);
            bossHPText.text = bossHP.ToString();
            waitTime -= 0.049f;
            audioSource.PlayOneShot(getHitSound, 1);
            if (bossHP < 1)
            {
                StartCoroutine(bossDeath());
            }
        }

        if (collision.gameObject.tag == "wall")
        {
            bossBody.velocity = new Vector2(-1 * prevVelo, 0f);
        }
    }
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPreFab) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -bulletSpeed);
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
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}