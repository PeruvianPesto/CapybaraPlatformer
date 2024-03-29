using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 4.5f;
    private Vector3 direction;

    private void Update()
    {
        transform.position += direction * Time.deltaTime * projectileSpeed;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(this.gameObject);   
    }
}