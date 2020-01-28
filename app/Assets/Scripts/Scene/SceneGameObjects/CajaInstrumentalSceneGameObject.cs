using UnityEngine;
using VRTK;
using NT;
using NT.SceneObjects;
using System.Collections.Generic;

public class CajaInstrumentalSceneGameObject : SceneGameObject
{
    public List<Transform> spawnPoints;

    public override bool CanHoldItem(SceneGameObject previewSGO)
    {
        MetallicBoxData metallicBoxData = (MetallicBoxData)data.data.GetValue();
        bool freeSpace = false;

        BoxCollider bc = previewSGO.GetComponentInChildren<BoxCollider>();
        if (bc != null)
        {
            // Spawn new tool and see if fits
            foreach (var sp in spawnPoints)
            {
                var hitCollider = Physics.OverlapBox(sp.position, bc.size/2, bc.transform.rotation);
                if (hitCollider.Length == 0)
                {
                    freeSpace = true;
                    break;
                }
            }
        }
        return freeSpace && previewSGO is ITool;
    }

    public override void HoldItem(SceneGameObject childOfElement, bool instancing = true)
    {
        /*ClosetData closetData = (ClosetData)data.data.GetDefaultValue();
        List<SceneGameObjectReference> slots = SlotsList(closetData);

        //Destroy(childOfElement.GetComponent<Rigidbody>());
        childOfElement.gameObject.SetActive(true);

        if (instancing)
        {

            bool stored = false;

            for (int i = 0; i < slots.Count && !stored; i++)
            {
                if (slots[i] == null)
                {
                    stored = true;
                    slots[i] = new SceneGameObjectReference(childOfElement);
                }
            }

            ListToSlots(ref closetData, slots);

            data.data.SetDefaultValue(closetData);
        }
        else
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].linkedSGO == childOfElement.data.id)
                {

                    childOfElement.transform.SetParent(slotPivot[i]);
                    childOfElement.transform.localPosition = Vector3.zero;
                    childOfElement.transform.localRotation = Quaternion.identity;

                    slotPivot[i].gameObject.SetActive(true);

                    return;
                }
            }

            Debug.Log("There is no slot fot ?¿" + childOfElement.data.id);
            childOfElement.gameObject.SetActive(false);
        }*/
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