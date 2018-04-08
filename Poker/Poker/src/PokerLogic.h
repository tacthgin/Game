#ifndef __POKER_LOGIC_H__
#define __POKER_LOGIC_H__

#include <random>
#include <vector>
#include "Poker.h"

class PokerLogic
{
public:
	PokerLogic();
	~PokerLogic();
	//获取发牌数组
	const std::vector<Poker>& getSendVector() { return _sendVec; }
	//创建发牌数组
	void createSendVector();
	//获取地主底牌数组
	const std::vector<Poker>& getLandlordVector() { return _landlordVec; }
	//创建地主底牌数组
	void createLandlordVector();
	const std::vector<std::vector<Poker>>& getHandVector() { return _handVec; }
	//创建手牌数组
	void createHandVector();
	//获取区间随机数
	int random(int min, int max);
	//获取不重复的随机数
	std::vector<int> RandomNotRepeat(int min, int max, int num);
	//打印牌组
	void printPokerVector(std::string title, const std::vector<Poker>& pokerVector);
private:
	//发牌数组
	std::vector<Poker> _sendVec;
	std::vector<Poker> _landlordVec;
	std::random_device _rd;
	std::vector<std::vector<Poker>> _handVec;
	int _landlordIndex;
};

#endif //__POKER_LOGIC_H__
