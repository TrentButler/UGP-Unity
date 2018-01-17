using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    public class Vehicle : ScriptableObject, IDamageable, IRepairable
    {
        #region MemberVariables
        private float _health;
        public float Health { get { return _health; } set { } }
        public float MaxHealth;

        private float _fuel;
        public float Fuel { get { return _fuel; } set { } }
        public float MaxFuel;

        private bool _isDestroyed;
        public bool Destroyed { get { return _isDestroyed; } set { } }

        public AmmoBox ammunition; //??????
        #endregion

        public void TakeDamage(float damageTaken)
        {
            //CHECK FOR HEALTH VALUE BEING BELOW 0.0F AFTER TAKING DAMAGE, 
            //IF SO SET '_isDestroyed' TO FALSE

            _health -= damageTaken;

            if (_health <= 0.0f)
                _isDestroyed = false;
        }

        public void TakeRepair(float repairTaken)
        {
            //INCREMENT VEHICLE HEALTH, CLAMP THE VALUE BETWEEN 0.0F AND THE 'MaxHealth'

            _health += healthTaken;
            _health = Mathf.Clamp(_health, 0.0f, MaxHealth);
        }
    }
}
