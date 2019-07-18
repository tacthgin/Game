#include "UIHandle.h"

const int WINDOW_STYLES = WS_OVERLAPPED | WS_SYSMENU | WS_CAPTION | WS_MINIMIZEBOX;
const char* const elsAbout = "俄罗斯方块\n                        by 小白\n";
const int els_width = 480;
const int els_height = 540;
static UIHandle * pInstance;

LRESULT CALLBACK WndProc(
						 HWND hwnd,      // handle to window
						 UINT uMsg,      // message identifier
						 WPARAM wParam,  // first message parameter
						 LPARAM lParam   // second message parameter
						 )
{
	MSGBOXPARAMS mbp;
	PAINTSTRUCT ps;
	HDC hdc;
	RECT rect;
	switch (uMsg)
	{
	case WM_CREATE:
		GetWindowRect(hwnd, &rect);
		rect.right = rect.left + els_width;
		rect.bottom = rect.top + els_height;
		AdjustWindowRect(&rect, WINDOW_STYLES, TRUE);
		MoveWindow(hwnd, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, TRUE);
		break;
	case WM_PAINT:
		{	  
			hdc = BeginPaint(hwnd, &ps);
			if (pInstance != NULL)
				pInstance->Paint(hdc);
			else
			{
				HDC memdc;
				memdc = CreateCompatibleDC(hdc);
				HBITMAP bitmap = (HBITMAP)LoadImage((HINSTANCE)GetModuleHandle(NULL)
					, MAKEINTRESOURCE(IDB_GIRL), IMAGE_BITMAP, 0, 0, LR_SHARED);
				SelectObject(memdc, bitmap);
				SetStretchBltMode(hdc, STRETCH_HALFTONE);
				StretchBlt(hdc, 0, 0, els_width, els_height, memdc, 0, 0, 497, 747, SRCCOPY);
				DeleteDC(memdc);
				DeleteObject(bitmap);
			}
			EndPaint(hwnd, &ps);
		}
		break;
	case WM_COMMAND:
		switch(LOWORD(wParam))
		{
		case IDM_QUIT:
			DestroyWindow(hwnd);
			break;
		case IDM_HELP:
			mbp.cbSize = sizeof(MSGBOXPARAMS);
			mbp.hwndOwner = hwnd;
			mbp.hInstance = (HINSTANCE)GetModuleHandle(NULL);
			mbp.lpszText = elsAbout;
			mbp.lpszCaption = "关于俄罗斯方块";
			mbp.lpszIcon = MAKEINTRESOURCE(IDI_ICON1);
			mbp.dwStyle = MB_USERICON;
			mbp.dwContextHelpId = 0;
			mbp.lpfnMsgBoxCallback = NULL;
			mbp.dwLanguageId = 0;
			MessageBoxIndirect(&mbp);
			break;
		case IDM_BEGIN:
			{	
				pInstance->SetGameBegin(TRUE);
				pInstance->Init_Ui(TRUE);
				SetTimer(hwnd, 1, pInstance->GetGmaeTime(), NULL);			
			}
			break;
		}
		break;
		case WM_TIMER:
			{
				KillTimer(hwnd, 1);
				pInstance->down();
				SetTimer(hwnd, 1, pInstance->GetGmaeTime(), NULL);	
			}
			break;
		case WM_KEYDOWN:
			if (pInstance->GetGameBegin())
			{
				switch((unsigned int)wParam)
				{
				case VK_DOWN:
					pInstance->down();
					break;
				case VK_LEFT:
					pInstance->left();
					break;
				case VK_RIGHT:
					pInstance->right();
					break;
				case VK_UP:
					pInstance->up();
					break;
				}
			}
			break;
		case WM_DESTROY:
			PostQuitMessage(0);
			break;
		default:
			return DefWindowProc(hwnd, uMsg, wParam, lParam);
			
	}
	return 0;
}

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) 
{
	MSG msg;
	WNDCLASSEX wce;
	HWND hwnd;
	
	
	// 设置窗口
	wce.cbSize = sizeof(WNDCLASSEX);
	wce.style = 0;
	wce.lpfnWndProc = (WNDPROC) WndProc;
	wce.cbClsExtra = wce.cbWndExtra = 0;
	wce.hInstance = hInstance;
	wce.hIcon = (HICON) LoadImage(hInstance, MAKEINTRESOURCE(IDI_ICON1), IMAGE_ICON, 32, 32, LR_SHARED);
	wce.hCursor = (HCURSOR) LoadImage(NULL, IDC_ARROW, IMAGE_CURSOR, 0, 0, LR_DEFAULTSIZE | LR_SHARED);
	wce.hbrBackground = (HBRUSH) (COLOR_BTNFACE + 1);
	wce.lpszMenuName = MAKEINTRESOURCE(IDR_MENU1);
	wce.lpszClassName = "ELS";
	wce.hIconSm = (HICON) LoadImage(hInstance, MAKEINTRESOURCE(IDI_ICON1), IMAGE_ICON, 16, 16, LR_SHARED);
	RegisterClassEx(&wce);
	hwnd = CreateWindow("ELS", "俄罗斯方块", WINDOW_STYLES,
		300, 100, CW_USEDEFAULT, CW_USEDEFAULT, NULL, NULL, hInstance, NULL);
	if (hwnd == NULL)
	{
		return 0;
	}
	ShowWindow(hwnd, SW_NORMAL);
	UpdateWindow(hwnd);	
	
	pInstance = UIHandle::GetInstance();
	pInstance->InitRes(hInstance, hwnd);
	// 接收消息
	while (GetMessage(&msg, NULL, 0, 0))
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	return msg.wParam;
}