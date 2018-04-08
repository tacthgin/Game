#include "Poker.h"

Poker::Poker(int type, int value)
:_type(type)
,_value(value)
{
	
}

bool Poker::operator<(const Poker & compare)
{
	return _value < compare._value;
}

bool Poker::operator>(const Poker & compare)
{
	return _value > compare._value;
}

bool Poker::operator==(const Poker & compare)
{
	return (compare._type == _type && compare._value == _value);
}

bool Poker::operator!=(const Poker & compare)
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

std::string Poker::toString()
{
	return "type:" + std::to_string(_type) + " value:" + std::to_string(_value);
}
