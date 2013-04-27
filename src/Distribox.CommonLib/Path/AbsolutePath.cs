//-----------------------------------------------------------------------
// <copyright file="AbsolutePath.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.CommonLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Absolute path.
    /// </summary>
    public class AbsolutePath
    {
        /// <summary>
        /// The base path.
        /// </summary>
        private string basePath;

        /// <summary>
        /// The road.
        /// </summary>
        private List<string> road = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.AbsolutePath"/> class.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="road">The road.</param>
        public AbsolutePath(string basePath, params string[] road)
        {
            if (basePath.EndsWith("\\") || basePath.EndsWith("/"))
            {
                basePath = basePath.Substring(0, basePath.Length - 1);
            }

            this.basePath = basePath + Properties.PathSep;
            this.road = road.ToList();
        }
        
        /// <summary>
        /// Implicit type conversion.
        /// </summary>
        /// <param name="path">The path.</param>
        public static implicit operator string(AbsolutePath path)
        {
            return path.ToString();
        }

        /// <summary>
        /// Enter the specified road.
        /// </summary>
        /// <param name="road">The road.</param>
        /// <returns>The full path.</returns>
        public AbsolutePath Enter(params string[] road)
        {
            List<string> newRoad = this.road.ToList();
            foreach (var x in road)
            {
                newRoad.Add(x);
            }

            return new AbsolutePath(this.basePath, newRoad.ToArray());
        }

        /// <summary>
        /// File the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Full path of the file.</returns>
        public string File(string file)
        {
            return this.ToString() + file;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Distribox.CommonLib.AbsolutePath"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Distribox.CommonLib.AbsolutePath"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.basePath);
            foreach (var x in this.road)
            {
                sb.Append(x);
                sb.Append(Properties.PathSep);
            }

            return sb.ToString();
        }
    }
}
