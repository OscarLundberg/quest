﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AxeSoftware.Quest.Functions;

namespace AxeSoftware.Quest.Scripts
{
    public class WhileScriptConstructor : IScriptConstructor
    {
        public string Keyword
        {
            get { return "while"; }
        }

        public IScript Create(string script, ScriptContext scriptContext)
        {
            string afterExpr;
            string param = Utility.GetParameter(script, out afterExpr);
            string loop = Utility.GetScript(afterExpr);
            IScript loopScript = ScriptFactory.CreateScript(loop);

            return new WhileScript(WorldModel, ScriptFactory, new Expression<bool>(param, WorldModel), loopScript);
        }

        public IScriptFactory ScriptFactory { get; set; }

        public WorldModel WorldModel { get; set; }
    }

    public class WhileScript : ScriptBase
    {
        private IFunction<bool> m_expression;
        private IScript m_loopScript;
        private WorldModel m_worldModel;
        private IScriptFactory m_scriptFactory;

        public WhileScript(WorldModel worldModel, IScriptFactory scriptFactory, IFunction<bool> expression, IScript loopScript)
        {
            m_worldModel = worldModel;
            m_scriptFactory = scriptFactory;
            m_expression = expression;
            m_loopScript = loopScript;
        }

        protected override ScriptBase CloneScript()
        {
            return new WhileScript(m_worldModel, m_scriptFactory, m_expression.Clone(), (IScript)m_loopScript.Clone());
        }

        protected override void ParentUpdated()
        {
            m_loopScript.Parent = Parent;
        }

        public override void Execute(Context c)
        {
            while (m_expression.Execute(c))
            {
                m_loopScript.Execute(c);
            }
        }

        public override string Save()
        {
            return SaveScript("while", m_loopScript, m_expression.Save());
        }

        public override string Keyword
        {
            get
            {
                return "while";
            }
        }

        public override object GetParameter(int index)
        {
            switch (index)
            {
                case 0:
                    return m_expression.Save();
                case 1:
                    return m_loopScript;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void SetParameterInternal(int index, object value)
        {
            switch (index)
            {
                case 0:
                    m_expression = new Expression<bool>((string)value, m_worldModel);
                    break;
                case 1:
                    // any updates to the script should change the script itself - nothing should cause SetParameter to be triggered.
                    throw new InvalidOperationException("Attempt to use SetParameter to change the script of a 'while' loop");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
