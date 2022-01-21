#include <Windows.h>

BOOL MonitorEnumProc(
    HMONITOR hMonitor,
    HDC unnamedParam2,
    LPRECT unnamedParam3,
    LPARAM lParam
)
{
    MONITORINFO mi = { 0 };
    mi.cbSize = sizeof(mi);
    GetMonitorInfo(hMonitor, &mi);
    if (mi.dwFlags != MONITORINFOF_PRIMARY)
    {
        RECT* rect = (RECT *)lParam;
        *rect = mi.rcWork;
        return FALSE;
    }
    return TRUE;
}

extern "C" __declspec(dllexport) void DisplayWindowInSecondaryMonitor(HWND hwnd)
{
    RECT rect = { 0 };
    EnumDisplayMonitors(NULL, NULL, MonitorEnumProc, (LPARAM)&rect);
    if (rect.left != 0 || rect.top != 0 || rect.right != 0 || rect.bottom != 0)
    {
        RECT wndrect = { 0 };
        GetWindowRect(hwnd, &wndrect);
        int w = wndrect.right - wndrect.left, h = wndrect.bottom - wndrect.top;
        int monitorw = rect.right - rect.left, monitorh = rect.bottom - rect.top;        
        MoveWindow(hwnd, rect.left + monitorw / 2 - w / 2, rect.top + monitorh / 2 - h / 2, w, h, FALSE);
    }
}

