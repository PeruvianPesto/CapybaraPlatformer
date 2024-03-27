using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickableObject : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pickUpText;
    private bool isPickable;
    private PlayerMovement playerController;


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
            playerController = collision.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pickUpText.gameObject.SetActive(false);
            isPickable = false;
            playerController = null;
        }
    }

    private void PickUp()
    {
        playerController.AddAmmo();
        Destroy(this.gameObject);
    }
}
