using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Reflection;

[System.Serializable, CreateAssetMenu(fileName = "New MerchantData", menuName = "MerchantData")]
public class MerchantData : ScriptableObject
{
    [SerializeField]
    public SerializableDict<string, int> merchantPrices = new SerializableDict<string, int>();// maps description of the item to its price
    [SerializeField]
    public SerializableDict<string, string> merchantMethods = new SerializableDict<string, string>(); // map descriprion of the item to the id of the method to be called to get the effect of the item
    [SerializeField]
    public SerializableDict<int, string> descriptions = new SerializableDict<int, string>();
    public SerializableDict<string, GameObject> holograms = new SerializableDict<string, GameObject>();

}

[Serializable]
public class SerializableDelegate : ISerializable, IXmlSerializable
{
    private delegate void DelegatePlaceholder();

    private DelegatePlaceholder _delegate;

    public SerializableDelegate()
    {
    }

    public SerializableDelegate(Delegate d)
    {
        if (d == null)
        {
            _delegate = null;
        }
        else
        {
            _delegate = () => d.DynamicInvoke();
        }
    }

    public Delegate Delegate => _delegate;

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("_delegate", _delegate.Target == null ? null : _delegate);
    }

    public void ReadXml(XmlReader reader)
    {
        string typeName = reader.GetAttribute("type");
        reader.ReadStartElement();
        if (!string.IsNullOrEmpty(typeName))
        {
            Type delegateType = Type.GetType(typeName);
            string methodString = reader.ReadElementString();
            MethodInfo methodInfo = delegateType.GetMethod(methodString);
            Delegate d = Delegate.CreateDelegate(delegateType, methodInfo);
            _delegate = () => d.DynamicInvoke();
        }
        reader.ReadEndElement();
    }

    public void WriteXml(XmlWriter writer)
    {
        if (_delegate == null)
        {
            return;
        }

        Type delegateType = _delegate.GetType();
        writer.WriteAttributeString("type", delegateType.AssemblyQualifiedName);
        writer.WriteStartElement("Method");
        writer.WriteString(_delegate.Method.ToString());
        writer.WriteEndElement();
    }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public static implicit operator SerializableDelegate(Delegate d)
    {
        return new SerializableDelegate(d);
    }

    public static implicit operator Delegate(SerializableDelegate sd)
    {
        return sd._delegate;
    }
}

