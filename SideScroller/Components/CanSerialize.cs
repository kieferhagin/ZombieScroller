using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SideScroller.Components
{
    interface CanSerialize
    {

        XmlElement Write(XmlDocument doc);

        void Read(XmlElement element);
    }
}
