#include <iostream>
#include <iterator>
#include "PokerLogic.h"

using namespace std;

int main()
{
	PokerLogic logic;
	logic.createSendVector();
	logic.printPokerVector("send", logic.getSendVector());
	logic.createLandlordVector();
	logic.printPokerVector("landlord", logic.getLandlordVector());
	logic.createHandVector();
	auto v = logic.getHandVector();
	for (auto iter = v.begin(); iter != v.end(); ++iter)
	{
		logic.printPokerVector("hand", *iter);
	}

	char c;
	cin >> c;
	while (c != 'q')
	{
		cout << "����Ҫ����q����ֹ����:" << endl;
		cin >> c;
	}

	return 0;
}