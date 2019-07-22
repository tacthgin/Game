#include "Uihandle.h"

UIHandle * UIHandle::_instance = NULL;

UIHandle::UIHandle() : _tetrisLevel(1), _tetrisScore(0), _gameTime(1000),_gameBegin(FALSE)
{
	srand((unsigned)time(NULL));
	_nextTetris.SetTetrisProperty(rand() % 7, rand() % 4);
	memset(_gameMap, -1, sizeof(int)* 10 * 18);
}

UIHandle::~UIHandle()
{
	ReleaseDC(_hWnd, _hDC);
	delete _instance;
	for (int i = 0; i < 7; i++)
	{
		DeleteObject(_bmpTetris[i]);
	}
	DeleteObject(_bmpGirl);
	DeleteObject(_bmpBoard);
}

void UIHandle::InitRes(HINSTANCE hinstance,HWND hwnd)
{
	_hinst = hinstance;
	_hWnd = hwnd;
	_hDC = GetDC(_hWnd);
	_bmpBoard = LoadResBmp(IDB_BG);
	_bmpGirl = LoadResBmp(IDB_GIRL);
	for (int i = 0; i < 7; i++)
	{
		_bmpTetris[i] = LoadResBmp(IDB_BLUE + i);
	}	
}
void UIHandle::InitUI(BOOL Restart)
{
	if (Restart)
	{
		memset(_gameMap, -1, sizeof(int)* 10 * 18);
		_tetrisLevel = 1;
		_tetrisScore = 0;
		_gameTime = 1000;
		InvalidateRect(_hWnd, NULL, FALSE);
	}else
	{
		RECT rect = {360, 30, 480, 150};
		InvalidateRect(_hWnd, &rect, FALSE);
	}
	_startPoint.x = 3;
	_startPoint.y = 0;
	srand((unsigned)time(NULL));
	_curretTetris = _nextTetris;
	_nextTetris.SetTetrisProperty(rand() % 7, rand() % 4);
	DrawGameMap();
}

void UIHandle::Down()
{
	ClearGameMap();
	if (HitTest(_startPoint.x, _startPoint.y + 1))
	{	
		DrawGameMap(TRUE);
		_startPoint.y += 1;
		DrawGameMap();
	}else
	{
		SetGameMap();
		if (-2 == GetCurrentScore())
		{
			_gameBegin = FALSE;
			KillTimer(_hWnd, 1);
			GameOver(_hDC);		
			return;
		}
		InitUI();
	}
}

void UIHandle::Left()
{
	ClearGameMap();
	if (HitTest(_startPoint.x - 1, _startPoint.y))
	{
		DrawGameMap(TRUE);
		_startPoint.x -= 1;
		DrawGameMap();
	}
	else
		SetGameMap();
}

void UIHandle::Right()
{
	ClearGameMap();
	if (HitTest(_startPoint.x + 1, _startPoint.y))
	{
		DrawGameMap(TRUE);
		_startPoint.x += 1;
		DrawGameMap();
	}
	else
		SetGameMap();
}

void UIHandle::Up()
{
	ClearGameMap();	
	int TetrisChange = _curretTetris.GetTetrisChange();
	int TetrisType = _curretTetris.GetTetrisType();
	_curretTetris.SetTetrisProperty(TetrisType, TetrisChange + 1);
	if (HitTest(_startPoint.x, _startPoint.y))
	{
		DrawGameMap(TRUE);
		DrawGameMap();
	}else
	{
		_curretTetris.SetTetrisProperty(TetrisType, TetrisChange);
		SetGameMap();
	}
}

void UIHandle::DrawGameMap(BOOL Clear)
{
	if(Clear)
	{
		RECT rect = {_startPoint.x * 30, _startPoint.y * 30, \
				_startPoint.x * 30 + 120, _startPoint.y * 30 + 120};			
		InvalidateRect(_hWnd, &rect, FALSE);
	}
	else
	{
		const POINT * p = NULL;
		int xx = 0;
		int yy = 0;
		int TetrisType = _curretTetris.GetTetrisType();
		p = _curretTetris.GetTetrisArray();
		HDC memdc;
		memdc = CreateCompatibleDC(_hDC);

		for (int i = 0; i < 4; i++)
		{
			xx = (_startPoint.x + p[i].x);
			yy = (_startPoint.y + p[i].y);
			_gameMap[xx][yy] = TetrisType;
			DrawTetris(_hDC, memdc, xx * 30, yy * 30, _bmpTetris[TetrisType]);
		}
		DeleteDC(memdc);
	}
}

void UIHandle::SetGameMap()
{
	const POINT * p = NULL;
	p = _curretTetris.GetTetrisArray();
	int TetrisType = _curretTetris.GetTetrisType();
	int xx = 0;
	int yy = 0;
	for (int i = 0; i < 4; i++)
	{
		xx = (_startPoint.x + p[i].x);
		yy = (_startPoint.y + p[i].y);
		_gameMap[xx][yy] = TetrisType;
	}
}

