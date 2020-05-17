using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using ifc2APDL;
using ifc2APDL.AnsysFactory;
using Utilities;
using Xbim.Ifc4.MaterialResource;


namespace ifc2APDL.Translator
{
    public class Worker
    {
        private readonly IfcStore _ifcModel;
        private readonly AnsysStore _ansysStore = new AnsysStore();

        private readonly List<AnsysFactory.models.AnsysElement> ElementSet = new List<AnsysFactory.models.AnsysElement>();
        private readonly Dictionary<string,IIfcMaterial> MaterialSet = new Dictionary<string, IIfcMaterial>();
        private readonly Dictionary<int, (double,double)> Profiles = new Dictionary<int, (double,double)>();//ID,b*h
        private readonly Dictionary<int, double> RealConstantSet = new Dictionary<int, double>();

        private readonly Dictionary<int, (double, double, double)> KPs = new Dictionary<int, (double, double, double)>();
        private readonly Dictionary<int, (int, int)> LineIDs = new Dictionary<int, (int, int)>();
        private readonly Dictionary<int, List<int>> AreaIDs = new Dictionary<int, List<int>>();
        private readonly Dictionary<int, (int, int)> AglueIDs = new Dictionary<int, (int, int)>();

        private readonly Dictionary<(int, int, int, int), Dictionary<int, (int, int)>> LineInfos = new Dictionary<(int, int, int, int), Dictionary<int, (int, int)>>();
        private readonly Dictionary<(int, int, int, int), Dictionary<int, List<int>>> AreaInfos = new Dictionary<(int, int, int, int), Dictionary<int, List<int>>>();
        private readonly Dictionary<(int, int, int, int), Dictionary<int, (int, int)>> AglueInfos = new Dictionary<(int, int, int, int), Dictionary<int, (int, int)>>();

        private readonly Dictionary<(int, int, int, int), (Dictionary<int, List<int>>, Dictionary<int, (int, int)>)> areaInfos = new Dictionary<(int, int, int, int), (Dictionary<int, List<int>>, Dictionary<int, (int, int)>)>();
        public Worker(string path)
        {
            if (File.Exists(path))
                _ifcModel = IfcStore.Open(path);
            else
                throw new ArgumentException("Invalid path to open ifc model");
        }

        public Worker(IfcStore model)
        {
            _ifcModel = model;
        }

