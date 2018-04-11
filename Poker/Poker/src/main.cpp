#include <iostream>
#include <iterator>
#include "PokerLogic.h"

using namespace std;

int main()
{
	Situation logic;
	logic.createSendVector();
	logic.printPokerVector("send", logic.getSendVector());
	logic.createLandlordVector();
	logic.printPokerVector("landlord", logic.getLandlordVector());
	logic.createHandVector();
	vector<Player> players = logic.getPlayer();
	for (auto &player : players)
	{
		logic.printPokerVector("hand", player.getHandVector());
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