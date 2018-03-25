using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CentralIp
{
    public class Respuesta
    {
        public String Tipo;
        public String Cuerpo;
        public XElement XML { get { return XElement.Parse(Cuerpo); } }
    }
}
