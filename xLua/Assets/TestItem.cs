using System;
using System.Collections;
using XLua;
using UnityEngine;

public abstract class TestItem
{

    protected PTest m_ptest;
    protected int m_index;

    public TestItem(PTest ptest, int index)
    {
        m_ptest = ptest;
        m_index = index;
    }

    public abstract IEnumerator Test();
}




public class TestLua : TestItem
{
    string m_luaFuncName;
    Transform m_trans;
    Func<Transform, double> m_f;

    public TestLua(PTest ptest, int index, Transform trans)
        : base(ptest, index)
    {
        m_luaFuncName = "Test" + index;
        m_trans = trans;
    }


    public override IEnumerator Test()
    {
        m_ptest.logText += m_luaFuncName + " Begin:\n";
        double totalMS = 0;

        m_ptest.scriptEnv.Get(m_luaFuncName, out m_f);
        int count = m_ptest.runCount;
        for (int i = 1; i <= count; ++i)
        {
            m_ptest.GC();
            yield return m_ptest.ws;


            object ret = m_f(m_trans);
            double t = (double)ret * 1000;
            totalMS += t;
            m_ptest.logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        m_ptest.logText += string.Format("{0} complete average ms: {1}\n", m_luaFuncName, totalMS / count);
    }
}


public class TestEmptyFunc : TestItem
{
    Action m_f;

    public TestEmptyFunc(PTest ptest, int index)
        : base(ptest, index)
    {
    }

    public override IEnumerator Test()
    {
        m_ptest.logText += "Test" + m_index + " Begin:\n";

        m_ptest.scriptEnv.Get("EmptyFunc", out m_f);
        int count = m_ptest.runCount;
        double totalMS = 0;
        for (int i = 1; i <= count; ++i)
        {
            m_ptest.GC();
            yield return m_ptest.ws;

            long ts = System.DateTime.Now.Ticks;
            for (int j = 0; j < 200000; ++j)
            {
                m_f();
            }
            double t = (double)((System.DateTime.Now.Ticks - ts) / 10000.0);

            totalMS += t;
            m_ptest.logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        m_ptest.logText += string.Format("Test{0} complete average ms: {1}\n", m_index, totalMS / count);
    }
}


public class TestGetLuaValue : TestItem
{
    string m_valueName;
    object m_value;

    public TestGetLuaValue(PTest ptest, int index, string valueName)
        : base(ptest, index)
    {
        m_valueName = valueName;
    }

    public override IEnumerator Test()
    {
        m_ptest.logText += "Test" + m_index + " Begin:\n";

        int count = m_ptest.runCount;
        double totalMS = 0;
        for (int i = 1; i <= count; ++i)
        {
            m_ptest.GC();
            yield return m_ptest.ws;

            long ts = System.DateTime.Now.Ticks;
            for (int j = 0; j < 200000; ++j)
            {
                m_value = m_ptest.scriptEnv.GetInPath<object>(m_valueName);
            }
            double t = (double)((System.DateTime.Now.Ticks - ts) / 10000.0);

            totalMS += t;
            m_ptest.logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        m_ptest.logText += string.Format("Test{0} complete average ms: {1}\n", m_index, totalMS / count);
    }
}
