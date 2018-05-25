using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UGP
{
    public class PlayerDressBehaviour : NetworkBehaviour
    {
        [SyncVar(hook = "OnSkinColorChange")] public Color SkinColor;
        [SyncVar(hook = "OnShirtColorChange")] public Color ShirtColor;
        [SyncVar(hook = "OnPantsColorChange")] public Color PantsColor;

        public SkinnedMeshRenderer Skin;
        public SkinnedMeshRenderer Shirt;
        public SkinnedMeshRenderer Pants;

        #region VehicleColors
        public Color Part0Color;
        public Color Part1Color;
        public Color Part2Color;
        public Color Part3Color;
        public Color Part4Color;
        public Color Part5Color;
        public Color Part6Color;
        public Color Part7Color;
        #endregion

        private void OnSkinColorChange(Color change)
        {
            SkinColor = change;
        }
        private void OnShirtColorChange(Color change)
        {
            ShirtColor = change;
        }
        private void OnPantsColorChange(Color change)
        {
            PantsColor = change;
        }

        public void Load(PlayerDress playerDress)
        {
            SkinColor = playerDress.SkinColor;
            ShirtColor = playerDress.ShirtColor;
            PantsColor = playerDress.PantsColor;
        }
        public void Load(Player_Dress playerDress, Vehicle_Dress vehicleDress)
        {
            SkinColor = playerDress.skin_color;
            ShirtColor = playerDress.shirt_color;
            PantsColor = playerDress.pants_color;

            Part0Color = vehicleDress.Part0Color;
            Part1Color = vehicleDress.Part1Color;
            Part2Color = vehicleDress.Part2Color;
            Part3Color = vehicleDress.Part3Color;
            Part4Color = vehicleDress.Part4Color;
            Part5Color = vehicleDress.Part5Color;
            Part6Color = vehicleDress.Part6Color;
            Part7Color = vehicleDress.Part7Color;
        }

        public List<Color> GetColors()
        {
            return new List<Color>() { Part0Color, Part1Color, Part2Color, Part3Color, Part4Color, Part5Color, Part6Color, Part7Color };
        }

        private void LateUpdate()
        {
            Skin.material.color = SkinColor;
            Shirt.material.color = ShirtColor;
            Pants.material.color = PantsColor;
        }
    }
}