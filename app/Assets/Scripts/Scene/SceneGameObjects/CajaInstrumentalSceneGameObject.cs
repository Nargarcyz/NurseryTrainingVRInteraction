using UnityEngine;
using NT.SceneObjects;
using System.Collections.Generic;
using System.Linq;

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

    }

    private Transform CheckSpawnpointFit(SceneGameObject previewSGO)
    {
        BoxCollider bc = previewSGO.GetComponentInChildren<BoxCollider>();
        if (bc != null)
        {
            foreach (var sp in spawnPoints)
            {
                var hitCollider = Physics.OverlapBox(sp.position, bc.size / 2, bc.transform.rotation, ~0, QueryTriggerInteraction.Ignore);
                var hitCollider2 = Physics.OverlapBox(sp.position, bc.size / 2, bc.transform.rotation * Quaternion.Euler(0, 90, 0), ~0, QueryTriggerInteraction.Ignore);
                if (hitCollider.Length == 0 || hitCollider2.Length == 0)
                {
                    return sp;
                }
            }
        }
        return null;
    }

    public override void LoadFromData(SceneGameObjectData data)
    {
        base.LoadFromData(data);
    }


    //public override void RestoreTransform()
    //{
    //    base.RestoreTransform();
    //}



    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Tool")
    //    {
    //        HoldItem(other.GetComponent<SceneGameObject>());
    //        MessageSystem.SendMessage("Item Added " + data.id);
    //    }
    //}


    // ¿¿Control de colisiones para que los objetos roten si se situan sobre la tapa??

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Tool")
        {
            VRTK_InteractableObject interactable = collision.gameObject.GetComponentInParent<VRTK_InteractableObject>();
            interactable.gameObject.transform.SetParent(surface.transform);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Tool")
        {
            VRTK_InteractableObject interactable = collision.gameObject.GetComponentInParent<VRTK_InteractableObject>();
            interactable.gameObject.transform.SetParent(null);
            // VRTK guarda el parent de antes de agarrar, y lo aplica al soltar. Debemos cambiarlo ahí tambien.
            interactable.GetPreviousState(out Transform preParent, out bool preKinem, out bool preGrab);
            interactable.OverridePreviousState(interactable.gameObject.transform.parent, preKinem, preGrab);
        }
    }
    */

    // OnTriggerExit --> Quitar de hijo de la caja

}