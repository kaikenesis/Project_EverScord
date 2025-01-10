using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotConditionNode : MonoBehaviour, IDecoratorNode
{
    public INode Child { get; set; }


    public void SetChildNode(INode childNode)
    {
        Child = childNode;
    }

    public bool Run()
    {
        return !Child.Run();
    }
}
