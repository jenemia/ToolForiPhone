#include "Node.h"

CNode::CNode(void)
{
	this->mID = -1;
	this->mPrice = 0;
	this->mTotalPrice = 0;
	this->mDepth = 0;
	this->mOil = 0;
	this->mSpendOil = 0;
	this->mPreNode = NULL;
	this->mListApproachNode.clear();
	this->mApproachCount = 0;
}


CNode::~CNode(void)
{
}
