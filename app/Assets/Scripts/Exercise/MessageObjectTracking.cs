using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObjectTracking : MonoBehaviour
{
    public GameObject _trackedObject;
    public GameObject trackedObject
    {
        get
        {
            return _trackedObject;
        }
        set
        {
            _trackedObject = value;
        }
    }
    public GameObject _parentObject;
    public GameObject parentObject
    {
        get
        {
            return _parentObject;
        }
        set
        {
            _parentObject = value;
            this.gameObject.transform.parent = _parentObject.transform;
            BoxCollider collider = _parentObject.gameObject.GetComponentInChildren<BoxCollider>();

            if (collider != null)
            {
                Vector3 colliderCenter = collider.transform.TransformPoint(collider.center);
                colliderCenter.y = 0;
                this.transform.position = colliderCenter;

                Vector3 offset = collider.size;
                offset.x -= 1f;  // Message size fixed offset
                offset.z = 0;
                this.transform.Translate(offset);
            }
            else
            {
                this.transform.position = _parentObject.transform.position;
                this.transform.Translate(new Vector3(1, 1, 0));
            }

            // this.gameObject.transform.localPosition = Vector3.zero;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void DestroyMessage()
    {
        Destroy(this, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (trackedObject != null && parentObject != null)
        {
            transform.LookAt(trackedObject.transform, Vector3.up);
            transform.RotateAround(this.transform.position, this.transform.up, 180);
        }
    }
}
