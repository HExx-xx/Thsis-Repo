using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysAmesh
    {
        string AL1 { get; set; }
        int AL2 { get; set; }
        int ninc { get; set; }
        public AnsysAmesh()
        {
            ninc = 1;
        }
        public AnsysAmesh(string s)
        {
            AL1 = s;
        }
        public override string ToString()
        {
            if (AL1 == "ALL")
                return string.Format($"AMESH,ALL");
            else
                return string.Format($"AMESH,AL1,{AL2},{ninc}");
        }
    }
}
