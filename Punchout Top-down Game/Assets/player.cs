using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    Rigidbody2D body;

    float horizontal;
    float vertical;
    float origYPosition;

    public float HP = 100.0f;
    public float runSpeed = 10.0f;

    // Bullet part
    public GameObject bulletPreFab;
    public float bulletSpeed;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        origYPosition = body.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Block();
        }
    }
    private void FixedUpdate()
    {
        body.velocity = new Vector2(horizontal * runSpeed, 0f);
        body.position = new Vector2(body.position.x ,origYPosition);
        if (vertical == -1f)
            body.position = new Vector2(body.position.x ,origYPosition - 1f);
    }
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPreFab) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, bulletSpeed);
    }
    private void Block()
    {
        Debug.Log("TODO: blocking script to ignore damage from boss and drain some stamina");
    }
}
