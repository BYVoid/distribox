//-----------------------------------------------------------------------
// <copyright file="GlobalFlag.cs" company="CompanyName">
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
    /// Global flag.
    /// </summary>
    public class GlobalFlag
    {
        /// <summary>
        /// Initializes static members of the <see cref="Distribox.CommonLib.GlobalFlag"/> class.
        /// </summary>
        static GlobalFlag()
        {
            AcceptFileEvent = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Distribox.CommonLib.GlobalFlag"/> accept file event.
        /// </summary>
        /// <value><c>true</c> if accept file event; otherwise, <c>false</c>.</value>
        public static bool AcceptFileEvent { get; set; }
    }
}
