using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using VNyanInterface;
using UnityEngine;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

namespace VNyanGamepadHelper
{
    public class VNyanGamepadHelper : MonoBehaviour, IVNyanPluginManifest
    {
        public string PluginName { get; } = "VNGH";
        public string Version { get; } = "1.0";
        public string Title { get; } = "VNyan Gamepad Helper";
        public string Author { get; } = "Lunazera";
        public string Website { get; } = "NA";

        private string pluginActiveParamName = "VNGHActive";
        public bool pluginActive = false;
        private bool pluginActive_new;

        private string gamepadInputSpeedParamName = "VNGHSpeed";
        public float gamepadInputSpeed = 2.5f;
        private float gamepadInputSpeed_new;

        
        /// <summary>
        /// Standardized Dictionary of gamepad keys read from VNyan
        /// </summary>
        public static Dictionary<string, float> gamepadDict = new Dictionary<string, float>
        {
            { "_gamepadA", 0f },
            { "_gamepadB", 0f },
            { "_gamepadX", 0f },
            { "_gamepadY", 0f },
            { "_gamepadDUp", 0f },
            { "_gamepadDDown", 0f },
            { "_gamepadDLeft", 0f },
            { "_gamepadDRight", 0f },
            { "_gamepadLS", 0f },
            { "_gamepadRS", 0f },
            { "_gamepadLT", 0f },
            { "_gamepadRT", 0f },
            { "_gamepadL3", 0f },
            { "_gamepadR3", 0f },
            { "_gamepadStart", 0f },
            { "_gamepadSelect", 0f },
            { "_gamepadLeftx", 0f },
            { "_gamepadLefty", 0f },
            { "_gamepadRightx", 0f },
            { "_gamepadRighty", 0f }
        };

        /// <summary>
        /// Creates list of Gamepad inputs to iterate over
        /// </summary>
        public List<string> gamepadKeys = new List<string>(gamepadDict.Keys.ToList());

        /// <summary>
        /// Method to control plugin state from string input: 1 = on, 0 = off 
        /// </summary>
        /// <param name="input"></param>
        public void setPluginOnOff(string input) => pluginActive = (input != "0");

        /// <summary>
        /// Uses MoveToward to smoothly increase gamepad value towards target (set from VNyan). Relies on Gamepad Input Speed, set within VNyan
        /// </summary>
        /// <param name="key">Name of the key in the gamepad dictionary we're using</param>
        /// <param name="currentValue">Current value from dictionary</param>
        /// <param name="targetValue">New target value from VNyan</param>
        public void setVNyanGamepadParam(string key, float currentValue, float targetValue)
        {
            float newValue = Mathf.MoveTowards(currentValue, targetValue, gamepadInputSpeed * Time.deltaTime);
            
            gamepadDict[key] = newValue;
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat("VNGH" + key, newValue);
        }

        /// <summary>
        /// Main listener method to check current input for gamepad from VNyan. If dictionary value = vnyan value, then nothing else happens.
        /// </summary>
        public void VNyanGamepadListener()
        {
            foreach (string key in gamepadKeys)
            {
                float VNyanInput = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(key);
                float gamepadDictValue = gamepadDict[key];

                if (VNyanInput != gamepadDictValue)
                {
                    setVNyanGamepadParam(key, gamepadDictValue, VNyanInput);
                }
            }
        }


        public void InitializePlugin()
        {
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(pluginActiveParamName, 0f);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(gamepadInputSpeedParamName, 3f);
            Debug.Log("VNGH Initialized!");

        }

        public void Update()
        {
            // Control Plugin On/Off State with VNyan Parameter
            pluginActive_new = (VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterString(pluginActiveParamName) != "0");
            if ( !(pluginActive_new == pluginActive) )
            {  
                pluginActive = pluginActive_new;
                Debug.Log("VNGH: State set to " + pluginActive);
            }

            gamepadInputSpeed_new = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(gamepadInputSpeedParamName);
            if (!(gamepadInputSpeed_new == gamepadInputSpeed))
            {
                gamepadInputSpeed = gamepadInputSpeed_new;
                Debug.Log("VNGH: Speed set to " + pluginActive);
            }

            // Only run rest of code while plugin is in active state
            if (pluginActive)
            {
                VNyanGamepadListener();
            }
        }
    }
}
