#pragma once

//花色
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

//牌值
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

//牌的类型
enum PokerCombineType
{
	CombineNull = 0, //错误类型
	Single = 1, //单牌
	Double = 2, //对子
	ThreeOne = 3, //三张
	ThreeTakeOne = 4, //三带一
	ThreeTakeTwo = 5, //三带二
	Bomb = 6, //炸弹
	BombTwoSingle = 7, //炸弹带2单张
	BombTwoDouble = 8, //炸弹带2双
	SingleLine = 9, //顺子
	DoubleLine = 10, //连对
	ThreeLine = 11, //三顺
	PlaneSingle = 12, //飞机带单张
	PlaneDouble = 13, //飞机带对子
	KingBomb = 14, //王炸
};

//权值
enum PokerWeight
{
	WeightSingle = 1,
	WeightDouble = 2,
	WeightThree = 3,
	WeightSinleLine = 4, //每多一张牌,权值+1
	WeightSinleLine = 5, //每多一对牌,权值+2
	WeightPlane = 6, //每一对飞机，权值在基础上+3
	WeightBomb = 7, //炸弹
};