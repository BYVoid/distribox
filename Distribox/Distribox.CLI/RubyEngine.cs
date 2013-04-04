using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronRuby;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace Distribox.CommonLib
{
    /// <summary>
    /// Ruby engine. Provide a interactive shell for user.
    /// </summary>
    public class RubyEngine
    {
        /// <summary>
        /// The ruby engine.
        /// </summary>
        private ScriptEngine m_engine = Ruby.CreateEngine();

        /// <summary>
        /// The virtual machine scope.
        /// </summary>
        private ScriptScope m_scope = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Distribox.CommonLib.RubyEngine"/> class.
        /// </summary>
        public RubyEngine()
        {
            m_scope = m_engine.CreateScope();

            // warm up
            DoString("0");
        }

        /// <summary>
        /// Gets or sets the <see cref="Distribox.CommonLib.RubyEngine"/> with the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public object this[String name]
        {
            get
            {
                return m_scope.GetVariable(name);
            }

            set
            {
                m_scope.SetVariable(name, value);
            }
        }

        /// <summary>
        /// Run ruby scripts.
        /// </summary>
        /// <param name="filename">Filename.</param>
        public void DoFile(String filename)
        {
            m_engine.Runtime.IO.RedirectToConsole();
            ScriptSource source = m_engine.CreateScriptSourceFromFile(filename);
            source.Execute(m_scope);
        }

        /// <summary>
        /// Run ruby code in string.
        /// </summary>
        /// <param name="code">Code.</param>
        public void DoString(String code)
        {
            m_engine.Runtime.IO.RedirectToConsole();
            ScriptSource source = m_engine.CreateScriptSourceFromString(code);
            source.Execute(m_scope);
        }

        /// <summary>
        /// Read eval print loop.
        /// </summary>
        public void Repl()
        {
            while (true)
            {
                try
                {
                    Console.Write("Ruby> ");
                    string line = Console.ReadLine();
                    if (line == "") return;
                    DoString(line);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
