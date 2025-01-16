using EverScord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBTree : BTree
{
    protected override void SetupTree()
    {
        root.PrintChildren();
    }
}
