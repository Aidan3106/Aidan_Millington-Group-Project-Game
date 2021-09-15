using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss : MonoBehaviour
{
    public int bossHP = 10;
    int bulletSpeed = 5;

    TextMesh bossHPText;

    //projectile
    public GameObject bulletPreFab;

    private void Start()
    {
        bossHPText = this.transform.Find("bossHP").GetComponent<TextMesh>();
        bossHPText.text = bossHP.ToString();
        StartCoroutine(bossShooting());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile")
        {
            bossHP--;
            Destroy(collision.gameObject);
            bossHPText.text = bossHP.ToString();
        }

        if (bossHP < 1)
        {
            Destroy(gameObject);
        }
    }
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPreFab) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        bullet.GetComponent<Rigidbody2D>().position = GetComponent<Rigidbody2D>().position;
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, -bulletSpeed);
    }
    IEnumerator bossShooting()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);
            Shoot();
        }
    }
}