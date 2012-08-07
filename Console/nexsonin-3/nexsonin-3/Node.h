#pragma once
#include <list>
#include <iostream>

using namespace std;

class CNode
{
public:
	int mID; //주유소 id
	int mPrice;
	int mTotalPrice; //해당 주유소까지 필요한 돈, 가중치가 된다.
	int mDepth; //트리 깊이
	int mOil; //현재 주유소에서 차량이 가지고 있는 기름
	int mSpendOil;
	int mApproachCount; //인접 주유소 개수
	CNode* mPreNode; //이전 주유소 기억
	list<CNode*> mListApproachNode; //인접 주유소 list
	
	CNode(void);
	~CNode(void);
};

