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
    public class AnsysElement                              //Defines a local element type from the element library.
    {
        public long ID { get; set; }
        public AnsysElementTypeEnum Type { get; set; }

        public override string ToString()
        {
            return string.Format($"ET,{ID},{Type}");
        }
    }
}
