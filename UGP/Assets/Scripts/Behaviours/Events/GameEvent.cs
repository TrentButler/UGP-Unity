using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UGPEVENTS
{

    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {
        private List<GameEventListener> listeners = new List<GameEventListener>();



        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }
        public void RegisterListener(GameEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                throw new InvalidOperationException("Duplicate key");
            }
            listeners.Add(listener);
        }
        /// <summary>
        /// remove a listener, this will happen when the listener object becomes disabled
        /// </summary>
        /// <param name="listener">the monobehaviour we are listening 'from'</param>
        public void UnregisterListener(GameEventListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
            else
            {
                throw new InvalidOperationException("No listener to remove");
            }
        }

    }
}