#include <iostream>
#include "PokerLogic.h"

using namespace std;

int main()
{
	char c;
	cin >> c;
	while (c != 'q')
	{
		cout << "你需要输入q来终止程序:" << endl;
		cin >> c;
	}

	return 0;
}