using UnityEngine;
using NT.SceneObjects;
using System.Collections.Generic;
using System.Linq;
using VRTK;

public class CajaInstrumentalSceneGameObject : SceneGameObject
{
    public List<Transform> spawnPoints;

    public override bool CanHoldItem(SceneGameObject previewSGO)
    {
        MetallicBoxData metallicBoxData = (MetallicBoxData)data.data.GetValue();
        Transform spawnPoint = CheckSpawnpointFit(previewSGO);
        bool freeSpace = spawnPoint != null;
       
        return freeSpace && previewSGO is ITool;
    }

    public override void HoldItem(SceneGameObject childOfElement, bool instancing = true)
    {
        MetallicBoxData metallicBoxData = (MetallicBoxData) data.data.GetDefaultValue();
        childOfElement.gameObject.SetActive(true);

        var spawnPoint = CheckSpawnpointFit(childOfElement, true);
        if (spawnPoint != null)
        {
            if (instancing)
            {
                if (metallicBoxData.toolsList == null)
                {
                    metallicBoxData.toolsList = new List<SceneGameObjectReference>();
                }
                metallicBoxData.toolsList.Add(new SceneGameObjectReference(childOfElement));
                data.data.SetDefaultValue(metallicBoxData);
            }

            childOfElement.transform.SetParent(spawnPoint);
            childOfElement.transform.localPosition = Vector3.zero;
            childOfElement.transform.localRotation = Quaternion.identity;
            // Evitar que los spawnpoint se muevan junto las herramientas
            childOfElement.transform.SetParent(this.gameObject.transform);
        }
        else
        {
            Debug.Log("Cannot place object with id " + childOfElement.data.id + " inside the container");
            childOfElement.gameObject.SetActive(false);
        }
    }

    private Transform CheckSpawnpointFit(SceneGameObject previewSGO, bool isInstantiating = false)
    {
        BoxCollider bc = previewSGO.GetComponentInChildren<BoxCollider>();
        if (bc != null)
        {
            List<int> numCollisions = new List<int>();
            foreach (var sp in spawnPoints)
            {
                LayerMask allLayers = ~0;
                var hitCollider = Physics.OverlapBox(sp.position, bc.size / 2, bc.transform.rotation, allLayers, QueryTriggerInteraction.Ignore);
                if (hitCollider.Length == 0)
                {
                    return sp;
                }

                numCollisions.Add(hitCollider.Length);
                // Rotate 90º? Rotate the same angle as the box?
                // var hitColliderRotated = Physics.OverlapBox(sp.position, bc.size / 2, bc.transform.rotation * Quaternion.Euler(0, 90, 0), allLayers, QueryTriggerInteraction.Ignore);
                // || hitColliderRotated.Length == 0
            }

            if (isInstantiating)
            {
                int minValuePosition = numCollisions.IndexOf(numCollisions.Min());
                return spawnPoints[minValuePosition];
            }
        }

        return null;
    }

    public override void LoadFromData(SceneGameObjectData data)
    {
        base.LoadFromData(data);
    }


    // Habilitar si la caja se va a poder mover
    //private void OnTriggerExit(Collision collision)
    //{
    //    if (collision.collider.tag == "Tool" && collision.transform.IsChildOf(this.gameObject.transform))
    //    {
    //        VRTK_InteractableObject interactable = collision.gameObject.GetComponentInParent<VRTK_InteractableObject>();
    //        interactable.gameObject.transform.SetParent(null);
    //        // VRTK guarda el parent de antes de agarrar, y lo aplica al soltar. Debemos cambiarlo ahí tambien.
    //        interactable.GetPreviousState(out Transform preParent, out bool preKinem, out bool preGrab);
    //        interactable.OverridePreviousState(interactable.gameObject.transform.parent, preKinem, preGrab);
    //    }
    //}
}