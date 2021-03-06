﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using ifc2APDL.Translator;

namespace Test
{

    [TestClass]
    public class TranslateTest
    {
        [TestMethod]
        public void TranslateShearWallTest()
        {
            const string PATH = "../../TestFiles/shearwall.ifc";
            const string OUTPUT = "../../TestFiles/shearwall-trans.txt";
            //const string PATH = "../../TestFiles/building.ifc";
            //const string OUTPUT = "../../TestFiles/trans.txt";

            using (var model = IfcStore.Open(PATH))
            {
                var worker = new Worker(model);
                worker.Run();
                worker.WriteAPDLFile(OUTPUT);
            }
            Assert.IsTrue(File.Exists(OUTPUT));
        }



    }
}
