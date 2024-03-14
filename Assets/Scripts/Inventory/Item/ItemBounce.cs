using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.inventory
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTransform;  //图片
        private BoxCollider2D coll; //扔的过程中需关闭碰撞体

        //扔的过程中的一些参数 
        private bool isGround; //是否到地面了
        private float distance; //距离
        public float gravity = -3.5f; 
        private Vector2 direction; //方向
        private Vector3 targetPos; //目标

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
            isGround=spriteTransform.position.y<=transform.position.y;  //spriteTransform多加了1.5f
            if(Vector3.Distance(transform.position,targetPos) > 0.1f) //如果整个物体和目标距离过大
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

