using System;
using System.Collections.Generic;
using UnityEngine;

namespace NT.SceneObjects
{

    [System.Serializable]
    public struct SinkData
    {

    }

    [CreateAssetMenu(fileName = "Sink", menuName = "NT/Scene/Sink")]
    public class SinkSceneObject : SceneObject<SinkData>
    {

    }
}