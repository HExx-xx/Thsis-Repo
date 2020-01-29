using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProductExtension;
using BuildingRepo;

class parament
{
    public static List<T> getList<T>(T t1, int n)
    {
        var tmp = new List<T>();
        for (int i = 0; i < n; i++)
        {
            tmp.Add(t1);
        }
        return tmp;
    }
}

namespace Test
{
    [TestClass]
    public class UnitTest1
    { 

        [TestMethod]
        public void TestMethod()
        {
            using (var FrameCoreCube = new Building_factory())
            {
                var para = new List<double>() { 4000, 5000, 4000 };
                var spacing = parament.getList<List<double>>(para,10);
                var span = parament.getList<List<double>>(para, 10);
                var Layer_height = parament.getList<double>(4000, 10);
                FrameCoreCube.GeneratePlacementMap(spacing, span, Layer_height);
                FrameCoreCube.Build();
            }
        }
    }

}

