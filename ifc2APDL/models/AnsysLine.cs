using System;
using System.Collections.Generic;
using System.Text;
using ifc2APDL.AnsysFactory;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysLine                            //Defines a line between two keypoints.
    {
        public int ID { get; set; }
        public AnsysKeypoint K1 { get; set; }
        public AnsysKeypoint K2 { get; set; }
        public AnsysLine(AnsysKeypoint k1,AnsysKeypoint k2)
        {
            K1 = k1;
            K2 = k2;
        }
        public override string ToString()
        {
            return string.Format($"L,{K1.ID},{K2.ID}");
        }

    }

}
 