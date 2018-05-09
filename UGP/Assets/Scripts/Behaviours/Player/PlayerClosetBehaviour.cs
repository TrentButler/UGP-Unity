using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
namespace UGP
{

    public class PlayerClosetBehaviour : MonoBehaviour
    {
        #region Players

        public GameObject Shannon;

        public Transform ShannonTrans;

        public GameObject Sandra;

        public int PlayerIndex;
        #endregion
        #region PlayerName
        public string PlayerName;


        public GameObject Panel;
        #endregion
        #region Colors
        public Color ShirtColor;
        public Color PantsColor;
        public Color SkinColor;

        public float Red = 255;
        public float Green = 128;
        public float Blue = 255;
        #endregion
        #region MeshRender
        public SkinnedMeshRenderer SandraShirt;
        public SkinnedMeshRenderer SandraPants;
        public SkinnedMeshRenderer SandraSkin;
        public SkinnedMeshRenderer ShannonShirt;
        public SkinnedMeshRenderer ShannonPants;
        public SkinnedMeshRenderer ShannonSkin;
        #endregion
        #region Sliders
        public Slider GlossySliderValue;
        public Slider MetallicSliderValue;
        public Slider Smoothness;
        public Slider RedSlider;
        public Slider GreenSlider;
        public Slider BlueSlider;
        #endregion

        private enum MeshType
        {
            Skin = 0,
            Shirt = 1,
            Pants = 2,
        }
        private MeshType type;

        public GameObject currentPlayer;
        public SkinnedMeshRenderer currentMeshRender;
        public Color currentColor;

        public void OnSandra()
        {
            currentPlayer = Sandra;
            ToggleSkin();
            PlayerIndex = 1;
            Shannon.SetActive(false);
            Sandra.SetActive(true);
        }
        public void MetallicSlider()
        {
            var Metallic = MetallicSliderValue.value;
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", Metallic);
        }
        public void GlossySlider()
        {
            var Gloss = GlossySliderValue.value;
            GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.SetFloat("_Glossiness", Gloss);
        }
        public void RedValueSlider()
        {

            Red = RedSlider.value;
            //ShannonShirt.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //SandraShirt.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //Pants.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //Skin.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
        }
        public void GreenValueSlider()
        {


            Green = GreenSlider.value;
            //Shirt.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //ShannonPants.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //SandraPants.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //Skin.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);

        }
        public void BlueValueSlider()
        {
            Blue = BlueSlider.value;
            //Shirt.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //Pants.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //ShannonSkin.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
            //SandraSkin.GetComponent<SkinnedMeshRenderer>().material.color = new Color(Red, Green, Blue);
        }
        public void OnShannon()
        {
            currentPlayer = Shannon;
            ToggleSkin();
            PlayerIndex = 0;
            Shannon.SetActive(true);
            Sandra.SetActive(false);
        }

        public void ToggleSkin()
        {
            if (currentPlayer == Shannon)
            {
                currentMeshRender = ShannonSkin;
                currentColor = SkinColor;
                type = MeshType.Skin;
            }
            else
            {
                currentMeshRender = SandraSkin;
                currentColor = SkinColor;
                type = MeshType.Skin;
            }
        }

        public void ToggleShirt()
        {
            if (currentPlayer == Shannon)
            {
                currentMeshRender = ShannonShirt;
                currentColor = ShirtColor;
                type = MeshType.Shirt;
            }
            else
            {
                currentMeshRender = SandraShirt;
                currentColor = ShirtColor;
                type = MeshType.Shirt;
            }
        }

        public void TogglePants()
        {
            if (currentPlayer == Shannon)
            {
                currentMeshRender = ShannonPants;
                currentColor = PantsColor;
                type = MeshType.Pants;
            }
            else
            {
                currentMeshRender = SandraPants;
                currentColor = PantsColor;
                type = MeshType.Pants;
            }
        }

        public void OnclickChangeColor()
        {
            Panel.SetActive(true);

            //GetComponent<ChangeColorBehaviour>().HoverCraft.GetComponent<MeshRenderer>().material.color = new Color(Red, Green, Blue);
        }
        // Use this for initialization
        void Start()
        {
            //Sandra.SetActive(false);
            OnShannon();
            ToggleShirt();
        }

        // Update is called once per frame
        void Update()
        {
            //ShannonShirt.material.color = ShirtColor;
            //ShannonPants.material.color = PantsColor;
            //ShannonSkin.material.color = SkinColor;

            RedValueSlider();
            GreenValueSlider();
            BlueValueSlider();
            //OnclickChangeColor();
            //MetallicSlider();
            //GlossySlider();

            currentMeshRender.material.color = new Vector4(Red, Green, Blue, 1);
            switch (type)
            {
                case MeshType.Skin:
                    {
                        SkinColor = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Shirt:
                    {
                        ShirtColor = currentMeshRender.material.color;
                        break;
                    }
                case MeshType.Pants:
                    {
                        PantsColor = currentMeshRender.material.color;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            currentPlayer.transform.Rotate(Vector3.up * Time.smoothDeltaTime);
        }
    }
}