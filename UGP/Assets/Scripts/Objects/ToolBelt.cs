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
        public List<Item> items = new List<Item>();

        //public Dictionary<string, List<Item>> toolBelt = new Dictionary<string, List<Item>>();

        private List<string> keys;
        private List<List<Item>> values;

        public AmmoBox ammunition;
        public int capacity;

        public void DumpStartItems()
        {
            items.ForEach(x => { AddItem(x); });
        }

        public void ClearToolBelt()
        {
            //toolBelt.Clear();
            //toolBelt = new Dictionary<string, List<Item>>();
        }
        
        public void AddItem(Item i)
        {
            //ITEMS ARE STACKABLE, BE SURE TO HANDLE THAT HERE
            //TEST OUT THE ADDING OF ITEMS, CHECK FOR 'type mismatch'
            //DETERMINE THE TYPE OF THE ITEM THAT IS BEING ADDED,
            //USE A SWITCH STATEMENT TO HANDLE THE SPECIFIC ADDING OF THE ITEM TYPE


            if (i != null)
            {
                items.Add(i);
            }
              


            #region OLD
            //string sItemType = i.GetType().ToString();


            //if(sItemType != null)
            //    if(!keys.Contains(sItemType))
            //    {
            //        //GENERATE A NEW KEY VALUE PAIR FOR THIS ITEM

            //        //MAKE SURE YOU KNOW WHICH LIST IS WHICH

            //        keys.Add(sItemType);
            //        values.Add(new List<Item>() { i });

            //        //toolBelt.Add(sItemType, );
            //    }

            //if(toolBelt.ContainsKey(sItemType))
            //{
            //    //ADD THE ITEM TO ITS LIST
            //    toolBelt[sItemType].Add(i);
            //}
            #endregion
        }

        public void RemoveItem(int index)
        {
            if(items.Count < 0)
            {
                var removeThis = items[index];
                if (!removeThis)
                    items.RemoveAt(index);
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
            //foreach(var k in myTarget.toolBelt.Keys)
            //{
            //    EditorGUI.BeginChangeCheck();

            //    List<Item> list = myTarget.toolBelt[k];

            //    //var so = new SerializedObject(list.ToArray());
            //    var so = new SerializedObject(myTarget);

            //    var sp = so.FindProperty("targetObjects");

            //    EditorGUILayout.PropertyField(sp, true);


            //    so.ApplyModifiedProperties();
                

            //    EditorGUI.EndChangeCheck();

                
            //}
            
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