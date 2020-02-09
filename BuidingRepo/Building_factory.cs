using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PresentationAppearanceResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.Ifc4.SharedComponentElements;
using Xbim.IO;

namespace BuildingRepo
{
    public class Building_factory : IDisposable
    {
        private readonly string _outputPath = "";
        private readonly string _projectName = "xx工程";
        private readonly string _buildingName = "xx楼板";

        private readonly IfcStore _model;//using external Alignment model as refenrence

        private IfcStore CreateAndInitModel(string projectname)
        {
            //first we need register essential information for the project
            var credentials = new XbimEditorCredentials
            {
                ApplicationDevelopersName = "hsx",
                ApplicationFullName = "IFC Model_Alignment for Building",
                ApplicationIdentifier = "",
                ApplicationVersion = "1.0",
                EditorsFamilyName = "HE",
                EditorsGivenName = "Shixin",
                EditorsOrganisationName = "HUST"
            };
            //create model by using method in IfcStore class,using memory mode,and IFC4x1 format
            var model = IfcStore.Create(credentials, XbimSchemaVersion.Ifc4x1, XbimStoreType.InMemoryModel);

            //begin a transition when do any change in a model
            using (var txn = model.BeginTransaction("Initialise Model"))
            {
                //add new IfcProject item to a certain container
                var project = model.Instances.New<IfcProject>
                    (p =>
                    {
                        //Set the units to SI (mm and metres)                      
                        p.Initialize(ProjectUnits.SIUnitsUK);
                        p.Name = projectname;
                    });
                // Now commit the changes, else they will be rolled back 
                // at the end of the scope of the using statement
                txn.Commit();
            }
            return model;
        }

        public Building_factory(string outputPath = "../../TestFiles/building.ifc")
        {
            _model = CreateAndInitModel(_projectName);
            InitWCS();
            _outputPath = outputPath;
        }

        private IfcCartesianPoint Origin3D { get; set; }
        private IfcDirection AxisX3D { get; set; }
        private IfcDirection AxisY3D { get; set; }
        private IfcDirection AxisZ3D { get; set; }
        private IfcAxis2Placement3D WCS { get; set; }
        private IfcCartesianPoint Origin2D { get; set; }
        private IfcDirection AxisX2D { get; set; }
        private IfcDirection AxisY2D { get; set; }
        private IfcAxis2Placement2D WCS2D { get; set; }
        private void InitWCS()
        {
            using (var txn = this._model.BeginTransaction("Initialise WCS"))
            {
                var context3D = this._model.Instances.OfType<IfcGeometricRepresentationContext>()
                .Where(c => c.CoordinateSpaceDimension == 3)
                .FirstOrDefault();
                if (context3D.WorldCoordinateSystem is IfcAxis2Placement3D wcs)
                {
                    WCS = wcs;
                    Origin3D = wcs.Location;
                    AxisZ3D = toolkit_factory.MakeDirection(_model, 0, 0, 1);
                    wcs.Axis = AxisZ3D;
                    AxisX3D = toolkit_factory.MakeDirection(_model, 1, 0, 0);
                    wcs.RefDirection = AxisX3D;
                    AxisY3D = toolkit_factory.MakeDirection(_model, 0, 1, 0);
                }

                var context2D = this._model.Instances.OfType<IfcGeometricRepresentationContext>()
                    .Where(c => c.CoordinateSpaceDimension == 2)
                    .FirstOrDefault();
                if (context2D.WorldCoordinateSystem is IfcAxis2Placement2D wcs2d)
                {
                    WCS2D = wcs2d;
                    Origin2D = wcs2d.Location;
                    AxisX2D = toolkit_factory.MakeDirection(_model, 1, 0);
                    wcs2d.RefDirection = AxisX2D;
                    AxisY2D = toolkit_factory.MakeDirection(_model, 0, 1);
                }

                txn.Commit();
            }
        }
        public void Dispose()
        {
            _model.Dispose();
        }

        //_placementMap 用于记录轴网的间距、跨度以及层高
        private Dictionary<int, (List<double> spacing, List<double> span, double height)> _placementMap = new Dictionary<int, (List<double> spacing, List<double> span, double height)>();

