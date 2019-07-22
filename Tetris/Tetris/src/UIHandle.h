#ifndef _UIHANDLE_H
#define _UIHANDLE_H

#include <time.h>
#include "Tetris.h"
#include "../resource.h"

class UIHandle
{
public:
	~UIHandle();
	HBITMAP LoadResBmp(int nResId)
	{
		return (HBITMAP) LoadImage(_hinst, MAKEINTRESOURCE(nResId), IMAGE_BITMAP, 0, 0, LR_DEFAULTSIZE | LR_SHARED);
	}
	void InitRes(HINSTANCE hinstance, HWND hwnd);
	void InitUI(BOOL Restart = FALSE);
	void Left();
	void Up();
	void Right();
	void Down();
	void ClearGameMap();
	void SetGameMap();
	void DrawGameMap(BOOL Clear = FALSE);
	BOOL HitTest(int xx, int yy);
	int GetCurrentScore();
	void DrawTetris(HDC dc, HDC memdc, int xx, int yy, HBITMAP bitmap);
	void Paint(HDC dc);
	void SetTetrisScoreAndLevel(HDC dc);
	void GameOver(HDC dc);
	int GetGmaeTime(){return _gameTime;}
	BOOL GetGameBegin(){return _gameBegin;}
	void SetGameBegin(BOOL GameBegin){_gameBegin = GameBegin;}
	static UIHandle * GetInstance()
	{
		if (_instance == NULL)
		{
			_instance = new UIHandle();
		}

		return _instance;
	}
private:	
	HBITMAP _bmpBoard, _bmpClear, _bmpTetris[7], _bmpGirl;
	HDC _hDC;
	HWND _hWnd;
	HINSTANCE _hinst;
	Tetris _curretTetris;
	Tetris _nextTetris;
	POINT _startPoint;
	int _tetrisScore;
	int _tetrisLevel;
	int _gameTime;
	BOOL _gameBegin;
	int _gameMap[10][18];
	UIHandle();
	static UIHandle * _instance;
};

#endif //_UIHANDLE_H