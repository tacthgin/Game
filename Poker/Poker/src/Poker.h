#pragma once

#include <string>
#include "Constants.h"

class Poker
{
public:
	Poker(int type, int value);
	bool operator<(const Poker& compare);
	bool operator>(const Poker& compare);
	bool operator==(const Poker& compare);
	bool operator!=(const Poker& compare);
	int getPokerType();
	int getPokerValue();
	std::string toString();
private:
	int _type = { PokerType::PokerTypeNull };
	int _value = { PokerValue::PokerValueNull };
};

