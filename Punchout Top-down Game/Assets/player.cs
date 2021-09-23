using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;
    float origYPosition;
    string sceneName;

    //sound part
    public AudioClip rangedAttackSound;
    public AudioClip blockSound;
    public AudioClip deathSound;
    public AudioClip dodgeSound;
    public AudioClip reloadSound;
    AudioSource audioSource;

    // Bullet part
    public GameObject bulletPreFab;
    public float bulletSpeed;

    //player stats
    public float HP = 10.0f;
    public float runSpeed = 5.0f;
    float baseHP;
    TextMesh playerHPText;
    int ammoCapacity = 10;
    int baseAmmoCapacity;
    int playerStamina = 10;
    int basePlayerStamina;
    TextMesh ammoText;
    TextMesh playerStaminaText;
    bool blocking = false;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        origYPosition = body.position.y;
        baseAmmoCapacity = ammoCapacity;
        StartCoroutine(ammoReload());
        ammoText = this.transform.Find("ammoText").GetComponent<TextMesh>();
        ammoText.text = baseAmmoCapacity.ToString();
        basePlayerStamina = playerStamina;
        playerStaminaText = this.transform.Find("playerStamina").GetComponent<TextMesh>();
        playerStaminaText.text = basePlayerStamina.ToString();
        baseHP = HP;
        playerHPText = this.transform.Find("playerHP").GetComponent<TextMesh>();
        playerHPText.text = baseHP.ToString();
        audioSource = GetComponent<AudioSource>();
        sceneName = SceneManager.GetActiveScene().name;
        //Physics2D.IgnoreCollision(.GetComponent<Collider2D>(), GetComponent<Collider2D>()); <- make the player ignore the bossmoving objects
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Fire1") && blocking == false)
        {
            if (ammoCapacity > 0)
            {
                Shoot();
                ammoCapacity--;
                ammoText.text = ammoCapacity.ToString();
            }
        }
        if (Input.GetButtonDown("Fire2") && playerStamina > 0)
        {
            blocking = true;
            Block();
            audioSource.PlayOneShot(blockSound, 1);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            blocking = false;
        }
        if (Input.GetButtonDown("Cancel"))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
    private void FixedUpdate()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        body.velocity = new Vector2(horizontal * runSpeed, 0f);
        switch (sceneName) {
            case "Map2":
                transform.up = new Vector3(
                    mousePosition.x - transform.position.x,
                    mousePosition.y - transform.position.y,
                    0f);
                break;
            case "Map3":
            case "Map4":
            case "Map5":
                body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
                transform.up = new Vector3(
                    mousePosition.x - transform.position.x,
                    mousePosition.y - transform.position.y,
                    0f);
                break;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile")
        {
            if (!blocking)
            {
                reduceHP();
            }
            Destroy(collision.gameObject);
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
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, bulletSpeed);
                break;
            case "Map2":
            case "Map3":
            case "Map4":
            case "Map5":
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.up.x * bulletSpeed, transform.up.y * bulletSpeed);
                bullet.transform.up = transform.up;
                break;
        }
        audioSource.PlayOneShot(rangedAttackSound, 1);
    }
    private void Block()
    {
        if (playerStamina > 0)
        {
            playerStamina--;
            playerStaminaText.text = playerStamina.ToString();
            //TODO: blocking script to ignore damage from boss and drain some stamina - stamina drain is done
        }
        //TODO: without stamina, you cannot block
    }
    private void reduceHP()
    {
        HP--;
        playerHPText.text = HP.ToString();
        if (HP == 0f)
        {
            StartCoroutine(playerDeath());
        }
    }

    IEnumerator ammoReload()
    {
        while (true) {
            yield return new WaitForSecondsRealtime(4);
            if (ammoCapacity < 10)
            {
                audioSource.PlayOneShot(reloadSound, 1);
                yield return new WaitWhile(() => audioSource.isPlaying);
                ammoCapacity = baseAmmoCapacity;
                ammoText.text = ammoCapacity.ToString();
            }
        }
    }
    IEnumerator playerDeath()
    {
        Time.timeScale = 0;
        audioSource.PlayOneShot(deathSound, 1);
        yield return new WaitWhile(() => audioSource.isPlaying);
        Time.timeScale = 1;
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }
}
