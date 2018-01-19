using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGP
{
    public class ToolWallBehaviour : MonoBehaviour
    {
        public GameObject model;

        public Transform originPoint;
        public float Spacing;

        public ToolBelt wallToolBelt;

        private List<GameObject> grid;
        public List<GameObject> items;

        public void GenerateGrid()
        {
            int r = wallToolBelt.capacity;
            for (int rows = 0; rows < r; rows++)
            {
                for (int cols = 0; cols < r; cols++)
                {
                    //CREATE A POSITION HORIZONTALLY AND VERTICALLY
                    Vector3 pos = new Vector3(cols * Spacing, -rows * Spacing, 0.0f);

                    var go = new GameObject();
                    go.transform.position = pos + originPoint.position;

                    grid.Add(go);
                }
            }
        }

        public void UpdateGrid()
        {
            var l = new List<Vector3>();
            int r = wallToolBelt.capacity;
            for (int rows = 0; rows < r; rows++)
            {
                for (int cols = 0; cols < r; cols++)
                {
                    //CREATE A POSITION HORIZONTALLY AND VERTICALLY
                    Vector3 pos = new Vector3(cols * Spacing, -rows * Spacing, 0.0f);
                    l.Add(pos + originPoint.position);
                }
            }

            for(int i = 0; i < grid.Count; i++)
            {
                grid[i].transform.position = l[i];
            }
        }

        public GameObject LoadItemPrefab(string type, Item i)
        {
            var sType = type;
            
            switch (sType)
            {
                case "UGP.Med":
                    {
                        var o = Resources.Load("RuntimePrefabs/DEBUGMedPrefab") as GameObject;
                        var obj = Instantiate(o);
                        obj.GetComponent<MedBehaviour>()._med = i as Med;
                        return obj;
                        break;
                    }

                case "UGP.Hammer":
                    {
                        break;
                    }

                case "UGP.RepairKit":
                    {
                        break;
                    }
            }
            return null;
        }

        public void PopulateGrid()
        {
            //ITERATE THROUGH ITEMS AND ADD A PREFAB AT A SPECIFIC LOCATION
            var l = wallToolBelt.items;
            
            for(int i = 0; i < items.Count; i++)
            {
                Destroy(items[i]);
            }
            
            items.Clear();
            items = new List<GameObject>();

            for(int i = 0; i < l.Count; i++)
            {
                var pos = grid[i].transform;
                var itemGO = LoadItemPrefab(l[i].GetType().ToString(), l[i]);

                itemGO.transform.position = pos.position;
                itemGO.transform.rotation = pos.rotation;

                //var itemGO = Instantiate(itemModel, pos.position, pos.rotation);
                items.Add(itemGO);
            }
        }
        
        void Start()
        {
            grid = new List<GameObject>();
            items = new List<GameObject>();

            GenerateGrid();
            PopulateGrid();
            UpdateGrid();
        }
        
        void Update()
        {
            PopulateGrid();
            UpdateGrid();
        }
    }
}
