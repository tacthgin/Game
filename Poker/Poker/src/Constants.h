#pragma once

//花色
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

//牌值
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

//牌的类型
enum PokerCombineType
{
	Zero = 0, //没牌出
	Single = 1, //单牌
	Double = 2, //对子
	Three = 3, //三张
	ThreeOne = 4, //三带一
	ThreeTwo = 5, //三带二
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