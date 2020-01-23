using System;
using System.Collections.Generic;
using System.Text;


namespace ifc2APDL.AnsysFactory.models
{
    public enum AnsysMaterialLableEnum
    {
         EX,PRXY,DENS,DMPR
    }

    public  class AnsysMaterial              //Defines a linear material property as a constant or a function of temperature.
    {
        public AnsysMaterialLableEnum Lab { get; set; }
        public int ID { get; set; }
        public double C0 {get;set;}
        public double C1 { get; set; }
        public double C2 { get; set; }
        public double C3 { get; set; }
        public double C4 { get; set; }
        

        public  AnsysMaterial()
        {
            C0 = 0;
            C1 = 0;
            C2 = 0;
            C3 = 0;
            C4 = 0;
        }
        public AnsysMaterial(int id,AnsysMaterialLableEnum lab)
        {
            Lab = lab;
            ID = id;
            C0 = 0;
            C1 = 0;
            C2 = 0;
            C3 = 0;
            C4 = 0;
        }
        public override string ToString()
        {
            return string.Format($"MP,{Lab},{ID},{C0}");
        }
    }

}
