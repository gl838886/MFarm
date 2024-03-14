using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.inventory
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTransform;  //ͼƬ
        private BoxCollider2D coll; //�ӵĹ�������ر���ײ��

        //�ӵĹ����е�һЩ���� 
        private bool isGround; //�Ƿ񵽵�����
        private float distance; //����
        public float gravity = -3.5f; 
        private Vector2 direction; //����
        private Vector3 targetPos; //Ŀ��

        private void Awake()
        {
            spriteTransform = transform.GetChild(0);
            coll= GetComponent<BoxCollider2D>();
            coll.enabled= false;
        }

        private void Update()
        {
            Bounce();
        }

        public void InitBounceItem(Vector3 target, Vector2 dir)
        {
            coll.enabled = false;
            direction = dir;
            targetPos= target;
            distance = Vector3.Distance(target,transform.position);
            spriteTransform.position += Vector3.up * 1.5f;
        }

        private void Bounce()
        {
            isGround=spriteTransform.position.y<=transform.position.y;  //spriteTransform�����1.5f
            if(Vector3.Distance(transform.position,targetPos) > 0.1f) //������������Ŀ��������
            {
                transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
            }
            if(!isGround)
            {
                spriteTransform.position += Vector3.up * gravity * Time.deltaTime;
            }
            else
            {
                //Debug.Log("hello");
                spriteTransform.position=transform.position;
                coll.enabled = true;
                
            }
        }
    }
}

