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
	//��ȡ��������
	const std::vector<Poker>& getSendVector() { return _sendVec; }
	//������������
	void createSendVector();
	//��ȡ������������
	const std::vector<Poker>& getLandlordVector() { return _landlordVec; }
	//����������������
	void createLandlordVector();
	const std::vector<std::vector<Poker>>& getHandVector() { return _handVec; }
	//������������
	void createHandVector();
	//��ȡ���������
	int random(int min, int max);
	//��ȡ���ظ��������
	std::vector<int> RandomNotRepeat(int min, int max, int num);
	//��ӡ����
	void printPokerVector(std::string title, const std::vector<Poker>& pokerVector);
private:
	//��������
	std::vector<Poker> _sendVec;
	std::vector<Poker> _landlordVec;
	std::random_device _rd;
	std::vector<std::vector<Poker>> _handVec;
	int _landlordIndex;
};

#endif //__POKER_LOGIC_H__
