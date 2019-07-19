#include "Uihandle.h"

UIHandle * UIHandle::m_pInstance = NULL;

UIHandle::UIHandle() : m_TetrisLevel(1), m_TetrisScore(0), m_GameTime(1000),m_GameBegin(FALSE)
{
	srand((unsigned)time(NULL));
	m_NextTetris.SetTetrisProperty(rand() % 7, rand() % 4);
	memset(game_map, -1, sizeof(int)* 10 * 18);
}

UIHandle::~UIHandle()
{
	ReleaseDC(m_hWnd, m_hDC);
	delete m_pInstance;
	for (int i = 0; i < 7; i++)
	{
		DeleteObject(m_bmpTetris[i]);
	}
	DeleteObject(m_bmpGirl);
	DeleteObject(m_bmpBoard);
}

void UIHandle::InitRes(HINSTANCE hinstance,HWND hwnd)
{
	m_hinst = hinstance;
	m_hWnd = hwnd;
	m_hDC = GetDC(m_hWnd);
	m_bmpBoard = LoadResBmp(IDB_BG);
	m_bmpGirl = LoadResBmp(IDB_GIRL);
	for (int i = 0; i < 7; i++)
	{
		m_bmpTetris[i] = LoadResBmp(IDB_BLUE + i);
	}	
}
void UIHandle::Init_Ui(BOOL Restart)
{
	if (Restart)
	{
		memset(game_map, -1, sizeof(int)* 10 * 18);
		m_TetrisLevel = 1;
		m_TetrisScore = 0;
		m_GameTime = 1000;
		InvalidateRect(m_hWnd, NULL, FALSE);
	}else
	{
		RECT rect = {360, 30, 480, 150};
		InvalidateRect(m_hWnd, &rect, FALSE);
	}
	m_StartPoint.x = 3;
	m_StartPoint.y = 0;
	srand((unsigned)time(NULL));
	m_CurretTetris = m_NextTetris;
	m_NextTetris.SetTetrisProperty(rand() % 7, rand() % 4);
	DrawGameMap();
}

void UIHandle::down()
{
	ClearGameMap();
	if (HitTest(m_StartPoint.x, m_StartPoint.y + 1))
	{	
		DrawGameMap(TRUE);
		m_StartPoint.y += 1;
		DrawGameMap();
	}else
	{
		SetGameMap();
		if (-2 == GetCurrentScore())
		{
			m_GameBegin = FALSE;
			KillTimer(m_hWnd, 1);
			GameOver(m_hDC);		
			return;
		}
		Init_Ui();
	}
}

void UIHandle::left()
{
	ClearGameMap();
	if (HitTest(m_StartPoint.x - 1, m_StartPoint.y))
	{
		DrawGameMap(TRUE);
		m_StartPoint.x -= 1;
		DrawGameMap();
	}
	else
		SetGameMap();
}

void UIHandle::right()
{
	ClearGameMap();
	if (HitTest(m_StartPoint.x + 1, m_StartPoint.y))
	{
		DrawGameMap(TRUE);
		m_StartPoint.x += 1;
		DrawGameMap();
	}
	else
		SetGameMap();
}

void UIHandle::up()
{
	ClearGameMap();	
	int TetrisChange = m_CurretTetris.GetTetrisChange();
	int TetrisType = m_CurretTetris.GetTetrisType();
	m_CurretTetris.SetTetrisProperty(TetrisType, TetrisChange + 1);
	if (HitTest(m_StartPoint.x, m_StartPoint.y))
	{
		DrawGameMap(TRUE);
		DrawGameMap();
	}else
	{
		m_CurretTetris.SetTetrisProperty(TetrisType, TetrisChange);
		SetGameMap();
	}
}

void UIHandle::DrawGameMap(BOOL Clear)
{
	if(Clear)
	{
		RECT rect = {m_StartPoint.x * 30, m_StartPoint.y * 30, \
				m_StartPoint.x * 30 + 120, m_StartPoint.y * 30 + 120};			
		InvalidateRect(m_hWnd, &rect, FALSE);
	}
	else
	{
		const POINT * p = NULL;
		int xx = 0;
		int yy = 0;
		int TetrisType = m_CurretTetris.GetTetrisType();
		p = m_CurretTetris.GetTetrisArray();
		HDC memdc;
		memdc = CreateCompatibleDC(m_hDC);

		for (int i = 0; i < 4; i++)
		{
			xx = (m_StartPoint.x + p[i].x);
			yy = (m_StartPoint.y + p[i].y);
			game_map[xx][yy] = TetrisType;
			DrawTetris(m_hDC, memdc, xx * 30, yy * 30, m_bmpTetris[TetrisType]);
		}
		DeleteDC(memdc);
	}
}

