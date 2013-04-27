//-----------------------------------------------------------------------
// <copyright file="RubyEngine.cs" company="CompanyName">
//     Copyright info.
// </copyright>
//-----------------------------------------------------------------------
namespace Distribox.CommonLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using IronRuby;
    using Microsoft.Scripting.Hosting;

    /// <summary>
    /// Ruby engine. Provide a interactive shell for user.
    /// </summary>
    public class RubyEngine
    {
        /// <summary>
        /// The ruby engine.
        /// </summary>
        private ScriptEngine engine = Ruby.CreateEngine();

        /// <summary>
        /// The virtual machine scope.
        /// </summary>
        private ScriptScope scope = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.RubyEngine"/> class.
        /// </summary>
        public RubyEngine()
        {
            this.scope = this.engine.CreateScope();

            // warm up
            this.DoString("0");
        }

        /// <summary>
        /// Sets the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="obj">The object.</param>
        public void SetVariable(string name, object obj)
        {
            this.scope.SetVariable(name, obj);
        }

        /// <summary>
        /// Run ruby scripts.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void DoFile(string filename)
        {
            this.engine.Runtime.IO.RedirectToConsole();
            ScriptSource source = this.engine.CreateScriptSourceFromFile(filename);
            source.Execute(this.scope);
        }

        /// <summary>
        /// Run ruby code in string.
        /// </summary>
        /// <param name="code">The code.</param>
        public void DoString(string code)
        {
            this.engine.Runtime.IO.RedirectToConsole();
            ScriptSource source = this.engine.CreateScriptSourceFromString(code);
            source.Execute(this.scope);
        }

        /// <summary>
        /// Read evaluation print loop.
        /// </summary>
        public void Repl()
        {
            while (true)
            {
                try
                {
                    Console.Write("Ruby> ");
                    string line = Console.ReadLine();
                    if (line == string.Empty)
                    {
                        return;
                    }

                    this.DoString(line);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
