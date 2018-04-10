#include "PokerLogic.h"
#include <iostream>
#include <functional>
#include <algorithm>
#include <set>

using namespace std;

PokerLogic::PokerLogic()
{
	createPlayer();
}

PokerLogic::~PokerLogic()
{
}

void PokerLogic::createSendVector()
{
	_sendVec.clear();
	for (int i = PokerType::Diamond; i <= PokerType::Spade; i++)
	{
		for (int j = PokerValue::Three; j <= PokerValue::Two; j++)
		{
			Poker poker(i, j);
			_sendVec.push_back(poker);
		}
	}

	_sendVec.push_back(Poker(PokerType::BJoker, PokerValue::BlackJoker));
	_sendVec.push_back(Poker(PokerType::RJocker, PokerValue::RedJoker));
	random_shuffle(_sendVec.begin(), _sendVec.end());
}

void PokerLogic::createLandlordVector()
{
	if (_sendVec.empty()) return;
	vector<int> indexVec = RandomNotRepeat(0, 53, 3);
	for (auto iter = indexVec.begin(); iter != indexVec.end(); ++iter)
	{
		_landlordVec.push_back(_sendVec[*iter]);
	}
	sort(_landlordVec.begin(), _landlordVec.end(), less<Poker>());
}

void PokerLogic::createPlayer()
{
	for (int i = 0; i <= 2; i++)
	{
		_playerVec.push_back(Player(i));
	}
}

void PokerLogic::createHandVector()
{
	vector<vector<Poker>> handVec;
	for (int i = 0; i < 3; i++)
	{
		handVec.push_back(vector<Poker>());
	}
	int index = 0;
	int contained = false;
	for (auto iter = _sendVec.begin(); iter != _sendVec.end(); ++iter)
	{
		contained = false;
		for(auto &m : _landlordVec)
		{
			if (m == *iter)
			{
				contained = true;
				break;
			}
		}
		if (contained) continue;

		handVec[index].push_back(*iter);
		++index;
		if (index == 3)
		{
			index = 0;
		}
	}

	for (int i = 0; i <= 2; i++)
	{
		_playerVec[i].setHandVector(handVec[i]);
	}
}

int PokerLogic::random(int min, int max)
{
	if (min == max) return min;
	mt19937 gen(_rd());
	uniform_int_distribution<int> dist(min, max);
	return dist(gen);
}

std::vector<int> PokerLogic::RandomNotRepeat(int min, int max, int num)
{
	set<int> randomSet;
	while (randomSet.size() != num)
	{
		randomSet.insert(random(min, max));
	}
	
	return vector<int>(randomSet.begin(), randomSet.end());
}

void PokerLogic::printPokerVector(std::string title, const std::vector<Poker>& pokerVector)
{
	cout << "print " + title + " vector" << endl;
	int count = 0;
	for (auto iter = pokerVector.begin(); iter != pokerVector.end(); ++iter)
	{
		cout << iter->toString() << "  ";
		++count;
		if (count == 10)
		{
			cout << endl;
			count = 0;
		}
	}
	cout << endl;
}
