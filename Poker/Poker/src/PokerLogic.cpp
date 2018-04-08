#include "PokerLogic.h"
#include <iostream>
#include <algorithm>

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

void PokerLogic::printPokerVector(std::string title, std::vector<Poker>& pokerVector)
{
	cout << "print " + title + " vector" << endl;
	for (auto iter = pokerVector.begin(); iter != pokerVector.end(); ++iter)
	{
		cout << iter->toString() << "  ";
	}
}
