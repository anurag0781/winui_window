#include "pch.h"
#include "core.h"

namespace model
{

	Sum::Sum(int x, int y) {
			a = x;
			b = y;
	}

	int Sum::getSum() {
		return a + b;
	}

}