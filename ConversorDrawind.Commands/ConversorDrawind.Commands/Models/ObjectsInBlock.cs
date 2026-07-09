using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;

namespace ConversorDrawind.Commands
{
    class ObjectsInBlock
    {
        public List<Line> lineList = new List<Line>();
        public List<DBText> dBTextList = new List<DBText>();
        public List<Arc> arcList = new List<Arc>();
        public List<Hatch> hatchList = new List<Hatch>();
        public Matrix3d matrix3d = new Matrix3d();
        public ObjectId textStyle;
        public ObjectId dimStyle;
    }
}
