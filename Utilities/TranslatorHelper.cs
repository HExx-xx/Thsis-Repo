using ifc2APDL.AnsysFactory.models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;




namespace Utilities
{
    public class TranslatorHelper
    {
        public static List<AnsysMaterial> TranslateMaterial(Dictionary<string, IIfcMaterial> set)
        {
            var mats = new List<AnsysMaterial>();
            int id = 1;
            foreach (var single in set)
            {
                var mp = new Dictionary<string, double>();
                foreach (var p_set in single.Value.HasProperties)
                {
                    foreach (var p in p_set.Properties)
                    {
                        if (p is IIfcPropertySingleValue pv && pv.NominalValue != null)
                            mp[pv.Name.ToString().ToUpper()] = (double)pv.NominalValue.Value;
                    }
                }
                var mat = new AnsysMaterial()
                {
                    ID = id
                };
                if (mp.ContainsKey("YOUNGMODULUS"))
                {
                    mat.Lab = AnsysMaterialLableEnum.EX;
                    mat.C0 = mp["YOUNGMODULUS"];
                    mats.Add(mat);
                    mat = new AnsysMaterial()
                    {
                        ID = id
                    };
                }
                if (mp.ContainsKey("POISSONRATIO"))
                {
                    mat.Lab = AnsysMaterialLableEnum.PRXY;
                    mat.C0 = mp["POISSONRATIO"];
                    mats.Add(mat);
                    mat = new AnsysMaterial()
                    {
                        ID = id
                    };
                }
                if (mp.ContainsKey("MASSDENSITY"))
                {
                    mat.Lab = AnsysMaterialLableEnum.DENS;
                    mat.C0 = mp["MASSDENSITY"];
                    mats.Add(mat);
                    mat = new AnsysMaterial()
                    {
                        ID = id
                    };
                }
                id++;

            }
            return mats;
        }

        public static AnsysElement TranslateElement(string s)
        {
            var ele = new AnsysElement();
            if (s == "Column" || s == "Beam")
                ele.Type = AnsysElementTypeEnum.BEAM188;
            else if (s == "Slab" || s == "Wall")
                ele.Type = AnsysElementTypeEnum.SHELL63;
            return ele;
        }
        public static (double, double) TranslateProfile(IIfcExtrudedAreaSolid solid)
        {
            double x = 0, y = 0;
            if (solid.SweptArea is IIfcRectangleProfileDef rp)
            {
                x = rp.XDim;
                y = rp.YDim;
            }
            return (x, y);
        }
        public static List<AnsysSection> TranslateSection(Dictionary<int, (double, double)> profiles)
        {
            var secs = new List<AnsysSection>();
            foreach (var pro in profiles)
            {
                var cross = new AnsysSection()
                {
                    type = new AnsysSecType()
                    {
                        ID = pro.Key,
                        type = AnsysSectionTypeEnum.BEAM,
                        subtype = AnsysSectionSubtypeEnum.RECT
                    },
                    data = new AnsysSecData() { v1 = pro.Value.Item1, v2 = pro.Value.Item2 }
                };
                secs.Add(cross);
            }
            return secs;
        }
        
      //  public static int TranslateProfileID((double,double)pro,Dictionary<int,(double,double)>Pros){}

        public static List<(double, double, double)> TranslateKP(IIfcExtrudedAreaSolid solid)
        {
            var kps = new List<(double, double, double)>();
            if (solid.SweptArea is IIfcRectangleProfileDef)
            {
                var pos = solid.Position;
                var dep = solid.Depth;
                var startpoint = ToXbimPoint3D(pos.Location);
                var endpoint = startpoint + ToXbimVector3D(pos.Axis).Normalized() * dep;
                kps.Add((startpoint.X, startpoint.Y, startpoint.Z));
                kps.Add((endpoint.X, endpoint.Y, endpoint.Z));
            }
            else if (solid.SweptArea is IIfcArbitraryClosedProfileDef acp)
            {
                if (acp.OuterCurve is IIfcPolyline pl)
                {
                    for (int i = 0; i < pl.Points.Count - 1; i++)
                    {
                        var kp = ToXbimPoint3D(pl.Points[i]);
                        kps.Add((kp.X, kp.Y, kp.Z));
                    }
                }
            }
            return kps;
        }


