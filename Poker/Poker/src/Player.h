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

	//�з�
	int callScore();
	void setCurrentScore(int score);
	//����
	std::vector<Poker> outCard(std::vector<Poker>& input);

	//����������
	PokerCombineType getPokerCombineType(std::vector<Poker>& pokerVec);
	//��ȡ��ͬ��Ŀ���Ƶĸ���
	int getPokerSameNum(std::vector<Poker>& pokerVec, int sameCount);
	//�õ�������Ŀ
	int getLinkNum(std::vector<Poker>& pokerVec, int sameCount);
	//�Ƿ������ը
	bool containKingBomb(std::vector<Poker>& pokerVec);
private:
	int _index = {0};
	bool _landlord = { false };
	int _currentScore = { 0 };
	std::vector<Poker> _handVec;
};

#endif __PLAYER_H__