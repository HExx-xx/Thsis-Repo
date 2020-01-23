using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public class AnsysKeypoint              //Defines a keypoint.
    {
        public long ID { get; set; }
        double X { get; set; }
        double Y { get; set; }
        double Z { get; set; }

        public AnsysKeypoint(long id,double x,double y,double z)
        {
            ID = id;
            X = x;
            Y = y;
            Z = z;
        }
        public override string ToString()
        {
            return string.Format($"K,{ID},{X},{Y},{Z}");
        }
    }
}
