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
        public static List<AnsysMaterial> TranslateMaterial(Dictionary<string,IIfcMaterial> set)
        {
            var mats = new List<AnsysMaterial>();
            int id = 1;
            foreach(var single in set)
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

        public static AnsysElement TranslateElement(List<IIfcDefinitionSelect> t)
        {
            var el = new AnsysElement();
            if (t.FirstOrDefault() is IIfcColumn || t.FirstOrDefault() is IIfcBeam)
                el.Type = AnsysElementTypeEnum.BEAM188;
            else if (t.FirstOrDefault() is IIfcSlab || t.FirstOrDefault() is IIfcWall)
                el.Type = AnsysElementTypeEnum.SHELL63;
            return el;
        }



        public static (double,double) TranslateProfile(IIfcExtrudedAreaSolid solid)
        {
            double x=0, y=0;
            if (solid.SweptArea is IIfcRectangleProfileDef rp)
            {
                x = rp.XDim;
                y = rp.YDim;
            }
            return (x,y);
        }
        public static List<AnsysSection>TranslateSection(Dictionary<int,(double,double)> profiles)
        { 
            var secs = new List<AnsysSection>();
            foreach(var pro in profiles)
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
                if(acp.OuterCurve is IIfcPolyline pl)
                {
                    for(int i = 0;i<pl.Points.Count-1;i++)
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
                if(acp.OuterCurve is IIfcPolyline pl)
                {
                    r = GetLength(pl.Points[0], pl.Points[1]);
                    for(int i =1;i<pl.Points.Count-1;i++)
                    {
                        var l = GetLength(pl.Points[i], pl.Points[i + 1]);
                        if (r > l)
                            r = l;
                    }
                }
            }
            return r;
              
        }
        public static List<(double,double,double)> TranslateWallKP(IIfcExtrudedAreaSolid solid,double realconstant)
        {
            var kps = new List<(double, double, double)>();
            (double, double, double) kp1 = (0, 0, 0);
            (double, double, double) kp2 = (0, 0, 0);
            (double, double, double) kp3 = (0, 0, 0);
            if (solid.SweptArea is IIfcArbitraryClosedProfileDef acp)
            {
                if(acp.OuterCurve is IIfcPolyline pl)
                {
                    for (int i = 0; i < pl.Points.Count - 1; i++)
                    {
                        if (GetLength(pl.Points[i], pl.Points[i + 1]) - realconstant < 0.01)
                            if (pl.Points[i].X == pl.Points[i + 1].X)
                            {
                                kp1.Item1 = pl.Points[i].X;
                                kp1.Item2 = (pl.Points[i].Y + pl.Points[i + 1].Y) / 2;
                                kp1.Item3 = pl.Points[i].Z;
                            }
                            else if (pl.Points[i].Y == pl.Points[i + 1].Y)
                            {
                                kp2.Item1 = (pl.Points[i].X + pl.Points[i + 1].X) / 2;
                                kp2.Item2 = pl.Points[i].Y;
                                kp2.Item3 = pl.Points[i].Z;
                            }
                    }
                    kp3.Item1 = kp2.Item1;
                    kp3.Item2 = kp1.Item2;
                    kp3.Item3 = kp1.Item3;
                }
            }
            kps.Add(kp1);
            kps.Add(kp2);
            kps.Add(kp3);
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
        public static (int,int,int,int)GetAreaID(List<(double,double,double)>kps,Dictionary<int,(double,double,double)>KPs)
        {
            int ID1=0,ID2=0,ID3=0,ID4 = 0;
            for(int i = 1;i<=KPs.Count;i++)
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
            return (ID1, ID2, ID3, ID4);
        }
        public static List<(int,int,int,int)> GetWallAreaID(List<(double, double, double)>kps,Dictionary<int, (double, double, double)> KPs,double height)
        {
            var ids = new List<(int, int, int, int)>();
            var node0 = (kps[0].Item1,kps[0].Item2, kps[0].Item3+ height);
            var node1= (kps[1].Item1, kps[1].Item2, kps[1].Item3 + height);
            var node2 = (kps[2].Item1, kps[2].Item2, kps[2].Item3 + height);
            int ID1 = 0, ID2 = 0, ID3 = 0, ID4 = 0, ID5 = 0, ID6 = 0;
            for (int i=1;i<=KPs.Count;i++)
            {
                if (KPs[i] == kps[0])
                    ID1 = i;
                if (KPs[i] == kps[2])
                    ID2 = i;
                if (KPs[i] == node2)
                    ID3 = i;
                if (KPs[i] == node0)
                    ID4 = i;
                if (KPs[i] == kps[1])
                    ID5 = i;
                if (KPs[i] == node1)
                    ID6 = i;
                    
            }
            ids.Add((ID1, ID2, ID3, ID4));
            ids.Add((ID2, ID3, ID6, ID5));
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
