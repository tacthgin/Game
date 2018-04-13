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
	//�з�
	int callScore();
	//����
	std::vector<Poker> outCard(std::vector<Poker>& input);

	//����������
	PokerCombineType getPokerCombineType(std::vector<Poker>& pokerVec);
	//��ȡ��ͬ��Ŀ���Ƶĸ���
	int getPokerSameNum(std::vector<Poker>& pokerVec, unsigned int sameCount);
	//�ó���ͬ����
	std::vector<int> getPokerSameVector(std::vector<Poker>& pokerVec, unsigned int sameCount);
	//�õ�������Ŀ
	int getLinkNum(std::vector<Poker>& pokerVec, int sameCount);
	//�õ�һ���������е�˳�ӻ��߶��ӻ�������
	std::vector<std::vector<int>> getLinkVector(std::vector<int>& pokerVec, int num);
	//�õ�һ���������е�����˳�ӵ���
	std::vector<int> getLinkVecotr(std::vector<Poker>& pokerVec);
	//���ز�������Ԫ��
	std::vector<int> noContain(const std::vector<int>& pokerVec, const std::vector<int>& compareVec);
	//�Ƿ������ը
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