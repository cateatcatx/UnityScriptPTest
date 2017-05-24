using System.Collections;
using UnityEngine;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;

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




public class TestDllWithTrans : TestItem
{
    string m_luaFuncName;
    Transform m_trans;

    public TestDllWithTrans(PTest ptest, int index, Transform trans)
        : base(ptest, index)
    {
        m_luaFuncName = "Test" + index;
        m_trans = trans;
    }


    public override IEnumerator Test()
    {
        m_ptest.logText += "Test" + m_index + " Begin:\n";
        double totalMS = 0;
#if ILRuntime
        IMethod method = m_ptest.appdomain.LoadedTypes["PTest"].GetMethod(m_luaFuncName, 1);
#else
        var method = m_ptest.GetStaticMethod("PTest", m_luaFuncName);
        object[] ps = new object[1];
        ps[0] = m_trans;
#endif
        int count = m_ptest.runCount;
        for (int i = 1; i <= count; ++i)
        {
            m_ptest.GC();
            yield return m_ptest.ws;
#if ILRuntime
            double t = (double)m_ptest.appdomain.Invoke(method, null, m_trans);
#else
            
            double t = (double)method.Invoke(null, ps);
#endif
            totalMS += t;
            m_ptest.logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        m_ptest.logText += string.Format("Test{0} complete average ms: {1}\n", m_index, totalMS / count);
    }
}


public class TestDll : TestItem
{
    string m_luaFuncName;

    public TestDll(PTest ptest, int index)
        : base(ptest, index)
    {
        m_luaFuncName = "Test" + index;
    }


    public override IEnumerator Test()
    {
        m_ptest.logText += "Test" + m_index + " Begin:\n";
        double totalMS = 0;
#if ILRuntime
        IMethod method = m_ptest.appdomain.LoadedTypes["PTest"].GetMethod(m_luaFuncName, 0);
#else
        var method = m_ptest.GetStaticMethod("PTest", m_luaFuncName);
#endif
        int count = m_ptest.runCount;
        for (int i = 1; i <= count; ++i)
        {
            m_ptest.GC();
            yield return m_ptest.ws;
#if ILRuntime
            double t = (double)m_ptest.appdomain.Invoke(method, null, null);
#else
            double t = (double)method.Invoke(null, null);
#endif
            totalMS += t;
            m_ptest.logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        m_ptest.logText += string.Format("Test{0} complete average ms: {1}\n", m_index, totalMS / count);
    }
}


public class TestEmptyFunc : TestItem
{
    public TestEmptyFunc(PTest ptest, int index)
        : base(ptest, index)
    {
    }

    public override IEnumerator Test()
    {
        m_ptest.logText += "Test" + m_index + " Begin:\n";
#if ILRuntime
        IType type = m_ptest.appdomain.LoadedTypes["PTest"];
#else
        System.Type type = m_ptest.assembly.GetType("PTest");
#endif
        int count = m_ptest.runCount;
        double totalMS = 0;
        for (int i = 1; i <= count; ++i)
        {
            m_ptest.GC();
            yield return m_ptest.ws;

            long ts = System.DateTime.Now.Ticks;
            for (int j = 0; j < 200000; ++j)
            {
#if ILRuntime
                IMethod method = type.GetMethod("EmptyFunc", 0);
                m_ptest.appdomain.Invoke(method, null, null);
#else
                var method = type.GetMethod("EmptyFunc");
                method.Invoke(null, null);
#endif
            }
            double t = (double)((System.DateTime.Now.Ticks - ts) / 10000.0);

            totalMS += t;
            m_ptest.logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        m_ptest.logText += string.Format("Test{0} complete average ms: {1}\n", m_index, totalMS / count);
    }
}


public class TestGetValue : TestItem
{
    string m_valueName;
    object m_value;

    public TestGetValue(PTest ptest, int index, string valueName)
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
#if ILRuntime
                IType type = m_ptest.appdomain.LoadedTypes["PTest"];
                System.Type ttt = type.ReflectionType;
#else
                var ttt = m_ptest.assembly.GetType("PTest");
#endif
                var pi = ttt.GetField(m_valueName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                m_value = pi.GetValue(null);
            }
            double t = (double)((System.DateTime.Now.Ticks - ts) / 10000.0);

            totalMS += t;
            m_ptest.logText += string.Format("{0}: ms: {1}\n", i, t);
        }

        m_ptest.logText += string.Format("Test{0} complete average ms: {1}\n", m_index, totalMS / count);
    }
}
