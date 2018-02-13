using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UGP
{
    [CreateAssetMenu(fileName = "ToolBelt", menuName = "ToolBelt", order = 4)]
    public class ToolBelt : Item
    {
        public int Capacity;
        public List<Med> medical = new List<Med>();
        public List<RepairKit> repairKits = new List<RepairKit>();
        public List<Item> misc = new List<Item>();

        public bool AddItem(Item i)
        {
            var sItemType = i.GetType().ToString();

            switch (sItemType)
            {
                case "UGP.Med":
                    {
                        if (medical.Count < Capacity)
                        {
                            if (i != null)
                                medical.Add(i as Med);
                                return true;
                        }
                        return false;
                    }

                case "UGP.RepairKit":
                    {
                        if (repairKits.Count < Capacity)
                        {
                            if (i != null)
                                repairKits.Add(i as RepairKit);
                                return true;
                        }
                        return false;
                    }

                case "UGP.Hammer":
                    {
                        if (misc.Count < Capacity)
                        {
                            if (i != null)
                                misc.Add(i as Hammer);
                                return true;
                        }
                        return false;
                    }

                default:
                    {
                        //CHECK IF ITEM IS NULL, THEN ADD TO 'misc' LIST
                        if (misc.Count < Capacity)
                        {
                            if (i != null)
                                misc.Add(i);
                                return true;
                        }
                        return false;
                    }
            }

            return false;
        }

        public void RemoveItem(string type)
        {
            //REMOVES THE FIRST ITEM FROM A SPECIFIC LIST
            var s = type;

            switch (s)
            {
                case "UGP.Med":
                    {
                        medical.RemoveAt(0);
                        break;
                    }

                case "UGP.RepairKit":
                    {
                        repairKits.RemoveAt(0);
                        break;
                    }

                case "UGP.Hammer":
                    {
                        misc.RemoveAt(0);
                        break;
                    }

                default:
                    {
                        misc.RemoveAt(0);
                        break;
                    }
            }
        }
        public void RemoveItem(Item i)
        {
            //FIND SPECIFIC ITEM IN ITS LIST, REMOVE IT

            var sItemType = i.GetType().ToString();

            switch (sItemType)
            {
                case "UGP.Med":
                    {
                        var m = i as Med;
                        if (medical.Contains(m))
                            medical.Remove(m);
                        break;
                    }

                case "UGP.RepairKit":
                    {
                        var r = i as RepairKit;
                        if (repairKits.Contains(r))
                            repairKits.Remove(r);
                        break;
                    }

                case "UGP.Hammer":
                    {
                        var h = i as Hammer;
                        if (misc.Contains(h))
                            misc.Remove(h);

                        break;
                    }
            }
        }
    }
}