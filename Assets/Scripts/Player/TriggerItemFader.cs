using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ItemFader[]faders = collision.GetComponentsInChildren<ItemFader>(); //注意这里是s复数，因为有树叶和树干
        if(faders.Length>0)
        {
            foreach(ItemFader item in faders)
            {
                item.fadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ItemFader[] faders = collision.GetComponentsInChildren<ItemFader>(); //注意这里是s复数，因为有树叶和树干
        if (faders.Length > 0)
        {
            foreach (ItemFader item in faders)
            {
                item.fadeIn();
            }
        }
    }
}
