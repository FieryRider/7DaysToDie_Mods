using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

public class MinEventActionPrintMessage : MinEventActionBase
{
    private string message;
    public override void Execute(MinEventParams _params)
    {
        Log.Out(this.message);
    }

    public override bool ParseXmlAttribute(XmlAttribute _attribute)
    {
        bool flag = base.ParseXmlAttribute(_attribute);
        if (!flag)
        {
            string name = _attribute.Name;
            if (name == "message")
            {
                this.message = _attribute.Value;
                return true;
            }
        }
        return flag;
    }

    public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
        => base.CanExecute(_eventType, _params);

}
