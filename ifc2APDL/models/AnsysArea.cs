using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysArea                      //Defines an area by connecting keypoints.
    {
        public int ID { get; set; }
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
        public AnsysArea(AnsysKeypoint k1,AnsysKeypoint k2,AnsysKeypoint k3,AnsysKeypoint k4=null, AnsysKeypoint k5 = null,
            AnsysKeypoint k6 = null, AnsysKeypoint k7 = null, AnsysKeypoint k8 = null, AnsysKeypoint k9 = null,
            AnsysKeypoint k10 = null, AnsysKeypoint k11 = null, AnsysKeypoint k12 = null, AnsysKeypoint k13 = null,
            AnsysKeypoint k14 = null, AnsysKeypoint k15 = null, AnsysKeypoint k16 = null, AnsysKeypoint k17 = null,
            AnsysKeypoint k18 = null)
        {
            P1 = k1;P2 = k2;P3 = k3;P4 = k4;P5 = k5;P6 = k6;P7 = k7;P8 = k8;
            P9 = k9;P10 = k10;P11 = k11;P12 = k12;P13 = k13;P14 = k14;
            P15 = k15;P16 = k16;P17 = k17;P18 = k18;
        }
        public override string ToString()
        {
            var P = new List<AnsysKeypoint>() { P4, P5, P6, P7, P8, P9, P10, P11, P12, P13, P14, P15, P16, P17, P18 };
            string ans = "";
            foreach (var p in P)
            {
                if (p != null)
                    ans += $",{p.ID}";
                else
                    break;
            }
            return string.Format($"A,{P1.ID},{P2.ID},{P3.ID}{ans}");
        }
    }
}
