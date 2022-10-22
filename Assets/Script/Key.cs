using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Get();
        }
    }
    public void Get()
    {
        Player.instance.Key = true;
        Destroy(gameObject);
    }

}
