using NT.SceneObjects;
using NT.Atributes;
using System.Collections;
using VRTK;
using UnityEngine;
namespace NT.Nodes.Other
{

    public class TeleportPlayer : FlowNode
    {
        public enum Destinations
        {
            Sink,
            Room,
        }


        public Destinations dest;

        public object GetValue()
        {
            return null;
        }

        public override string GetDisplayName()
        {
            return "Teleport Player";
        }


        public override IEnumerator ExecuteNode(NodeExecutionContext context)
        {
            if (dest == Destinations.Sink)
            {
                MessageSystem.SendMessage("Go to sink");
            }
            else
            {
                MessageSystem.SendMessage("Go to room");
            }
            // var setup = VRTK_SDKManager.GetLoadedSDKSetup();
            // Debug.Log("<color=green>nameof" + nameof(objPos).ToString() + "</color>");
            // GameObject positionGameObject = GetInputValue<SceneGameObject>(nameof(objPos), null).gameObject;

            // if (setup == null)
            // {
            //     Debug.Log("No Setup, can't teleport");
            //     yield return null;
            // }
            // if (positionGameObject != null)
            // {
            //     Debug.Log("Valid position passed: " + positionGameObject.transform.position);
            //     setup.actualBoundaries.transform.position = positionGameObject.transform.position;
            // }
            // else
            // {
            //     Debug.Log(positionGameObject);
            //     Debug.Log("Invalid position passed");
            //     setup.actualBoundaries.transform.position = Vector3.zero;
            // }

            yield return null;
        }

    }
}
