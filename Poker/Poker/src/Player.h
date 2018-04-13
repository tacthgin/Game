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
	void addPoker(std::vector<Poker>& srcPokerVec, const std::vector<Poker>& addPokerVec);
	void removePoker(std::vector<Poker>& srcPokerVec, std::vector<Poker>& removePokerVec);
	void removeSamePoker(std::vector<Poker>& srcPokerVec, std::vector<int>& removePokerVec);
	//叫分
	int callScore();
	//出牌
	std::vector<Poker> outCard(std::vector<Poker>& input);

	//获得组合类型
	PokerCombineType getPokerCombineType(std::vector<Poker>& pokerVec);
	//获取相同数目的牌的个数
	int getPokerSameNum(std::vector<Poker>& pokerVec, unsigned int sameCount);
	//得出相同的牌
	std::vector<int> getPokerSameVector(std::vector<Poker>& pokerVec, unsigned int sameCount);
	//得到连对数目
	int getLinkNum(std::vector<Poker>& pokerVec, int sameCount);
	//得到一个牌数组中的顺子或者对子或者三张
	std::vector<std::vector<int>> getLinkVector(std::vector<int>& pokerVec, int num);
	//得到一个牌数组中的所有顺子的牌
	std::vector<int> getLinkVecotr(std::vector<Poker>& pokerVec);
	//返回不包含的元素
	std::vector<int> noContain(const std::vector<int>& pokerVec, const std::vector<int>& compareVec);
	//是否包含王炸
	bool containKingBomb(std::vector<Poker>& pokerVec);

	std::vector<int> getSinglePoker(std::vector<int>& pokerVec);
	std::vector<int> getNoRepeatPoker(std::vector<int>& pokerVec);
	void analysisPoker();
	void analySisPoker1();
private:
	int _index = {0};
	bool _landlord = { false };
	std::vector<Poker> _handVec;
};

#endif __PLAYER_H__