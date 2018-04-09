#ifndef __POKER_H__
#define __POKER_H__

#include <string>
#include "Constants.h"

class Poker
{
public:
	Poker(int type, int value);

	Poker& operator=(const Poker& copy);
	//±È½Ïº¯Êý
	bool operator<(const Poker& compare) const;
	bool operator>(const Poker& compare) const;
	bool operator==(const Poker& compare) const;
	bool operator!=(const Poker& compare) const;
	
	int getPokerType();
	int getPokerValue();
	std::string toString() const;
private:
	int _type = { PokerType::PokerTypeNull };
	int _value = { PokerValue::PokerValueNull };
};

#endif //__POKER_H__