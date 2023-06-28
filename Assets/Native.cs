/* 
 * Native dll invocation helper by Francis R. Griffiths-Keam
 * www.runningdimensions.com/blog
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using AOT;
using System.Security.AccessControl;

public static class Native
{
    public static T Invoke<T, T2>(IntPtr library, params object[] pars)
    {
        IntPtr funcPtr = GetProcAddress(library, typeof(T2).Name);
        if (funcPtr == IntPtr.Zero)
        {
            int errno = Marshal.GetLastWin32Error();
            Debug.LogWarning("Could not gain reference to method address (" + typeof(T2).Name + " - Error: " + errno + ")");
            return default(T);
        }

        var func = Marshal.GetDelegateForFunctionPointer(GetProcAddress(library, typeof(T2).Name), typeof(T2));
        if (func == null) {
            int errno = Marshal.GetLastWin32Error();
            Debug.LogWarning("Could not get delegate for method (" + typeof(T2).Name + " - Error: " + errno + ")");
            return default(T);
        }

        try { 
            return (T)func.DynamicInvoke(pars);
        } catch (Exception e) {
            Debug.LogError("Unhandled exception in external function (" + typeof(T2).Name + ") - " + e.Message);
            return default(T);
        }
    }

    public static void Invoke<T>(IntPtr library, params object[] pars)
    {
        IntPtr funcPtr = GetProcAddress(library, typeof(T).Name);
        if (funcPtr == IntPtr.Zero)
        {
            int errno = Marshal.GetLastWin32Error();
            Debug.LogWarning("Could not gain reference to method address (" + typeof(T).Name + " - Error: " + errno + ")");
            return;
        }

        var func = Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T));
        if (func == null) {
            int errno = Marshal.GetLastWin32Error();
            Debug.LogWarning("Could not get delegate for method (" + typeof(T).Name + " - Error: " + errno + ")");
            return;
        }

        try {
            func.DynamicInvoke(pars);
        }
        catch (Exception e) {
            Debug.LogError("Unhandled exception in external function (" + typeof(T).Name + ") - " + e.Message);
        }
    }

#if UNITY_EDITOR_WIN
    [DllImport("kernel32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
#else
    [DllImport("libdl.dylib")] private static extern IntPtr dlopen(String filename, int flags);
    [DllImport("libdl.dylib")] private static extern IntPtr dlsym(IntPtr handle, string symbol);
    [DllImport("libdl.dylib")] private static extern IntPtr dlerror();
    [DllImport("libdl.dylib")] private static extern int dlclose(IntPtr handle);

    public static bool FreeLibrary(IntPtr handle) {
        return dlclose(handle) == 0;
    }

    public static IntPtr LoadLibrary(string filename, string suffix = ".bundle/Contents/MacOS/Manhattan") {
        dlerror(); // clear previous errors
        var res = dlopen(filename + suffix, 2 /* RTLD_NOW */ );
        var err = dlerror();
        if (err != IntPtr.Zero)
            throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(err));
        return res;
    }

    public static IntPtr GetProcAddress(IntPtr handle, string name)
    {
        dlerror(); // clear previous errors
        var res = dlsym(handle, name);
        var err = dlerror();
        if (err != IntPtr.Zero)
            throw new Exception("dlsym: " + Marshal.PtrToStringAnsi(err));
        return res;
    }
#endif

    //------------------------------------------------------------------------------------------------
    //[DllImport("DebugLogPlugin", CallingConvention = CallingConvention.Cdecl)]
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void DebugCallback_Type(IntPtr request, int color, int size);
    enum Color { red, green, blue, black, white, yellow, orange };
    
    [MonoPInvokeCallback(typeof(DebugCallback_Type))]
    static void DebugCallback(IntPtr request, int color, int size)
    {
        //Ptr to string
        string debug_string = Marshal.PtrToStringAnsi(request, size);
        UnityEngine.Debug.Log("<color=#889EAD>Manhattan: " + debug_string + "</color>");
    }
    
    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void RegisterDebugCallback(DebugCallback_Type cb);
    
    // Use this for initialization
    public static bool EnableDebugging(IntPtr library)
    {
        IntPtr funcPtr = GetProcAddress(library, "RegisterDebugCallback");
        if (funcPtr == IntPtr.Zero)
        {
            Debug.LogWarning("Library does not support debug output.");
            return false;
        }
        
        Invoke<RegisterDebugCallback>(library, (DebugCallback_Type)DebugCallback);
        return true;
    }
    
    public static void DisableDebugging(IntPtr library)
    {
        IntPtr funcPtr = GetProcAddress(library, "RegisterDebugCallback");
        if (funcPtr == IntPtr.Zero)
        {
            return;
        }
        
        Invoke<RegisterDebugCallback>(library, (DebugCallback_Type)null);
    }
}