        //GeneratePlacementMap函数：将TEST中输入的轴网层高等信息解析到类Building的成员_placementMap中保存。
        public void GeneratePlacementMap(List<List<double>> Column_spacing, List<List<double>> Column_span, List<double> height)
        {
            if ((Column_spacing.Count != Column_span.Count) || (Column_spacing.Count != height.Count) || (Column_span.Count != height.Count))
                throw new InvalidOperationException("Must Pair the spacing ,span and height!");
            //这里还要判断那个高度是不是匹配
            for (int i = 0; i < Column_spacing.Count; i++)
            {
                _placementMap[i] = (Column_spacing[i], Column_span[i], height[i]);
            }
        }

        private void CreateMaterial(IfcDefinitionSelect t)
        {
            var material = _model.Instances.New<IfcMaterial>(mat =>
            {
                mat.Name = "C40";
                mat.Category = "Concrete";
            });
            _model.Instances.New<IfcRelAssociatesMaterial>(ram =>
            {
                ram.RelatingMaterial = material;
                ram.RelatedObjects.Add(t);
            });
            var MaterialCommon = _model.Instances.New<IfcMaterialProperties>(mp =>
            {
                mp.Name = "MaterialCommon";
                mp.Material = material;
                var massDensity = _model.Instances.New<IfcPropertySingleValue>(p =>
                {
                    p.Name = "MassDensity";
                    p.NominalValue = new IfcMassDensityMeasure(2.5e-6);
                });
                mp.Properties.Add(massDensity);
            });
            var MaterialMechanical = _model.Instances.New<IfcMaterialProperties>(mp =>
            {
                mp.Name = "MagterialMechanical";
                mp.Material = material;
                var youngModulus = _model.Instances.New<IfcPropertySingleValue>(p =>
                {
                    p.Name = "YoungModulus";
                    p.NominalValue = new IfcModulusOfElasticityMeasure(3.3e4);
                });
                var PoissonRatio = _model.Instances.New<IfcPropertySingleValue>(p =>
                {
                    p.Name = "PoissonRatio";
                    p.NominalValue = new IfcPositiveRatioMeasure(0.3);
                });
                mp.Properties.AddRange(new List<IfcPropertySingleValue>() { youngModulus, PoissonRatio });
            });

        }



        //building code
        #region
        public void Build()
        {
            InitWCS();
            var site = toolkit_factory.CreateSite(_model, "Structure Site");
            var Columns = ColumnBuild();
            var Beams = BeamBuild();
            var Slabs = SlabBuild();
            var Walls = WallBuild();
            var Axis = BuildAxis();
            foreach (var column in Columns)
                toolkit_factory.AddPrductIntoSpatial(_model, site, column, "Add column to site");
            foreach (var beam in Beams)
                toolkit_factory.AddPrductIntoSpatial(_model, site, beam, "Add beam to site");
            foreach (var slab in Slabs)
                toolkit_factory.AddPrductIntoSpatial(_model, site, slab, "Add slab to site");
            foreach (var wall in Walls)
                toolkit_factory.AddPrductIntoSpatial(_model, site, wall, "Add wall to site");
            toolkit_factory.AddPrductIntoSpatial(_model, site, Axis, "Add Axis to site");
            _model.SaveAs(_outputPath, StorageType.Ifc);
        }
        #endregion

