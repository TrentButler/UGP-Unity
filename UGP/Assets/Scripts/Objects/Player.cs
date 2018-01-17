using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Player", menuName = "Player", order = 0)]
    public class Player : ScriptableObject, ICollector, IDamageable, IHealable
    {
        //EVERYTHING THAT MAKES UP THE PLAYER
        //  - HEALTH
        //  - TOOLBELT

        #region MemberVariables
        private float _health;
        public float Health { get { return _health; } set { } }
        public float MaxHealth;

        private bool _isAlive;
        public bool Alive { get { return _isAlive; } set { } }

        public ToolBelt toolBelt;
        #endregion 

        public void TakeItem(Item item)
        {
            toolBelt.AddItem(item);
        }

        public void TakeItems(List<Item> items)
        {
            var i = items;
            i.ForEach(item => { toolBelt.AddItem(item); });
        }

        public void TakeDamage(float damageTaken)
        {
            //CHECK FOR HEALTH VALUE BEING BELOW 0.0F AFTER TAKING DAMAGE, 
            //IF SO SET '_isAlive' TO FALSE

            _health -= damageTaken;
            
            if (_health <= 0.0f)
                _isAlive = false;
        }

        public void TakeHealth(float healthTaken)
        {
            //INCREMENT PLAYER HEALTH, CLAMP THE VALUE BETWEEN 0.0F AND THE 'MaxHealth'

            _health += healthTaken;
            _health = Mathf.Clamp(_health, 0.0f, MaxHealth);
        }
    }
}