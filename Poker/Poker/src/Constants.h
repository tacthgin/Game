#pragma once

//��ɫ
enum PokerType
{
	PokerTypeNull = 0,
	Diamond = 1,
	Club,
	Heart,
	Spade,
	BlackJoker,
	RedJocker
};

//��ֵ
enum PokerValue
{
	PokerValueNull = 0,
	PokerValue3 = 3,
	PokerValue4 = 4,
	PokerValue5 = 5,
	PokerValue6 = 6,
	PokerValue7 = 7,
	PokerValue8 = 8,
	PokerValue9 = 9,
	PokerValue10 = 10,
	PokerValueJ = 11,
	PokerValueQ = 12,
	PokerValueK = 13,
	PokerValueA = 14,
	PokerValue2 = 15,
	PokerValueBlackJoker = 16,
	PokerValueRedJoker = 17
};

//�Ƶ�����
enum PokerCombineType
{
	Zero = 0, //û�Ƴ�
	Single = 1, //����
	Double = 2, //����
	Three = 3, //����
	ThreeOne = 4, //����һ
	ThreeTwo = 5, //������
	Bomb = 6, //ը��
	BombTwoSingle = 7, //ը����2����
	BombTwoDouble = 8, //ը����2˫
	SingleLine = 9, //˳��
	DoubleLine = 10, //����
	ThreeLine = 11, //��˳
	PlaneSingle = 12, //�ɻ�������
	PlaneDouble = 13, //�ɻ�������
	KingBomb = 14, //��ը
};