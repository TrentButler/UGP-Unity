using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace UGP
{
    public class PickASceneBehaviour : MonoBehaviour
    {
        //Enumerate the directory 'Assets/Scenes' into a list
        // Create a button for each scene in the list
        // Name the button the name of the scene
        // Change the scene on button click

        public GameObject Button;
        public GameObject Panel;

        public List<string> GetScenes()
        {
            //GET THE DIRECTORY 'Assets/Scenes' 

            var path = Directory.GetCurrentDirectory() + "\\Assets\\Scenes";

            var raw_scenes = Directory.GetFiles(path).ToList();
            var scenes = new List<string>();

            //REMOVE THE BEGINING OF THE FILE PATH
            foreach (var scene in raw_scenes)
            {
                if (scene.Contains(".meta"))
                {
                    continue;
                }
                else
                {
                    var scene_name = "";

                    for (int i = scene.Length - 1; i > 0; i--)
                    {
                        if (scene[i] == '\\')
                        {
                            break;
                        }
                        scene_name += scene[i];
                    }

                    name = new string(scene_name.ToCharArray().Reverse().ToArray());

                    scenes.Add(name);
                }
            }

            var s = new List<string>();
            for (int i = 0; i < scenes.Count; i++)
            {
                s.Add(scenes[i].Substring(0, scenes[i].Length - 6));
            }

            var ordered_scenes = s.OrderBy(x => int.Parse(x.Substring(0, 2))).ToList();
            return ordered_scenes;
        }

        private void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        // Use this for initialization
        void Start()
        {
            var allScenes = GetScenes();

            allScenes.ForEach(scene =>
            {
                //INSTANTIATE A BUTTON ON THE PANEL FOR EACH SCENE
                var button = Instantiate(Button);
                button.transform.SetParent(Panel.transform);

                button.GetComponentInChildren<Text>().text = scene;
                button.GetComponent<OnButtonClick>().data = scene;
                button.GetComponent<OnButtonClick>().onClick += LoadScene;
            });
        }
    }
}