#ifndef __PLAYER_H__
#define __PLAYER_H__

#include <vector>
#include "Poker.h"

class Player
{
public:
	Player(int index);
	void setHandVector(const std::vector<Poker>& handCardVec);
	const std::vector<Poker>& getHandVector() { return _handVec; }
	bool isLandlord() { return _landlord; }
	void setLandlord(bool landlord) { _landlord = landlord; }
	void addCard(const std::vector<Poker>& addPokerVec);
	void removeCard(std::vector<Poker>& removePokerVec);

	//叫分
	int callScore();
	void setCurrentScore(int score);
	//出牌
	std::vector<Poker> outCard(std::vector<Poker>& input);

	//获得组合类型
	PokerCombineType getPokerCombineType(std::vector<Poker>& pokerVec);
	//获取相同数目的牌的个数
	int getPokerSameNum(std::vector<Poker>& pokerVec, int sameCount);
	//得到连对数目
	int getLinkNum(std::vector<Poker>& pokerVec, int sameCount);
	//是否包含王炸
	bool containKingBomb(std::vector<Poker>& pokerVec);
private:
	int _index = {0};
	bool _landlord = { false };
	int _currentScore = { 0 };
	std::vector<Poker> _handVec;
};

#endif __PLAYER_H__