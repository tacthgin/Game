#ifndef __WEIGHT_STRUCT_H__
#define __WEIGHT_STRUCT_H__

#include <vector>
#include "Poker.h"

class WeightStruct
{
public:
	WeightStruct();
	~WeightStruct();
	void addOne(int value);
	void addOne(const std::vector<int>& valueVec);
	void addTwo(int value);
	void addTwo(const std::vector<int>& valueVec);
	void addThree(int value);
	void addThree(const std::vector<int>& valueVec);
	void addBomb(int value);
	void addBomb(const std::vector<int>& valueVec);
	void addSingleLine(const std::vector<int>& value);
	void addSingleLine(const std::vector<std::vector<int>>& valueVec);
	void addDoubleLine(const std::vector<int>& value);
	void addDoubleLine(const std::vector<std::vector<int>>& valueVec);
	void addPlane(const std::vector<int>& value);
	void addPlane(const std::vector<std::vector<int>>& valueVec);
private:
	std::vector<int> _single;
	std::vector<int> _double; //对子
	std::vector<int> _three; //三张
	std::vector<int> _bomb;

	std::vector<std::vector<int>> _singleLine; //顺子
	std::vector<std::vector<int>> _doubleLine; //连对
	std::vector<std::vector<int>> _plane; //飞机
	
	int _handCount;
	int _weight;
};

#endif //__WEIGHT_STRUCT_H__