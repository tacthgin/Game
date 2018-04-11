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

void Player::addPoker(std::vector<Poker>& srcPokerVec, const std::vector<Poker>& addPokerVec)
{
	if (addPokerVec.empty()) return;
	srcPokerVec.insert(srcPokerVec.end(), addPokerVec.begin(), addPokerVec.end());
	sort(srcPokerVec.begin(), srcPokerVec.end(), greater<Poker>());
}

void Player::removePoker(std::vector<Poker>& srcPokerVec, std::vector<Poker>& removePokerVec)
{
	if (removePokerVec.empty()) return;

	auto iter = srcPokerVec.begin();
	for (auto &m : removePokerVec)
	{
		while (iter != srcPokerVec.end())
		{
			if (*iter == m)
			{
				iter = srcPokerVec.erase(iter);
			}
			else
			{
				++iter;
			}
			break;
		}
	}
}

void Player::removeSamePoker(std::vector<Poker>& srcPokerVec, std::vector<int>& removePokerVec)
{
	if (removePokerVec.empty()) return;

	auto iter = srcPokerVec.begin();
	for (auto m : removePokerVec)
	{
		while (iter != srcPokerVec.end())
		{
			if (iter->getPokerValue() == m)
			{
				while (iter->getPokerValue() == m && iter != srcPokerVec.end())
				{
					iter = srcPokerVec.erase(iter);
				}
				break;
			}
			else
			{
				++iter;
			}
		}
	}
}

int Player::callScore()
{
	return 0;
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

	sort(pokerVec.begin(), pokerVec.end(), greater<Poker>());

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

int Player::getPokerSameNum(std::vector<Poker>& pokerVec, unsigned int sameCount)
{
	if (pokerVec.empty()) return 0;
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

std::vector<int> Player::getPokerSameVector(std::vector<Poker>& pokerVec, unsigned int sameCount)
{
	vector<int> temp;
	if (!pokerVec.empty())
	{
		Poker& poker = pokerVec[0];
		int count = 0;
		for (auto &m : pokerVec)
		{
			if (poker.getPokerValue() == m.getPokerValue())
			{
				++count;
			}
			else if (count == sameCount)
			{
				temp.push_back(m.getPokerValue());
				count = 1;
			}
			poker = m;
		}
	}
	return temp;
}

int Player::getLinkNum(std::vector<Poker>& pokerVec, int sameCount)
{
	if (pokerVec.empty()) return 0;
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

std::vector<std::vector<int>> Player::getLinkVector(std::vector<int>& pokerVec)
{
	vector<vector<int>> result;
	if (!pokerVec.empty())
	{
		auto iter = pokerVec.begin();
		while (iter != pokerVec.end())
		{
			vector<int> temp;
			int front = *iter;
			temp.push_back(front);
			for (auto next = iter + 1; next != pokerVec.end(); ++next)
			{
				if (front = *next + 1)
				{
					temp.push_back(*next);
					front = *next;
				}
				else
				{
					break;
				}
			}

			int size = temp.size();
			if (size > 1)
			{
				iter += size - 1;
				result.push_back(temp);
			}
		}
	}
	
	return result;
}

bool Player::containKingBomb(std::vector<Poker>& pokerVec)
{
	if (pokerVec.size() >= 2)
	{
		return (pokerVec[0].getPokerValue() + pokerVec[1].getPokerValue() == PokerValue::BlackJoker + PokerValue::RedJoker);
	}
	return false;
}

void Player::analysisPoker()
{
	vector<Poker> tempHandVec = _handVec;
	sort(tempHandVec.begin(), tempHandVec.end(), greater<Poker>());
	//��ը
	if (containKingBomb(tempHandVec))
	{
		removeSamePoker(tempHandVec, { PokerValue::RedJoker, PokerValue::BlackJoker });
	}

	//ը��
	vector<int> temp = getPokerSameVector(tempHandVec, 4);
	if (!temp.empty())
	{
		removeSamePoker(tempHandVec, temp);
	}

	//����
	temp = getPokerSameVector(tempHandVec, 3);
	if (!temp.empty())
	{
		vector<vector<int>> threeLine = getLinkVector(temp);
		if (!threeLine.empty())
		{

		}
	}

	//˳��

}

