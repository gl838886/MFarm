using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.inventory
{
    public class PickUpItem : MonoBehaviour
    {
        //private Item item;


        //捡起物品的流程
        //1.碰撞，获得碰撞物品的信息，也就是item
        //2.是否可捡起
        //3.将其添加到背包并将外面的物品销毁
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                 if(item.itemDetails.canPickUp)
                {
                    InventoryManager.Instance.AddItem(item, true);
                    //Destroy(item.gameObject); 这里也可以销毁，但应放在库存管理中
                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}

