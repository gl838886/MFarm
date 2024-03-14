using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mfarm.transition
{
    public class Teleport : MonoBehaviour
    {
        //[SceneName]
        public string sceneName;
        public Vector3 targetPosition;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player") //ֻ����Ҳſ��Դ��ͣ�NPC������
            {
                EventHandler.CallTransitionEvent(sceneName, targetPosition);
            }
        }
    }
}

