using ifc2APDL.AnsysFactory.models;
using System;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc4.Interfaces;

namespace Utilities
{
    public class TranslatorHelper
    {
        public static List<AnsysMaterial> TranslateMaterial(IIfcMaterial m)
        {
            var mp = new Dictionary<string, double>();
            foreach (var p_set in m.HasProperties)
            {
                foreach (var p in p_set.Properties)
                {
                    if (p is IIfcPropertySingleValue pv && pv.NominalValue != null)
                        mp[pv.Name.ToString().ToUpper()] = (double)pv.NominalValue.Value;
                }
            }
            var mats = new List<AnsysMaterial>();
            var mat = new AnsysMaterial()
            {
                ID = m.EntityLabel
            };
            if (mp.ContainsKey("YoungModulus"))
            {
                mat.Lab = AnsysMaterialLableEnum.EX;
                mat.C0 = mp["YoungModulus"];
                mats.Add(mat);
            }
            if(mp.ContainsKey("PoissonRatio"))
            {
                mat.Lab = AnsysMaterialLableEnum.PRXY;
                mat.C0 = mp["PoissonRatio"];
                mats.Add(mat);
            }
            if(mp.ContainsKey("MassDensity"))
            {
                mat.Lab = AnsysMaterialLableEnum.DENS;
                mat.C0 = mp["MassDensity"];
                mats.Add(mat);
            }
            return mats;
        }
    }
}
