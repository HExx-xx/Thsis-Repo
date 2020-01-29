using System;
using System.Collections.Generic;
using System.Text;

namespace ifc2APDL.AnsysFactory.models
{
    public enum AnsysSectionTypeEnum
    { 
        AXIS,BEAM,COMB,CONTACT,GENB,GENS,JOINT,LINK,PIPE,PRETENSION,REINF,SHELL,SUPPORT,TAPER
    }
    public enum AnsysSectionSubtypeEnum
    {
        RECT,QUAD,CSOLID,CTUBE,CHAN,I,Z,L,T,HATS,HREC,ASEC,MESH,
        MATRIX,
        CIRCLE,SPHERE,CYLINDER,NORMAL,BOLT,RADIUS,
        ELASTIC,PLASTIC,
        UNIV,REVO,SLOT,PINP,PRIS,CYLI,PLAN,WELD,ORIE,SPHE,GENE,SCRE,
        DISC,SMEAR,
        BLOCK
    }
    public class AnsysSecType
    {
        public int ID { get; set; }
        public AnsysSectionTypeEnum type { get; set; }
        public AnsysSectionSubtypeEnum subtype { get; set; }
        public override string ToString()
        {
            return string.Format($"SECTYPE,{ID},{type},{subtype}");
        }
    }
}
