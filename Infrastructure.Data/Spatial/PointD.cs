﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Spatial
{
   [Serializable]
   public class PointD
    {
        public double X;
        public double Y;

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Point ToPoint()
        {
            return new Point((int)X, (int)Y);
        }

        public override bool Equals(object obj)
        {
            return obj is PointD && this == (PointD)obj;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(PointD a, PointD b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(PointD a, PointD b)
        {
            return !(a == b);
        }
    }
}
