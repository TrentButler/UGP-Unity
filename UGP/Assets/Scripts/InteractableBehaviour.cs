using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        public GameObject Interactor;
        [SerializeField]
        private GameEventArgs Interactor_Set;
        [SerializeField]
        private GameEventArgs Interactor_Release;
        [SerializeField]
        private GameEventArgs Interaction_Start;
        [SerializeField]
        private GameEventArgs Interaction_End;

        [SerializeField]
        private bool interactedWith = false;
        public void Interact(object token)
        {
            if (interactedWith)
            {
                //close it
                interactedWith = false;
                Interaction_End.Raise(gameObject);
            }
            else
            {
                //open it
                interactedWith = true;                
                Interaction_Start.Raise(gameObject);
            }
        }

        public void SetInteractor(params Object[] args)
        {
            Interactor = (GameObject)args[1];
            Interactor.GetComponent<IInteractor>().Interaction_Set(this);
            Interactor_Set.Raise(gameObject, Interactor);
        }

        public void ReleaseInteractor(params Object[] args)
        {
            if (args[0] == gameObject && Interactor != null)
            {
                Interactor.GetComponent<IInteractor>().Interaction_Release();
                Interactor_Release.Raise(gameObject, Interactor);
                Interactor = null;
            }
        }
    }
}
