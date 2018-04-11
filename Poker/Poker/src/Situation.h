#ifndef __POKER_LOGIC_H__
#define __POKER_LOGIC_H__

#include <random>
#include "Poker.h"
#include "Player.h"

class Situation
{
public:
	Situation();
	~Situation();
	//获取发牌数组
	const std::vector<Poker>& getSendVector() { return _sendVec; }
	//创建发牌数组
	void createSendVector();
	//获取地主底牌数组
	const std::vector<Poker>& getLandlordVector() { return _landlordVec; }
	//创建地主底牌数组
	void createLandlordVector();
	//创建三个玩家
	void createPlayer();
	std::vector<Player> getPlayer() { return _playerVec; }
	//创建手牌数组
	void createHandVector();
	//获取区间随机数
	int random(int min, int max);
	//获取不重复的随机数
	std::vector<int> RandomNotRepeat(int min, int max, int num);
	//打印牌组
	void printPokerVector(std::string title, const std::vector<Poker>& pokerVector);
private:
	std::random_device _rd;
	//发牌数组
	std::vector<Poker> _sendVec;
	//底牌
	std::vector<Poker> _landlordVec;
	//玩家
	std::vector<Player> _playerVec;
	//谁是地主
	int _landlordIndex = {-1};
};

#endif //__POKER_LOGIC_H__