        //建立轴网 Axis Net
        #region
        public IfcGrid BuildAxis()
        {
            using (var txn = this._model.BeginTransaction("Create"))
            {
                List<IfcGridAxis> uAxes = new List<IfcGridAxis>();
                List<IfcGridAxis> vAxes = new List<IfcGridAxis>();

                var XLines = new List<IfcPolyline>();
                var YLines = new List<IfcPolyline>();

                double x1 = -5000, x2 = 5000, y1 = 0, y2 = 0;
                foreach (var i in _placementMap[0].spacing)
                    x2 += i;
                for (int i = 0; i <= _placementMap[0].span.Count; i++)
                {
                    XLines.Add(toolkit_factory.MakePolyLine(_model, toolkit_factory.MakeCartesianPoint(_model, x1, y1), toolkit_factory.MakeCartesianPoint(_model, x2, y1)));
                    if (i != _placementMap[0].span.Count)
                        y1 += _placementMap[0].span[i];
                }

                x1 = 0; x2 = 0; y1 = -5000; y2 = 5000;
                foreach (var i in _placementMap[0].span)
                    y2 += i;
                for (int i = 0; i <= _placementMap[0].spacing.Count; i++)
                {
                    YLines.Add(toolkit_factory.MakePolyLine(_model, toolkit_factory.MakeCartesianPoint(_model, x1, y1), toolkit_factory.MakeCartesianPoint(_model, x1, y2)));
                    if (i != _placementMap[0].spacing.Count)
                        x1 += _placementMap[0].spacing[i];
                }

                for (int i = 0; i < XLines.Count; i++)//与X轴平行的轴线，从下到上 A->Z
                {
                    string s = Convert.ToChar('A' + i).ToString();
                    uAxes.Add(toolkit_factory.MakeGridAxis(_model, s, XLines[i]));
                }

                for (int i = 0; i < YLines.Count; i++)  //与Y轴平行的轴线，从左到右 1->10
                {
                    string s = Convert.ToString(1 + i);
                    vAxes.Add(toolkit_factory.MakeGridAxis(_model, s, YLines[i]));
                }

                var axis = this._model.Instances.New<IfcGrid>();
                axis.Name = "testAxis";
                axis.ObjectType = "Single_AxisNet";
                axis.UAxes.AddRange(uAxes);
                axis.VAxes.AddRange(vAxes);

                var curveSet = this._model.Instances.New<IfcGeometricCurveSet>();
                curveSet.Elements.AddRange(XLines);
                curveSet.Elements.AddRange(YLines);
                var shape = toolkit_factory.MakeShapeRepresentation(_model, 0, "FootPrint", "GeometricCurveSet", curveSet);
                toolkit_factory.SetSurfaceColor(_model, curveSet, 124 / 255.0, 51 / 255.0, 49 / 255.0, 0.15);

                axis.Representation = this._model.Instances.New<IfcProductDefinitionShape>(pd => pd.Representations.Add(shape));
                axis.PredefinedType = IfcGridTypeEnum.RECTANGULAR;
                txn.Commit();
                return axis;

            }
        }

        #endregion

        //column code
        #region
        public List<IfcColumn> ColumnBuild()
        {
            var Column = new List<IfcColumn>();

            double XDim = 400;       //柱子尺寸 400*400
            double YDim = 400;

            double z = 0;
            for (int k = 0; k < _placementMap.Count; k++)
            {
                double y = 0;
                for (int j = 0; j <= _placementMap[k].span.Count; j++)
                {
                    double x = 0;
                    for (int i = 0; i <= _placementMap[k].spacing.Count; i++)
                    {
                        (double, double, double) startPoint;
                        if (i == 0 || i == _placementMap[k].spacing.Count || j == 0 || j == _placementMap[k].span.Count)
                            startPoint = (x, y, z);
                        else
                        {
                            if (i != _placementMap[k].spacing.Count)
                                x += _placementMap[k].spacing[i];
                            continue;
                        }
                        Column.Add(CreateColumn(startPoint, _placementMap[k].height, XDim, YDim));
                        if (i != _placementMap[k].spacing.Count)
                            x += _placementMap[k].spacing[i];
                    }
                    if (j != _placementMap[k].span.Count)
                        y += _placementMap[k].span[j];
                }
                z += _placementMap[k].height;
            }
            return Column;

        }

        private IfcColumn CreateColumn((double x, double y, double z) startPoint, double height, double xDim, double yDim)
        {
            var endpoint = (startPoint.x, startPoint.y, startPoint.z + height);
            return CreateColumn(startPoint, endpoint, xDim, yDim);
        }


