﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NT.Atributes;
using NT.SceneObjects;
namespace NT.Nodes.Other
{
    public class DestroyObject : FlowNode
    {
        [NTInput] public SceneGameObjectReference objectToDestroy;
        // Start is called before the first frame update
        public object GetValue()
        {
            return null;
        }

        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            GameObject.DestroyImmediate(GetInputValue<SceneGameObject>(nameof(objectToDestroy), null).gameObject);
            yield return null;
        }

        public override string GetDisplayName()
        {
            return "Display Hint";
        }
    }
}
