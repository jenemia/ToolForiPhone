#pragma once
#include <list>
#include <iostream>
#include "HighG.h"

using namespace std;

extern list<int> mExtraApproachNode;

class CNode
{
public:
	int mID; //������ id
	int mPrice;
	int mTotalPrice; //�ش� �����ұ��� �ʿ��� ��, ����ġ�� �ȴ�.
	int mDepth; //Ʈ�� ����
	int mOil; //���� �����ҿ��� ������ ������ �ִ� �⸧
	int mSpendOil;
	int mApproachCount; //���� ������ ����
	CNode* mPreNode; //���� ������ ���
	list<CNode*> mListApproachNode; //���� ������ list
	
	CNode(void);
	~CNode(void);
};

