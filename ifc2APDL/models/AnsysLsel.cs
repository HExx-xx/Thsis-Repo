using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public enum LselTypeEnum
    {
        S,R,A,U,ALL,NONE,INVE,STAT
    }
    public enum LselItemEnum
    {
        LINE,EXT,LOC,TAN1,TAN2,NDIV,SPACE,MAT,TYPE,REAL,ESYS,SEC,LENGTH,RADIUS,HPT,LCCA
    }
    public enum LselCompEnum
    {
        X,Y,Z
    }
    class AnsysLsel
    {
        LselTypeEnum Type{ get; set; }
        LselItemEnum Item { get; set; }
        LselCompEnum Comp { get; set; }
        double VMIN { get; set; }
        double VMAX { get; set; }
        int VINC { get; set; }
        int KSWP { get; set; }
        public override string ToString()
        {
            if (Type == LselTypeEnum.S || Type == LselTypeEnum.R || Type == LselTypeEnum.A || Type == LselTypeEnum.U)
                return string.Format($"LSEL,{Type},{Item},{Comp},{VMIN},{VMAX},{VINC},{KSWP}");
            else
                return string.Format($"LSEL,{Type}");
        }
    }
}
