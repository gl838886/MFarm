using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.inventory
{
    public class PickUpItem : MonoBehaviour
    {
        //private Item item;


        //������Ʒ������
        //1.��ײ�������ײ��Ʒ����Ϣ��Ҳ����item
        //2.�Ƿ�ɼ���
        //3.������ӵ����������������Ʒ����
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                 if(item.itemDetails.canPickUp)
                {
                    InventoryManager.Instance.AddItem(item, true);
                    //Destroy(item.gameObject); ����Ҳ�������٣���Ӧ���ڿ�������
                    EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                }
            }
        }
    }
}

