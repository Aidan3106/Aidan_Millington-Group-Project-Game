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
    public float HP = 100.0f;
    public float runSpeed = 10.0f;
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

    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Fire1"))
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
        body.velocity = new Vector2(horizontal * runSpeed, 0f);
        body.position = new Vector2(body.position.x ,origYPosition);
        if (vertical == -1f)
        {
            body.position = new Vector2(body.position.x, origYPosition - 1f);
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
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, bulletSpeed);
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
            audioSource.PlayOneShot(reloadSound, 1);
            yield return new WaitWhile(() => audioSource.isPlaying);
            ammoCapacity = baseAmmoCapacity;
            ammoText.text = ammoCapacity.ToString();
        }
    }
    IEnumerator playerDeath()
    {
        Time.timeScale = 0;
        audioSource.PlayOneShot(deathSound, 1);
        yield return new WaitWhile(() => audioSource.isPlaying);
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
