using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    class AnsysAatt
    {
        AnsysMaterial mat { get; set; }
        AnsysRealConstant real { get; set; }
        AnsysMaterial type { get; set; }
        AnsysSecType secn { get; set; }
        public override string ToString()
        {
            return string.Format($"AATT,{mat.ID},{real.ID},{type.ID},,{secn.ID}");
        }
    }
}
