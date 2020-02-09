using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    class AnsysLatt                      //Associates element attributes with the selected, unmeshed lines.
    {
        AnsysMaterial mat { get; set; }
        AnsysRealConstant real { get; set; }
        AnsysMaterial type { get; set; }
        AnsysSecType secnum { get; set; }
        AnsysKeypoint k1 { get; set; }
        AnsysKeypoint k2 { get; set; }
        

        public override string ToString()
        {
            return string.Format($"LATT,{mat.ID},{real.ID},{type.ID},,{k1.ID},{k2.ID},{secnum.ID}");
        }

    }
}
