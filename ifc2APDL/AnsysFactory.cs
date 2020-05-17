using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ifc2APDL.AnsysFactory.models;

namespace ifc2APDL.AnsysFactory
{
    public class AnsysStore
    {
        private readonly Dictionary<long, AnsysElement> _elements = new Dictionary<long, AnsysElement>();
        private readonly Dictionary<int, List<AnsysMaterial>> _materials = new Dictionary<int, List<AnsysMaterial>>();
        private readonly Dictionary<int, AnsysSection> _sections = new Dictionary<int, AnsysSection>();
        private readonly Dictionary<int, AnsysRealConstant> _realconstants = new Dictionary<int, AnsysRealConstant>();
        private readonly Dictionary<long, AnsysKeypoint> _nodes = new Dictionary<long, AnsysKeypoint>();
        private readonly Dictionary<long, AnsysLine> _lines = new Dictionary<long, AnsysLine>();
        private readonly Dictionary<int, AnsysArea> _areas = new Dictionary<int, AnsysArea>();
        private readonly Dictionary<AnsysAatt, List<AnsysAglue>> _aglues = new Dictionary<AnsysAatt, List<AnsysAglue>>();
        private readonly Dictionary<AnsysLatt, List<AnsysLine>> _latts = new Dictionary<AnsysLatt, List<AnsysLine>>();
        private readonly Dictionary<AnsysAatt, List<AnsysArea>> _aatts = new Dictionary<AnsysAatt, List<AnsysArea>>();
        private readonly Dictionary<AnsysAatt, (List<AnsysArea>, List<AnsysAglue>)> _areainfo = new Dictionary<AnsysAatt, (List<AnsysArea>, List<AnsysAglue>)>();
        public AnsysKeypoint LongToAnsyskeypoint(long i)
        {
            return _nodes[i];
        }
        public AnsysArea IntToAnsysarea(int i)
        {
            return _areas[i];
        }

        public AnsysElement IntToAnsysElement(int i)
        {
            return _elements[i];
        }

        public AnsysMaterial  IntToAnsysMaterial(int i)
        {
                return _materials[i][0];
        }

        public AnsysSection IntToAnsysSection(int i)
        {
            if (i == 0)
                return null;
            else
                return _sections[i];
        }
        public AnsysRealConstant IntToAnsysRealConstant(int i)
        {
            if (i == 0)
                return null;
            else
                return _realconstants[i];
        }

        public void AddElement(List<AnsysElement> eles)
        {
            foreach (var ele in eles)
                AddElement(ele);
        }
        public void AddElement(AnsysElement ele)
        {
            if (!_elements.ContainsKey(ele.ID))
                _elements[ele.ID] = ele;
        }

        public void AddMaterial(AnsysMaterial mat)
        {
            var matcollector = new List<AnsysMaterial>();
            if (!_materials.ContainsKey(mat.ID))
            {
                
                _materials.Add(mat.ID, matcollector);
                _materials[mat.ID].Add(mat);
            }
            else
            {
                _materials[mat.ID].Add(mat);
            }
        }
        public void AddMaterial(List<AnsysMaterial> mats)
        {
            foreach (var mat in mats)
                AddMaterial(mat);
        }
        public void AddSection(AnsysSection sec)
        {
            if (!_sections.ContainsKey(sec.type.ID))
                _sections[sec.type.ID] = sec;
        }
        public void AddSection(List<AnsysSection> secs)
        {
            foreach (var sec in secs)
                AddSection(sec);
        }
        public void AddRealConstant(AnsysRealConstant rel)
        {
            if (!_realconstants.ContainsKey(rel.ID))
                _realconstants[rel.ID] = rel;
        }
        public void AddRealConstant(List<AnsysRealConstant>rels)
        {
            foreach (var rel in rels)
                AddRealConstant(rel);
        }
        public void AddKeypoint(AnsysKeypoint node)
        {
            if (!_nodes.ContainsKey(node.ID))
                _nodes[node.ID] = node;
        }
        public void AddKeypoint(List<AnsysKeypoint> nodes)
        {
            foreach (var node in nodes)
                AddKeypoint(node);
        }

