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
        public void Load(Player_Dress playerDress)
        {
            SkinColor = playerDress.skin_color;
            ShirtColor = playerDress.shirt_color;
            PantsColor = playerDress.pants_color;
        }

        private void LateUpdate()
        {
            Skin.material.color = SkinColor;
            Shirt.material.color = ShirtColor;
            Pants.material.color = PantsColor;
        }
    }
}