#pragma once
#include "core.h"

namespace bridge
{
    public ref class SumBridge sealed
    {
    public:
		SumBridge();
		int getSum(int x, int y);
    };
}
