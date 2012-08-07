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
	list<CNode*>::iterator iter;
	if( !this->mListApproachNode.empty() )
	{
		for( iter = this->mListApproachNode.begin(); iter != this->mListApproachNode.end(); iter++ )
		{		
			int num = (*iter)->mID;
			mExtraApproachNode.push_back(num);
			(*iter)->mPreNode = NULL;
		}
		this->mListApproachNode.clear();

	}
}
