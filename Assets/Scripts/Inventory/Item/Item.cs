using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mfarm.cropPlant;


namespace Mfarm.inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D coll;
        public ItemDetails itemDetails;

        private void Awake()
        {
            spriteRenderer= GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if(itemID!=0)
            {
                Init(itemID);
            }
        }

        public void Init(int ID)
        {
            itemID = ID;

            //通过InventoryManager
            itemDetails = InventoryManager.Instance.getItemDetails(itemID);

            if(itemDetails != null )
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite:itemDetails.itemIcon;

                //修改coll的大小，锚点在下面
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
            //如果是稻草-可收割的场景
            if(itemDetails.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropDetails(itemDetails.itemID);
                gameObject.AddComponent<ItemInteract>();
            }
        }
    }
}
