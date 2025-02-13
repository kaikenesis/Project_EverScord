using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    [CreateAssetMenu(menuName = "EverScord/AI/BlackBoard", fileName = "newBlackBoard")]
    public class BaseBlackBoard : ScriptableObject
    {
        protected Dictionary<string, object> values = new Dictionary<string, object>();

        public void SetValue(string key, object value)
        {
            if (values.ContainsKey(key))
            {
                values[key] = value;
                return;
            }
            else
            {
                values.Add(key, value);
            }
        }

        protected T GetValue<T>(string key) where T : class
        {
            if (!values.ContainsKey(key))
                return null;

            return values[key] as T;
        }

        protected bool DeleteData(string key)
        {
            if (!values.ContainsKey(key))
                return false;

            values.Remove(key);
            return true;
        }

    }
}
