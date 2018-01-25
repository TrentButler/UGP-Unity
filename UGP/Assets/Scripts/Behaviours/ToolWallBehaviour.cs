using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UGP
{
    public class ToolWallBehaviour : MonoBehaviour
    {
        public Canvas canvas;

        public Vector2 CanvasBounds;

        public float ItemSpacing;
        public int rows;
        public int cols;

        public ToolBelt wallToolBelt;

        private List<Vector3> grid;
        public List<GameObject> items;
        
        public void OnButtonClick()
        {
            Debug.Log("BUTTON PRESS");
        }

        public void GenerateGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    //CREATE A POSITION HORIZONTALLY AND VERTICALLY
                    Vector3 p = new Vector3(c * ItemSpacing, -r * ItemSpacing, 0.0f);
                    var pos = canvas.transform.position;
                    
                    Vector3 position = p + pos;

                    grid.Add(position);
                }
            }
        }

        public void UpdateGrid()
        {
            var l = new List<Vector3>();

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    //CREATE A POSITION HORIZONTALLY AND VERTICALLY
                    Vector3 p = new Vector3(c * ItemSpacing, -r * ItemSpacing, 0.0f);
                    var pos = canvas.transform.position;

                    Vector3 position = p + pos;

                    l.Add(position);
                }
            }

            grid = l;

            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.position = grid[i];
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


        //NEEDS WORK
        //POPULATE 'grid' WITH A UI BUTTON ELEMENT FOR EACH ITEM IN THE LIST(S)
        public void PopulateGrid()
        {
            //ITERATE THROUGH ITEMS AND ADD A PREFAB AT A SPECIFIC LOCATION
            var l = wallToolBelt.medical;
            
            for(int i = 0; i < items.Count; i++)
            {
                Destroy(items[i]);
            }
            
            items.Clear();
            items = new List<GameObject>();

            for(int i = 0; i < 10; i++)
            {
                var pos = grid[i];
                //var itemGO = LoadItemPrefab(l[i].GetType().ToString(), l[i]);
                //var itemGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                var o = Resources.Load("RuntimePrefabs/UI/MedButtonPrefab") as GameObject;
                var itemGO = Instantiate(o);
                itemGO.transform.SetParent(canvas.transform);

                //var button = itemGO.GetComponent<Button>();
                //button.onClick.AddListener(OnButtonClick);

                itemGO.transform.position = pos;
                //itemGO.transform.rotation = pos.rotation;

                //var itemGO = Instantiate(itemModel, pos.position, pos.rotation);
                items.Add(itemGO);
            }
        }
        
        void Start()
        {
            grid = new List<Vector3>();
            items = new List<GameObject>();

            canvas = Instantiate(canvas);
            canvas.transform.SetParent(transform);
            canvas.transform.position = transform.position;
            canvas.transform.rotation = transform.rotation;

            GenerateGrid();
            PopulateGrid();
            UpdateGrid();
        }
        
        void Update()
        {
            canvas.transform.position = transform.position;
            canvas.transform.rotation = transform.rotation;
            

            var pos = canvas.transform.position;
            var r = new Rect(pos, CanvasBounds);

            var canvasRect = canvas.GetComponent<RectTransform>();
            var c = canvasRect.rect;
            c = r;

            UpdateGrid();
            //PopulateGrid();            
        }
    }
}
