using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private int damage;
    private float timeToLive = 5;

    private bool flipped = false;

    public void SetDamage(int damage) {
        this.damage = damage;
    }

    private void Update() {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0) {
            Destroy(gameObject);
        }
        if (GetComponent<Rigidbody2D>().velocity.x < 0 && !flipped) {
            flipped = true;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Player.current.Damage(damage);
        }
        Destroy(gameObject);
    }

}
