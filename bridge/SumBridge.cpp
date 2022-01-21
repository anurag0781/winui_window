#include "pch.h"
#include "SumBridge.h"
#include "core.h"

using namespace bridge;
using namespace Platform;
using namespace model;

SumBridge::SumBridge()
{
}

int SumBridge::getSum(int x, int y) {
	Sum ob = Sum(x, y);
	int result = ob.getSum();
	return result;
}
