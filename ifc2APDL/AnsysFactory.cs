using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ifc2APDL.AnsysFactory.models;

namespace ifc2APDL.AnsysFactory
{
    public class AnsysStore
    {
        private readonly Dictionary<long, AnsysKeypoint> _nodes = new Dictionary<long, AnsysKeypoint>();
        private readonly Dictionary<long, AnsysElement> _elements = new Dictionary<long, AnsysElement>();
        private readonly Dictionary<int, List<AnsysMaterial>> _materials = new Dictionary<int, List<AnsysMaterial>>();
        private readonly Dictionary<int, AnsysSection> _sections = new Dictionary<int, AnsysSection>();

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
            if (!_materials[mat.ID].Contains(mat))
                _materials[mat.ID].Add(mat);
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

        public void WriteAPDLFile(string path)
        {
            using (var sw = new StreamWriter(path, false, Encoding.GetEncoding("XX-XX")))
            {
                string head = "!--------------------------------------\n" +
                    "\t!Ansys Test File\n" +
                    $"\t!Data/Time:{DateTime.Now}\n" +
                    "\t!Produced by :ifc2APDL.AnsysFactory\n" +
                    "\t!Author:HSX\n" +
                    "!--------------------------------------";
                sw.WriteLine(head);
                head = "/PREP";
                sw.WriteLine(head);

                head = "\n!Elements";
                sw.WriteLine(head);
                foreach (var ele in _elements)
                    sw.WriteLine(ele.Value);

                head = "\n!Materials";
                sw.WriteLine(head);
                foreach (var mat in _materials)
                    sw.WriteLine(mat.Value);

                head = "\n!Sections";
                sw.WriteLine(head);
                foreach (var sec in _sections)
                    sw.WriteLine(sec.Value);



            }
        }
    }
}
