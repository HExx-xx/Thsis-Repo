using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    class AnsysAmesh
    {
        string AL1 { get; set; }
        int AL2 { get; set; }
        int ninc { get; set; }
        public AnsysAmesh()
        {
            ninc = 1;

        }
        public override string ToString()
        {
            if (AL1 == "ALL")
                return string.Format($"LMESH,ALL");
            else
                return string.Format($"LMESH,AL1,{AL2},{ninc}");
        }
    }
}
