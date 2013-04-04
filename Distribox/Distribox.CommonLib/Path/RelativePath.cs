using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distribox.CommonLib
{
    public class RelativePath
    {
        private List<string> Road = new List<string>();

        public RelativePath(params string[] road)
        {
            this.Road = road.ToList();
        }

        public RelativePath Enter(params string[] road)
        {
            List<string> newRoad = this.Road.ToList();
            foreach (var x in road)
            {
                newRoad.Add(x);
            }

            return new RelativePath(newRoad.ToArray());
        }

        public string File(string file)
        {
            return this.ToString() + file;
        }

        public List<string> GetRoads()
        {
            return Road;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var x in Road)
            {
                sb.Append(x);
                sb.Append(Properties.PathSep);
            }

            return sb.ToString();
        }
    }
}
