#include "PokerLogic.h"
#include <iostream>
#include <functional>
#include <algorithm>
#include <set>

using namespace std;

PokerLogic::PokerLogic()
{
	
}

PokerLogic::~PokerLogic()
{
}

void PokerLogic::createSendVector()
{
	for (int i = PokerType::Diamond; i <= PokerType::Spade; i++)
	{
		for (int j = PokerValue3; j <= PokerValue2; j++)
		{
			Poker poker(i, j);
			_sendVec.push_back(poker);
		}
	}

	_sendVec.push_back(Poker(PokerType::BlackJoker, PokerValue::PokerValueBlackJoker));
	_sendVec.push_back(Poker(PokerType::RedJocker, PokerValue::PokerValueRedJoker));
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

void PokerLogic::createHandVector()
{
	for (int i = 0; i < 3; i++)
	{
		_handVec.push_back(vector<Poker>());
	}
	int index = 0;
	int contained = false;
	for (auto iter = _sendVec.begin(); iter != _sendVec.end(); ++iter)
	{
		contained = false;
		for(auto landlordIter = _landlordVec.begin(); landlordIter != _landlordVec.end(); ++landlordIter)
		{
			if (*landlordIter == *iter)
			{
				contained = true;
			}
		}
		if (contained) continue;

		_handVec[index].push_back(*iter);
		++index;
		if (index == 3)
		{
			index = 0;
		}
	}

	for (auto iter = _handVec.begin(); iter != _handVec.end(); ++iter)
	{
		sort(iter->begin(), iter->end(), greater<Poker>());
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
