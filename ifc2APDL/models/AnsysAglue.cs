using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysAglue
    {
        public AnsysArea area1 { get; set; }
        public AnsysArea area2 { get; set; }
        public AnsysAglue(AnsysArea a1,AnsysArea a2)
        {
            area1 = a1;
            area2 = a2;
        }
             
        public override string ToString()
        {
            return string.Format($"AGLUE,{area1.ID},{area2.ID}");
        }
    }
}
