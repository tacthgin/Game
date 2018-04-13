#include "Player.h"
#include <iostream>
#include <algorithm>
#include <functional>

#include "WeightStruct.h"

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

std::vector<std::vector<int>> Player::getLinkVector(std::vector<int>& pokerVec, int num)
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
			auto next = iter + 1;
			for (; next != pokerVec.end(); ++next)
			{
				if (front == *next)
				{
					continue;
				}else if (front = *next + 1)
				{
					temp.push_back(*next);
					front = *next;
				}
				else
				{
					break;
				}
			}

			iter = next;
			if (temp.size() >= num)
			{
				result.push_back(temp);
			}
		}
	}
	
	return result;
}

std::vector<int> Player::getLinkVecotr(std::vector<Poker>& pokerVec)
{
	vector<int> result;
	if (!pokerVec.empty())
	{
		auto iter = pokerVec.begin();
		while (iter != pokerVec.end())
		{
			vector<int> temp;
			int front = iter->getPokerValue();
			temp.push_back(front);
			auto next = iter + 1;
			for (; next != pokerVec.end(); ++next)
			{
				int currentValue = next->getPokerValue();
				if (front == currentValue)
				{
					continue;
				}
				else if (front = currentValue + 1)
				{
					temp.push_back(currentValue);
					front = currentValue;
				}
				else
				{
					break;
				}
			}

			iter = next;
			if (temp.size() >= 5)
			{
				result.insert(result.end(), temp.begin(), temp.end());
			}
		}
	}

}

std::vector<int> Player::noContain(const std::vector<int>& pokerVec, const std::vector<int>& compareVec)
{
	vector<int> result;
	auto iter1 = pokerVec.begin();
	auto iter2 = compareVec.begin();
	while (iter1 != pokerVec.end() && iter2 != compareVec.end())
	{
		if (*iter1 == *iter2)
		{
			++iter1;
			++iter2;
		}
		else if (*iter1 > *iter2)
		{
			++iter1;
		}
		else
		{
			result.push_back(*iter2);
			++iter2;
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

std::vector<int> Player::getSinglePoker(std::vector<int>& pokerVec)
{
	vector<int> result;
	if (!pokerVec.empty())
	{
		if (pokerVec.size() >= 2)
		{
			for (int i = 0; i < 2; i++)
			{
				if (pokerVec[0] == PokerValue::RedJoker || pokerVec[0] == PokerValue::BlackJoker)
				{
					result.push_back(pokerVec[0]);
					pokerVec.erase(pokerVec.begin());
				}
			}
		}

		vector<int> link = getNoRepeatPoker(pokerVec);
		if (!link.empty())
		{
			result.insert(result.end(), link.begin(), link.end());
		}
	}
	return result;
}

std::vector<int> Player::getNoRepeatPoker(std::vector<int>& pokerVec)
{
	vector<int> result;
	if (!result.empty())
	{
		vector<int> link;
		auto begin = pokerVec.begin();
		link.push_back(*begin);
		for (auto iter = pokerVec.begin() + 1; iter != pokerVec.end(); ++iter)
		{
			if (*begin = *iter + 1)
			{
				link.push_back(*iter);
			}
			else if (link.size() >= 5)
			{
				link.clear();
			}
			else
			{
				result.insert(result.end(), link.begin(), link.end());
			}
		}
	}

	return result;
}

void Player::analysisPoker()
{
	vector<Poker> tempHandVec = _handVec;
	sort(tempHandVec.begin(), tempHandVec.end(), greater<Poker>());
	//王炸
	if (containKingBomb(tempHandVec))
	{
		removeSamePoker(tempHandVec, { PokerValue::RedJoker, PokerValue::BlackJoker });
	}

	//炸弹
	vector<int> temp = getPokerSameVector(tempHandVec, 4);
	if (!temp.empty())
	{
		removeSamePoker(tempHandVec, temp);
	}

	//三张
	temp = getPokerSameVector(tempHandVec, 3);
	vector<vector<int>> threeLine;
	if (!temp.empty())
	{
		//三顺
		threeLine = getLinkVector(temp, 2);
		if (!threeLine.empty())
		{
			
		}
	}

	//顺子
	vector<Poker> link = tempHandVec;
	
}

void Player::analySisPoker1()
{
	WeightStruct ws;
	vector<Poker> tempHandVec = _handVec;
	sort(tempHandVec.begin(), tempHandVec.end(), greater<Poker>());
	//获取独立的单张
	vector<int> single = getSinglePoker(getPokerSameVector(tempHandVec, 1));

	vector<int> two = getPokerSameVector(tempHandVec, 2);

	vector<int> three = getPokerSameVector(tempHandVec, 3);
	
	vector<int> link = getLinkVecotr(tempHandVec);
	vector<int> indenpentTwo = noContain(link, two);
	vector<int> indenpentThree = noContain(link, three);
	
	vector<int> remove;
	removeSamePoker(tempHandVec, single);
	removeSamePoker(tempHandVec, indenpentTwo);
	removeSamePoker(tempHandVec, indenpentThree);

	sort(tempHandVec.begin(), tempHandVec.end(), less<Poker>());


}

