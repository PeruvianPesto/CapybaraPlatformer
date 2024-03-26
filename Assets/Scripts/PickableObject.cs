using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickableObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pickUpText;
    private bool isPickable;


    private void Start()
    {
        pickUpText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isPickable && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickUpText.gameObject.SetActive(true);
            isPickable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickUpText.gameObject.SetActive(false);
            isPickable = false;
        }
    }

    private void PickUp()
    {
        Destroy(this.gameObject);
    }
}
