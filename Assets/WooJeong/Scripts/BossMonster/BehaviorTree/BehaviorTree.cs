using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BehaviorTree")]
public class BehaviorTree : ScriptableObject
{
    public BehaviorNode root;
    public List<BehaviorNode> nodes = new();

    public BehaviorNode CreateNode(System.Type type)
    {
        BehaviorNode node = ScriptableObject.CreateInstance(type) as BehaviorNode;
        Debug.Log(node + " " + node.GetType());
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(BehaviorNode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(BehaviorNode parent, BehaviorNode child)
    {
        parent.children.Add(child);
    }

    public void RemoveChild(BehaviorNode parent, BehaviorNode child)
    {
        parent.children.Remove(child);
    }

    public List<BehaviorNode> GetChildren(BehaviorNode parent)
    {
        return parent.children;
    }
}
