using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class AbsolutePath
    {
        private string BasePath;
        private List<string> Road = new List<string>();

        public AbsolutePath(string basePath, params string[] road)
        {
            if (basePath.EndsWith("\\") || basePath.EndsWith("/"))
            {
                basePath = basePath.Substring(0, basePath.Length - 1);
            }

            this.BasePath = basePath + Properties.PathSep;
            this.Road = road.ToList();
        }

        public AbsolutePath Enter(params string[] road)
        {
            List<string> newRoad = this.Road.ToList();
            foreach (var x in road)
            {
                newRoad.Add(x);
            }

            return new AbsolutePath(this.BasePath, newRoad.ToArray());
        }

        public AbsolutePath Enter(RelativePath road)
        {
            List<string> newRoad = this.Road.ToList();
            foreach (var x in road.GetRoads())
            {
                newRoad.Add(x);
            }

            return new AbsolutePath(this.BasePath, newRoad.ToArray());
        }

        public string File(string file)
        {
            return this.ToString() + file;
        }

        public static implicit operator string(AbsolutePath path)
        {
            return path.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BasePath);
            foreach (var x in Road)
            {
                sb.Append(x);
                sb.Append(Properties.PathSep);
            }

            return sb.ToString();
        }
    }
}