        public void Run()
        {
            try
            {
                TranslateBuilding();
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void TranslateBuilding()
        {
            TranslateColumn();
            TranslateBeam();
            TranslateSlab();
            TranslateWall();
            AddAll();
        }


        public void WriteAPDLFile(string path)   //接口：供测试单元调用
        {
            if (File.Exists(path))
                Console.WriteLine($"Warning:operation will overwrite the existing file {path}");
            string dir = new FileInfo(path).Directory.FullName;
            if (!Directory.Exists(dir))
                throw new ArgumentException($"Directory {dir} doesn't exist");
            _ansysStore.WriteAPDLFile(path);
        }

        


        private void TranslateColumn()
        {
           var columns = _ifcModel.Instances.OfType<IIfcColumn>()
                .Where(ea => ea.PredefinedType == IfcColumnTypeEnum.COLUMN)
                .ToList();

            //translate element & eleID
            if (columns == null)
                throw new InvalidOperationException("A buliding without columns cannot be processed!");
            else
            {
                var ele = TranslatorHelper.TranslateElement("Column");
                int flag = 0;   //标记
                for (int i = 0; i < ElementSet.Count; i++)
                {
                    if (ele.Type == ElementSet[i].Type)
                    { flag = 1; break; }  //ElementSet中存在同名的单元类型，则改变标记
                }
                if (flag == 0)   //若标记物未变，说明ElementSet中不存在同名的单元类型，将改单元类型的编号设为当前集合数+1，并将该ansyselement加入集合中
                {
                    ele.ID = ElementSet.Count + 1;
                    ElementSet.Add(ele);
                }
            }


            //translate material  
            foreach (var column in columns)
            {
                var material = (IIfcMaterial)column.Material;
                var name = material.Name;
                if (!MaterialSet.ContainsKey(name))
                {
                    MaterialSet.Add(name, material);
                }
            }


            //translate sections  
            foreach (var column in columns)
            {
                var solid = column.Representation.Representations[0].Items
                        .Where(geo => geo is IIfcExtrudedAreaSolid)
                        .Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var pro = TranslatorHelper.TranslateProfile(solid);
                if(!Profiles.ContainsValue(pro))
                {
                    int i = Profiles.Count + 1;
                    Profiles.Add(i, pro);
                }
            }


            //translate kps & lines
                //get the set of kps
            foreach (var column in columns)
            {
                var solid = column.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var keypoints = TranslatorHelper.TranslateKP(solid);
                foreach(var kp in keypoints)
                {
                    if(!KPs.ContainsValue(kp))
                    {
                        KPs.Add(KPs.Count + 1, kp);
                    }
                }
            }


               //get the set of line IDs
            foreach(var column in columns)
            {
                int matID=0, realID=0, eleID = 0,secID =0;
                     ////get matID
                var material = (IIfcMaterial)column.Material;  
                var name = material.Name;
                int n = 1;
                foreach(var mat in MaterialSet)
                {
                    if (mat.Key == name)
                        matID = n;
                    else
                        n++;
                }

                ////get eleID
                var ele = TranslatorHelper.TranslateElement("Column");
                for(int i=0;i<ElementSet.Count;i++)
                {
                    if (ElementSet[i].Type == ele.Type)
                        eleID = ElementSet[i].ID;
                }

                var solid = column.Representation.Representations[0].Items.   
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();

                    ////get secID
                var pro = TranslatorHelper.TranslateProfile(solid);
                foreach(var pair in Profiles)
                {
                    if (pair.Value == pro)
                        secID = pair.Key;
                }

                var m = 0;
                var keypoints = TranslatorHelper.TranslateKP(solid);
                var line = TranslatorHelper.GetLineID(keypoints,KPs);

                if(!LineIDs.ContainsValue(line))
                {
                    LineIDs.Add(LineIDs.Count + 1, line);
                    m = LineIDs.Count;
                }
                else
                {
                    for (int i = 0; i < LineIDs.Count; i++)
                        if (LineIDs[i] == line)
                            m = i;
                }

                if(!LineInfos.ContainsKey((matID, realID, eleID, secID)))
                {
                    var collector = new Dictionary<int,(int,int)>();
                    LineInfos.Add((matID, realID, eleID, secID), collector);
                    LineInfos[(matID, realID, eleID, secID)].Add(m,line);
                }
                else
                {
                    LineInfos[(matID, realID, eleID, secID)].Add(m,line);
                }
            }

        }

        private void TranslateBeam()
        {

                var beams = _ifcModel.Instances.OfType<IIfcBeam>()
                .Where(ea => ea.PredefinedType == IfcBeamTypeEnum.USERDEFINED)
                .ToList();

            if (beams == null)
                throw new InvalidOperationException("A buliding without beams cannot be processed!");
            else
            {
                var ele = TranslatorHelper.TranslateElement("Beam");
                int flag = 0;   //标记
                for(int i =0;i<ElementSet.Count;i++)
                {
                    if (ele.Type == ElementSet[i].Type)
                    { flag = 1; break; }  //ElementSet中存在同名的单元类型，则改变标记
                }
                if(flag==0)   //若标记物未变，说明ElementSet中不存在同名的单元类型，将改单元类型的编号设为当前集合数+1，并将该ansyselement加入集合中
                {
                    ele.ID = ElementSet.Count + 1;
                    ElementSet.Add(ele);
                }
            }

            //translate material
            foreach (var beam in beams)
            {
                var material = (IIfcMaterial)beam.Material;
                var name = material.Name;
                if (!MaterialSet.ContainsKey(name))
                {
                    MaterialSet.Add(name, material);
                }
            }


            //translate sections            
            foreach (var beam in beams)
            {
                var solid = beam.Representation.Representations[0].Items
                    .Where(geo => geo is IIfcExtrudedAreaSolid)
                    .Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var pro = TranslatorHelper.TranslateProfile(solid);

                if (!Profiles.ContainsValue(pro))
                {
                    int i = Profiles.Count + 1;
                    Profiles.Add(i, pro);
                }
            }


            //translate kps & lines

            //get the set of kps
            foreach (var beam in beams)
            {
                var solid = beam.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var keypoints = TranslatorHelper.TranslateKP(solid);
                foreach (var kp in keypoints)
                {
                    if (!KPs.ContainsValue(kp))
                    {
                        KPs.Add(KPs.Count + 1, kp);
                    }
                }
            }

            //get the set of line IDs
            foreach (var beam in beams)
            {
                int matID = 0, realID = 0, eleID = 0, secID = 0;
                var material = (IIfcMaterial)beam.Material;
                var name = material.Name;
                int n = 1;
                foreach (var mat in MaterialSet)
                {
                    if (mat.Key == name)
                        matID = n;
                    else
                        n++;
                }
                var ele = TranslatorHelper.TranslateElement("Beam");
                for (int i = 0; i < ElementSet.Count; i++)
                {
                    if (ElementSet[i].Type == ele.Type)
                        eleID = ElementSet[i].ID;
                }

                var solid = beam.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();

                var pro = TranslatorHelper.TranslateProfile(solid);
                foreach (var pair in Profiles)
                {
                    if (pair.Value == pro)
                        secID = pair.Key;
                }

                var m = 0;
                var keypoints = TranslatorHelper.TranslateKP(solid);
                var line = TranslatorHelper.GetLineID(keypoints, KPs);
                if (!LineIDs.ContainsValue(line))
                {
                    LineIDs.Add(LineIDs.Count + 1, line);
                    m = LineIDs.Count;
                }
                else
                {
                    for (int i = 0; i < LineIDs.Count; i++)
                        if (LineIDs[i] == line)
                            m = i;
                }

                if (!LineInfos.ContainsKey((matID, realID, eleID, secID)))
                {
                    var collector = new Dictionary<int, (int, int)>();
                    LineInfos.Add((matID, realID, eleID, secID), collector);
                    LineInfos[(matID, realID, eleID, secID)].Add(m, line);
                }
                else
                {
                    LineInfos[(matID, realID, eleID, secID)].Add(m, line);
                }
            }

        }

        private void TranslateSlab()
        {
            var slabs = _ifcModel.Instances.OfType<IIfcSlab>()
                .Where(ea => ea.PredefinedType == IfcSlabTypeEnum.USERDEFINED)
                .ToList();
            if (slabs == null)
                throw new InvalidOperationException("A buliding without slabs cannot be processed!");
            else
            {
                var ele = TranslatorHelper.TranslateElement("Slab");
                int flag = 0;   //标记
                for (int i = 0; i < ElementSet.Count; i++)
                {
                    if (ele.Type == ElementSet[i].Type)
                    { flag = 1; break; }  //ElementSet中存在同名的单元类型，则改变标记
                }
                if (flag == 0)   //若标记物未变，说明ElementSet中不存在同名的单元类型，将改单元类型的编号设为当前集合数+1，并将该ansyselement加入集合中
                {
                    ele.ID = ElementSet.Count + 1;
                    ElementSet.Add(ele);
                }
            }

            //translate material
            foreach (var slab in slabs)
            {
                var material = (IIfcMaterial)slab.Material;
                var name = material.Name;
                if (!MaterialSet.ContainsKey(name))
                {
                    MaterialSet.Add(name, material);
                }
            }


            //translate realconstants
            foreach (var slab in slabs)
            {
                var solid = slab.Representation.Representations[0].Items
                        .Where(geo => geo is IIfcExtrudedAreaSolid)
                        .Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var r = solid.Depth;
                if(!RealConstantSet.ContainsValue(r))
                {
                    int i = RealConstantSet.Count + 1;
                    RealConstantSet.Add(i, r);
                }
            }

            //translate kps & areas
                 //kps
            foreach (var slab in slabs)
            {
                var solid = slab.Representation.Representations[0].Items
                        .Where(geo => geo is IIfcExtrudedAreaSolid)
                        .Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();

                var keypoints = TranslatorHelper.TranslateKP(solid);
                foreach (var kp in keypoints)
                {
                    if (!KPs.ContainsValue(kp))
                    {
                        KPs.Add(KPs.Count + 1, kp);
                    }
                }
            }

                //areas
            foreach (var slab in slabs)
            {
                int matID = 0, realID = 0, eleID = 0, secID = 0;
                var material = (IIfcMaterial)slab.Material;
                var name = material.Name;
                int n = 1;
                foreach (var mat in MaterialSet)
                {
                    if (mat.Key == name)
                        matID = n;
                    else
                        n++;
                }
                var ele = TranslatorHelper.TranslateElement("Slab");
                for (int i = 0; i < ElementSet.Count; i++)
                {
                    if (ElementSet[i].Type == ele.Type)
                        eleID = ElementSet[i].ID;
                }

                var solid = slab.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();

                var pro = TranslatorHelper.TranslateProfile(solid);
                var r = solid.Depth;
                foreach (var pair in RealConstantSet)
                {
                    if (pair.Value == r)
                        realID = pair.Key;
                }

                var m = 0;
                var keypoints = TranslatorHelper.TranslateKP(solid);

                var area = TranslatorHelper.GetAreaID(keypoints, KPs);
                if (!AreaIDs.ContainsValue(area))
                {
                    AreaIDs.Add(AreaIDs.Count + 1, area);
                    m = AreaIDs.Count;
                }
                else
                {
                    for (int i = 0; i < AreaIDs.Count; i++)
                        if (AreaIDs[i] == area)
                            m = i;
                }

                if(!areaInfos.ContainsKey((matID, realID, eleID, secID)))
                {
                    var collector1 = new Dictionary<int, List<int>>();
                    var collector2 = new Dictionary<int, (int, int)>();
                    areaInfos.Add((matID, realID, eleID, secID), (collector1, collector2));
                    areaInfos[(matID, realID, eleID, secID)].Item1.Add(m, area);
                }
                else
                {
                    areaInfos[(matID, realID, eleID, secID)].Item1.Add(m, area);
                }


                //if(!AreaInfos.ContainsKey((matID, realID, eleID, secID)))
                //{
                //    var collector = new Dictionary<int,List<int>>();
                //    AreaInfos.Add((matID, realID, eleID, secID), collector);
                //    AreaInfos[(matID, realID, eleID, secID)].Add(m, area);
                //}
                //else
                //{
                //    AreaInfos[(matID, realID, eleID, secID)].Add(m, area);
                //}
            }
        }



        private void TranslateWall()
        {
            var walls = _ifcModel.Instances.OfType<IIfcWall>()
                .Where(ea => ea.PredefinedType == IfcWallTypeEnum.USERDEFINED)
                .ToList();
            if (walls == null)
                throw new InvalidOperationException("A buliding without walls cannot be processed!");
            else
            {
                var ele = TranslatorHelper.TranslateElement("Wall");
                int flag = 0;   //标记
                for (int i = 0; i < ElementSet.Count; i++)
                {
                    if (ele.Type == ElementSet[i].Type)
                    { flag = 1; break; }  //ElementSet中存在同名的单元类型，则改变标记
                }
                if (flag == 0)   //若标记物未变，说明ElementSet中不存在同名的单元类型，将改单元类型的编号设为当前集合数+1，并将该ansyselement加入集合中
                {
                    ele.ID = ElementSet.Count + 1;
                    ElementSet.Add(ele);
                }
            }

            //translate material
            foreach (var wall in walls)
            {
                var material = (IIfcMaterial)wall.Material;
                var name = material.Name;
                if (!MaterialSet.ContainsKey(name))
                {
                    MaterialSet.Add(name, material);
                }
            }


            //translate realconstants & kps & areas 
            //get the realconstants & set of kps
            foreach (var wall in walls)
            {
                var solid = wall.Representation.Representations[0].Items
                    .Where(geo => geo is IIfcExtrudedAreaSolid)
                    .Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var r = TranslatorHelper.TranslateWallR(solid);
                if (!RealConstantSet.ContainsValue(r))
                {
                    int i = RealConstantSet.Count + 1;
                    RealConstantSet.Add(i, r);
                }
                var height = solid.Depth;
                var keypoints = TranslatorHelper.TranslateWallKP(solid, r, height);
                foreach (var kp in keypoints)
                {
                    if (!KPs.ContainsValue(kp))
                    {
                        KPs.Add(KPs.Count + 1, kp);
                    }
                }
            }




            //get the set of area IDs  & aglue IDs
            foreach (var wall in walls)
            {
                int matID = 0, realID = 0, eleID = 0, secID = 0;

                //get matID
                var material = (IIfcMaterial)wall.Material;
                var name = material.Name;
                int n = 1;
                foreach (var mat in MaterialSet)
                {
                    if (mat.Key == name)
                        matID = n;
                    else
                        n++;
                }

                //get eleID
                var ele = TranslatorHelper.TranslateElement("Wall");
                for (int i = 0; i < ElementSet.Count; i++)
                {
                    if (ElementSet[i].Type == ele.Type)
                        eleID = ElementSet[i].ID;
                }

                //get realID
                var solid = wall.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var r = TranslatorHelper.TranslateWallR(solid);
                foreach (var pair in RealConstantSet)
                {
                    if (pair.Value == r)
                        realID = pair.Key;
                }

                //trans Area
                int[] id = { 0, 0 };
                var height = solid.Depth;
                var keypoints = TranslatorHelper.TranslateWallKP(solid, r, height);
                var areas = TranslatorHelper.GetWallAreaID(keypoints, KPs);
                for (int i = 0; i < areas.Count; i++)
                {
                    if (!AreaIDs.ContainsValue(areas[i]))
                    {
                        AreaIDs.Add(AreaIDs.Count + 1, areas[i]);
                        id[i] = AreaIDs.Count;
                    }
                    else
                    {
                        for (int j = 0; j < AreaIDs.Count; j++)
                            if (AreaIDs[j] == areas[i])
                                id[i] = j;
                    }

                    if (!areaInfos.ContainsKey((matID, realID, eleID, secID)))
                    {
                        var collector1 = new Dictionary<int, List<int>>();
                        var collector2 = new Dictionary<int, (int, int)>();
                        areaInfos.Add((matID, realID, eleID, secID), (collector1, collector2));
                        areaInfos[(matID, realID, eleID, secID)].Item1.Add(id[i], areas[i]);
                    }
                    else
                    {
                        areaInfos[(matID, realID, eleID, secID)].Item1.Add(id[i], areas[i]);
                    }



                    //if (!AreaInfos.ContainsKey((matID, realID, eleID, secID)))
                    //{
                    //    var collector = new Dictionary<int, List<int>>();
                    //    AreaInfos.Add((matID, realID, eleID, secID), collector);
                    //    AreaInfos[(matID, realID, eleID, secID)].Add(id[i], areas[i]);
                    //}
                    //else
                    //{
                    //    AreaInfos[(matID, realID, eleID, secID)].Add(id[i], areas[i]);
                    //}
                }
                var m = areaInfos[(matID, realID, eleID, secID)].Item2.Count + 1;
                areaInfos[(matID, realID, eleID, secID)].Item2.Add(m, ((id[0], id[1])));


                    //if (!AglueInfos.ContainsKey((matID, realID, eleID, secID)))
                    //{
                    //    var collector = new Dictionary<int, (int, int)>();
                    //    AglueInfos.Add((matID, realID, eleID, secID), collector);
                    //    var m = AglueInfos[(matID, realID, eleID, secID)].Values.Count + 1;
                    //    AglueInfos[(matID, realID, eleID, secID)].Add(m, (id[0], id[1]));
                    //}
                    //else
                    //{
                    //    var m = AglueInfos[(matID, realID, eleID, secID)].Values.Count + 1;
                    //    AglueInfos[(matID, realID, eleID, secID)].Add(m, (id[0], id[1]));
                    //}

            }
        }


        //Add   all  into _ansysStore
        public void AddAll()
        {
            //ADD ELEMENT
            _ansysStore.AddElement(ElementSet);

            //ADD MATERIAL
            var mat = TranslatorHelper.TranslateMaterial(MaterialSet);
            _ansysStore.AddMaterial(mat);

            //ADD SECTION
            var secs = TranslatorHelper.TranslateSection(Profiles);
            _ansysStore.AddSection(secs);

            //ADD R
            foreach (var r in RealConstantSet)
            {
                var R = new AnsysFactory.models.AnsysRealConstant(r.Key, r.Value, r.Value, r.Value, r.Value);
                _ansysStore.AddRealConstant(R);
            }

            //ADD KP
            var keypoints = TranslatorHelper.TranslateKeypoint(KPs);
            _ansysStore.AddKeypoint(keypoints);

            //ADD LINE  
            foreach (var id in LineIDs)
            {
                var line = new AnsysFactory.models.AnsysLine(_ansysStore.LongToAnsyskeypoint(id.Value.Item1), _ansysStore.LongToAnsyskeypoint(id.Value.Item2));
                _ansysStore.AddLine(line);
            }

            foreach (var info in LineInfos)
            {
                var Line = new List<AnsysFactory.models.AnsysLine>();
                foreach (var id in info.Value)
                {
                    var line = new AnsysFactory.models.AnsysLine(_ansysStore.LongToAnsyskeypoint(id.Value.Item1), _ansysStore.LongToAnsyskeypoint(id.Value.Item2));
                    Line.Add(line);
                }
                var latt = new AnsysFactory.models.AnsysLatt(_ansysStore.IntToAnsysMaterial(info.Key.Item1), _ansysStore.IntToAnsysRealConstant(info.Key.Item2),
                                                                                             _ansysStore.IntToAnsysElement(info.Key.Item3), _ansysStore.IntToAnsysSection(info.Key.Item4));
                _ansysStore.AddLineAndLatt(latt, Line);

            }




            //ADD AREA
            for (int i = 1; i <= AreaIDs.Count; i++)
            {
                var n = AreaIDs[i].Count;
                var ID1 = _ansysStore.LongToAnsyskeypoint(AreaIDs[i][0]);
                var ID2 = _ansysStore.LongToAnsyskeypoint(AreaIDs[i][1]);
                var ID3 = _ansysStore.LongToAnsyskeypoint(AreaIDs[i][2]);
                if (n == 3)
                {
                    _ansysStore.AddArea(new AnsysFactory.models.AnsysArea(i, ID1, ID2, ID3));
                }
                else if (n == 4)
                {
                    var ID4 = _ansysStore.LongToAnsyskeypoint(AreaIDs[i][3]);
                    _ansysStore.AddArea(new AnsysFactory.models.AnsysArea(i, ID1, ID2, ID3, ID4));
                }
                else if (n == 5)
                {
                    var ID4 = _ansysStore.LongToAnsyskeypoint(AreaIDs[i][3]);
                    var ID5 = _ansysStore.LongToAnsyskeypoint(AreaIDs[i][4]);
                    _ansysStore.AddArea(new AnsysFactory.models.AnsysArea(i, ID1, ID2, ID3, ID4, ID5));
                }
            }

            //foreach (var info in AreaInfos)
            //{
            //    var Area = new List<AnsysFactory.models.AnsysArea>();
            //    foreach(var id in info.Value)
            //    {
            //        var n = id.Value.Count;
            //        if(n==3)
            //        {
            //            var area = new AnsysFactory.models.AnsysArea(id.Key,_ansysStore.LongToAnsyskeypoint(id.Value[0]), _ansysStore.LongToAnsyskeypoint(id.Value[1]), _ansysStore.LongToAnsyskeypoint(id.Value[2]));
            //            Area.Add(area);
            //        }
            //        if(n==4)
            //        {
            //            var area = new AnsysFactory.models.AnsysArea(id.Key,_ansysStore.LongToAnsyskeypoint(id.Value[0]), _ansysStore.LongToAnsyskeypoint(id.Value[1]),
            //                                                                                          _ansysStore.LongToAnsyskeypoint(id.Value[2]), _ansysStore.LongToAnsyskeypoint(id.Value[3]));
            //            Area.Add(area);
            //        }
            //        if(n==5)
            //        {
            //            var area = new AnsysFactory.models.AnsysArea(id.Key,_ansysStore.LongToAnsyskeypoint(id.Value[0]), _ansysStore.LongToAnsyskeypoint(id.Value[1]),
            //                            _ansysStore.LongToAnsyskeypoint(id.Value[2]), _ansysStore.LongToAnsyskeypoint(id.Value[3]),_ansysStore.LongToAnsyskeypoint(id.Value[4]));
            //            Area.Add(area);
            //        }
            //    }
            //    var aatt = new AnsysFactory.models.AnsysAatt(_ansysStore.IntToAnsysMaterial(info.Key.Item1), _ansysStore.IntToAnsysRealConstant(info.Key.Item2),
            //                                                                                 _ansysStore.IntToAnsysElement(info.Key.Item3), _ansysStore.IntToAnsysSection(info.Key.Item4));
            //    _ansysStore.AddAreaAndAatt(aatt, Area);
            //}

            ////ADD AGLUE
            //foreach(var info in AglueInfos)
            //{
            //    var aatt = new AnsysFactory.models.AnsysAatt(_ansysStore.IntToAnsysMaterial(info.Key.Item1), _ansysStore.IntToAnsysRealConstant(info.Key.Item2),
            //                                                                                 _ansysStore.IntToAnsysElement(info.Key.Item3), _ansysStore.IntToAnsysSection(info.Key.Item4));
            //    var Aglue = new List<AnsysFactory.models.AnsysAglue>();
            //    foreach (var id in info.Value)
            //    {
            //        var aglue = new AnsysFactory.models.AnsysAglue(_ansysStore.IntToAnsysarea(id.Value.Item1), _ansysStore.IntToAnsysarea(id.Value.Item2));
            //        Aglue.Add(aglue);
            //    }
            //    _ansysStore.AddAglueAndAatt(aatt,Aglue);
            //}


            //ADD AREA & AGLUE
            foreach(var info in areaInfos)
            {
                var aatt = new AnsysFactory.models.AnsysAatt(_ansysStore.IntToAnsysMaterial(info.Key.Item1), _ansysStore.IntToAnsysRealConstant(info.Key.Item2),
                                                                                            _ansysStore.IntToAnsysElement(info.Key.Item3), _ansysStore.IntToAnsysSection(info.Key.Item4));
                var Area = new List<AnsysFactory.models.AnsysArea>();
                var Aglue = new List<AnsysFactory.models.AnsysAglue>();
                foreach(var areas in info.Value.Item1)
                {
                    var n = areas.Value.Count;
                    if (n == 3)
                    {
                        var area = new AnsysFactory.models.AnsysArea(areas.Key, _ansysStore.LongToAnsyskeypoint(areas.Value[0]), _ansysStore.LongToAnsyskeypoint(areas.Value[1]), _ansysStore.LongToAnsyskeypoint(areas.Value[2]));
                        Area.Add(area);
                    }
                    if (n == 4)
                    {
                        var area = new AnsysFactory.models.AnsysArea(areas.Key, _ansysStore.LongToAnsyskeypoint(areas.Value[0]), _ansysStore.LongToAnsyskeypoint(areas.Value[1]),
                                                                                                      _ansysStore.LongToAnsyskeypoint(areas.Value[2]), _ansysStore.LongToAnsyskeypoint(areas.Value[3]));
                        Area.Add(area);
                    }
                    if (n == 5)
                    {
                        var area = new AnsysFactory.models.AnsysArea(areas.Key, _ansysStore.LongToAnsyskeypoint(areas.Value[0]), _ansysStore.LongToAnsyskeypoint(areas.Value[1]),
                                        _ansysStore.LongToAnsyskeypoint(areas.Value[2]), _ansysStore.LongToAnsyskeypoint(areas.Value[3]), _ansysStore.LongToAnsyskeypoint(areas.Value[4]));
                        Area.Add(area);
                    }
                }
                foreach(var aglues in info.Value.Item2)
                {
                           var aglue = new AnsysFactory.models.AnsysAglue(_ansysStore.IntToAnsysarea(aglues.Value.Item1), _ansysStore.IntToAnsysarea(aglues.Value.Item2));
                           Aglue.Add(aglue);
                }
                _ansysStore.AddAreaAndAglue(aatt, Area, Aglue);
            }
        }

    }
}
