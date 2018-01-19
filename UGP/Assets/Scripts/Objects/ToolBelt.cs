using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGP
{
    [CreateAssetMenu(fileName = "ToolBelt", menuName = "ToolBelt", order = 4)]
    public class ToolBelt : Item
    {
        public List<Item> startItems = new List<Item>();

        public Dictionary<string, List<Item>> toolBelt = new Dictionary<string, List<Item>>();

        public AmmoBox ammunition;
        public int capacity;

        public void DumpStartItems()
        {
            startItems.ForEach(x => { AddItem(x); });
        }

        public void ClearToolBelt()
        {
            toolBelt.Clear();
            toolBelt = new Dictionary<string, List<Item>>();
        }
        
        //NEEDS WORK
        public void AddItem(Item i)
        {
            //ITEMS ARE STACKABLE, BE SURE TO HANDLE THAT HERE
            //TEST OUT THE ADDING OF ITEMS, CHECK FOR 'type mismatch'
            //DETERMINE THE TYPE OF THE ITEM THAT IS BEING ADDED,
            //USE A SWITCH STATEMENT TO HANDLE THE SPECIFIC ADDING OF THE ITEM TYPE

            string sItemType = i.GetType().ToString();

            if(!toolBelt.ContainsKey(sItemType))
            {
                //GENERATE A NEW KEY VALUE PAIR FOR THIS ITEM
                toolBelt.Add(sItemType, new List<Item>() { i });
            }

            if(toolBelt.ContainsKey(sItemType))
            {
                //ADD THE ITEM TO ITS LIST
                toolBelt[sItemType].Add(i);
            }
        }

        public void RemoveItem(int index)
        {
            if(startItems.Count < 0)
            {
                var removeThis = startItems[index];
                if (!removeThis)
                    startItems.RemoveAt(index);
            }   
        }
    }


    #region TOOLBELT_INSPECTOR
#if UNITY_EDITOR
    [CustomEditor(typeof(ToolBelt))]
    public class InspectorToolBelt : Editor
    {
        ToolBelt myTarget;

        private void OnEnable()
        {
            myTarget = (ToolBelt)target;
        }


        //NEEDS WORK
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            //ITERATE THROUGH THE DICTIONARY
            //SAVE EACH ENTRY TO THE TOOLBELT 
            //DISPLAY EACH ENTRY TO THE INSPECTOR
            foreach(var k in myTarget.toolBelt.Keys)
            {
                EditorGUI.BeginChangeCheck();

                List<Item> list = myTarget.toolBelt[k];

                //var so = new SerializedObject(list.ToArray());
                var so = new SerializedObject(myTarget);

                var sp = so.FindProperty("targetObjects");

                EditorGUILayout.PropertyField(sp, true);


                so.ApplyModifiedProperties();
                

                EditorGUI.EndChangeCheck();

                
            }
            
            if(GUILayout.Button("Save ToolBelt"))
            {
                myTarget.DumpStartItems();
            }

            GUILayout.Space(10);
            if(GUILayout.Button("Clear ToolBelt"))
            {
                myTarget.ClearToolBelt();
            }
        }


    }

#endif
    #endregion
}