void UIHandle::ClearGameMap()
{
	const POINT * p = NULL;
	p = _curretTetris.GetTetrisArray();
	int xx = 0;
	int yy = 0;
	for (int i = 0; i < 4; i++)
	{
		xx = (_startPoint.x + p[i].x);
		yy = (_startPoint.y + p[i].y);
		_gameMap[xx][yy] = -1;
	}
}

BOOL UIHandle::HitTest(int xx, int yy)
{
	const POINT * p = NULL;
	p = _curretTetris.GetTetrisArray();

	for (int i = 0; i < 4; i++)
	{
		int x = xx, y = yy;
		x += p[i].x;
		y += p[i].y;

		if (-1 != _gameMap[x][y])
		{
			return FALSE;
		}else if (x < 0 || x >= 10 || y < 0 || y >= 18)
		{
			return FALSE;
		}
	}

	return TRUE;
}

int UIHandle::GetCurrentScore()
{
	int ScoreLine = 0, ScoreCount = 0, EmptyLine = 0, EmptyCount = 0;
	int i = 0, j = 0;
	for (j = 0; j < 10; j++)
	{
		if (_gameMap[j][0] != -1)
			return -2;
	}

	for (i = 17; i > 0; i--)
	{
		for (j = 0; j < 10; j++ )
		{
			if (_gameMap[j][i] == -1 )
			{
				EmptyCount++;
				if (EmptyCount == 9)
				{
					EmptyLine = i;
				}
				else
				  break;			
			}
			else if (j == 9)
			{
				ScoreCount++;
				ScoreLine = i;
			}
		}

		if (EmptyCount == 9)
			break;
		else 
			EmptyCount = 0;
	}

	if (ScoreCount > 0)
	{
		ScoreLine = ScoreLine + ScoreCount -1;
	}else
	{
		return -1;
	}

	for ( i = ScoreLine; i > ScoreLine - ScoreCount; i--)
	{
		for (j = 0; j < 10; j++)
		{
			_gameMap[j][i] = -1;
		}
	}
	for (i = ScoreLine - ScoreCount; i > EmptyLine; i--)
	{
		for (j = 0; j < 10; j++)
		{
			_gameMap[j][i + ScoreCount] = _gameMap[j][i];
			_gameMap[j][i] = -1;
		}
	}

	if (4 == ScoreCount)
	{
		_tetrisScore += 5;
	}
	else
		_tetrisScore += ScoreCount;

	if (100 * _tetrisLevel == _tetrisScore)
	{
		_tetrisLevel++;
		_gameTime -= 100 * _tetrisLevel;
		if (_gameTime < 50)
			_gameTime = 50;
	}
	Paint(_hDC);
	return 0;	
}

void UIHandle::DrawTetris(HDC dc, HDC memdc, int xx, int yy, HBITMAP bitmap)
{
	SelectObject(memdc, bitmap);
	BitBlt(dc, xx, yy, 30, 30, memdc, 0, 0, SRCCOPY);
}

void UIHandle::Paint(HDC dc)
{
	HDC memdc;
	memdc = CreateCompatibleDC(dc);

	SelectObject(memdc, _bmpBoard);                     //底图
	BitBlt(dc, 0, 0, 300, 540, memdc, 0, 0, SRCCOPY);
	SetStretchBltMode(dc, STRETCH_HALFTONE);
	SelectObject(memdc, _bmpGirl);
	StretchBlt(dc, 300, 0, 180, 540, memdc, 0, 0, 497, 747, SRCCOPY);
	int i, xx, yy;
	int j = _nextTetris.GetTetrisType();
	const POINT * p = _nextTetris.GetTetrisArray();
	
	for (i = 0; i < 4; i++)
	{
		xx = (12 + p[i].x);
		yy = (1 + p[i].y);
		DrawTetris(dc, memdc, xx * 30, yy * 30, _bmpTetris[j]);
	}
	
	SetTetrisScoreAndLevel(dc);
	
	for (i = 17; i >= 0 ; i--)
	{
		for (j = 0; j < 10; j++)
		{
			if (_gameMap[j][i] != -1)
			{
				DrawTetris(dc, memdc, 30 * j, 30 * i, _bmpTetris[_gameMap[j][i]]);
			}
		}
	}
	DeleteDC(memdc);
}

void UIHandle::SetTetrisScoreAndLevel(HDC dc)
{
	WCHAR ScoreStr[15] = L"得分:";
	WCHAR ScoreStr1[10] = L"";
	_itow_s(_tetrisScore, ScoreStr1, 10);
	wcscat_s(ScoreStr, ScoreStr1);
	TextOutW(dc, 360, 215, ScoreStr, wcslen(ScoreStr));
	WCHAR LevelStr[15] = L"等级:";
	WCHAR LevelStr1[10] = L"";
	_itow_s(_tetrisLevel, LevelStr1, 10);
	wcscat_s(LevelStr, LevelStr1);
	TextOutW(dc, 360, 235, LevelStr, wcslen(LevelStr));
}

void UIHandle::GameOver(HDC dc)
{
	WCHAR GameOver[] = L"游戏结束";
	TextOutW(dc, 360, 255, GameOver, wcslen(GameOver));
}


	