        private IfcColumn CreateColumn((double x, double y, double z) startPoint, (double x, double y, double z) endPoint, double xDim, double yDim)
        {
            using (var txn = this._model.BeginTransaction("CreateColumn"))
            {
                var column = this._model.Instances.New<IfcColumn>();
                column.Name = "testColumn";
                column.ObjectType = "Single_Column";

                CreateMaterial(column);

                var point1 = toolkit_factory.MakeCartesianPoint(_model, startPoint.x, startPoint.y, startPoint.z);
                var point2 = toolkit_factory.MakeCartesianPoint(_model, endPoint.x, endPoint.y, endPoint.z);
                var profile = toolkit_factory.MakeRectangleProf(_model, xDim, yDim);


                var solid = this._model.Instances.New<IfcExtrudedAreaSolid>();
                solid.SweptArea = profile;
                solid.ExtrudedDirection = toolkit_factory.MakeDirection(_model, 0, 0, 1);
                var solid_direction = toolkit_factory.MakeDirection(_model, point1, point2);
                solid.Position = toolkit_factory.MakeLocalAxisPlacement(_model, point1, solid_direction);
                solid.Depth = toolkit_factory.GetLength(point1, point2);


                toolkit_factory.SetSurfaceColor(_model, solid, 124.0 / 255.0, 51.0 / 255.0, 49.0 / 255.0, 0.15);
                var shape = toolkit_factory.MakeShapeRepresentation(_model, 3, "Body", "AdvancedSweptSolid", solid);

                column.Representation = this._model.Instances.New<IfcProductDefinitionShape>(pd => pd.Representations.Add(shape));
                column.PredefinedType = IfcColumnTypeEnum.USERDEFINED;


                txn.Commit();
                return column;

            }

        }
        #endregion

        //beam code
        #region
        public List<IfcBeam> BeamBuild()
        {
            var Beam = new List<IfcBeam>();
            double width = 200;     //xDim
            double height = 400;    //yDim

            //建主梁            
            double z = 0;
            for (int k = 0; k < _placementMap.Count; k++)
            {
                z += _placementMap[k].height;
                double y = 0;
                for (int j = 0; j <= _placementMap[k].span.Count; j++)
                {
                    double x = 0;
                    for (int i = 0; i < _placementMap[k].spacing.Count; i++)
                    {

                        (double, double, double) shape_heart = (x, y, z);
                        (double, double, double) extruded_point = (x + _placementMap[k].spacing[i], y, z);

                        if (x == 4000 && y == 4000 || (x == 4000 && y == 9000))   //连梁部分
                        {
                            shape_heart = (x + 2000, y, z);
                            extruded_point = (x + 3000, y, z);
                        }

                        Beam.Add(CreateBeam(shape_heart, extruded_point, width, height));
                        x += _placementMap[k].spacing[i];
                    }
                    if (j != _placementMap[k].span.Count)
                        y += _placementMap[k].span[j];
                }
            }

            //次梁
            z = 0;
            for (int k = 0; k < _placementMap.Count; k++)
            {
                z += _placementMap[k].height;
                double x = 0;
                for (int j = 0; j <= _placementMap[k].spacing.Count; j++)
                {
                    double y = 0;
                    for (int i = 0; i < _placementMap[k].span.Count; i++)
                    {
                        (double, double, double) shape_heart = (x, y, z);
                        (double, double, double) extruded_point = (x, y + _placementMap[k].span[i], z);

                        if (y == 4000 && x == 4000 || (y == 4000 && x == 9000))    //连梁部分
                        {
                            shape_heart = (x, y + 2000, z);
                            extruded_point = (x, y + 3000, z);
                        }

                        Beam.Add(CreateBeam(shape_heart, extruded_point, width, height, 124, 10, 10));  //red = 124, green =10, blue =10 建出来为红色，将次梁渲染成红色
                        y += _placementMap[k].span[i];
                    }
                    if (j != _placementMap[k].spacing.Count)
                        x += _placementMap[k].spacing[j];
                }
            }



            return Beam;
        }


