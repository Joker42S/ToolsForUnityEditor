#define InEditorMode

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if InEditorMode
public class SerializableDictionary<TKey, TValue> : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> _keys = null;
    [SerializeField]
    private List<TValue> _values = null;
    private Dictionary<TKey, TValue> serializedDict = null;
    private FieldInfo dictField = null;

    public void OnBeforeSerialize()
    {
        serializedDict = GetDict(GetDictField());
        if (CheckDictData())
        {
            if (serializedDict == null)
            {
                _keys = null;
                _values = null;
            }
            else
            {
                _keys = new List<TKey>(serializedDict.Keys);
                _values = new List<TValue>(serializedDict.Values);
            }
        }
    }
    public void OnAfterDeserialize()
    {
        if (CheckDictData())
        {
            if (_keys != null && _values != null)
            {
                if (serializedDict == null)
                {
                    serializedDict = new Dictionary<TKey, TValue>();
                }
                else
                {
                    serializedDict.Clear();
                }
                for (int i = 0; i != Mathf.Min(_keys.Count, _values.Count); i++)
                    serializedDict.Add(_keys[i], _values[i]);
            }
            else
            {
                serializedDict = null;
            }
            SetDict(GetDictField(), serializedDict);
        }
    }

    private bool CheckDictData()
    {
        if (_values == null && _keys == null)
        {
            return true;
        }
        if (_values == null || _keys == null)
        {
            return false;
        }
        if (_values.Count != _keys.Count)
        {
            Debug.LogError("Numbers of keys and values should be same!");
            return false;
        }
        if (IsDuplicated(_keys))
        {
            Debug.LogError("Duplicated keys!");
            return false;
        }
        return true;
    }

    private bool IsDuplicated<T>(List<T> list)
    {
        int len = list.Count;
        for (int i = 0; i < len - 1; ++i)
        {
            for (int j = i + 1; j < len; ++j)
            {
                if (Equals(list[i], list[j]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    Dictionary<TKey, TValue> GetDict(FieldInfo dictField)
    {
        var myDict = dictField.GetValue(this);
        return myDict as Dictionary<TKey, TValue>;
    }

    void SetDict(FieldInfo dictField, Dictionary<TKey, TValue> dict)
    {
        dictField.SetValue(this, dict);
    }
    FieldInfo GetDictField()
    {
        if (dictField == null)
        {
            System.Type type = this.GetType();
            FieldInfo myDictField = null;
            foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
            {
                var att = field.GetCustomAttributes(false);
                if (att.Length > 0 && att[0].GetType() == typeof(SerializedDict))
                {
                    if (field.FieldType == typeof(Dictionary<TKey, TValue>))
                    {
                        myDictField = field;
                        break;
                    }
                    else
                        throw new System.Exception("Dictionary type cannot match.");
                }
            }
            if (myDictField == null)
            {
                throw new System.Exception("No dictionary!");
            }
            else
            {
                dictField = myDictField;
            }
        }
        return dictField;
    }
}

[System.AttributeUsage(System.AttributeTargets.Field)]
public class SerializedDict : System.Attribute
{

}

#else
public class SerializableDictionary<TKey, TValue> : MonoBehaviour
{
    
}
#endif