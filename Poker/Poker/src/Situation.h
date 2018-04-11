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
	//��ȡ��������
	const std::vector<Poker>& getSendVector() { return _sendVec; }
	//������������
	void createSendVector();
	//��ȡ������������
	const std::vector<Poker>& getLandlordVector() { return _landlordVec; }
	//����������������
	void createLandlordVector();
	//�����������
	void createPlayer();
	std::vector<Player> getPlayer() { return _playerVec; }
	//������������
	void createHandVector();
	//��ȡ���������
	int random(int min, int max);
	//��ȡ���ظ��������
	std::vector<int> RandomNotRepeat(int min, int max, int num);
	//��ӡ����
	void printPokerVector(std::string title, const std::vector<Poker>& pokerVector);
private:
	std::random_device _rd;
	//��������
	std::vector<Poker> _sendVec;
	//����
	std::vector<Poker> _landlordVec;
	//���
	std::vector<Player> _playerVec;
	//˭�ǵ���
	int _landlordIndex = {-1};
};

#endif //__POKER_LOGIC_H__
