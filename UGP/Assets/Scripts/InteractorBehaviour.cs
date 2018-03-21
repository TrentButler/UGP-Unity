using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class InteractorBehaviour : MonoBehaviour, IInteractor
    {
        public IInteractable currentInteractable;

        public void Interaction_Set(IInteractable interactable)
        {
            if (currentInteractable != null)
                return;

            currentInteractable = interactable;
        }

        public void Interaction_Release()
        {
            currentInteractable = null;
        }

        public void Update()
        {
            if (currentInteractable != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    currentInteractable.Interact(currentInteractable);
                }
            }
        }
    }
}
