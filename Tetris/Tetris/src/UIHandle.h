#ifndef _UIHANDLE_H
#define _UIHANDLE_H

#include <time.h>
#include "Tetris.h"
#include "../resources/resource.h"

class UIHandle
{
public:
	~UIHandle();
	HBITMAP LoadResBmp(int nResId)
	{
		return (HBITMAP) LoadImage(m_hinst, MAKEINTRESOURCE(nResId), IMAGE_BITMAP, 0, 0, LR_DEFAULTSIZE | LR_SHARED);
	}
	void InitRes(HINSTANCE hinstance, HWND hwnd);
	void Init_Ui(BOOL Restart = FALSE);
	void left();
	void up();
	void right();
	void down();
	void ClearGameMap();
	void SetGameMap();
	void DrawGameMap(BOOL Clear = FALSE);
	BOOL HitTest(int xx, int yy);
	int GetCurrentScore();
	void DrawTetris(HDC dc, HDC memdc, int xx, int yy, HBITMAP bitmap);
	void Paint(HDC dc);
	void SetTetrisScoreAndLevel(HDC dc);
	void GameOver(HDC dc);
	int GetGmaeTime(){return m_GameTime;}
	BOOL GetGameBegin(){return m_GameBegin;}
	void SetGameBegin(BOOL GameBegin){m_GameBegin = GameBegin;}
	static UIHandle * GetInstance()
	{
		if (m_pInstance == NULL)
		{
			m_pInstance = new UIHandle();
			return m_pInstance;
		}
	}
private:	
	HBITMAP m_bmpBoard, m_bmpClear, m_bmpTetris[7], m_bmpGirl;
	HDC m_hDC;
	HWND m_hWnd;
	HINSTANCE m_hinst;
	Tetris m_CurretTetris;
	Tetris m_NextTetris;
	POINT m_StartPoint;
	int m_TetrisScore;
	int m_TetrisLevel;
	int m_GameTime;
	BOOL m_GameBegin;
	int game_map[10][18];
	UIHandle();
	static UIHandle * m_pInstance;
};

#endif //_UIHANDLE_H