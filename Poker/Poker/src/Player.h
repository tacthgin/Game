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
	void addCard(const std::vector<Poker>& addCardVec);
	void removeCard(std::vector<Poker>& removeCardVec);



private:
	int _index = {0};
	bool _landlord = { false };
	std::vector<Poker> _handVec;
};

#endif __PLAYER_H__