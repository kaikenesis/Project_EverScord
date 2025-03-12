using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    [CreateAssetMenu(menuName = "EverScord/AI/BlackBoard", fileName = "newBlackBoard")]
    public class BaseBlackBoard : ScriptableObject
    {
        protected Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
        protected Dictionary<string, int> intValues = new Dictionary<string, int>();
        protected Dictionary<string, float> floatValues = new Dictionary<string, float>();
        protected Dictionary<string, object> objectValues = new Dictionary<string, object>();

        public void SetValue(string key, bool value)
        {
            if (boolValues.ContainsKey(key))
            {
                boolValues[key] = value;
                return;
            }
            else
            {
                boolValues.Add(key, value);
            }
        }

        public void SetValue(string key, int value)
        {
            if (intValues.ContainsKey(key))
            {
                intValues[key] = value;
                return;
            }
            else
            {
                intValues.Add(key, value);
            }
        }

        public void SetValue(string key, float value)
        {
            if (floatValues.ContainsKey(key))
            {
                floatValues[key] = value;
                return;
            }
            else
            {
                floatValues.Add(key, value);
            }
        }

        public void SetValue(string key, object value)
        {
            if (objectValues.ContainsKey(key))
            {
                objectValues[key] = value;
                return;
            }
            else
            {
                objectValues.Add(key, value);
            }
        }

        public void GetValue(string key, out bool value)
        {
            if (!boolValues.ContainsKey(key))
            {
                value = false;
                return;
            }

            value = boolValues[key];
        }

        public void GetValue(string key, out int value)
        {
            if (!intValues.ContainsKey(key))
            {
                value = 0;
                return;
            }

            value = intValues[key];
        }

        public void GetValue(string key, out float value)
        {
            if (!floatValues.ContainsKey(key))
            {
                value = 0;
                return;
            }

            value = floatValues[key];
        }

        public void GetValue<T>(string key, out T value) where T : class
        {
            if (!objectValues.ContainsKey(key))
            {
                value = null;
                return;
            }

            value = objectValues[key] as T;
        }
    }
}
