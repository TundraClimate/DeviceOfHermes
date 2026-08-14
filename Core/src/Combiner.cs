using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using HarmonyLib;
using HarmonyExtension;
using Castle.DynamicProxy;

namespace DeviceOfHermes;

internal static class Combiner
{
    public static object CreateClass(object target, IEnumerable<object> children, Type[] interfaces)
    {
        return gen.CreateClassProxyWithTarget(target.GetType().BaseType, interfaces, target, new CombinableInterceptor(children));
    }

    static ProxyGenerator gen = new ProxyGenerator();
}

internal class CombinableInterceptor(IEnumerable<object> children) : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var method = invocation.Method;
        var resType = method.ReturnType;

        var fn = (Func<object, object?[], object>)_cache.GetOrAdd(method, MakeDelegate);

        if (NeqFld(invocation.Proxy, invocation.InvocationTarget))
        {
            CopyFields(invocation.Proxy, invocation.InvocationTarget);

            foreach (var child in _children)
            {
                CopyFields(invocation.InvocationTarget, child);
            }
        }

        if (!method.DeclaringType.IsInterface)
        {
            invocation.Proceed();
        }

        object? origin = invocation.ReturnValue;
        object? res = null;

        foreach (var child in _children)
        {
            if (child is null)
            {
                continue;
            }

            if (!method.DeclaringType.IsInstanceOfType(child))
            {
                return;
            }

            if (resType == typeof(void))
            {
                fn(child, invocation.Arguments);
            }
            else
            {
                var called = fn(child, invocation.Arguments);

                if (!Equals(origin, called))
                {
                    res = called;
                }
            }
        }

        if (res is not null)
        {
            invocation.ReturnValue = res;
        }
    }

    private static Func<object, object[], object> MakeDelegate(MethodInfo method)
    {
        var targetExp = Expression.Parameter(typeof(object), "target");
        var argsExp = Expression.Parameter(typeof(object[]), "args");

        var instanceExp = Expression.Convert(targetExp, method.DeclaringType);

        var parameterExps = method.GetParameters()
            .Select((p, i) => Expression.Convert(Expression.ArrayIndex(argsExp, Expression.Constant(i)), p.ParameterType))
            .ToArray();

        var callExp = Expression.Call(
            instanceExp,
            method,
            parameterExps
        );

        if (method.ReturnType == typeof(void))
        {
            var body = Expression.Block(
                callExp,
                Expression.Constant(null)
            );

            return Expression
                .Lambda<Func<object, object[], object>>(
                    body,
                    targetExp,
                    argsExp
                )
                .Compile();
        }

        var result = Expression.Convert(
            callExp,
            typeof(object)
        );

        return Expression
            .Lambda<Func<object, object[], object>>(
                result,
                targetExp,
                argsExp
            )
            .Compile();
    }

    private void CopyFields(object? src, object? dst)
    {
        if (src is null || dst is null)
        {
            return;
        }

        var srcType = src.GetType();
        var dstType = dst.GetType();

        Type? ty = srcType;

        while (ty is not null)
        {
            var fields = _fieldsCache.GetOrAdd(ty, _ => ty.GetFields(AccessTools.all));

            foreach (var field in fields)
            {
                if (!field.DeclaringType.IsAssignableFrom(dstType))
                {
                    continue;
                }

                var access = _accessCache.GetOrAdd(field, _ => ty.FieldRefAccess<object>(field.Name));

                var srcFld = access(src);
                ref var dstFld = ref access(dst);

                if (!Equals(srcFld, dstFld))
                {
                    dstFld = srcFld;
                }
            }

            ty = ty.BaseType;
        }
    }

    private bool NeqFld(object? proxy, object? target)
    {
        if (proxy is null || target is null)
        {
            return true;
        }

        var fields = _fieldsCache.GetOrAdd(target.GetType(), ty => ty.GetFields(AccessTools.all));

        foreach (var field in fields)
        {
            var access = _accessCache.GetOrAdd(field, _ => target.GetType().FieldRefAccess<object>(field.Name));

            var srcFld = access(proxy);
            var dstFld = access(target);

            if (!Equals(srcFld, dstFld))
            {
                return true;
            }
        }

        return false;
    }

    private List<object> _children = children.ToList();

    private static ConcurrentDictionary<MethodInfo, Delegate> _cache = new();

    private static ConcurrentDictionary<Type, FieldInfo[]> _fieldsCache = new();

    private static ConcurrentDictionary<FieldInfo, AccessTools.FieldRef<object, object>> _accessCache = new();
}
