using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.SqlServer.Types;

namespace Infrastructure.Data.Spatial
{
    public static class SpatialDataHelper
    {
        public static SqlGeometry ConstructSqlGeometry(this List<PointD> lines)
        {
            var geomBuilder = new SqlGeometryBuilder();

            // SqlGeometryBuilder.SetSrid Must be called at the beginning of each geometry sequence
            geomBuilder.SetSrid(0);
            geomBuilder.BeginGeometry(OpenGisGeometryType.GeometryCollection);

            if (lines.Count > 0)
            {
                if (lines.Count == 1) //check if its a point or a line and start a geometry for the point or line
                    geomBuilder.BeginGeometry(OpenGisGeometryType.Point);
                else
                    geomBuilder.BeginGeometry(OpenGisGeometryType.LineString);

                int count = 0;
                foreach (PointD p in lines) //add all points
                {
                    if (count == 0)
                        geomBuilder.BeginFigure(p.X, p.Y);
                    else
                        geomBuilder.AddLine(p.X, p.Y);

                    count++;
                }

                geomBuilder.EndFigure();
                geomBuilder.EndGeometry();
            }

            geomBuilder.EndGeometry();

            SqlGeometry constructedSqlGeometry = geomBuilder.ConstructedGeometry;
            return constructedSqlGeometry;
        }

        public static SqlParameter GetGeometrySqlParameter(string paramName, List<PointD> lines, bool makeValid)
        {
            SqlParameter sqlParam = new SqlParameter();
            sqlParam.ParameterName = paramName;
            sqlParam.UdtTypeName = "geometry";

            SqlGeometryBuilder geomBuilder = new SqlGeometryBuilder();

            // SqlGeometryBuilder.SetSrid Must be called at the beginning of each geometry sequence
            geomBuilder.SetSrid(0);
            geomBuilder.BeginGeometry(OpenGisGeometryType.GeometryCollection);

            if (lines.Count > 0)
            {
                if (lines.Count == 1) //check if its a point or a line and start a geometry for the point or line
                    geomBuilder.BeginGeometry(OpenGisGeometryType.Point);
                else
                    geomBuilder.BeginGeometry(OpenGisGeometryType.LineString);

                int count = 0;
                foreach (PointD p in lines) //add all points
                {
                    if (count == 0)
                        geomBuilder.BeginFigure(p.X, p.Y);
                    else
                        geomBuilder.AddLine(p.X, p.Y);

                    count++;
                }

                geomBuilder.EndFigure();
                geomBuilder.EndGeometry();
            }

            geomBuilder.EndGeometry();

            SqlGeometry constructed = geomBuilder.ConstructedGeometry;
            if (makeValid)
            {
                //Note required to convert into a geometry instance with a valid Open Geospatial Consortium (OGC) type.
                // this can cause the points to shift - use with caution...
                constructed = constructed.MakeValid();
            }
            sqlParam.Value = constructed;

            return sqlParam;
        }

    }
}
