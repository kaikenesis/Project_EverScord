using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICompositeNode : INode
{
    List<INode> ChildList { get; set; }
}
