using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public enum AnsysElementTypeEnum
    {
        LINK1,LINK8,LINK10,
        BEAM3,BEAM4,BEAM188,BEAM189,BEAM54,BEAM44,BEAM23,BEAM24,
        PLANE42,PLANE82,PLANE182,
        PLANE2,
        SOLID45,SOLID95, SOLID73, SOLID185, SOLID92, SOLID72,
        SHELL93, SHELL63, SHELL41, SHELL43, SHELL181, SHELL61, SHELL208, SHELL209
    }
    public class AnsysElement
    {
        public int ID { get; set; }
        public AnsysElementTypeEnum Type { get; set; }
        public int Kop1 { get; set; }
        public int Kop2 { get; set; }
        public int Kop3 { get; set; }
        public int Kop4 { get; set; }
        public int Kop5 { get; set; }
        public int Kop6 { get; set; }
        public int Inopr { get; set; }
        public AnsysElement()
        {
            Kop1 = 0; Kop2 = 0; Kop3 = 0; Kop4 = 0; Kop5 = 0; Kop6 = 0; Inopr = 0;
        }
        public AnsysElement(int id, AnsysElementTypeEnum type)
        {
            ID = id;Type = type;
        }
        public override string ToString()
        {
            return string.Format($"ET,{ID},{Type},{Kop1}, {Kop2} , {Kop3}, {Kop4}, {Kop5}, {Kop6}, {Inopr}");
        }
    }
}
