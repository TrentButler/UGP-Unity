using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    [CreateAssetMenu(fileName = "ToolBelt", menuName = "ToolBelt", order = 4)]
    public class ToolBelt : Item
    {
        public List<Item> items = new List<Item>();
        public AmmoBox ammunition;
        public int capacity;

        //NEEDS WORK
        public void AddItem(Item i)
        {
            //ITEMS ARE STACKABLE, BE SURE TO HANDLE THAT HERE
            //TEST OUT THE ADDING OF ITEMS, CHECK FOR 'type mismatch'

            

            //DETERMINE THE TYPE OF THE ITEM THAT IS BEING ADDED,
            //USE A SWITCH STATEMENT TO HANDLE THE SPECIFIC ADDING OF THE ITEM TYPE

            //var t = i.GetType();

            //switch(t)
            //{
            //    case Med:
            //        {
            //            break;
            //        }
            //}



            if (items.Contains(i))
            {
                var item = i;
                Debug.Log(item);
            }

            if (!items.Contains(i))
                if(items.Count < capacity)
                    items.Add(i);
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
}