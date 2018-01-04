using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Trent
{
    [CreateAssetMenu(fileName = "AmmoBox", menuName = "AmmoBox", order = 3)]
    public class AmmoBox : Item, ICollectable
    {
        //HOLDS ALL AMMO TYPES
        //ASSAULT
        //SHOTGUN
        //SNIPER
        //ROCKET

        public AssaultRound assault;
        public ShotgunRound shotgun;
        public SniperRound sniper;
        public RocketRound rocket;

        private void Awake()
        {
            assault = CreateInstance<AssaultRound>() as AssaultRound;
            shotgun = CreateInstance<ShotgunRound>() as ShotgunRound;
            sniper = CreateInstance<SniperRound>() as SniperRound;
            rocket = CreateInstance<RocketRound>() as RocketRound;
        }

        AmmoBox(AssaultRound assaultR, ShotgunRound shotgunR,
            SniperRound sniperR, RocketRound rocketR)
        {
            assault = assaultR;
            shotgun = shotgunR;
            sniper = sniperR;
            rocket = rocketR;
        }

        public Item ItemGivenOnPickup(ICollector c)
        {
            throw new System.NotImplementedException();
        }

        public List<Item> ItemsGivenOnPickup(ICollector c)
        {
            return new List<Item>() { assault, shotgun, sniper, rocket };
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
            myTarget.assault.Count = EditorGUILayout.IntSlider("Assault", myTarget.assault.Count, 0, 180);
            myTarget.shotgun.Count = EditorGUILayout.IntSlider("Shotgun", myTarget.shotgun.Count, 0, 180);
            myTarget.sniper.Count = EditorGUILayout.IntSlider("Sniper", myTarget.sniper.Count, 0, 180);
            myTarget.rocket.Count = EditorGUILayout.IntSlider("Rocket", myTarget.rocket.Count, 0, 180);

            GUILayout.Space(20);
            if (GUILayout.Button("Save"))
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
#endif
    #endregion
}