        public static double TranslateWallR(IIfcExtrudedAreaSolid solid)
        {
            double r = 0;
            if (solid.SweptArea is IIfcArbitraryClosedProfileDef acp)
            {
                if (acp.OuterCurve is IIfcPolyline pl)
                {
                    r = GetLength(pl.Points[0], pl.Points[1]);
                    for (int i = 1; i < pl.Points.Count - 1; i++)
                    {
                        var l = GetLength(pl.Points[i], pl.Points[i + 1]);
                        if (r > l)
                            r = l;
                    }
                }
            }
            return r;
        } // GET RealConstant

        public static List<(double, double, double)> TranslateWallKP(IIfcExtrudedAreaSolid solid, double realconstant, double height)
        {
            var kps = new List<(double, double, double)>();
            if (solid.SweptArea is IIfcArbitraryClosedProfileDef acp)
            {
                if (acp.OuterCurve is IIfcPolyline pl)
                {
                    if (pl.Points.Count == 7)
                    {
                        double x1, x2, x3, y1, y2, y3, z = pl.Points[0].Z;
                        if (GetLength(pl.Points[0], pl.Points[5]) - realconstant < 0.01)
                        {
                            if (pl.Points[0].X == pl.Points[5].X)
                            {
                                x1 = pl.Points[0].X; y1 = (pl.Points[0].Y + pl.Points[5].Y) / 2;
                                x3 = (pl.Points[2].X + pl.Points[3].X) / 2; y3 = pl.Points[2].Y;
                                x2 = x3; y2 = y1;
                            }
                            else     //0、5点y轴坐标一样
                            {
                                x1 = (pl.Points[0].X + pl.Points[5].X) / 2; y1 = pl.Points[0].Y;
                                x3 = pl.Points[2].X; y3 = (pl.Points[2].Y + pl.Points[3].Y) / 2;
                                x2 = x1; y2 = y3;
                            }
                        }
                        else if (GetLength(pl.Points[0], pl.Points[1]) - realconstant < 0.01)
                        {
                            if (pl.Points[0].X == pl.Points[1].X)
                            {
                                x1 = pl.Points[0].X; y1 = (pl.Points[0].Y + pl.Points[1].Y) / 2;
                                x3 = (pl.Points[3].X + pl.Points[4].X) / 2; y3 = pl.Points[3].Y;
                                x2 = x3; y2 = y1;
                            }
                            else
                            {
                                x1 = (pl.Points[0].X + pl.Points[1].X) / 2; y1 = pl.Points[0].Y;
                                x3 = pl.Points[3].X; y3 = (pl.Points[3].Y + pl.Points[4].Y) / 2;
                                x2 = x1; y2 = y3;
                            }
                        }
                        else
                        {
                            if (pl.Points[1].X == pl.Points[2].X)
                            {
                                x1 = pl.Points[1].X; y1 = (pl.Points[1].Y + pl.Points[2].Y) / 2;
                                x3 = (pl.Points[4].X + pl.Points[5].X) / 2; y3 = pl.Points[4].Y;
                                x2 = x3; y2 = y1;
                            }
                            else
                            {
                                x1 = (pl.Points[1].X + pl.Points[2].X) / 2; y1 = pl.Points[1].Y;
                                x3 = pl.Points[4].X; y3 = (pl.Points[4].Y + pl.Points[5].Y) / 2;
                                x2 = x1; y2 = y3;
                            }
                        }
                        kps.AddRange(new List<(double, double, double)> { (x1, y1, z), (x2, y2, z), (x3, y3, z), (x1, y1, z + height), (x2, y2, z + height), (x3, y3, z + height) });
                    }
                    if (pl.Points.Count == 9)
                    {
                        double x1, x2, x3, x4, y1, y2, y3, y4, z = pl.Points[0].Z;
                        var tmps = new List<(double, double)>();
                        if (GetLength(pl.Points[0], pl.Points[7]) - realconstant < 0.01)
                        {
                            if (pl.Points[0].X == pl.Points[7].X)
                                tmps.Add((pl.Points[0].X, (pl.Points[0].Y + pl.Points[7].Y) / 2));
                            else if (pl.Points[0].Y == pl.Points[7].Y)
                                tmps.Add(((pl.Points[0].X + pl.Points[7].X) / 2, pl.Points[0].Y));
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            if (GetLength(pl.Points[i], pl.Points[i + 1]) - realconstant < 0.01)
                                if (pl.Points[i].X == pl.Points[i + 1].X)
                                    tmps.Add((pl.Points[i].X, (pl.Points[i].Y + pl.Points[i + 1].Y) / 2));
                                else if (pl.Points[i].Y == pl.Points[i + 1].Y)
                                    tmps.Add(((pl.Points[i].X + pl.Points[i + 1].X) / 2, pl.Points[i].Y));
                        }
                        if((tmps[0].Item1!=tmps[1].Item1)&&(tmps[0].Item1!=tmps[2].Item1)&&(tmps[1].Item1!=tmps[2].Item1))
                        {
                            if(tmps[0].Item2==tmps[1].Item2)
                            {
                                x1 = tmps[0].Item1; y1 = tmps[0].Item2;
                                x2 = tmps[1].Item1;y2 = tmps[1].Item2;
                                x4= tmps[2].Item1; y4 = tmps[2].Item2;
                                x3 =x4; y3 =y1;
                            }
                            else if(tmps[0].Item2 == tmps[2].Item2)
                            {
                                x1 = tmps[0].Item1; y1 = tmps[0].Item2;
                                x2 = tmps[2].Item1; y2 = tmps[2].Item2;
                                x4 = tmps[1].Item1; y4 = tmps[1].Item2;
                                x3 = x4; y3 = y1;
                            }
                            else
                            {
                                x1 = tmps[1].Item1; y1 = tmps[1].Item2; 
                                x2 = tmps[2].Item1; y2 = tmps[2].Item2;
                                x4 = tmps[0].Item1; y4 = tmps[0].Item2;
                                x3 = x4; y3 = y1;
                            }
                        }
                        else
                        {
                            if(tmps[0].Item1 == tmps[1].Item1)
                            {
                                x1 = tmps[0].Item1; y1 = tmps[0].Item2;
                                x2 = tmps[1].Item1; y2 = tmps[1].Item2;
                                x4 = tmps[2].Item1; y4 = tmps[2].Item2;
                                x3 = x1; y3 = y4;
                            }
                            else if (tmps[0].Item1 == tmps[2].Item1)
                            {
                                x1 = tmps[0].Item1; y1 = tmps[0].Item2;
                                x2 = tmps[2].Item1; y2 = tmps[2].Item2;
                                x4 = tmps[1].Item1; y4 = tmps[1].Item2;
                                x3 = x1; y3 = y4;
                            }
                            else
                            {
                                x1 = tmps[1].Item1; y1 = tmps[1].Item2;
                                x2 = tmps[2].Item1; y2 = tmps[2].Item2;
                                x4 = tmps[0].Item1; y4 = tmps[0].Item2;
                                x3 = x1; y3 = y4;
                            }
                        }
                        kps.AddRange(new List<(double, double, double)> { (x1, y1, z), (x2, y2, z), (x3, y3, z), (x4, y4, z), (x1, y1, z + height), (x2, y2, z + height), (x3, y3, z + height), (x4, y4, z + height) });
                    }
                }
             }
            return kps;
        }


