using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{                                        //Generates nodes and line elements along lines.
    class AnsysLmesh
    {
        string NL1 { get; set; }
        int NL2 { get; set; }
        int ninc { get; set; }
        public AnsysLmesh()
        {
            ninc = 1;
        }
        public AnsysLmesh(string s)
        {
            NL1 = s;
        }
        public override string ToString()
        {
            if (NL1 == "ALL")
                return string.Format($"LMESH,ALL");
            else
                return string.Format($"LMESH,NL1,{NL2},{ninc}");
        }
    }
}
