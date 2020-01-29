using System;
using System.IO;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using ifc2APDL;
using ifc2APDL.AnsysFactory;

namespace ifc2APDL.Translator
{
    public class Worker
    {
        private readonly IfcStore _ifcModel;
        private readonly AnsysStore _ansysStore = new AnsysStore();

        public Worker(string path)
        {
            if (File.Exists(path))
                _ifcModel = IfcStore.Open(path);
            else
                throw new ArgumentException("Invalid path to open ifc model");
            //Initialise();
        }
        public Worker(IfcStore model)
        {
            _ifcModel = model;
            //Initialise();
        }

        //Ansys种无单位初始化命令。ansys有默认单位系统
        //private void Initialise()
        //{
        //    try
        //    {
        //        if(_ifcModel==null)
        //            throw new InvalidOperationException("Empty model cannot be processed");
        //        InitialiseUnitSystem();
                
        //    }
        //    catch(InvalidOperationException e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    catch(Exception e)
        //    {
        //        Console.Write(e.Message);
        //    }
        //}


        public void Run()
        {
            try
            {
                Translate();
            }
            catch(Exception)
            {
                throw;
            }
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

        private void Translate()
        {

        }

    }
}