        public void AddLine(AnsysLine line)
        {
            long n = _lines.Count;
            _lines[n + 1] = line;
        }
        public void AddLine(List<AnsysLine> lines)
        {
            foreach (var line in lines)
                AddLine(line);
        }
        public void AddLineAndLatt(AnsysLatt latt, List<AnsysLine>lines)
        {
            _latts[latt] = lines;
        }
        public void AddAreaAndAglue(AnsysAatt aatt, List<AnsysArea> areas, List<AnsysAglue> aglues)
        {
            _areainfo[aatt] = (areas, aglues);
            //_areainfo[aatt].Item1 = areas;
            //_areainfo[aatt].Item2 = aglues;
        }
        //public void AddAreaAndAatt(AnsysAatt aatt,List<AnsysArea>areas)
        //{
        //    _aatts[aatt]=areas;
        //}
        //public void AddAglueAndAatt(AnsysAatt aatt,List<AnsysAglue>aglues)
        //{
        //    _aglues[aatt] = aglues;
        //}
        public void AddArea(AnsysArea area)
        {
            int n = _areas.Count;
            _areas[n + 1] = area;
        }
        public void AddArea(List<AnsysArea> areas)
        {
            foreach (var area in areas)
                AddArea(area);
        }

        
        public void WriteAPDLFile(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.GetEncoding("GB2312")))
            {
                string head = "!--------------------------------------\n" +
                    "\t!Ansys Test File\n" +
                    $"\t!Data/Time:{DateTime.Now}\n" +
                    "\t!Produced by :ifc2APDL.AnsysFactory\n" +
                    "\t!Author:HSX\n" +
                    "!--------------------------------------";
                sw.WriteLine(head);
                head = "/PREP7";
                sw.WriteLine(head);

                head = "\n!Elements";
                sw.WriteLine(head);
                foreach (var ele in _elements)
                    sw.WriteLine(ele.Value);

                head = "\n!Materials";
                sw.WriteLine(head);
                foreach (var mat in _materials)
                {
                    foreach (var single in mat.Value)
                        sw.WriteLine(single);
                }

                head = "\n!Sections";
                sw.WriteLine(head);
                foreach (var sec in _sections)
                    sw.WriteLine(sec.Value);

                head = "\n!RealConstants";
                sw.WriteLine(head);
                foreach (var rel in _realconstants)
                    sw.WriteLine(rel.Value);

                head = "\n!--------------------------------------" +
                    "Modeling" +
                    "--------------------------------------";
                sw.WriteLine(head);

                head = "\n!Build KeyPoints";
                sw.WriteLine(head);
                foreach (var node in _nodes)
                    sw.WriteLine(node.Value);

                head = "\n/view,1,1,1,1" + "\n/ angle,1" + "\n/ rep";
                sw.WriteLine(head);


                head = "\n!Build Lines";
                sw.WriteLine(head);

                foreach (var LineInfo in _latts)
                {
                    foreach(var l in LineInfo.Value)
                        sw.WriteLine(l);
                    sw.WriteLine(LineInfo.Key);
                    sw.WriteLine(new AnsysLmesh("ALL"));
                    sw.Write("\n");
                }

                head = "\n!Build Areas";
                sw.WriteLine(head);
                foreach(var info in _areainfo)
                {
                    foreach (var a in info.Value.Item1)
                        sw.WriteLine(a);
                    foreach (var g in info.Value.Item2)
                        sw.WriteLine(g);
                    sw.WriteLine(info.Key);
                    sw.WriteLine(new AnsysAmesh("ALL"));
                    sw.Write("\n");
                }
                //foreach(var AreaInfo in _aatts)
                //{
                //    foreach (var a in AreaInfo.Value)
                //        sw.WriteLine(a);

                //    var tmpAattKey = new AnsysAatt();
                //    tmpAattKey = AreaInfo.Key;

                //    if (_aglues.ContainsKey(tmpAattKey))
                //    {
                //        foreach (var g in _aglues[AreaInfo.Key])
                //            sw.WriteLine(g);
                //    }

                //    sw.WriteLine(AreaInfo.Key);
                //    sw.WriteLine(new AnsysAmesh("ALL"));
                //    sw.Write("\n");
                //}

            }
        }
    }
}
