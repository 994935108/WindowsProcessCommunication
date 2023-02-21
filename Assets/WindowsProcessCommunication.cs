using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/// <summary>
/// ���ݵ����ݽṹ
/// </summary>
public struct COPYDATASTRUCT
{
    public IntPtr dwData;
    public int cData;
    [MarshalAs(UnmanagedType.LPStr)]
    public string lpData;
}


/// <summary>
/// �յ���Ϣ�����ݽṹ���������COPYDATASTRUCT����
/// </summary>
public struct CWPRETSTRUCT
{
    public IntPtr lparam;//ָ������Ϣ����
    public IntPtr wparam;//��Ϣ����
    public uint message;//��Ϣ����
    public IntPtr hwnd;//������Ϣ�ľ��
}

public class WindowsProcessCommunication
{
    //ȫ�ּ���
    private const int WH_CALLWNDPROC = 4;
    private const int WM_COPYDATA = 0x004A;
    [DllImport("User32.dll")]
    public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref COPYDATASTRUCT IParam);
    [DllImport("User32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


    //����һ������
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint dwThreadId);


    //�Ƴ���Ӧ�¼��ļ���
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern bool UnhookWindowsHookEx(int idHook);


    // ���ݵ�ǰ�¼�����һ��������
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
    //ί��
    private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    public delegate void ReciveMsgCallback(string msg);

    private static ReciveMsgCallback reciveMsgCallback;
    static int hookID = 0;
    
    private static IntPtr myWindows;


    /// <summary>
    /// Ŀ�괰��Ľ�������
    /// </summary>
    /// <param name="msgContent"></param>
    /// <param name="targetWindow"></param>
    public static void SendMessage(string msgContent, IntPtr targetWindow)
    {
        myWindows = FindWindow(null, Application.productName);


        String strSent = msgContent;


        if (targetWindow != IntPtr.Zero)
        {
            byte[] arr = System.Text.Encoding.Default.GetBytes(strSent);
            int len = arr.Length;
            COPYDATASTRUCT cdata;
            cdata.dwData = (IntPtr)100;
            cdata.lpData = strSent;
            cdata.cData = len + 1;
            SendMessage(targetWindow, WM_COPYDATA, myWindows, ref cdata);

            //Debug.LogError(msgContent);
        }

    }
    public static void HookLoad(ReciveMsgCallback rmsg)
    {
        reciveMsgCallback = rmsg;
        Debug.LogError("�󶨻ص�");


        HookProc lpfn = new HookProc(Hook);

        IntPtr hInstance = IntPtr.Zero;
        hookID = SetWindowsHookEx(WH_CALLWNDPROC, lpfn, hInstance, (uint)AppDomain.GetCurrentThreadId());

        if (hookID<0) { 
            UnhookWindowsHookEx(hookID);

        }
        

    }


    //ж�ع���
    public static void UnhookWindowsHookEx()
    {
        if (hookID>0)
        {
            UnhookWindowsHookEx(hookID);
        }
    }

    private static unsafe int Hook(int nCode, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            CWPRETSTRUCT m = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
            if (m.message == WM_COPYDATA)
            {
                COPYDATASTRUCT cdata = (COPYDATASTRUCT)Marshal.PtrToStructure(m.lparam, typeof(COPYDATASTRUCT));

                reciveMsgCallback?.Invoke(cdata.lpData);

            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return 0;
        }
    }
}
