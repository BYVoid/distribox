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
        private ScriptEngine m_engine = Ruby.CreateEngine();
        private ScriptScope m_scope = null;

        public RubyEngine()
        {
            m_scope = m_engine.CreateScope();

            // warm up
            DoString("0");
        }

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

        public void DoFile(String filename)
        {
            m_engine.Runtime.IO.RedirectToConsole();
            ScriptSource source = m_engine.CreateScriptSourceFromFile(filename);
            source.Execute(m_scope);
        }

        public void DoString(String code)
        {
            m_engine.Runtime.IO.RedirectToConsole();
            ScriptSource source = m_engine.CreateScriptSourceFromString(code);
            source.Execute(m_scope);
        }

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
