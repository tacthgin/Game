#include "Player.h"
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

void Player::addCard(const std::vector<Poker>& addCardVec)
{
	if (addCardVec.empty()) return;
	_handVec.insert(_handVec.end(), addCardVec.begin(), addCardVec.end());
	sort(_handVec.begin(), _handVec.end(), greater<Poker>());
}

void Player::removeCard(std::vector<Poker>& removeCardVec)
{
	if (removeCardVec.empty()) return;
	sort(removeCardVec.begin(), removeCardVec.end(), greater<Poker>());
	vector<Poker> temp;
	auto iter = _handVec.begin();
	for (auto &m : removeCardVec)
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
