using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysLatt                      //Associates element attributes with the selected, unmeshed lines.
    {
        AnsysMaterial mat { get; set; }
        AnsysRealConstant real { get; set; }
        AnsysElement type { get; set; }
        AnsysSection secnum { get; set; }
        AnsysKeypoint k1 { get; set; }
        AnsysKeypoint k2 { get; set; }

        
        public AnsysLatt(AnsysMaterial Mat=null, AnsysRealConstant Real=null, AnsysElement Type=null, AnsysSection Sec =null)
        {
            mat = Mat;
            real = Real;
            type = Type;
            secnum = Sec;
        }
        public override string ToString()
        {
            string ans = "LATT";
            if (mat == null) ans += ",";
            else ans +=string.Format($",{mat.ID}");
            if (real == null) ans += ",";
            else ans += string.Format($",{real.ID}");
            if (type == null) ans += ",";
            else ans+= string.Format($",{type.ID}");
            if (k1 == null) ans += ",,";
            else ans+= string.Format($",,{k1.ID}");
            if (k2 == null) ans += ",";
            else ans+=string.Format($",{k2.ID}");
            if (secnum == null) ans += ",";
            else ans+= string.Format($",{secnum.type.ID}");
            return ans;
        }
    }
}
