using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysAatt
    {
        AnsysMaterial mat { get; set; }
        AnsysRealConstant real { get; set; }
        AnsysElement type { get; set; }
        AnsysSection secn { get; set; }
        public AnsysAatt(AnsysMaterial Mat = null, AnsysRealConstant Real = null, AnsysElement Type = null, AnsysSection Sec = null)
        {
            mat = Mat;
            real = Real;
            type = Type;
            secn = Sec;
        }
        public override string ToString()
        {
            string ans = "AATT";
            if (mat == null) ans += ",";
            else ans += string.Format($",{mat.ID}");
            if (real == null) ans += ",";
            else ans += string.Format($",{real.ID}");
            if (type == null) ans += ",";
            else ans += string.Format($",{type.ID}");
            if (secn == null) ans += ",,";
            else ans += string.Format($",{secn.type.ID}");
            return ans;
        }
    }
}