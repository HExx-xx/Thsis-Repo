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
        private readonly Dictionary<int, (double, double, double)> KPs = new Dictionary<int, (double, double, double)>();
        private readonly Dictionary<int, (int, int)> LineIDs = new Dictionary<int, (int, int)>();
        private readonly Dictionary<int, (int, int, int, int)> AreaIDs = new Dictionary<int, (int, int, int, int)>();
        private readonly Dictionary<int, (int, int)> AglueIDs = new Dictionary<int, (int, int)>();
        private readonly Dictionary<string,IIfcMaterial> MaterialSet = new Dictionary<string, IIfcMaterial>();
        private readonly Dictionary<int, (double,double)> Profiles = new Dictionary<int, (double,double)>();//ID,b*h
        private readonly Dictionary<int, double> RealConstantSet = new Dictionary<int, double>();
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


        public void WriteAPDLFile(string path)
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
                .Where(ea => ea.PredefinedType == IfcColumnTypeEnum.USERDEFINED)
                .ToList();
            if (columns == null)
                throw new InvalidOperationException("A buliding without columns cannot be processed!");
 
            
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
                var solid = column.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var keypoints = TranslatorHelper.TranslateKP(solid);

                var line = TranslatorHelper.GetLineID(keypoints,KPs);
                if(!LineIDs.ContainsValue(line))
                {
                    LineIDs.Add(LineIDs.Count + 1, line);
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
                var solid = beam.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var keypoints = TranslatorHelper.TranslateKP(solid);

                var line = TranslatorHelper.GetLineID(keypoints, KPs);
                if (!LineIDs.ContainsValue(line))
                {
                    LineIDs.Add(LineIDs.Count + 1, line);
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
                 //get the set of kps
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

            //get the set of area IDs
            foreach (var slab in slabs)
            {
                var solid = slab.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var keypoints = TranslatorHelper.TranslateKP(solid);

                var area = TranslatorHelper.GetAreaID(keypoints, KPs);
                if (!AreaIDs.ContainsValue(area))
                {
                    AreaIDs.Add(AreaIDs.Count + 1, area);
                }
            }
        }

        private void TranslateWall()
        {
            var walls = _ifcModel.Instances.OfType<IIfcWall>()
                .Where(ea => ea.PredefinedType == IfcWallTypeEnum.USERDEFINED)
                .ToList();
            if (walls == null)
                throw new InvalidOperationException("A buliding without walls cannot be processed!");


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
                var keypoints = TranslatorHelper.TranslateWallKP(solid, r);
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
                var solid = wall.Representation.Representations[0].Items.
                Where(geo => geo is IIfcExtrudedAreaSolid).
                Select(geo => (IIfcExtrudedAreaSolid)geo).FirstOrDefault();
                var r = TranslatorHelper.TranslateWallR(solid);
                var keypoints = TranslatorHelper.TranslateWallKP(solid,r);
                var height = solid.Depth;
                var areas = TranslatorHelper.GetWallAreaID(keypoints, KPs,height);
                int[] id= { 0,0};
                for (int i= 0;i < areas.Count; i++)
                {
                    AreaIDs.Add(AreaIDs.Count + 1, areas[i]);
                    id[i]= AreaIDs.Count;
                }
                AglueIDs.Add(AglueIDs.Count + 1, (id[0], id[1]));
            }
        }


        //Add   all  into _ansysStore
        public void AddAll()
        {
            //ADD KP
            var keypoints = TranslatorHelper.TranslateKeypoint(KPs);
            _ansysStore.AddKeypoint(keypoints);

            //ADD LINE
            foreach (var id in LineIDs)
            {
                var line = new AnsysFactory.models.AnsysLine(_ansysStore.IntToAnsyskeypoint(id.Value.Item1), _ansysStore.IntToAnsyskeypoint(id.Value.Item2));
                _ansysStore.AddLine(line);
            }
            //ADD AREA
            foreach(var id in AreaIDs)
            {
                var area = new AnsysFactory.models.AnsysArea(_ansysStore.IntToAnsyskeypoint(id.Value.Item1),
                    _ansysStore.IntToAnsyskeypoint(id.Value.Item2), _ansysStore.IntToAnsyskeypoint(id.Value.Item3),
                    _ansysStore.IntToAnsyskeypoint(id.Value.Item4));
                _ansysStore.AddArea(area);
            }
            //ADD AGLUE
            foreach(var id in AglueIDs)
            {
                var aglue = new AnsysFactory.models.AnsysAglue(_ansysStore.IntToAnsysarea(id.Value.Item1), _ansysStore.IntToAnsysarea(id.Value.Item2));
                _ansysStore.AddAglue(aglue);
            }

            //ADD MATERIAL
            var mat = TranslatorHelper.TranslateMaterial(MaterialSet);
            _ansysStore.AddMaterial(mat);

            //ADD SECTION
            var secs = TranslatorHelper.TranslateSection(Profiles);
            _ansysStore.AddSection(secs);

            //ADD R
            foreach(var r in RealConstantSet)
            {
                var R = new AnsysFactory.models.AnsysRealConstant(r.Key, r.Value, r.Value, r.Value, r.Value);
                _ansysStore.AddRealConstant(R);
            }
        }

    }
}
