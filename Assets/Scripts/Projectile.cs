using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed;
    private float direction;
    private float lifetime;
    public bool hit;

    private BoxCollider2D boxCollider;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        hit = true;
        boxCollider.enabled = false;
        animator.SetTrigger("Explode");
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(false);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;

        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    public void Deactivated()
    {
        gameObject.SetActive(false);    
    }
}
