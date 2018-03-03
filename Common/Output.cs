using System.Collections;
using System.Collections.Generic;

#if _CLIENT
using UnityEngine;
#endif

public static class Output
{
    public static void Log(string text)
    {
#if _CLIENT
        Debug.Log(text);
#else
        System.Console.WriteLine(text);
#endif
    }

    public static void Log(System.Exception exception)
    {
        var exceptionMessage = string.Format("{0}\n{1}", exception.Message, exception.StackTrace);

#if _CLIENT
        Debug.Log(exceptionMessage);
#else
        System.Console.WriteLine(exceptionMessage);
#endif
    }

    public static void LogFormat(string format, params object[] args)
    {
#if _CLIENT
        Debug.LogFormat(format, args);
#else
        var text = string.Format(format, args);
        System.Console.WriteLine(text);
#endif
    }

    public static void LogFormat(System.Exception exception, string format, params object[] args)
    {
        var exceptionMessage = string.Format("{0}\n{1}", exception.Message, exception.StackTrace);

#if _CLIENT
        Debug.Log(exceptionMessage);
        Debug.LogFormat(format, args);
#else
        var text = string.Format(format, args);
        System.Console.WriteLine(exceptionMessage);
        System.Console.WriteLine(text);
#endif
    }

    public static void LogWarning(string text)
    {
#if _CLIENT
        Debug.LogWarning(text);
#else
        System.Console.WriteLine(text);
#endif
    }

    public static void LogWarning(System.Exception exception)
    {
        var exceptionMessage = string.Format("{0}\n{1}", exception.Message, exception.StackTrace);

#if _CLIENT
        Debug.LogWarning(exceptionMessage);
#else
        System.Console.WriteLine(exceptionMessage);
#endif
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
#if _CLIENT
        Debug.LogWarningFormat(format, args);
#else
        var text = string.Format(format, args);
        System.Console.WriteLine(text);
#endif
    }

    public static void LogWarningFormat(System.Exception exception, string format, params object[] args)
    {
        var exceptionMessage = string.Format("{0}\n{1}", exception.Message, exception.StackTrace);

#if _CLIENT
        Debug.LogWarning(exceptionMessage);
        Debug.LogWarningFormat(format, args);
#else
        var text = string.Format(format, args);
        System.Console.WriteLine(exceptionMessage);
        System.Console.WriteLine(text);
#endif
    }

    public static void LogError(string text)
    {
#if _CLIENT
        Debug.LogError(text);
#else
        System.Console.WriteLine(text);
#endif
    }

    public static void LogError(System.Exception exception)
    {
        var exceptionMessage = string.Format("{0}\n{1}", exception.Message, exception.StackTrace);

#if _CLIENT
        Debug.LogError(exceptionMessage);
#else
        System.Console.WriteLine(exceptionMessage);
#endif
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
#if _CLIENT
        Debug.LogErrorFormat(format, args);
#else
        var text = string.Format(format, args);
        System.Console.WriteLine(text);
#endif
    }

    public static void LogErrorFormat(System.Exception exception, string format, params object[] args)
    {
        var exceptionMessage = string.Format("{0}\n{1}", exception.Message, exception.StackTrace);

#if _CLIENT
        Debug.LogError(exceptionMessage);
        Debug.LogErrorFormat(format, args);
#else
        var text = string.Format(format, args);
        System.Console.WriteLine(exceptionMessage);
        System.Console.WriteLine(text);
#endif
    }
}