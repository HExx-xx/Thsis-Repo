using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ifc2APDL.AnsysFactory;
using ifc2APDL.AnsysFactory.models;

namespace Test
{
    [TestClass]
    public class AnsysTest
    {
        [TestMethod]
        public void AnsysWriteFileTest()
        {
            string PATH = "../../TestFiles/ansys-test.txt";
            var ans = new AnsysStore();

            //Add element
            var ele = new List<AnsysElement>();
            {
                new AnsysElement() { ID = 1, Type = AnsysElementTypeEnum.BEAM188 };
                new AnsysElement() { ID = 2, Type = AnsysElementTypeEnum.SHELL63 };
            }
            ans.AddElement(ele);

            //Add material
            var mat = new List<AnsysMaterial>()
            {
                new AnsysMaterial(1,AnsysMaterialLableEnum.EX,3.3e+10),
                new AnsysMaterial(1, AnsysMaterialLableEnum.PRXY, 0.2),
                new AnsysMaterial(1,AnsysMaterialLableEnum.DENS,2500),
                new AnsysMaterial(2,AnsysMaterialLableEnum.EX,2e+11),
                new AnsysMaterial(2,AnsysMaterialLableEnum.PRXY,0.3),
                new AnsysMaterial(2,AnsysMaterialLableEnum.DENS,7850)
            };
            ans.AddMaterial(mat);

            //Add section
            var sec = new List<AnsysSection>()
            {
                new AnsysSection()
                {
                    type=new AnsysSecType() { ID=1,type=AnsysSectionTypeEnum.BEAM,subtype=AnsysSectionSubtypeEnum.RECT},
                    data=new AnsysSecData() { v1 = 0.4, v2 = 0.4 }
                },
                new AnsysSection()
                {
                    type=new AnsysSecType(){ID=2,type=AnsysSectionTypeEnum.BEAM,subtype=AnsysSectionSubtypeEnum.RECT},
                    data=new AnsysSecData(){v1=0.2,v2=0.4}
                }
            };
            ans.AddSection(sec);

            ans.WriteAPDLFile(PATH);
            Assert.IsTrue(File.Exists(PATH));
        }
    }
}
