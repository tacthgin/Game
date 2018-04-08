#pragma once

#include <vector>
#include "Poker.h"

class PokerLogic
{
public:
	PokerLogic();
	~PokerLogic();
	void createSendVector();
	void printPokerVector(std::string title, std::vector<Poker>& pokerVector);
private:
	std::vector<Poker> _sendVec;
};

