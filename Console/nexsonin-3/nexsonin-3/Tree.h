#pragma once
#include <iostream>
#include <fstream>
#include <iomanip>
#include <list>
#include <map>
#include <queue>
#include <stack>

#include "HighG.h"
#include "Node.h"

using namespace std;
extern list<int> mExtraApproachNode;
class CTree
{
private :
	int mBegin; //���� ��� id
	int mEnd; //���� ��� id
	int* mGasStation; //������ �迭. index:id , value:�⸧��
	int mGasCount; //������ ����
	int** mAdjacencyMatrix; //�� ��尣 �Ÿ��� ���� �������
	int mLineCount; // ����(��) �� 

	CNode* mHeadNode; //���� ���
	CNode* mCurrentNode;
	list<int> mListMinimumLoad; //�ּ��� �� ����
	queue<int> mQueueWorkLoad; //��带 �Էµ� ������� ó��
	list<CNode*> mListPassNode; //�ѹ� ������ ��带 �����Ͽ� ���߿� �ٷ� ����ϱ�.
	list<CNode*>::iterator mIterNode;
	int mCurrentDepth;


	void Insert(int id); //������ id�� ���� ��ü �ֱ�
	void ApproachNodeInsert( int distance, CNode* preNode, int nowID );
	void BlockMatrix( int row, int col );
public:
	
	CTree(void);
	~CTree(void);
	void InitWithTxt(); //�ؽ�Ʈ ������ �о� ������ �� �� ����
	void DisplayMatrix(); //InitWithTxt()�� ���� ���� ������ ���� ��� ����
	void Start();
	void Result();
};

