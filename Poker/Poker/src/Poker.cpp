#include "Poker.h"

Poker::Poker(int type, int value)
:_type(type)
,_value(value)
{
	
}

bool Poker::operator<(const Poker & compare) const
{
	return _value < compare._value;
}

bool Poker::operator>(const Poker & compare) const
{
	return _value > compare._value;
}

bool Poker::operator==(const Poker & compare) const
{
	return (compare._type == _type && compare._value == _value);
}

bool Poker::operator!=(const Poker & compare) const
{
	return !((*this) == compare);
}

int Poker::getPokerType()
{
	return _type;
}

int Poker::getPokerValue()
{
	return _value;
}

std::string Poker::toString() const
{
	std::string symbol[6] = {"D", "C", "H", "S", "B", "R"};
	std::string value[15] = { "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2", "BJOKER", "RJOKER" };
	return value[_value - 3];
}
