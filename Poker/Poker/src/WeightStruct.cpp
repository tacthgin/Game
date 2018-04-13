#include "WeightStruct.h"

#include <algorithm>
#include <functional>

using namespace std;

WeightStruct::WeightStruct()
{
}

WeightStruct::~WeightStruct()
{
}

void WeightStruct::addOne(int value)
{
	_single.push_back(value);
}

void WeightStruct::addOne(const std::vector<int>& valueVec)
{
	_single.insert(_single.end(), valueVec.begin(), valueVec.end());
}

void WeightStruct::addTwo(int value)
{
	_double.push_back(value);
}

void WeightStruct::addTwo(const std::vector<int>& valueVec)
{
	_double.insert(_double.end(), valueVec.begin(), valueVec.end());
}

void WeightStruct::addThree(int value)
{
	_three.push_back(value);
}

void WeightStruct::addThree(const std::vector<int>& valueVec)
{
	_three.insert(_three.end(), valueVec.begin(), valueVec.end());
}

void WeightStruct::addBomb(int value)
{
	_bomb.push_back(value);
}

void WeightStruct::addBomb(const std::vector<int>& valueVec)
{
	_bomb.insert(_bomb.end(), valueVec.begin(), valueVec.end());
}

void WeightStruct::addSingleLine(const std::vector<int>& value)
{
	_singleLine.push_back(value);
}

void WeightStruct::addSingleLine(const std::vector<std::vector<int>>& valueVec)
{
	_singleLine.insert(_singleLine.end(), valueVec.begin(), valueVec.end());
}

void WeightStruct::addDoubleLine(const std::vector<int>& value)
{
	_doubleLine.push_back(value);
}

void WeightStruct::addDoubleLine(const std::vector<std::vector<int>>& valueVec)
{
	_doubleLine.insert(_doubleLine.end(), valueVec.begin(), valueVec.end());
}

void WeightStruct::addPlane(const std::vector<int>& value)
{
	_plane.push_back(value);
}

void WeightStruct::addPlane(const std::vector<std::vector<int>>& valueVec)
{
	_plane.insert(_plane.end(), valueVec.begin(), valueVec.end());
}