void UIHandle::SetGameMap()
{
	const POINT * p = NULL;
	p = m_CurretTetris.GetTetrisArray();
	int TetrisType = m_CurretTetris.GetTetrisType();
	int xx = 0;
	int yy = 0;
	for (int i = 0; i < 4; i++)
	{
		xx = (m_StartPoint.x + p[i].x);
		yy = (m_StartPoint.y + p[i].y);
		game_map[xx][yy] = TetrisType;
	}
}

void UIHandle::ClearGameMap()
{
	const POINT * p = NULL;
	p = m_CurretTetris.GetTetrisArray();
	int xx = 0;
	int yy = 0;
	for (int i = 0; i < 4; i++)
	{
		xx = (m_StartPoint.x + p[i].x);
		yy = (m_StartPoint.y + p[i].y);
		game_map[xx][yy] = -1;
	}
}

BOOL UIHandle::HitTest(int xx, int yy)
{
	const POINT * p = NULL;
	p = m_CurretTetris.GetTetrisArray();

	for (int i = 0; i < 4; i++)
	{
		int x = xx, y = yy;
		x += p[i].x;
		y += p[i].y;

		if (-1 != game_map[x][y])
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
		if (game_map[j][0] != -1)
			return -2;
	}

	for (i = 17; i > 0; i--)
	{
		for (j = 0; j < 10; j++ )
		{
			if (game_map[j][i] == -1 )
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
			game_map[j][i] = -1;
		}
	}
	for (i = ScoreLine - ScoreCount; i > EmptyLine; i--)
	{
		for (j = 0; j < 10; j++)
		{
			game_map[j][i + ScoreCount] = game_map[j][i];
			game_map[j][i] = -1;
		}
	}

	if (4 == ScoreCount)
	{
		m_TetrisScore += 5;
	}
	else
		m_TetrisScore += ScoreCount;

	if (100 * m_TetrisLevel == m_TetrisScore)
	{
		m_TetrisLevel++;
		m_GameTime -= 100 * m_TetrisLevel;
		if (m_GameTime < 50)
			m_GameTime = 50;
	}
	Paint(m_hDC);
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

	SelectObject(memdc, m_bmpBoard);                     //底图
	BitBlt(dc, 0, 0, 300, 540, memdc, 0, 0, SRCCOPY);
	SetStretchBltMode(dc, STRETCH_HALFTONE);
	SelectObject(memdc, m_bmpGirl);
	StretchBlt(dc, 300, 0, 180, 540, memdc, 0, 0, 497, 747, SRCCOPY);
	int i, xx, yy;
	int j = m_NextTetris.GetTetrisType();
	const POINT * p = m_NextTetris.GetTetrisArray();
	
	for (i = 0; i < 4; i++)
	{
		xx = (12 + p[i].x);
		yy = (1 + p[i].y);
		DrawTetris(dc, memdc, xx * 30, yy * 30, m_bmpTetris[j]);
	}
	
	SetTetrisScoreAndLevel(dc);
	
	for (i = 17; i >= 0 ; i--)
	{
		for (j = 0; j < 10; j++)
		{
			if (game_map[j][i] != -1)
			{
				DrawTetris(dc, memdc, 30 * j, 30 * i, m_bmpTetris[game_map[j][i]]);
			}
		}
	}
	DeleteDC(memdc);
}

void UIHandle::SetTetrisScoreAndLevel(HDC dc)
{
	WCHAR ScoreStr[15] = L"得分:";
	WCHAR ScoreStr1[10] = L"";
	_itow_s(m_TetrisScore, ScoreStr1, 10);
	wcscat_s(ScoreStr, ScoreStr1);
	TextOutW(dc, 360, 215, ScoreStr, wcslen(ScoreStr));
	WCHAR LevelStr[15] = L"等级:";
	WCHAR LevelStr1[10] = L"";
	_itow_s(m_TetrisLevel, LevelStr1, 10);
	wcscat_s(LevelStr, LevelStr1);
	TextOutW(dc, 360, 235, LevelStr, wcslen(LevelStr));
}

void UIHandle::GameOver(HDC dc)
{
	WCHAR GameOver[] = L"游戏结束";
	TextOutW(dc, 360, 255, GameOver, wcslen(GameOver));
}


	



