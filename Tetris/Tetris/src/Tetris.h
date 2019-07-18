#ifndef _TETRIS_H
#define _TETRIS_H
#include <windows.h>

class Tetris
{
public:
	Tetris();
	Tetris(const Tetris & rhs);
	~Tetris();
	Tetris& operator=(const Tetris & rhs);
	const POINT* GetTetrisArray(){return m_TeArray;}
	const int GetTetrisType(){return m_SelType;}
	int GetTetrisChange(){return m_SelChange;}
	void SetTetrisProperty(int SelType, int SelChange);
private:
	static int m_Tetris[7][32];
	POINT m_TeArray[4];
	int m_SelType;
	int m_SelChange;
};

#endif //_TETRIS_H
