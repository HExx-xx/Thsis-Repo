using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysSecData                //Describes the geometry of a section.
    {
        public double v1 { get; set; }
        public double v2 { get; set; }
        public double v3 { get; set; }
        public double v4 { get; set; }
        public double v5 { get; set; }
        public double v6 { get; set; }
        public double v7 { get; set; }
        public double v8{ get; set; }
        public double v9{ get; set; }
        public double v10 { get; set; }
        public double v11 { get; set; }
        public double v12 { get; set; }
        public AnsysSecData()
        {
            v1 = 0; v2 = 0; v3 = 0; v4 = 0;
            v5 = 0; v6 = 0; v7 = 0; v8 = 0;
            v9 = 0; v10 = 0; v11 = 0; v12 = 0;
        }
        public override string ToString()
        {
            string ans="SECDATA";
            List<double> vs = new List<double>() { v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12 };
            foreach(var v in vs)
            {
                if (v != 0)
                    ans =ans+","+v.ToString();
                if (v == 0)
                    return ans;
            }
            return ans;
        }
    }
}
