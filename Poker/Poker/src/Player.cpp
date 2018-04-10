#include "Player.h"
#include <iostream>
#include <algorithm>
#include <functional>

using namespace std;

Player::Player(int index)
:_index(index)
{
	
}

void Player::setHandVector(const std::vector<Poker>& handCardVec)
{
	_handVec = handCardVec;
	sort(_handVec.begin(), _handVec.end(), greater<Poker>());
}

void Player::addCard(const std::vector<Poker>& addPokerVec)
{
	if (addPokerVec.empty()) return;
	_handVec.insert(_handVec.end(), addPokerVec.begin(), addPokerVec.end());
	sort(_handVec.begin(), _handVec.end(), greater<Poker>());
}

void Player::removeCard(std::vector<Poker>& removePokerVec)
{
	if (removePokerVec.empty()) return;
	sort(removePokerVec.begin(), removePokerVec.end(), greater<Poker>());
	vector<Poker> temp;
	auto iter = _handVec.begin();
	for (auto &m : removePokerVec)
	{
		while (iter != _handVec.end())
		{
			if (*iter == m)
			{
				++iter;
				break;
			}
			temp.push_back(*iter);
			++iter;
		}
	}
	_handVec = temp;
}

int Player::callScore()
{
	if (_index + 1 > _currentScore)
	{
		return _index + 1;
	}
	return 0;
}

void Player::setCurrentScore(int score)
{
	_currentScore = score;
}

std::vector<Poker> Player::outCard(std::vector<Poker>& input)
{
	return std::vector<Poker>();
}

PokerCombineType Player::getPokerCombineType(std::vector<Poker>& pokerVec)
{
	if (pokerVec.empty()) return PokerCombineType::CombineNull;
	uint32_t size = pokerVec.size();
	if (size == 1) return PokerCombineType::Single;
	if (size == 2)
	{
		if (pokerVec[0].getPokerValue() + pokerVec[1].getPokerValue() == PokerValue::BlackJoker + PokerValue::RedJoker)
		{
			return PokerCombineType::KingBomb;
		}
		else if (getPokerSameNum(pokerVec, 2) == 1)
		{
			return PokerCombineType::Double;
		}
	}else if (size == 3 && getPokerSameNum(pokerVec, 3) == 1)
	{
		return PokerCombineType::ThreeOne;
	}else if (size == 4)
	{
		if (getPokerSameNum(pokerVec, 4) == 1)
		{
			return PokerCombineType::Bomb;
		}
		else if (getPokerSameNum(pokerVec, 3) == 1)
		{
			return PokerCombineType::ThreeTakeOne;
		}
	}else if (size == 5)
	{
		if (getPokerSameNum(pokerVec, 3) == 1 && getPokerSameNum(pokerVec, 2) == 1)
		{
			return PokerCombineType::ThreeTakeTwo;
		}
		else if (getLinkNum(pokerVec, 1) == 5)
		{
			return PokerCombineType::SingleLine;
		}
	}
	else
	{
		if ( size == 6 && 
			getPokerSameNum(pokerVec, 4) == 1 &&
			!containKingBomb(pokerVec) &&
			getPokerSameNum(pokerVec, 2) == 0)
		{
			return PokerCombineType::BombTwoSingle;
		}

		if (size == 8 &&
			getPokerSameNum(pokerVec, 4) == 1 &&
			getPokerSameNum(pokerVec, 2) == 2)
		{
			return PokerCombineType::BombTwoDouble;
		}

		if (getLinkNum(pokerVec, 1) == size)
		{
			return PokerCombineType::SingleLine;
		}
		else if (getLinkNum(pokerVec, 2) == size / 2 && size % 2 == 0)
		{
			return PokerCombineType::DoubleLine;
		}
		else if (getLinkNum(pokerVec, 3) == size / 3 && size % 3 == 0)
		{
			return PokerCombineType::ThreeLine;
		}

		if (getLinkNum(pokerVec, 3) * 3 + getPokerSameNum(pokerVec, 1) == size)
		{
			return PokerCombineType::PlaneSingle;
		}
		else if (getLinkNum(pokerVec, 3) * 3 + getPokerSameNum(pokerVec, 2) * 2 == size)
		{
			return PokerCombineType::PlaneDouble;
		}
	}

	return PokerCombineType::CombineNull;
}

int Player::getPokerSameNum(std::vector<Poker>& pokerVec, int sameCount)
{
	if (pokerVec.empty()) return 0;
	sort(pokerVec.begin(), pokerVec.end(), greater<Poker>());
	Poker& poker = pokerVec[0];
	int count = 0;
	int sameNum = 0;
	for (auto &m : pokerVec)
	{
		if (poker.getPokerValue() == m.getPokerValue())
		{
			++count;
		}
		else if (count == sameCount)
		{
			++sameNum;
			count = 1;
		}
		poker = m;
	}
	return sameNum;
}

int Player::getLinkNum(std::vector<Poker>& pokerVec, int sameCount)
{
	if (pokerVec.empty()) return 0;
	sort(pokerVec.begin(), pokerVec.end(), greater<Poker>());
	Poker& poker = pokerVec[0];
	int count = 0;
	int linkNum = 0;
	for (auto &m : pokerVec)
	{
		if (poker.getPokerValue() == m.getPokerValue())
		{
			++count;
		}
		else if (count > sameCount) 
		{
			if (poker.getPokerValue() == m.getPokerValue() + 1)
			{
				++linkNum;
			}
			else
			{
				linkNum = 0;
			}
			count = 1;
		}
		poker = m;
	}
	return linkNum + 1;
}

bool Player::containKingBomb(std::vector<Poker>& pokerVec)
{
	if (pokerVec.size() >= 2)
	{
		sort(pokerVec.begin(), pokerVec.end(), greater<Poker>());
		return (pokerVec[0].getPokerValue() + pokerVec[1].getPokerValue() == PokerValue::BlackJoker + PokerValue::RedJoker);
	}
	return false;
}

