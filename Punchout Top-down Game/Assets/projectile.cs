using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "wall")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag == "player")
        {
            collision.gameObject.GetComponent<player>().HP--;
            TextMesh playerHPText = collision.gameObject.transform.Find("playerHP").GetComponent<TextMesh>();
            playerHPText.text = collision.gameObject.GetComponent<player>().HP.ToString();
            Destroy(gameObject);
        }
    }
}