        public static (int,int) GetLineID(List<(double,double,double)> kps, Dictionary<int, (double, double, double)>KPs)
        {
            int ID1=0, ID2=0;
            for (int i = 1; i <= KPs.Count; i++)
            {
                if (KPs[i] == kps[0])
                    ID1 = i;
                if (KPs[i] == kps[1])
                    ID2 = i;
            }
            return (ID1, ID2);
            
        }
        public static List<int>GetAreaID(List<(double,double,double)>kps,Dictionary<int,(double,double,double)>KPs)
        {
            var areaID= new List<int>();
            int ID1=0,ID2=0,ID3=0,ID4 = 0,ID5=0;
            if (kps.Count == 3)
            {
                for (int i = 1; i <= KPs.Count; i++)
                {
                    if (KPs[i] == kps[0])
                        ID1 = i;
                    if (KPs[i] == kps[1])
                        ID2 = i;
                    if (KPs[i] == kps[2])
                        ID3 = i;
                }
                areaID.AddRange(new List<int> { ID1, ID2, ID3 } );
            }
            if (kps.Count == 4)
            {
                for (int i = 1; i <= KPs.Count; i++)
                {
                    if (KPs[i] == kps[0])
                        ID1 = i;
                    if (KPs[i] == kps[1])
                        ID2 = i;
                    if (KPs[i] == kps[2])
                        ID3 = i;
                    if (KPs[i] == kps[3])
                        ID4 = i;
                }
                areaID.AddRange(new List<int>{ ID1, ID2, ID3, ID4 });
            }
            if (kps.Count == 5)
            {
                for (int i = 1; i <= KPs.Count; i++)
                {
                    if (KPs[i] == kps[0])
                        ID1 = i;
                    if (KPs[i] == kps[1])
                        ID2 = i;
                    if (KPs[i] == kps[2])
                        ID3 = i;
                    if (KPs[i] == kps[3])
                        ID4 = i;
                    if (KPs[i] == kps[4])
                        ID5 = i;
                }
                areaID.AddRange(new List<int> { ID1, ID2, ID3, ID4, ID5 });
            }
            return areaID;
        }


