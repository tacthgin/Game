#pragma once

//��ɫ
enum PokerType
{
	PokerTypeNull = 0,
	Diamond = 1,
	Club,
	Heart,
	Spade,
	BJoker,
	RJocker
};

//��ֵ
enum PokerValue
{
	Zero = 0,
	Three = 3,
	Four = 4,
	Five = 5,
	Six = 6,
	Seven = 7,
	Eight = 8,
	Nine = 9,
	Ten = 10,
	J = 11,
	Q = 12,
	K = 13,
	A = 14,
	Two = 15,
	BlackJoker = 16,
	RedJoker = 17
};

//�Ƶ�����
enum PokerCombineType
{
	CombineNull = 0, //��������
	Single = 1, //����
	Double = 2, //����
	ThreeOne = 3, //����
	ThreeTakeOne = 4, //����һ
	ThreeTakeTwo = 5, //������
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

//Ȩֵ
enum PokerWeight
{
	WeightSingle = 1,
	WeightDouble = 2,
	WeightThree = 3,
	WeightSinleLine = 4, //ÿ��һ����,Ȩֵ+1
	WeightSinleLine = 5, //ÿ��һ����,Ȩֵ+2
	WeightPlane = 6, //ÿһ�Էɻ���Ȩֵ�ڻ�����+3
	WeightBomb = 7, //ը��
};