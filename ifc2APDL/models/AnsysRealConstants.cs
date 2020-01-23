using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    class AnsysRealConstants
    {
        public int ID { get; set; }
        public double r1 { get; set; }
        public double r2 { get; set; }
        public double r3 { get; set; }
        public double r4 { get; set; }
        public double r5 { get; set; }
        public double r6 { get; set; }
        public AnsysRealConstants()
        {
            r1 = 0;r2 = 0;r3 = 0;
            r4 = 0;r5 = 0;r6 = 0;
        }

        public override string ToString()
        {
            return string.Format($"R,{ID},{r1},{r2},{r3},{r4},{r5},{r6}");
        }
    }
}
