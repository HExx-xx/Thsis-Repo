using System;
using System.Collections.Generic;
using System.Text;


namespace ifc2APDL.AnsysFactory.models
{
    public enum AnsysMaterialLableEnum
    {
         EX,PRXY,DENS
    }

    public  class AnsysMaterial             
    {
        public AnsysMaterialLableEnum Lab { get; set; }
        public int ID { get; set; }
        public double C0 {get;set;}
        public double C1 { get; set; }
        public double C2 { get; set; }
        public double C3 { get; set; }
        public double C4 { get; set; }
        public AnsysMaterial()
        {
            C1 = 0; C2 = 0;
            C3 = 0; C4 = 0;
        }
        public AnsysMaterial(int id, AnsysMaterialLableEnum lab, double c0)
        {
            Lab = lab; ID = id; C0 = c0;
            C1 = 0; C2 = 0; C3 = 0; C4 = 0;
        }
        public override string ToString()
        {
            return string.Format($"MP,{Lab},{ID},{C0},{C1},{C2},{C3},{C4}");
        }
    }
}
