using NT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XNode;

public class UGUIListElement : MonoBehaviour
{
    public string fieldName;
    public List<GUIProperty> propertyObjects;
    [HideInInspector] public List<GUIProperty.PropertyType> propertyTypes;

    [HideInInspector] public Node node;
    private NodePort port;

    void Start()
    {
        if (node == null)
        {
            Destroy(this);
            return;
        }
        port = node.GetPort(fieldName);
        //graph = GetComponentInParent<RuntimeGraph>();

        // Save each property
        propertyTypes = new List<GUIProperty.PropertyType>();
        CheckPropertiesTypeConfiguration(port.ValueType);
        // Set properties values
        ConfigureGUIProperties();
    }

    private void CheckPropertiesTypeConfiguration(Type portType)
    {
        if (ReflectionUtilities.isTupleType(portType))
        {
            var tupleTypes = GetTupleElementTypes(portType);

            foreach (var type in tupleTypes)
            {
                CheckPropertiesTypeConfiguration(type);
            }
        }
        else if (portType == typeof(bool))
        {
            propertyTypes.Add(GUIProperty.PropertyType.Boolean);
        }
        else if (portType.IsNumber())
        {
            propertyTypes.Add(GUIProperty.PropertyType.Number);
        }
        else if (portType.IsEnum)
        {
            propertyTypes.Add(GUIProperty.PropertyType.Enumeration);
        }
        else
        {
            propertyTypes.Add(GUIProperty.PropertyType.String);
        }
    }

    private IEnumerable<Type> GetTupleElementTypes(Type type)
    {
        var fieldInfo = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var field in fieldInfo)
        {
            yield return field.FieldType;
        }
    }


    private void ConfigureGUIProperties()
    {
        for(int i=0; i<propertyObjects.Count; i++)
        {
            var currentProperty = propertyObjects[i];
            //var valuePath = fieldName;
            var valuePath = "reglas";
            var selectedObject = ReflectionUtilities.GetValueOf(new List<string>(valuePath.Split('/')), node);
            var currentType = propertyTypes[i];

            currentProperty.SetData(selectedObject, valuePath, currentType);
            currentProperty.OnValueChanged.AddListener(ValueChanged);
        }
    }


    private void ValueChanged(object value, string path)
    {
        object n = node;
        ReflectionUtilities.SetValueOf(ref n, value, new List<string>(path.Split('/')));
    }

    // Update is called once per frame
    private void Update()
    {

    }

}
