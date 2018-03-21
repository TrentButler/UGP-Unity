using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    [RequireComponent(typeof(BoxCollider))]
    public class PhysicsTriggerListener : MonoBehaviour
    {
        [SerializeField]
        GameEventArgs onEnter;
        [SerializeField]
        GameEventArgs onExit;
        [SerializeField]
        GameEventArgs onStay;

        public string ListenerTag;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(ListenerTag))
            {
                onEnter.Raise(gameObject, other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(ListenerTag))
            {
                onExit.Raise(gameObject, other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(ListenerTag))
            {
                onStay.Raise(gameObject, other.gameObject);
            }
        }
    }
}
