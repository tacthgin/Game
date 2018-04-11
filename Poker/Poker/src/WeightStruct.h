#ifndef __WEIGHT_STRUCT_H__
#define __WEIGHT_STRUCT_H__

#include <vector>
#include "Poker.h"

class WeightStruct
{
public:
	WeightStruct(const std::vector<Poker>& pokerVec);
	~WeightStruct();
	void analysisPoker();
private:
	std::vector<Poker> _pokerVec;
	std::vector<int> _three; //����
	std::vector<int> _singleLine; //˳��
	std::vector<int> _double; //����
	std::vector<int> _single;
	std::vector<int> _bomb;
	int _handCount;
	int _weight;
};

#endif //__WEIGHT_STRUCT_H__