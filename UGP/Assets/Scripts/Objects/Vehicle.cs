using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UGP
{
    [CreateAssetMenu(fileName = "Vehicle")]
    public class Vehicle : ScriptableObject, IDamageable, IRepairable
    {
        #region MemberVariables
        private float _health;
        public float Health { get { return _health; } set { _health = value; } }
        public float MaxHealth;

        private float _fuel;
        public float Fuel { get { return _fuel; } set { _fuel = value; } }
        public float MaxFuel;

        private bool _isDestroyed;
        public bool Destroyed { get { return _isDestroyed; } set { _isDestroyed = value; } }

        private bool _fuelDepleted;
        public bool FuelDepeleted { get { return _fuelDepleted; } set { _fuelDepleted = value; } }

        public AmmoBox ammunition; //??????
        #endregion

        public void TakeDamage(float damageTaken)
        {
            //CHECK FOR HEALTH VALUE BEING BELOW 0.0F AFTER TAKING DAMAGE, 
            //IF SO SET '_isDestroyed' TO FALSE

            _health -= damageTaken;

            if (_health <= 0.0f)
                _isDestroyed = true;
        }

        public void TakeRepair(float repairTaken)
        {
            //INCREMENT VEHICLE HEALTH, CLAMP THE VALUE BETWEEN 0.0F AND THE 'MaxHealth'

            _health += repairTaken;
            _health = Mathf.Clamp(_health, 0.0f, MaxHealth);
        }

        public void Refuel(float refuelTaken)
        {
            _fuel += refuelTaken;
            _health = Mathf.Clamp(_fuel, 0.0f, MaxFuel);
        }

        public void UseFuel(float fuelTaken)
        {
            _fuel -= fuelTaken;

            if (_fuel <= 0.0f)
                _fuelDepleted = true;
        }
    }
}