        //咖啡色梁     （ red = 124,green = 51, blue = 49 建出来为咖啡色，将梁默认渲染成咖啡色）
        private IfcBeam CreateBeam((double x, double y, double z) shape_heart, (double x, double y, double z) extruded_point, double width, double height,
                                                         double red = 124, double green = 51, double blue = 49)
        {
            using (var txn = this._model.BeginTransaction("CreateBeam"))
            {
                var beam = this._model.Instances.New<IfcBeam>();
                beam.Name = "testBeam";
                beam.ObjectType = "Single_Beam";

                CreateMaterial(beam);

                var point1 = toolkit_factory.MakeCartesianPoint(_model, shape_heart);
                var point2 = toolkit_factory.MakeCartesianPoint(_model, extruded_point);

                var profile = toolkit_factory.MakeRectangleProf(_model, width, height);

                var solid = this._model.Instances.New<IfcExtrudedAreaSolid>();
                solid.SweptArea = profile;
                solid.ExtrudedDirection = toolkit_factory.MakeDirection(_model, 0, 0, 1);
                var solid_direction = toolkit_factory.MakeDirection(_model, point1, point2);
                solid.Position = toolkit_factory.MakeLocalAxisPlacement(_model, point1, solid_direction);
                solid.Depth = toolkit_factory.GetLength(point1, point2);

                toolkit_factory.SetSurfaceColor(_model, solid, red / 255.0, green / 255.0, blue / 255.0, 0.15);

                var shape = toolkit_factory.MakeShapeRepresentation(_model, 3, "Body", "AdvancedSweptSolid", solid);

                beam.Representation = this._model.Instances.New<IfcProductDefinitionShape>(pd => pd.Representations.Add(shape));
                beam.PredefinedType = IfcBeamTypeEnum.USERDEFINED;

                txn.Commit();
                return beam;
            }
        }

        #endregion


        //slab code
        #region
        public List<IfcSlab> SlabBuild()
        {
            //写创建过程
            var Slab = new List<IfcSlab>();
            double thickness = 100;
            List<(double, double, double)> Points = new List<(double, double, double)>();

            double z = 0;
            for (int k = 0; k < _placementMap.Count; k++)
            {
                z += _placementMap[k].height;
                double y = 0;
                for (int j = 0; j < _placementMap[k].span.Count; j++)
                {
                    double x = 0;
                    double b = _placementMap[k].span[j];
                    for (int i = 0; i < _placementMap[k].spacing.Count; i++)
                    {
                        double a = _placementMap[k].spacing[i];
                        if (x == 4000 && y == 4000)
                        {
                            x += _placementMap[k].spacing[i];
                            continue;
                        }
                        var tmp = new List<(double, double, double)> { (x, y, z), (x + a, y, z), (x + a, y + b, z), (x, y + b, z), (x, y, z) };
                        Points.AddRange(tmp);
                        Slab.Add(CreateSlab(Points, thickness));
                        Points.Clear();
                        x += _placementMap[k].spacing[i];
                    }
                    y += _placementMap[k].span[j];
                }
            }
            return Slab;
        }

        //绿色的板
        private IfcSlab CreateSlab(List<(double x, double y, double z)> Points, double thickness)
        {
            using (var txn = this._model.BeginTransaction("CreateSlab"))
            {
                var slab = this._model.Instances.New<IfcSlab>();
                slab.Name = "testSlab";
                slab.ObjectType = "Single_Slab";

                CreateMaterial(slab);

                var PointSet = new List<IfcCartesianPoint>();
                foreach (var point in Points)
                {
                    PointSet.Add(toolkit_factory.MakeCartesianPoint(_model, point.x, point.y, point.z));
                }
                var profile = toolkit_factory.MakeArbitraryProfile(_model, PointSet);


                var solid = this._model.Instances.New<IfcExtrudedAreaSolid>(); //extruded area solid:拉伸区域实体。
                                                                               //有四个重要参数：SweptArea、ExtrudedDirection、Position、Depth
                solid.SweptArea = profile;
                solid.ExtrudedDirection = toolkit_factory.MakeDirection(_model, 0, 0, 1);   //拉伸方向为z轴
                //var solid_direction=toolkit_factory.MakeDirection(_model, point1, point2);
                //solid.Position = toolkit_factory.MakeLocalAxisPlacement(_model, point1, solid_direction);

                solid.Depth = thickness;
                solid.ExtrudedDirection = toolkit_factory.MakeDirection(_model, 0, 0, 1);

                toolkit_factory.SetSurfaceColor(_model, solid, 124.0 / 255.0, 200.0 / 255.0, 49.0 / 255.0, 0.15);
                var shape = toolkit_factory.MakeShapeRepresentation(_model, 3, "Body", "AdvancedSweptSolid", solid);

                slab.Representation = this._model.Instances.New<IfcProductDefinitionShape>(pd => pd.Representations.Add(shape));
                slab.PredefinedType = IfcSlabTypeEnum.USERDEFINED;

                txn.Commit();
                return slab;
            }
        }
        #endregion


