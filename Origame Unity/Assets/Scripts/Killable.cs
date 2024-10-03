using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Killable : MonoBehaviour
{
    protected Vector2 respawnPos;

    [SerializeField] protected float respawnCooldown = 5;
    
    protected string[] killTags = {"kill", "laser"};

    protected Rigidbody2D rb;
    protected Animator anim;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        respawnPos = transform.position;
    }

    protected bool IsKillObject(GameObject obj) {
        return killTags.Contains(obj.tag.ToLower());
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (IsKillObject(collision.gameObject)) {
            DieAndRespawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (IsKillObject(collision.gameObject)) {
            DieAndRespawn();
        }
    }

    protected void DieAndRespawn() {
        Die();
        Invoke(nameof(Respawn), respawnCooldown);
    }

    protected virtual void Die() {
        if (anim != null) {
            anim.SetBool("dead", true);
        }

        if (rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    protected virtual void Respawn() {
        if (anim != null) {
            anim.SetBool("dead", false);
        }
        
        if (rb != null) {
            rb.velocity = Vector2.zero;
        }

        transform.eulerAngles = Vector3.zero;
        transform.position = respawnPos;
    }
}
