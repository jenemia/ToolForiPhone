#pragma once
#include <iostream>
#include <fstream>
#include <iomanip>
#include <list>
#include <map>
#include <queue>

#include "Node.h"

using namespace std;

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
	queue<int> mQueueWorkedLoad; //��带 �Էµ� ������� ó��
	map<int, CNode*> mMapPassNode;
	int mCurrentDepth;

	void Insert(int id); //������ id�� ���� ��ü �ֱ�

public:
	CTree(void);
	~CTree(void);
	void InitWithTxt(); //�ؽ�Ʈ ������ �о� ������ �� �� ����
	void DisplayMatrix(); //InitWithTxt()�� ���� ���� ������ ���� ��� ����
	void Start();
};

