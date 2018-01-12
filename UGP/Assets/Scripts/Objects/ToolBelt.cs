using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trent
{
    [CreateAssetMenu(fileName = "ToolBelt", menuName = "ToolBelt", order = 4)]
    public class ToolBelt : Item
    {
        public List<Item> items = new List<Item>();
        public AmmoBox ammunition;
        public int capacity;

        public void AddItem(Item i)
        {
            if (!items.Contains(i))
                items.Add(i);
        }

        public void RemoveItem(int index)
        {
            var removeThis = items[index];
            if(!removeThis)
                items.RemoveAt(index);
        }
    }
}