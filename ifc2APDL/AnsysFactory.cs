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
        private readonly Dictionary<int, AnsysAglue> _aglues = new Dictionary<int, AnsysAglue>();
        public AnsysKeypoint IntToAnsyskeypoint(long i)
        {
            return _nodes[i];
        }
        public AnsysArea IntToAnsysarea(int i)
        {
            return _areas[i];
        }

        public void AddElement(AnsysElement element)
        {
            if (!_elements.ContainsKey(element.ID))
                _elements[element.ID] = element;
        }
        public void AddElement(List<AnsysElement> elements)
        {
            foreach (var element in elements)
                AddElement(element);
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
        public void AddAglue(AnsysAglue aglue)
        {
            int n = _aglues.Count;
            _aglues[n + 1] = aglue;
        }
        public void AddAglue(List<AnsysAglue> aglues)
        {
            foreach (var aglue in aglues)
                AddAglue(aglue);
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
                foreach (var line in _lines)
                    sw.WriteLine(line.Value);
                head = "\n!Build Areas";
                sw.WriteLine(head);
                foreach (var area in _areas)
                    sw.WriteLine(area.Value);
                head = "\n!Build Aglues";
                sw.WriteLine(head);
                foreach (var aglue in _aglues)
                    sw.WriteLine(aglue.Value);
            }
        }
    }
}
