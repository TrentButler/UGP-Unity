using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace UGP
{
    public class ServerBrowser : MonoBehaviour
    {
        public Button JoinMatchButton;
        public ScrollRect ServerScrollView;
        public InputField createdMatchName;
        public Slider matchLimitSlider;
        private List<GameObject> buttons = new List<GameObject>();
        
        private void ListMatches()
        {
            NetworkManager.singleton.matchMaker.ListMatches(0, 10, "", true, 0, 0, Populate);
        }

        private void Populate(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            for(int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i]);
            }

            buttons.Clear();

            matches.ForEach(match =>
            {
                var buttontitle = string.Format("{0} => PLAYERS({1})", match.name, match.currentSize);
                var content = ServerScrollView.content;
                var content_width = content.rect.width;

                var button = Instantiate(JoinMatchButton, content);

                var new_scale = button.GetComponent<RectTransform>().localScale;
                new_scale.x = content_width;
                button.transform.localScale = new_scale;

                var text = button.GetComponentInChildren<Text>();
                text.text = buttontitle;

                var server_button = button.GetComponent<NetworkOnButtonClick>();
                server_button.info = match;

                buttons.Add(button.gameObject);
            });
        }

        public void CreateMatch()
        {
            var matchLimit = (uint)matchLimitSlider.value;
            NetworkManager.singleton.StartMatchMaker();
            NetworkManager.singleton.matchMaker.CreateMatch(createdMatchName.text, matchLimit, true, "", "", "", 0, 0, NetworkManager.singleton.OnMatchCreate);
        }

        private void Awake()
        {
            NetworkManager.singleton.StartMatchMaker();
            //manager.StartMatchMaker();
        }

        private void LateUpdate()
        {
            ListMatches();
        }
    }
}