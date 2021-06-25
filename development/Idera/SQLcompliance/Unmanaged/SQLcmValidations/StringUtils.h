#ifndef __STRINGUTILS_H_
#define __STRINGUTILS_H_

#include <string>
#include <vector>

using namespace std;

class StringUtils
{

public:

	static int SplitString(const wstring& input, const wstring& delimiter, vector<wstring>& results, bool includeEmpties = true);

};

#endif