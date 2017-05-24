using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DllEnv
{
    public abstract IEnumerator Init(System.Action onDone);

    public abstract object InvokeStatic(string type, string method, object p1);
}
