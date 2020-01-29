using System;
using System.Collections.Generic;
using System.Text;
using ifc2APDL.AnsysFactory.models;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysSection
    {
        public AnsysSecType type { get; set; }
        public AnsysSecData data { get; set; }
        public override string ToString()
        {
            return string.Format($"{ type.ToString()}\n"+
                $"{data.ToString()}");
        }
    }
}
