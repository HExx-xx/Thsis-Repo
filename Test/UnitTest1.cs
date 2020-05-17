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
    //[TestClass]
    //public class UnitTest1
    //{ 

    //    [TestMethod]
    //    public void TestMethod()
    //    {
    //        using (var FrameCoreCube = new Building_factory())
    //        {
    //            var para = new List<double>() { 4000, 5000, 4000 };
    //            var spacing = parament.getList<List<double>>(para,10);
    //            var span = parament.getList<List<double>>(para, 10);
    //            var Layer_height = parament.getList<double>(4000, 10);
    //            FrameCoreCube.GeneratePlacementMap(spacing, span, Layer_height);
    //            FrameCoreCube.Build();
    //        }
    //    }
    //}
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod2()
        {
            using (var ShearWall = new Building_factory())
            {
                int z1 = 4200, z2 = 3600, z3 = 2800;
                //COLUMN
                var cols1 = new List<(double, double, double)>() {                                                                                                                    (43600,0,z1),(43600+6800,0,z1),(43600+6800*2,0,z1),(43600+6800*3,0,z1),
                (0,6000,z1),(7200,6000,z1),(14400,6000,z1),(21600,6000,z1),(28800,6000,z1),(36000,6000,z1),(43200,6000,z1),                      (43600,6000,z1),(43600+6800,6000,z1),(43600+6800*2,6000,z1),(43600+6800*3,6000,z1),
                (0,13200,z1),(7200,13200,z1),(14400,13200,z1),(21600,13200,z1),(28800,13200,z1),(36000,13200,z1),(43200,13200,z1),         (43600,12200,z1),(43600+6800,12200,z1),(43600+6800*2,12200,z1),(43600+6800*3,12200,z1),
                (3100,20400,z1),(7600,20400,z1),(13600,20400,z1),(19600,20400,z1),(25600,20400,z1),(31600,20400,z1),(37600,20400,z1),(43200,20400,z1),
                (3100,27000,z1),(7600,27000,z1),(13600,27000,z1),(19600,27000,z1),(25600,27000,z1),(31600,27000,z1),(37600,27000,z1),(43200,27000,z1),
                (3100,33200,z1),(7600,33200,z1),(13600,33200,z1),(19600,33200,z1),(25600,33200,z1),(31600,33200,z1),(37600,33200,z1),(43200,33200,z1),
                (43600,20400,z1),(43600,27000,z1),(43600,33200,z1),(46496,29919,z1),(51198,25217,z1),(55795,20620,z1),(60709,15706,z1),(52259,17085,z1),(47663,21682,z1)
                };
                var cols2 = new List<(double, double, double)>() { };
                for (int i = 0; i < cols1.Count; i++)
                {
                    (double, double, double) item = (cols1[i].Item1, cols1[i].Item2, cols1[i].Item3 + z2);
                    cols2.Add(item);
                }
                var cols3 = new List<(double, double, double)>() { };
                for (int i = 0; i < cols2.Count; i++)
                {
                    (double, double, double) item = (cols2[i].Item1, cols2[i].Item2, cols2[i].Item3 + z2);
                    cols3.Add(item);
                }
                var cols4 = new List<(double, double, double)>()
                {
                    (0, 6000, z1+z2*2+z3), (7200, 6000,  z1+z2*2+z3), (14400, 6000,  z1+z2*2+z3), (21600, 6000, z1+z2*2+z3), (28800, 6000,  z1+z2*2+z3), (36000, 6000, z1+z2*2+z3), (43200, 6000, z1+z2*2+z3),
                    (0, 13200, z1+z2*2+z3), (7200, 13200, z1+z2*2+z3), (14400, 13200, z1+z2*2+z3), (21600, 13200, z1+z2*2+z3), (28800, 13200, z1+z2*2+z3), (36000, 13200, z1+z2*2+z3), (43200, 13200, z1+z2*2+z3),
                    (3100, 20400, z1+z2*2+z3), (7600, 20400, z1+z2*2+z3), (13600, 20400, z1+z2*2+z3), (19600, 20400, z1+z2*2+z3), (25600, 20400, z1+z2*2+z3), (31600, 20400, z1+z2*2+z3), (37600, 20400, z1+z2*2+z3), (43200, 20400, z1+z2*2+z3)
                };           
                var cols5 = new List<(double, double, double)>() { };
                for (int i = 0; i < cols4.Count; i++)
                {
                    (double, double, double) item = (cols4[i].Item1, cols4[i].Item2, cols4[i].Item3 + z3);
                    cols5.Add(item);
                }
                var cols6 = new List<(double, double, double)>() { };
                for (int i = 0; i < cols5.Count; i++)
                {
                    (double, double, double) item = (cols5[i].Item1, cols5[i].Item2, cols5[i].Item3 + z3);
                    cols6.Add(item);
                }
                var cols7 = new List<(double, double, double)>() { };
                for (int i = 0; i < cols6.Count; i++)
                {
                    (double, double, double) item = (cols6[i].Item1, cols6[i].Item2, cols6[i].Item3 + z3);
                    cols7.Add(item);
                }
                var cols8 = new List<(double, double, double)>() { };
                for (int i = 0; i < cols7.Count; i++)
                {
                    (double, double, double) item = (cols7[i].Item1, cols7[i].Item2, cols7[i].Item3 + z3);
                    cols8.Add(item);
                }
                var cols9 = new List<(double, double, double)>() { };
                for (int i = 0; i < cols8.Count; i++)
                {
                    (double, double, double) item = (cols8[i].Item1, cols8[i].Item2, cols8[i].Item3 + z3);
                    cols9.Add(item);
                }

                List<List<(double, double, double)>> colList = new List<List<(double, double, double)>>() { cols1, cols2, cols3, cols4, cols5, cols6, cols7, cols8, cols9 };
                ShearWall.GenerateColumnMap(colList);

                //BEAM
                List<((double, double, double), (double, double, double))> FromLineToBeamList(List<List<(double,double,double)>> line)
                {
                   var  List= new List<((double, double, double), (double, double, double))>();
                    foreach(var l in line)
                    {
                        for (int i = 0; i+1< l.Count; i++)
                        {
                            List.Add((l[i], l[i + 1]));
                        }
                    }                   
                    return List;
                }
                var beamLine1 = new List<List<(double, double, double)>>()
                {
                    new List<(double, double, double)>(){ (3100,20400,z1), (7600, 20400,z1), (13600,20400,z1), (19600,20400,z1), (25600, 20400, z1), (31600, 20400, z1), (37600, 20400, z1), (43200, 20400, z1)},
                    new List<(double, double, double)>(){(3100,27000,z1),(7600,27000,z1),(13600,27000,z1),(19600,27000,z1),(25600,27000,z1),(31600,27000,z1),(37600,27000,z1),(43200,27000,z1)},
                    new List<(double, double, double)>(){(3100,33200,z1),(7600,33200,z1),(13600,33200,z1),(19600,33200,z1),(25600,33200,z1),(31600,33200,z1),(37600,33200,z1),(43200,33200,z1)},

                    new List<(double, double, double)>(){ (43600, 0, z1), (43600 + 6800, 0, z1), (43600 + 6800 * 2, 0, z1), (43600 + 6800 * 3, 0, z1) },
                    new List<(double, double, double)>(){(43600,6000,z1),(43600+6800,6000,z1),(43600+6800*2,6000,z1),(43600+6800*3,6000,z1) },
                    new List<(double, double, double)>(){(43600,12200,z1),(43600+6800,12200,z1),(43600+6800*2,12200,z1),(43600+6800*3,12200,z1) },

                    new List<(double, double, double)>(){ (3100, 20400, z1), (3100, 27000, z1), (3100, 33200, z1)},
                    new List<(double, double, double)>(){ (7600, 20400, z1), (7600, 27000, z1), (7600, 33200, z1) },
                    new List<(double, double, double)>(){ (13600, 20400, z1), (13600, 27000, z1), (13600,33200,z1)},
                    new List<(double, double, double)>(){ (19600, 20400, z1),(19600,27000,z1),(19600,33200,z1)},
                    new List<(double, double, double)>(){ (25600, 20400, z1), (25600, 27000, z1),(25600,33200,z1)},
                    new List<(double, double, double)>(){ (31600, 20400, z1), (31600, 27000, z1),(31600,33200,z1)},
                    new List<(double, double, double)>(){ (37600, 20400, z1), (37600, 27000, z1),(37600,33200,z1)},
                    new List<(double, double, double)>(){ (43200, 20400, z1), (43200, 27000, z1), (43200,33200,z1)},

                    new List<(double, double, double)>(){ (43600, 0, z1), (43600,6000,z1),(43600,12200,z1), (43600, 20400, z1), (43600, 27000, z1), (43600, 33200,z1) },
                    new List<(double, double, double)>(){ (43600 + 6800, 0, z1), (43600 + 6800, 6000, z1), (43600 + 6800, 12200, z1) },
                    new List<(double, double, double)>(){ (43600 + 6800 * 2, 0, z1), (43600 + 6800 * 2, 6000, z1), (43600 + 6800 * 2, 12200, z1) },
                    new List<(double, double, double)>(){ (43600 + 6800 * 3, 0, z1), (43600 + 6800 * 3, 6000, z1), (43600 + 6800 * 3, 12200, z1) },

                    new List<(double, double, double)>(){(43600,33200,z1),(46496,29919,z1), (51198, 25217, z1) , (55795, 20620, z1) , (60709, 15706, z1) ,(64000,12200,z1)},
                     new List<(double, double, double)>(){ (46496, 29919, z1) ,(43600,27000,z1), (47663, 21682, z1), (51198, 25217, z1) },
                     new List<(double, double, double)>(){ (60709, 15706, z1), (57200,12200,z1), (52259, 17085, z1), (55795, 20620, z1)},
                     new List<(double, double, double)>(){(43600,20400,z1),(47663,21682,z1),(52259,17085,z1),(50400,12200,z1)},
                };
                var beamList1 = FromLineToBeamList(beamLine1);
                var beamList2 = new List<((double, double, double), (double, double, double))>();
                for (int i = 0; i < beamList1.Count; i++)
                {
                    ((double, double, double),(double,double,double)) item = ((beamList1[i].Item1.Item1, beamList1[i].Item1.Item2, beamList1[i].Item1.Item3 + z2), (beamList1[i].Item2.Item1, beamList1[i].Item2.Item2, beamList1[i].Item2.Item3 + z2));
                    beamList2.Add(item);
                }
                var beamList3 = new List<((double, double, double), (double, double, double))>();
                for (int i = 0; i < beamList2.Count; i++)
                {
                    ((double, double, double), (double, double, double)) item = ((beamList2[i].Item1.Item1, beamList2[i].Item1.Item2, beamList2[i].Item1.Item3 + z2), (beamList2[i].Item2.Item1, beamList2[i].Item2.Item2, beamList2[i].Item2.Item3 + z2));
                    beamList3.Add(item);
                }

                var beamList = new List<List<((double, double, double), (double, double, double))>>() { beamList1, beamList2,beamList3 };
                ShearWall.GenerateBeamMap(beamList);

                //Slab
                List<List<(double, double, double)>> FromMatrixToList(List<double>x,List<double>y,List<double >z)
                {
                    var list = new List<List<(double, double, double)>>();
                    double x0, y0,z0=0,a=0,b=0;
                    for (int k = 0; k < z.Count; k++)
                    {
                        z0 += z[k];
                        x0 = x[0];
                        for (int i = 1; i < x.Count; i++)
                        {
                            a = x[i];
                            y0 = y[0];
                            for (int j = 1; j < y.Count; j++)
                            {
                                b = y[j];
                                var tmp = new List<(double, double, double)> { (x0, y0, z0), (x0 + a, y0, z0), (x0 + a, y0 + b, z0), (x0, y0 + b, z0), (x0, y0, z0) };
                                list.Add(tmp);
                                y0 += b;
                            }
                            x0 += a;
                        }
                    }
                    return list;
                }
                var slabList1 = FromMatrixToList(new List<double>() { 43600, 6800, 6800, 6800 }, new List<double>() { 0, 6000, 6200 }, new List<double>() { 4200, 3600, 3600 });
                var slabList2 = FromMatrixToList(new List<double>() { 0, 7200, 7200, 7200, 7200, 7200, 7200 }, new List<double> { 6000,7200, 7200 }, new List<double>() { 4200, 3600, 3600,2800,2800,2800,2800,2800,2800 });
                var slabList3 = FromMatrixToList(new List<double> { 3100, 4500, 6000, 6000, 6000, 6000, 6000, 5600 }, new List<double> { 20400, 6600, 6200 }, new List<double>() { 4200, 3600, 3600 });
                var slabList4 = new List<List<(double, double, double)>>
                {
                    new List<(double, double, double)>{(43600,33200,z1),(43600,27000,z1),(46496,29919,z1), (43600, 33200, z1) },
                    new List<(double, double, double)>{ (43600, 27000, z1) , (43600, 20400, z1), (47663,21682,z1), (43600, 27000, z1) },
                    new List<(double, double, double)>{(43600, 27000, z1), (47663, 21682, z1),(51198,25217,z1), (46496, 29919, z1), (43600, 27000, z1) },
                    new List<(double, double, double)>{ (43600, 20400, z1),(43600,12200,z1),(50400,12200,z1),(52259,17085,z1), (47663, 21682, z1), (43600, 20400, z1)},
                    new List<(double, double, double)>{(47663, 21682, z1), (52259, 17085, z1),(55795,20620,z1), (51198, 25217, z1),(47663, 21682, z1)},
                    new List<(double, double, double)>{ (50400, 12200, z1) , (57200, 12200, z1), (52259, 17085, z1), (50400, 12200, z1) },
                    new List<(double, double, double)>{ (52259, 17085, z1) , (57200, 12200, z1),(60709,15706,z1), (55795, 20620, z1), (52259, 17085, z1) },
                    new List<(double, double, double)>{ (57200, 12200, z1) ,(64000,12200,z1), (60709, 15706, z1), (57200, 12200, z1) }
                };
                var slabList5 = new List<List<(double, double, double)>>();
                for(int i = 0;i<slabList4.Count;i++)
                {
                    var tmp = new List<(double, double, double)>();
                    for(int j = 0;j<slabList4[i].Count;j++)
                    {
                        tmp.Add((slabList4[i][j].Item1, slabList4[i][j].Item2, slabList4[i][j].Item3 + z2));
                    }
                    slabList5.Add(tmp);
                }
                var slabList6 = new List<List<(double, double, double)>>();
                for (int i = 0; i < slabList4.Count; i++)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int j = 0; j < slabList4[i].Count; j++)
                    {
                        tmp.Add((slabList4[i][j].Item1, slabList4[i][j].Item2, slabList4[i][j].Item3 + z2*2));
                    }
                    slabList6.Add(tmp);
                }

                var slabList = new List<List<List<(double, double, double)>>>() { slabList1,slabList2,slabList3, slabList4 ,slabList5,slabList6};
                ShearWall.GenerateSlabMap(slabList);

                //Wall
                const double EPSON = 0.000001;
                List<List<(double, double, double)>> FromInputToPointList(List<((double, double, double), (double, double, double, double))> list)
                {
                    var List = new List<List<(double, double, double)>>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        var CentralP = list[i].Item1;
                        var Info = list[i].Item2;
                        int count = 0;
                        if (Info.Item1 > EPSON)
                            count++;
                        if (Info.Item2 > EPSON)
                            count++;
                        if (Info.Item3 > EPSON)
                            count++;
                        if (Info.Item4 > EPSON)
                            count++;

                        var TmpPs = new List<(double, double, double)>();
                        if (count == 2)
                        {
                            if (Info.Item1 > EPSON && Info.Item3 > EPSON)
                            {
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - 100, CentralP.Item3));
                            }
                            else if (Info.Item2 > EPSON && Info.Item4 > EPSON)
                            {
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - 100, CentralP.Item3));
                            }
                            else if (Info.Item1 > EPSON && Info.Item4 > EPSON)
                            {
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + 100, CentralP.Item3));
                            }
                            else if (Info.Item2 > EPSON && Info.Item3 > EPSON)
                            {
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + 100, CentralP.Item3));
                            }
                        }
                        if (count == 3)
                        {
                            if (Info.Item1 < EPSON && Info.Item1 > -EPSON)
                            {

                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + 100, CentralP.Item3));
                            }
                            else if (Info.Item2 < EPSON && Info.Item2 > -EPSON)
                            {
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - 100, CentralP.Item3));
                            }
                            else if (Info.Item3 < EPSON && Info.Item3 > -EPSON)
                            {
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - Info.Item4, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 - 100, CentralP.Item3));
                            }
                            else if (Info.Item4 < EPSON && Info.Item4 > -EPSON)
                            {
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + Info.Item3, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - 100, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 - Info.Item2, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 - 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + Info.Item1, CentralP.Item2 + 100, CentralP.Item3));
                                TmpPs.Add((CentralP.Item1 + 100, CentralP.Item2 + 100, CentralP.Item3));
                            }
                        }
                        List.Add(TmpPs);
                    }
                    return List;
                }

                var input = new List<((double, double, double), (double, double, double, double))>()
                {
                     ((0,6000,0),(1500,0,1500,0)),  ((7200,6000,0),(1400,1400,2700,0)),  ((14400,6000,0),(900,900,2700,0)),   ((21600,6000,0),(1400,1400,2700,0)),
                     ((28800,6000,0),(900,900,2700,0)),  ((36000,6000,0),(1400,1400,2700,0)),  ((43200,6000,0),(0,1500,1500,0)),

                     ((0,13200,0),(3400,0,700,700)),  ((7200,13200,0),(600,0,0,2100)),  ((14400,13200,0),(600,600,0,2100)),   ((21600,13200,0),(600,600,0,2100)),
                     ((28800,13200,0),(600,600,0,2100)),  ((36000,13200,0),(0,600,0,2100)),  ((43200,13200,0),(0,1700,700,700)),

                     ((0,20400,0),(1600,0,0,600)),  ((7200,20400,0),(900,900,0,2200)),  ((14400,20400,0),(900,900,0,1700)),   ((21600,20400,0),(900,900,0,1700)),
                     ((28800,20400,0),(900,900,0,1700)),  ((36000,20400,0),(900,900,0,2200)),  ((43200,20400,0),(0,1600,0,600))
                };
                var wallList1 = FromInputToPointList(input);
                var wallList2 = new List<List<(double, double, double)>>();
                foreach(var l in wallList1)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z1));
                    wallList2.Add(tmp);
                }
                var wallList3 = new List<List<(double, double, double)>>();
                foreach (var l in wallList2)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z2));
                    wallList3.Add(tmp);
                }
                var wallList4 = new List<List<(double, double, double)>>();
                foreach (var l in wallList3)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z2));
                    wallList4.Add(tmp);
                }
                var wallList5 = new List<List<(double, double, double)>>();
                foreach (var l in wallList4)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z3));
                    wallList5.Add(tmp);
                }
                var wallList6 = new List<List<(double, double, double)>>();
                foreach (var l in wallList5)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z3));
                    wallList6.Add(tmp);
                }
                var wallList7 = new List<List<(double, double, double)>>();
                foreach (var l in wallList6)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z3));
                    wallList7.Add(tmp);
                }
                var wallList8 = new List<List<(double, double, double)>>();
                foreach (var l in wallList7)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z3));
                    wallList8.Add(tmp);
                }
                var wallList9 = new List<List<(double, double, double)>>();
                foreach (var l in wallList8)
                {
                    var tmp = new List<(double, double, double)>();
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add((l[i].Item1, l[i].Item2, l[i].Item3 + z3));
                    wallList9.Add(tmp);
                }
                var wallList = new List<List<List<(double, double, double)>>>() { wallList1, wallList2, wallList3, wallList4, wallList5, wallList6, wallList7, wallList8,wallList9 };

                ShearWall.GenerateWallMap(wallList);

                                                       
                ShearWall.Build();
            }
        }
    }
}