        //wall code
        #region
        public List<IfcWall> WallBuild()
        {
            var Wall = new List<IfcWall>();
            List<(double, double, double)> Points = new List<(double, double, double)>();
            double z = 0;
            for (int k = 0; k < _placementMap.Count-1; k++)
            {     //点逆时针输入，先输入L型外边的三个点。
                z += _placementMap[k].height;
                var tmp = new List<(double, double, double)> {(4000-100,4000+2000,z) , (4000-100, 4000-100, z), (4000 + 2000, 4000 - 100, z), 
                    (4000+2000, 4000+100, z),(4000+100, 4000+100, z), (4000+100, 4000+2000, z),(4000-100,4000+2000,z)};
                Points.AddRange(tmp);
                Wall.Add(CreateWall(Points, 4000));
                Points.Clear();
                tmp = new List<(double, double, double)> { (9000+100,4000+2000,z),(9000+100,4000-100,z),(9000-2000,4000-100,z),
                    (9000-2000,4000+100,z),(9000-100,4000+100,z),(9000-100,4000+2000,z),(9000+100,4000+2000,z)};
                Points.AddRange(tmp);
                Wall.Add(CreateWall(Points, 4000));
                Points.Clear();
                tmp = new List<(double, double, double)> { (4000-100,9000-2000,z),(4000-100,9000+100,z),(4000+2000,9000+100,z),
                    (4000+2000,9000-100,z),(4000+100,9000-100,z),(4000+100,9000-2000,z),(4000-100,9000-2000,z)};
                Points.AddRange(tmp);
                Wall.Add(CreateWall(Points, 4000));
                Points.Clear();
                tmp = new List<(double, double, double)> { (9000-2000,9000+100,z),(9000+100,9000+100,z),(9000+100,9000-2000,z),
                    (9000-100,9000-2000,z),(9000-100,9000-100,z),(9000-2000,9000-100,z),(9000-2000,9000+100,z)};
                Points.AddRange(tmp);
                Wall.Add(CreateWall(Points, 4000));
                Points.Clear();

            }
            return Wall;
        }

        private IfcWall CreateWall(List<(double x, double y, double z)> Points, double height)
        {
            using (var txn = this._model.BeginTransaction("CreateSlab"))
            {
                var wall = this._model.Instances.New<IfcWall>();
                wall.Name = "testWall";
                wall.ObjectType = "ShearWall";

                CreateMaterial(wall);

                var PointSet = new List<IfcCartesianPoint>();
                foreach (var point in Points)
                {
                    PointSet.Add(toolkit_factory.MakeCartesianPoint(_model, point.x, point.y, point.z));
                }
                var profile = toolkit_factory.MakeArbitraryProfile(_model, PointSet);


                var solid = this._model.Instances.New<IfcExtrudedAreaSolid>(); //extruded area solid:拉伸区域实体。
                                                                               //有四个重要参数：SweptArea、ExtrudedDirection、Position、Depth
                solid.SweptArea = profile;
                solid.ExtrudedDirection = toolkit_factory.MakeDirection(_model, 0, 0, 1);   //拉伸方向为z轴

                solid.Depth = height;
                solid.ExtrudedDirection = toolkit_factory.MakeDirection(_model, 0, 0, 1);

                toolkit_factory.SetSurfaceColor(_model, solid, 124.0 / 255.0, 124 / 255.0, 124.0 / 255.0, 0.15);
                var shape = toolkit_factory.MakeShapeRepresentation(_model, 3, "Body", "AdvancedSweptSolid", solid);

                wall.Representation = this._model.Instances.New<IfcProductDefinitionShape>(pd => pd.Representations.Add(shape));
                wall.PredefinedType = IfcWallTypeEnum.USERDEFINED;

                txn.Commit();
                return wall;
            }
        }
        #endregion
    }
}
