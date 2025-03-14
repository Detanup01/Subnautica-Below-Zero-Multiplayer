﻿namespace Subnautica.Client.Modules
{
    using TMPro;

    using UnityEngine;

    public class PingLatency
    {
        private static GameObject pingLatencyGameObject;

        private static TextMeshProUGUI pingLatencyComponent;

        public static void OnWorldLoaded()
        {
            CreatePingLatencyGameObject();
        }

        public static void CreatePingLatencyGameObject()
        {
            if (pingLatencyGameObject != null)
            {
                GameObject.Destroy(pingLatencyGameObject);
            }

            pingLatencyGameObject = GameObject.Instantiate<GameObject>(ErrorMessage.main.prefabMessage);
            pingLatencyGameObject.SetActive(true);

            pingLatencyComponent = pingLatencyGameObject.GetComponent<TextMeshProUGUI>();
            pingLatencyComponent.rectTransform.SetParent(ErrorMessage.main.messageCanvas, false);

            Vector3 position = pingLatencyComponent.rectTransform.localPosition;

            pingLatencyComponent.rectTransform.localPosition = new Vector2(-95f + position.x, 95f + position.y);
            pingLatencyComponent.fontSize = 14f;
            pingLatencyComponent.text = "";
        }

        public static void SetPingText(long ping)
        {
            if (pingLatencyComponent != null)
            {
                if (ping <= 150)
                {
                    pingLatencyComponent.text = string.Format("{0} ms", ping);
                }
                else if (ping <= 250)
                {
                    pingLatencyComponent.text = string.Format("<color=orange>{0} ms</color>", ping);
                }
                else
                {
                    pingLatencyComponent.text = string.Format("<color=red>{0} ms</color>", ping);
                }
            }
        }
    }
}
