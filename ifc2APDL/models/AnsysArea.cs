using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    class AnsysArea                      //Defines an area by connecting keypoints.
    {
        public AnsysKeypoint P1 { get; set; }
        public AnsysKeypoint P2 { get; set; }
        public AnsysKeypoint P3 { get; set; }
        public AnsysKeypoint P4 { get; set; }
        public AnsysKeypoint P5 { get; set; }
        public AnsysKeypoint P6 { get; set; }
        public AnsysKeypoint P7 { get; set; }
        public AnsysKeypoint P8 { get; set; }
        public AnsysKeypoint P9 { get; set; }
        public AnsysKeypoint P10 { get; set; }
        public AnsysKeypoint P11 { get; set; }
        public AnsysKeypoint P12 { get; set; }
        public AnsysKeypoint P13 { get; set; }
        public AnsysKeypoint P14 { get; set; }
        public AnsysKeypoint P15 { get; set; }
        public AnsysKeypoint P16 { get; set; }
        public AnsysKeypoint P17 { get; set; }
        public AnsysKeypoint P18 { get; set; }


        public AnsysArea()
        {
            P4 = null;P5 = null;P6 = null;P7 = null;P8 = null;P9 = null;
            P10 = null;P11 = null;P12 = null;P13 = null;P14 = null;
            P15 = null;P16 = null;P17 = null;P18 = null;
        }
        public override string ToString()
        {
            var P = new List<AnsysKeypoint>() { P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18 };
            string ans = "";
            foreach (var p in P)
            {
                if (p != null)
                    ans += $",{p}";
                else
                    break;
            }
            return string.Format($"A,{P1},{P2},{P3}{ans}");
        }
    }
}
