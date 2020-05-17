using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysRealConstant
    {
        public int ID { get; set; }
        public double r1 { get; set; }
        public double r2 { get; set; }
        public double r3 { get; set; }
        public double r4 { get; set; }
        public double r5 { get; set; }
        public double r6 { get; set; }
        public AnsysRealConstant()
        {
            r1 = 0; r2 = 0; r3 = 0; r4 = 0; r5 = 0; r6 = 0;
        }
        public AnsysRealConstant(int id, double n1 = 0, double n2 = 0, double n3 = 0, double n4 = 0, double n5 = 0, double n6 = 0)
        {
            ID = id; r1 = n1; r2 = n2; r3 = n3;
            r4 = n4; r5 = n5; r6 = n6;
        }
        public override string ToString()
        {
            return string.Format($"R,{ID},{r1},{r2},{r3},{r4},{r5},{r6}");
        }
    }
}
