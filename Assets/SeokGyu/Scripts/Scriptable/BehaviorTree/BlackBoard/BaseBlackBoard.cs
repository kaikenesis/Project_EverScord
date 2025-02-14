using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    [CreateAssetMenu(menuName = "EverScord/AI/BlackBoard", fileName = "newBlackBoard")]
    public class BaseBlackBoard : ScriptableObject
    {
        protected Dictionary<string, int> intValues = new Dictionary<string, int>();
        protected Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
        protected Dictionary<string, object> objectValues = new Dictionary<string, object>();

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
                Debug.Log($"{key} : {value}");
            }
        }

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
                Debug.Log($"{key} : {value}");
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
                Debug.Log($"{key} : {value}");
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
            Debug.Log($"{boolValues[key]}");
        }

        public void GetValue(string key, out int value)
        {
            if (!intValues.ContainsKey(key))
            {
                value = 0;
                return;
            }

            value = intValues[key];
            Debug.Log($"{intValues[key]}");
        }

        public void GetValue<T>(string key, out T value) where T : class
        {
            if (!objectValues.ContainsKey(key))
            {
                value = null;
                return;
            }

            value = objectValues[key] as T;
            Debug.Log($"{objectValues[key]}");
        }
    }
}