        public static List<List<int>> GetWallAreaID(List<(double, double, double)>kps,Dictionary<int, (double, double, double)> KPs)
        {
            var ids = new List<List<int>>();
            int ID1 = 0, ID2 = 0, ID3 = 0, ID4 = 0, ID5 = 0, ID6 = 0, ID7 = 0, ID8 = 0;
            if (kps.Count == 6)
            {
                for (int i = 1; i <= KPs.Count; i++)
                {
                    if (KPs[i] == kps[0]) ID1 = i;
                    if (KPs[i] == kps[1]) ID2 = i;
                    if (KPs[i] == kps[2]) ID3 = i;
                    if (KPs[i] == kps[3]) ID4 = i;
                    if (KPs[i] == kps[4]) ID5 = i;
                    if (KPs[i] == kps[5]) ID6 = i;
                }
                ids.Add(new List<int> { ID1, ID2, ID5, ID4 });
                ids.Add(new List<int> { ID2, ID3, ID6, ID5 });
            }
            else
            {
                for (int i = 1; i <= KPs.Count; i++)
                {
                    if (KPs[i] == kps[0]) ID1 = i;
                    if (KPs[i] == kps[1]) ID2 = i;
                    if (KPs[i] == kps[2]) ID3 = i;
                    if (KPs[i] == kps[3]) ID4 = i;
                    if (KPs[i] == kps[4]) ID5 = i;
                    if (KPs[i] == kps[5]) ID6 = i;
                    if (KPs[i] == kps[6]) ID7 = i;
                    if (KPs[i] == kps[7]) ID8 = i;
                }
                ids.Add(new List<int> { ID1, ID2, ID6, ID5 });
                ids.Add(new List<int> { ID3, ID4, ID8, ID7 });
            }
            return ids;
        }


        public static List<AnsysKeypoint> TranslateKeypoint(Dictionary<int, (double, double, double)> KPs)
        {
            var ks = new List<AnsysKeypoint>();
            foreach (var kp in KPs)
            {
                var k = new AnsysKeypoint(kp.Key, kp.Value.Item1, kp.Value.Item2, kp.Value.Item3);
                ks.Add(k);
            }

            return ks;
        }

        public static XbimVector3D ToXbimVector3D(IIfcDirection d)
        {
            return new XbimVector3D(d.X, d.Y, d.Z);
        }

        public static XbimPoint3D ToXbimPoint3D(IIfcCartesianPoint p)
        {
            return new XbimPoint3D(p.X, p.Y, p.Z);
        }

        public static double GetLength(IIfcCartesianPoint p1, IIfcCartesianPoint p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.Z - p2.Z) * (p1.Z - p2.Z));
        }
    }
}
