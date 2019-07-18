#include "Tetris.h"

int Tetris::m_Tetris[7][32] = { 
							{0,0,0,1,1,0,1,1, 0,0,0,1,1,0,1,1, 0,0,0,1,1,0,1,1, 0,0,0,1,1,0,1,1},
							{0,0,0,1,0,2,0,3, 0,0,1,0,2,0,3,0, 0,0,0,1,0,2,0,3, 0,0,1,0,2,0,3,0},
							{0,0,0,1,0,2,1,2, 0,1,1,1,2,1,2,0, 0,0,1,0,1,1,1,2, 0,0,1,0,2,0,0,1},		
							{1,0,1,1,1,2,0,2, 0,0,1,0,2,0,2,1, 0,0,0,1,0,2,1,0, 0,0,0,1,1,1,2,1},		
							{1,0,1,1,0,1,0,2, 0,0,1,0,1,1,2,1, 1,0,1,1,0,1,0,2, 0,0,1,0,1,1,2,1},		
							{0,0,0,1,1,1,1,2, 2,0,1,0,1,1,0,1, 0,0,0,1,1,1,1,2, 2,0,1,0,1,1,0,1},		
							{1,0,0,1,1,1,2,1, 1,0,0,1,1,1,1,2, 0,0,1,0,2,0,1,1, 1,0,1,1,2,1,1,2}
							};

Tetris::Tetris()
{
	
}

Tetris::Tetris(const Tetris & rhs)
{
	m_SelType = rhs.m_SelType;
	m_SelChange = rhs.m_SelChange;
	for (int i = 0; i < 4; i++)
	{
		m_TeArray[i] = rhs.m_TeArray[i];
	}
}

Tetris& Tetris::operator=(const Tetris & rhs)
{
	m_SelType = rhs.m_SelType;
	m_SelChange = rhs.m_SelChange;
	for (int i = 0; i < 4; i++)
	{
		m_TeArray[i] = rhs.m_TeArray[i];
	}
	return *this;
}

Tetris::~Tetris()
{
	
}
void Tetris::SetTetrisProperty(int SelType, int SelChange)
{
	m_SelType = SelType;
	m_SelChange = SelChange;
	if (m_SelChange >= 4)
		m_SelChange = 0;

	for (int i = 0; i < 4; i++)
	{
		m_TeArray[i].x = m_Tetris[m_SelType][m_SelChange * 8 + i * 2];
		m_TeArray[i].y = m_Tetris[m_SelType][m_SelChange * 8 + i * 2 + 1];
	}
}
