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

        public dynamic this[String name]
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

        public void SetVariable(String name, Object obj)
        {
            m_scope.SetVariable(name, obj);
        }

        private StringBuilder sb = new StringBuilder();

        private IEnumerable<String> ReplList()
        {
            while (true)
            {
                Console.Write("Ruby> ");
                if (sb.Length != 0) Console.Write("... ");
                String line = Console.ReadLine();
                if (line == "") break;
                yield return line;
            }
        }

        public void Repl()
        {
            foreach (var line in ReplList())
            {
                try
                {
                    sb.Append(line);
                    DoString(sb.ToString());
                    if (Console.CursorLeft != 0) Console.WriteLine();
                    sb.Clear();
                }
                catch (Exception e)
                {
                    if (e.Message.StartsWith("syntax error, unexpected end of file")) continue;
                    Console.WriteLine(e.Message);
                    sb.Clear();
                }
            }
        }
    }
}
