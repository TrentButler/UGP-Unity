using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGP
{

    //NEEDS WORK, 
    //FUNCTION(S) FOR HANDLING WHEN PLAYER SHOOTS A GIVEN AMMOTYPE
    //FUNCTION(S) FOR HANDLING WHEN PLAYER PICKSUP ANOTHER AMMOBOX
    //FUNCTION(S) FOR HANDLING WHEN PLAYER DROPS A GIVEN AMMOTYPE

    [CreateAssetMenu(fileName = "AmmoBox", menuName = "AmmoBox", order = 3)]
    public class AmmoBox : Item, ICollectable
    {
        //HOLDS ALL AMMO TYPES
        //ASSAULT
        //SHOTGUN
        //SNIPER
        //ROCKET
        public List<Ammo> ammunition = new List<Ammo>();

        public Item ItemGivenOnPickup(ICollector c)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> ItemsGivenOnPickup(ICollector c)
        {
            var items = new List<Item>();
            ammunition.ForEach(x => items.Add(x));
            c.TakeItems(items);
            return items;
        }
    }
    
    #region AMMOBOX_INSPECTOR
#if UNITY_EDITOR
    [CustomEditor(typeof(AmmoBox))]
    [CanEditMultipleObjects]
    public class InspectorAmmoBox : Editor
    {
        AmmoBox myTarget;
        private void OnEnable()
        {
            myTarget = (AmmoBox)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            foreach(var a in myTarget.ammunition)
            {
                EditorGUI.BeginChangeCheck();
                var so = new SerializedObject(a);
                var sp = so.FindProperty("Count");
                sp.intValue = EditorGUILayout.IntSlider(a.name, sp.intValue, 0, 100);                
                so.ApplyModifiedProperties();
                EditorGUI.EndChangeCheck();
            }
            
        }
    }
#endif
    #endregion
}