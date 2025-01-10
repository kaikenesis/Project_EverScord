using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDecoratorNode : INode
{
    INode Child { get; set; }
}
